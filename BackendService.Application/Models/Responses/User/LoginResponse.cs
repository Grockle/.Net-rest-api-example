namespace BackendService.Application.Models.Responses.User
{
    public class LoginResponse
    {
        public bool IsLoggedIn { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}