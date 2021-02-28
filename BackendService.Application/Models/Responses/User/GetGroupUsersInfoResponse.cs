namespace BackendService.Application.Models.Responses.User
{
    public class GetGroupUsersInfoResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}