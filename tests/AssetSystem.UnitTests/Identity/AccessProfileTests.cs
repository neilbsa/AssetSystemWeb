using AssetSystem.Identity.Domain.Access;
using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.UnitTests.Identity;

public class AccessProfileTests
{
    [Fact]
    public void Administrators_do_not_need_assigned_companies()
    {
        var profile = AccessProfile.Create(["administrator"], []);

        Assert.True(profile.IsAdministrator);
        Assert.Equal(["Administrator"], profile.Roles); // normalised to the canonical spelling
    }

    [Fact]
    public void Regular_users_need_at_least_one_company()
    {
        var ex = Assert.Throws<DomainException>(() => AccessProfile.Create(["Common"], []));

        Assert.Equal("access.company_required", ex.Code);
    }

    [Fact]
    public void At_least_one_role_is_required() =>
        Assert.Throws<DomainException>(() => AccessProfile.Create([], [Guid.NewGuid()]));

    [Fact]
    public void Unknown_roles_are_rejected() =>
        Assert.Throws<DomainException>(() => AccessProfile.Create(["SuperUser"], [Guid.NewGuid()]));

    [Fact]
    public void Profiles_with_the_same_roles_and_companies_are_equal()
    {
        var company = Guid.NewGuid();

        Assert.Equal(AccessProfile.Create(["Common"], [company]), AccessProfile.Create(["common"], [company, company]));
    }
}
