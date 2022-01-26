namespace TicketService.Events
{
    public class TicketCreatedEvent
    {
        public int TicketId { get; set; }
        public string MovieTitle { get; set; }
        public string MoviePicUrl { get; set; }
        public int Seat { get; set; }
        public int Room { get; set; }
        public string Date { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
    }
}
