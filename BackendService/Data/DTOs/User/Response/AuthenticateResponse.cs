using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackendService.Data.DTOs.User.Response
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
        public string JWToken { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}