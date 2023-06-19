namespace hbk.Models.Requests.Employees
{
    public class AddEmployeeRequest
    {
        public int Id { get; set; }
        public string PersonnelNumber { get; set; } 
        public string FullName { get; set; }

        public string PositionTitle { get; set; } 
        public string Department { get; set; } 
        public string Phone { get; set; } 
        public string Company { get; set; } 
        public string DirectPhone { get; set; }
        public string Mobile { get; set; } 
        public string WhatsAppMobile { get; set; } 
        public string OfficeNumber { get; set; } 
        public string Email { get; set; } 
        public string Status { get; set; } 
        public string linkImg { get; set; }
        public bool IsLocal { get; set; }

    }
}
