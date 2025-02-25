namespace GameStore.Api.Models
{

    //Это DTO (Data Transfer Object) для входа пользователя. Он содержит данные, которые клиент отправляет при попытке авторизоваться, например, имя пользователя и пароль
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}


