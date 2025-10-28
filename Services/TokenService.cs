using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using FirstAPI.Model;
using Microsoft.IdentityModel.Tokens;

namespace FirstAPI.Services
{
    public class TokenService
    {
        public static object GenerateToken(Carro carro)
        {
            var key = Encoding.ASCII.GetBytes(Key.Secret);
            var tokenConfig = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim("carroId", carro.id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenConfig);
            var tokenString = tokenHandler.WriteToken(token);

            return new
            {
                token = tokenString;
        }
    }
}
