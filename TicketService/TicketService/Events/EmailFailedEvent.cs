using MassTransit;
using System.Threading.Tasks;

namespace TicketService.Events
{
    public class EmailFailedEvent
    {
        public int TicketId { get; set; }
    }

    public class EmailFailedEventConsumer : IConsumer<EmailFailedEvent>
    {
        private readonly Services.TicketService _ticketService; 
        public EmailFailedEventConsumer(Services.TicketService ticketService)
        {
            this._ticketService = ticketService;
        }

        public async Task Consume(ConsumeContext<EmailFailedEvent> context)
        {
            _ticketService.DeleteTicket(context.Message.TicketId); 
        }
    }
}
