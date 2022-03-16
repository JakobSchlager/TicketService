using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketDbLib;
using TicketDbLib.Entities;
using TicketService.DTOs;
using MassTransit;
using TicketService.Events;

namespace TicketService.Services
{
    public class TicketService
    {
        private readonly TicketDbContext _ticketDbContext;
        private readonly IBus _bus;
        private readonly RoomService _roomService;
        private readonly MovieService _movieService;

        public TicketService(TicketDbContext ticketDbContext, RoomService roomService, MovieService movieService, IBus bus)
        {
            this._ticketDbContext = ticketDbContext;
            this._bus = bus;
            this._roomService = roomService;
            this._movieService = movieService;
        }

        public List<TicketDto> GetAll()
        {
            return _ticketDbContext.Tickets.Select(x => new TicketDto
            {
                Id = x.Id,
                SeatId = x.SeatId,
                PresentationId = x.PresentationId,
                CustomerFirstname = x.CustomerFirstname,
                CustomerLastname = x.CustomerLastname,
                CustomerEmail = x.CustomerEmail,
            })
            .ToList();
        }

        public List<int> GetAllReservedSeats(int id)
        {
            return _ticketDbContext.Tickets
                .Where(x => x.PresentationId == id)
                .Select(x => x.SeatId)
                .ToList();
        }

        public async Task<TicketDto> AddTicket(TicketDto ticketDto)
        {
            if (!IsSeatAvailable(ticketDto.PresentationId, ticketDto.SeatId)) throw new Exception("Seat is already taken!");

            var ticket = _ticketDbContext.Tickets.Add(new Ticket
            {
                Id = 0,
                SeatId = ticketDto.SeatId,
                PresentationId = ticketDto.PresentationId,
                CustomerFirstname = ticketDto.CustomerFirstname,
                CustomerLastname = ticketDto.CustomerLastname,
                CustomerEmail = ticketDto.CustomerEmail,
            }).Entity;

            _ticketDbContext.SaveChanges();

            Console.WriteLine("Saved Changes");

            var presentationDto = await _movieService.GetPresentation(ticket.PresentationId);
            Console.WriteLine("Got PresentationDto");
            var movieDto = await _movieService.GetMovie(presentationDto.MovieId);
            Console.WriteLine("Got movieDto");
            var seatDto = await _roomService.GetSeat(ticket.SeatId);
            Console.WriteLine("Got seatDto");

            await _bus.Publish(new TicketCreatedEvent
            {
                Firstname = ticketDto.CustomerFirstname,
                Lastname = ticketDto.CustomerLastname,
                Email = ticketDto.CustomerEmail,
                Address = "Kino Addresse",
                TicketId = ticket.Id,
                Date = presentationDto.StartTime,
                MoviePicUrl = movieDto.Image,
                MovieTitle = movieDto.Title,
                Room = seatDto.RoomId,
                Seat = seatDto.Id,
            });
            
            Console.WriteLine("Sent out event");
            
            return new TicketDto
            {
                Id = ticket.Id,
                SeatId = ticket.SeatId,
                PresentationId = ticket.PresentationId,
                CustomerFirstname = ticket.CustomerFirstname,
            };
        }

        public TicketDto UpdateTicket(int id, TicketDto ticketDto)
        {
            if (!IsSeatAvailable(ticketDto.PresentationId, ticketDto.SeatId)) throw new Exception("Seat is already taken!");

            var ticketToUpdate = _ticketDbContext.Tickets.Find(id);

            ticketToUpdate.SeatId = ticketDto.SeatId;
            ticketToUpdate.PresentationId = ticketDto.PresentationId;
            ticketToUpdate.CustomerFirstname = ticketDto.CustomerFirstname;
            ticketToUpdate.CustomerLastname = ticketDto.CustomerLastname;
            ticketToUpdate.CustomerEmail = ticketDto.CustomerEmail;

            _ticketDbContext.SaveChanges();

            return new TicketDto
            {
                Id = ticketToUpdate.Id,
                SeatId = ticketToUpdate.SeatId,
                PresentationId = ticketToUpdate.PresentationId,
                CustomerFirstname = ticketDto.CustomerFirstname,
                CustomerLastname = ticketDto.CustomerLastname,
                CustomerEmail = ticketDto.CustomerEmail,
            };
        }
        public TicketDto DeleteTicket(int ticketId)
        {
            var ticket = _ticketDbContext.Tickets.FirstOrDefault(x => x.Id == ticketId);
            if (ticket == null)
            {
                Console.WriteLine($"TicketService::DelteTicket, Ticket Cant be deleted: {ticketId} does not exist"); 
                return null;
            }

            _ticketDbContext.Tickets.Remove(ticket);
            _ticketDbContext.SaveChanges(); 

            return new TicketDto
            {
                Id = ticket.Id,
                SeatId = ticket.SeatId,
                PresentationId = ticket.PresentationId,
                CustomerFirstname = ticket.CustomerFirstname,
                CustomerLastname = ticket.CustomerLastname,
                CustomerEmail = ticket.CustomerEmail,
            };
        }
        private bool IsSeatAvailable(int presentationId, int seatId) =>
            _ticketDbContext.Tickets.All(x => !(x.PresentationId == presentationId && x.SeatId == seatId));
    }
}
