using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace AssetSystem.IntegrationTests;

/// <summary>
/// The whole story across all four modules: register an asset, give it to an employee, watch the
/// Assets module react to the Consignments module's event.
/// </summary>
[Collection("Api")] // all API test classes share one collection so they run sequentially (see ApiFixture)
public class AssetLifecycleTests(ApiFixture api) : IClassFixture<ApiFixture>
{
    private static string Unique() => Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

    private static async Task<(string BranchId, string DepartmentId)> ReferenceDataAsync(ApiClient client)
    {
        var branches = (await client.Get("/api/organization/branches")).Body;
        var departments = (await client.Get("/api/organization/departments")).Body;
        var branch = branches.EnumerateArray().First(b => b.GetProperty("assetTag").GetString() == "DVO");
        return (branch.GetProperty("id").GetString()!, departments[0].GetProperty("id").GetString()!);
    }

    private static object NewAsset(string branchId, string serial, string category = "Company") => new
    {
        category,
        branchId,
        description = "Dell Optiplex desktop",
        vendor = "Dell",
        price = 54000,
        lifeSpanMonths = 36,
        warrantyMonths = 12,
        invoiceNumber = $"INV-{Unique()}",
        invoiceDate = "2026-01-10",
        supplier = new { name = "PC Express", address = "Davao City", contactNumber = "0917", email = "sales@pcx.test" },
        items = new[]
        {
            new { typeCode = "M", description = "Dell 24in monitor", partNumber = "P2422H", serialNumber = serial, keyDetail = "N/A", controlNumber = "", remarks = "" },
            new { typeCode = "KYB", description = "Dell keyboard", partNumber = "KB216", serialNumber = "N/A", keyDetail = "N/A", controlNumber = "", remarks = "" },
        },
    };

    private async Task<string> RegisterEmployeeAsync(ApiClient client, string departmentId, string branchId)
    {
        var (status, body) = await client.Post("/api/organization/employees", new
        {
            employeeNumber = $"E{Unique()}", firstName = "Neil", middleName = "S", lastName = "Abarabar",
            email = "neil@example.test", departmentId, branchId,
        });
        Assert.Equal(HttpStatusCode.Created, status);
        return body.GetString()!;
    }

    [Fact]
    public async Task Api_requires_a_signed_in_user_with_a_company()
    {
        api.SkipIfUnavailable();

        Assert.Equal(HttpStatusCode.Unauthorized, (await api.Anonymous.GetAsync("/api/assets", TestContext.Current.CancellationToken)).StatusCode);

        var login = await api.Anonymous.PostAsJsonAsyncCompat("/api/auth/login", new { userName = ApiFixture.AdminUser, password = "wrong" });
        Assert.Equal(HttpStatusCode.Forbidden, login);
    }

    [Fact]
    public async Task Registering_an_asset_generates_numbers_and_rejects_duplicate_serials()
    {
        api.SkipIfUnavailable();
        var client = await api.SignInAsync();
        var (branchId, _) = await ReferenceDataAsync(client);
        var serial = $"SN-{Unique()}";

        var (status, created) = await client.Post("/api/assets", NewAsset(branchId, serial));

        Assert.Equal(HttpStatusCode.Created, status);
        Assert.StartsWith("2CS", created.GetProperty("assetNumber").GetString());
        Assert.Equal(["CVCDVOM-", "CVCDVOKYB-"], created.GetProperty("itemCodes").EnumerateArray().Select(c => c.GetString()![..^3]));

        var (duplicate, problem) = await client.Post("/api/assets", NewAsset(branchId, serial.ToLowerInvariant()));
        Assert.Equal(HttpStatusCode.Conflict, duplicate);
        Assert.Equal("item.serial_in_use", problem.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Issuing_a_consignment_marks_the_items_in_use_and_blocks_a_second_issue()
    {
        api.SkipIfUnavailable();
        var client = await api.SignInAsync();
        var (branchId, departmentId) = await ReferenceDataAsync(client);
        var employeeId = await RegisterEmployeeAsync(client, departmentId, branchId);

        var asset = (await client.Post("/api/assets", NewAsset(branchId, $"SN-{Unique()}"))).Body;
        var assetId = asset.GetProperty("id").GetString()!;
        var available = (await client.Get($"/api/assets/items/lookup?code={asset.GetProperty("assetNumber").GetString()}")).Body;
        var itemIds = available.EnumerateArray().Select(i => i.GetProperty("id").GetString()!).ToArray();
        Assert.Equal(2, itemIds.Length);

        var (status, consignment) = await client.Post("/api/consignments", new { employeeId, issuedOn = "2026-10-01", remarks = "", itemIds });
        Assert.Equal(HttpStatusCode.Created, status);

        // Assets reacts asynchronously to the ConsignmentIssued event.
        await ApiClient.EventuallyAsync(async () =>
        {
            var details = (await client.Get($"/api/assets/{assetId}")).Body;
            return details.GetProperty("items").EnumerateArray().All(i => i.GetProperty("status").GetString() == "InUse");
        }, "both items to become InUse");

        var again = await client.Post("/api/consignments", new { employeeId, issuedOn = "2026-10-01", remarks = "", itemIds });
        Assert.Equal(HttpStatusCode.Conflict, again.Status);

        var details2 = (await client.Get($"/api/consignments/{consignment.GetProperty("id").GetString()}")).Body;
        Assert.Equal("Open", details2.GetProperty("status").GetString());
        Assert.All(details2.GetProperty("lines").EnumerateArray(), l => Assert.Equal("Active", l.GetProperty("status").GetString()));

        var history = (await client.Get($"/api/assets/items/{itemIds[0]}/history")).Body;
        Assert.Single(history.GetProperty("consignments").EnumerateArray());
    }

    [Fact]
    public async Task Reissuing_a_returned_item_supersedes_the_earlier_consignment()
    {
        api.SkipIfUnavailable();
        var client = await api.SignInAsync();
        var (branchId, departmentId) = await ReferenceDataAsync(client);
        var first = await RegisterEmployeeAsync(client, departmentId, branchId);
        var second = await RegisterEmployeeAsync(client, departmentId, branchId);

        var asset = (await client.Post("/api/assets", NewAsset(branchId, $"SN-{Unique()}", "Salesman"))).Body;
        var assetId = asset.GetProperty("id").GetString()!;
        var item = (await client.Get($"/api/assets/items/lookup?code={asset.GetProperty("assetNumber").GetString()}")).Body[0];
        var itemId = item.GetProperty("id").GetString()!;

        var one = (await client.Post("/api/consignments", new { employeeId = first, issuedOn = "2026-09-01", remarks = "", itemIds = new[] { itemId } })).Body;
        await ApiClient.EventuallyAsync(async () => (await ItemStatus(client, itemId)) == "InUse", "item to be reserved");

        // Item comes back: mark it defective, repair it, and it is available again.
        Assert.Equal(HttpStatusCode.NoContent, (await client.Put($"/api/assets/{assetId}/items/{itemId}/status", new { status = "Defective" })).Status);
        Assert.Equal(HttpStatusCode.NoContent, (await client.Put($"/api/assets/{assetId}/items/{itemId}/status", new { status = "Available" })).Status);

        var two = (await client.Post("/api/consignments", new { employeeId = second, issuedOn = "2026-10-01", remarks = "", itemIds = new[] { itemId } })).Body;

        await ApiClient.EventuallyAsync(async () =>
        {
            var older = (await client.Get($"/api/consignments/{one.GetProperty("id").GetString()}")).Body;
            return older.GetProperty("lines")[0].GetProperty("status").GetString() == "Superseded";
        }, "the first consignment's line to be superseded");

        var newer = (await client.Get($"/api/consignments/{two.GetProperty("id").GetString()}")).Body;
        Assert.Equal("Active", newer.GetProperty("lines")[0].GetProperty("status").GetString());
    }

    [Fact]
    public async Task Item_status_rules_are_enforced()
    {
        api.SkipIfUnavailable();
        var client = await api.SignInAsync();
        var (branchId, _) = await ReferenceDataAsync(client);
        var asset = (await client.Post("/api/assets", NewAsset(branchId, $"SN-{Unique()}"))).Body;
        var assetId = asset.GetProperty("id").GetString()!;
        var itemId = (await client.Get($"/api/assets/{assetId}")).Body.GetProperty("items")[0].GetProperty("id").GetString()!;

        var (status, problem) = await client.Put($"/api/assets/{assetId}/items/{itemId}/status", new { status = "InUse" });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, status);
        Assert.Equal("Item.use_consignment", problem.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Validation_errors_come_back_as_problem_details()
    {
        api.SkipIfUnavailable();
        var client = await api.SignInAsync();

        var (status, problem) = await client.Post("/api/assets", new
        {
            category = "Company", branchId = Guid.NewGuid(), description = "", vendor = "", price = -1, lifeSpanMonths = 1,
            warrantyMonths = 1, invoiceNumber = "", invoiceDate = "2026-01-01",
            supplier = new { name = "", address = "", email = "nope" }, items = Array.Empty<object>(),
        });

        Assert.Equal(HttpStatusCode.BadRequest, status);
        Assert.True(problem.GetProperty("errors").EnumerateObject().Any());
    }

    private static async Task<string> ItemStatus(ApiClient client, string itemId) =>
        (await client.Get($"/api/assets/items/{itemId}")).Body.GetProperty("status").GetString()!;
}

internal static class HttpClientExtensions
{
    public static async Task<HttpStatusCode> PostAsJsonAsyncCompat(this HttpClient http, string url, object body)
    {
        using var response = await http.PostAsync(url, JsonContent.Create(body));
        return response.StatusCode;
    }
}
