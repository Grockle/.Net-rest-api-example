namespace BackendService.Data.DTOs.User.Response
{
    public class UserInfoDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
        public int UserId { get; set; }
    }
}