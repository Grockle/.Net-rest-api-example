namespace BackendService.Application.Models.Responses.Transaction
{
    public class GroupExpenseResponseItem
    {
        public string Category { get; set; }
        public double CategoryTotal { get; set; }
        public double Percentage { get; set; }
    }
}