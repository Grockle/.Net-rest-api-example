namespace BackendService.Data.DTOs.Transaction.Response
{
    public class ExpenseGroupInfoDto
    {
        public string Category { get; set; }
        public double CategoryTotal { get; set; }
        public double Percentage { get; set; }
    }
}