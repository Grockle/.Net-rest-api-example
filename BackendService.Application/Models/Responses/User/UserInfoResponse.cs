namespace BackendService.Application.Models.Responses.User
{
    public class UserInfoResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
        public int UserId { get; set; }
    }
}