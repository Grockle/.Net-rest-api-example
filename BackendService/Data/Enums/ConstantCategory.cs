using System.Collections.Generic;
using BackendService.Data.DTOs.Personal;

namespace BackendService.Data.Enums
{
    public class ConstantCategory
    {
        public static List<string> ExpenseCategory = new List<string>{ "Yiyecek", "Gezi", "Fatura" };
        public static List<string> IncomeCategory = new List<string>{ "Maaş", "Yatırım", "Kira" };
        public static List<string> AccountCategory = new List<string>{ "Banka Hesabı", "Kredi Kartı", "Nakit" };
        public static List<string> GroupExpenseCategory = new List<string>{ "Yiyecek", "Gezi", "Fatura", "Kira" };
    }
}