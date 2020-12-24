namespace BackendService.Data.DTOs.Group.Response
{
    public class GetUserGroupsDto
    {
        public int GroupId { get; set; }
        public string GroupShareCode { get; set; }
        public string GroupName { get; set; }
        public int AdminId { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
    }
}