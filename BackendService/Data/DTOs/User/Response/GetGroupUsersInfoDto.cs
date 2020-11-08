namespace BackendService.Data.DTOs.User.Response
{
    public class GetGroupUsersInfoDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}