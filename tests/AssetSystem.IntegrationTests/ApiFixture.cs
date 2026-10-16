using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;

namespace AssetSystem.IntegrationTests;

/// <summary>
/// Boots the real application against a throw-away PostgreSQL database (one per test class).
/// Point TEST_POSTGRES at a server whose user may create databases; tests are skipped when it is unreachable.
/// </summary>
public sealed class ApiFixture : IAsyncLifetime
{
    public const string AdminUser = "administrator";
    public const string AdminPassword = "Admin#12345";

    private static readonly string ServerConnection =
        Environment.GetEnvironmentVariable("TEST_POSTGRES")
        ?? "Host=localhost;Username=assetsystem;Password=assetsystem;Database=postgres";

    private readonly string _database = $"it_{Guid.NewGuid():N}";
    private WebApplicationFactory<Program>? _factory;

    public bool Available { get; private set; }
    public HttpClient Anonymous { get; private set; } = default!;

    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async ValueTask InitializeAsync()
    {
        try
        {
            await using var connection = new NpgsqlConnection(ServerConnection);
            await connection.OpenAsync();
            await using var create = new NpgsqlCommand($"CREATE DATABASE \"{_database}\"", connection);
            await create.ExecuteNonQueryAsync();
        }
        catch (Exception)
        {
            return; // no database server: tests will skip themselves
        }

        var databaseConnection = new NpgsqlConnectionStringBuilder(ServerConnection) { Database = _database }.ConnectionString;
        // Program.cs reads configuration while the host is being built, before a test host can override it,
        // so settings go in through environment variables (hence: tests do not run in parallel).
        var settings = new Dictionary<string, string>
        {
            ["ConnectionStrings__Postgres"] = databaseConnection,
            ["Jwt__SigningKey"] = "integration-tests-signing-key-0123456789abcdef",
            ["Seeding__SampleData"] = "true",
            ["Seeding__AdminUserName"] = AdminUser,
            ["Seeding__AdminPassword"] = AdminPassword,
            ["FileStore__RootPath"] = Path.Combine(Path.GetTempPath(), $"assetsystem-it-{_database}"),
        };
        foreach (var (key, value) in settings) Environment.SetEnvironmentVariable(key, value);

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder.UseEnvironment("Testing"));
        Anonymous = _factory.CreateClient();
        Available = true;
    }

    public void SkipIfUnavailable() =>
        Assert.SkipUnless(Available, "No PostgreSQL server reachable. Set TEST_POSTGRES to run integration tests.");

    /// <summary>A client signed in as the seeded administrator, working in the seeded company.</summary>
    public async Task<ApiClient> SignInAsync()
    {
        var login = await Anonymous.PostAsJsonAsync("/api/auth/login", new { userName = AdminUser, password = AdminPassword });
        login.EnsureSuccessStatusCode();
        var session = await login.Content.ReadFromJsonAsync<JsonElement>(Json);
        var companyId = session.GetProperty("companies")[0].GetProperty("id").GetString()!;

        var client = _factory!.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.GetProperty("token").GetString());
        var selected = await client.PostAsJsonAsync("/api/auth/select-company", new { companyId });
        selected.EnsureSuccessStatusCode();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", (await selected.Content.ReadFromJsonAsync<JsonElement>(Json)).GetProperty("token").GetString());
        return new ApiClient(client);
    }

    public async ValueTask DisposeAsync()
    {
        _factory?.Dispose();
        if (!Available) return;

        await using var connection = new NpgsqlConnection(ServerConnection);
        await connection.OpenAsync();
        NpgsqlConnection.ClearAllPools();
        await using var drop = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{_database}\" WITH (FORCE)", connection);
        await drop.ExecuteNonQueryAsync();
    }
}

/// <summary>Small JSON helpers so the tests read like the story they tell.</summary>
public sealed class ApiClient(HttpClient http)
{
    public HttpClient Http { get; } = http;

    public async Task<(HttpStatusCode Status, JsonElement Body)> SendAsync(HttpMethod method, string url, object? body = null)
    {
        using var request = new HttpRequestMessage(method, url);
        if (body is not null) request.Content = JsonContent.Create(body);
        using var response = await Http.SendAsync(request);
        var text = await response.Content.ReadAsStringAsync();
        var json = string.IsNullOrEmpty(text) ? default : JsonSerializer.Deserialize<JsonElement>(text, ApiFixture.Json);
        return (response.StatusCode, json);
    }

    public Task<(HttpStatusCode Status, JsonElement Body)> Get(string url) => SendAsync(HttpMethod.Get, url);
    public Task<(HttpStatusCode Status, JsonElement Body)> Post(string url, object body) => SendAsync(HttpMethod.Post, url, body);
    public Task<(HttpStatusCode Status, JsonElement Body)> Put(string url, object body) => SendAsync(HttpMethod.Put, url, body);

    /// <summary>Messages between modules are delivered asynchronously; poll until the expected state shows up.</summary>
    public static async Task EventuallyAsync(Func<Task<bool>> condition, string because, int seconds = 15)
    {
        var deadline = DateTime.UtcNow.AddSeconds(seconds);
        while (DateTime.UtcNow < deadline)
        {
            if (await condition()) return;
            await Task.Delay(200);
        }
        Assert.Fail($"Timed out waiting for: {because}");
    }
}
