namespace BackendService.Application.Models.Responses.Group
{
    public class GetGroupJoinResponse
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}