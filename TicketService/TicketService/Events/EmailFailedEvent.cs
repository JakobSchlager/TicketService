using MassTransit;
using System;
using System.Threading.Tasks;

namespace NotificationService.Events
{
    public class EmailFailedEvent
    {
        public int TicketId { get; set; }
    }

    public class EmailFailedEventConsumer : IConsumer<EmailFailedEvent>
    {
        private readonly TicketService.Services.TicketService _ticketService; 
        public EmailFailedEventConsumer(TicketService.Services.TicketService ticketService)
        {
            this._ticketService = ticketService;
        }

        public async Task Consume(ConsumeContext<EmailFailedEvent> context)
        {
            Console.WriteLine("EmailFailedEvent Reveivid."); 
            _ticketService.DeleteTicket(context.Message.TicketId); 
            Console.WriteLine("Ticket Deleted (async)."); 
        }
    }
}
