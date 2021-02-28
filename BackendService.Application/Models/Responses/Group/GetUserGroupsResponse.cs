namespace BackendService.Application.Models.Responses.Group
{
    public class GetUserGroupsResponse
    {
        public int GroupId { get; set; }
        public string GroupShareCode { get; set; }
        public string GroupName { get; set; }
        public int AdminId { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public double Budget { get; set; }
    }
}