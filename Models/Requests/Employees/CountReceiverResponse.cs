namespace hbk.Models.Requests.Employees
{
    public class CountReceiverResponse
    {
        public int Id { get; set; }
        public string EmployeeImg { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        public int Count { get; set; }
    }
}
