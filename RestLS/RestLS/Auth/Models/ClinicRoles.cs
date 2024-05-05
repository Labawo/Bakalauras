namespace RestLS.Auth.Models;

public static class ClinicRoles
{
    public const string Admin = nameof(Admin);
    public const string Doctor = nameof(Doctor);
    public const string Patient = nameof(Patient);

    public static readonly IReadOnlyCollection<string> All = new[] { Admin, Doctor, Patient };
}