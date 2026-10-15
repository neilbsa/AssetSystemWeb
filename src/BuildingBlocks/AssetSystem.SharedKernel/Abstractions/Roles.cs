namespace AssetSystem.SharedKernel.Abstractions;

public static class Roles
{
    public const string Administrator = "Administrator";
    public const string Common = "Common";
}

public static class AppClaims
{
    /// <summary>Claim naming the company the user is currently working in.</summary>
    public const string CompanyId = "company_id";
}
