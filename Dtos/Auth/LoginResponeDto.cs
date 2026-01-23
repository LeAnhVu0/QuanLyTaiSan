namespace QuanLyTaiSan.Dtos.Auth
{
    public class LoginResponeDto
    {
        public string? Username { get; set; }
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
        public string Message { get; set; }
    }
}
