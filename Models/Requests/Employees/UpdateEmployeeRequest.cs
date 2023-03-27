namespace hbk.Models.Requests.Employees
{
    public class UpdateEmployeeRequest
    {


        public string PersonnelNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public string PositionTitle { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public string Company { get; set; }  = string.Empty;
        public string DirectPhone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string WhatsAppMobile { get; set; } = string.Empty;

      
        public string OfficeNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string linkImg { get; set; } = string.Empty;
        public bool IsLocal { get; set; }
    }
}
