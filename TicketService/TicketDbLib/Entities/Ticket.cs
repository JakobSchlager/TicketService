using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketDbLib.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public int PresentationId { get; set; }
        public int SeatId { get; set; }
        public string CustomerFirstname { get; set; }
        public string CustomerLastname { get; set; }
        public string CustomerEmail { get; set; }
    }

    public class TicketEntityTypeConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("ticket");

            builder.HasData(new Ticket
            {
                Id = 1,
                PresentationId = 1,
                CustomerFirstname = "Jakob",
                CustomerLastname = "Schlager",
                CustomerEmail = "jakob.sschlager@gmail.com", 
                SeatId = 15,
            });
            builder.HasData(new Ticket
            {
                Id = 2,
                PresentationId = 1,
                CustomerFirstname = "Thomas",
                CustomerLastname = "Wahlmüller",
                CustomerEmail = "Thomas.Wahlmüller@gmail.com", 
                SeatId = 18,
            });
            builder.HasData(new Ticket
            {
                Id = 3,
                PresentationId = 2,
                CustomerFirstname = "Florian",
                CustomerLastname = "Auer",
                CustomerEmail = "Florian.Auer@gmail.com", 
                SeatId = 30,
            });
            builder.HasData(new Ticket
            {
                Id = 4,
                PresentationId = 2,
                CustomerFirstname = "Fabian",
                CustomerLastname = "Graml",
                CustomerEmail = "Fabian.Graml@gmail.com", 
                SeatId = 31,
            });
        }
    }
}
