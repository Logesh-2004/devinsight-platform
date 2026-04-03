namespace DevInsightAPI.Constants
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Developer = "Developer";

        public static bool IsValid(string? role)
        {
            return string.Equals(role, Admin, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(role, Developer, StringComparison.OrdinalIgnoreCase);
        }

        public static string Normalize(string? role)
        {
            if (string.Equals(role, Admin, StringComparison.OrdinalIgnoreCase))
            {
                return Admin;
            }

            return Developer;
        }
    }
}
