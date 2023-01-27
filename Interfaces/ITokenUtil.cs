public interface ITokenUtil
{
    public string GenerateJwtToken( int id, string username, List<string>? roles = null);
    public int? ValidateToken(string? token);
}