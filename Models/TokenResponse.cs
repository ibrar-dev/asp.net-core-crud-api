namespace CustomerAPI.Models
{
    public class TokenResponse
    {
        public int Id { get; set; }
        public string JWTToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
