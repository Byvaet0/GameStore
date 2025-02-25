using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using GameStore.Api.Configurations;
using GameStore.Api.Entities;

namespace GameStore.Api.Services
{

    //AuthService: Сервис, который генерирует JWT для пользователей после успешной аутентификации.
    public class AuthService
    {
        private readonly JwtSettings _jwtSettings;

        public AuthService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateJwtToken(User user)  //GenerateJwtToken: Метод, который генерирует JWT токен для пользователя
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));  //SymmetricSecurityKey: Создаём ключ для подписи токена с использованием секретного ключа.
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);  //SigningCredentials: Определяет, как токен будет подписан. В данном случае используется алгоритм HMAC SHA256.

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),   //Claims: Это утверждения (заявления), которые будут содержаться в токене. 
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  //В примере это имя пользователя (ClaimTypes.Name) и роль пользователя (ClaimTypes.Role).
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name) 
            };

            var token = new JwtSecurityToken(    //JwtSecurityToken: Создаём сам токен, указывая его издателя, аудиторию, утверждения и время истечения.
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: credentials  //credentials — это объект типа SigningCredentials, который определяет, какой ключ и какой алгоритм подписи будет использоваться для подписи JWT.
            );

            return new JwtSecurityTokenHandler().WriteToken(token);   //WriteToken: Преобразует объект JwtSecurityToken в строку (сам токен).
        }
    }
}
