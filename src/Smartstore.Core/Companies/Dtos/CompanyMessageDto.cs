namespace Smartstore.Core.Companies.Dtos
{
    public class CompanyMessageDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Data => Message;
        public int? CompanyGuestCustomerId { get; set; }
        public int? CompanyCustomerId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool Sent { get; set; }

        public string IconUrl => "https://placeimg.com/30/30/face";
        //public int TicketId { get; set; }
    }
}
