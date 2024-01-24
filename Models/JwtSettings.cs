namespace FirstApi.Models;
public class JwtSettings
{
    public string? Secret { get; set; }
    public int ExpiresHour { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
}
