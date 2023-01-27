using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Office_Management_.NET_MVC_Angular_JWT.Utils;
class JWTTokenUtil : ITokenUtil
{
    private readonly IConfiguration _configuration;

    public JWTTokenUtil(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    string ITokenUtil.GenerateJwtToken(int id, string username, List<string>? roles)
    {
        var claims = new List<Claim>
        {
            // new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Sid, id.ToString()),
        };

        roles?.ForEach(role =>
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        });

        var jwtKey = _configuration["JwtKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey == null ? "" : jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

        var token = new JwtSecurityToken(
            _configuration["JwtIssuer"],
            _configuration["JwtAudience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    int? ITokenUtil.ValidateToken(string? token)
    {
        if (token == null)
        {
            return null;
        }

        var jwtKey = _configuration["JwtKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey == null ? "" : jwtKey));
        try
        {
            new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.Sid).Value);
            // var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;

            // return user id from JWT token if validation successful
            return userId;
        }
        catch (Exception e)
        {
            // return null if validation fails
            Console.WriteLine(e.Message);
            return null;
        }
    }

    

}



