namespace Identity.Settings;

public class JwtSettings
{
    public string JWT_Secret { get; set; } = string.Empty;
    public string JWT_ISSUER_TOKEN { get; set; } = string.Empty;
    public string JWT_AUDIENCE_TOKEN { get; set; } = string.Empty;
    public double ExpiryInMinutes { get; set; }
}