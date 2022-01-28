using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketDbLib;
using TicketDbLib.Entities;
using TicketService.DTOs;
using MassTransit; 

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
                CustomerName = x.CustomerName,
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
                CustomerName = ticketDto.CustomerName,
            }).Entity;

            _ticketDbContext.SaveChanges();

            var presentationDto = await _movieService.GetPresentation(ticket.PresentationId);
            var movieDto = await _movieService.GetMovie(presentationDto.MovieId);
            var seatDto = await _roomService.GetSeat(ticket.SeatId); 

            await _bus.Publish(new TicketCreatedEvent
            {
                Firstname = ticketDto.CustomerName,
                Lastname = "Schlager",
                Address = "Kino Addresse",
                TicketId = ticketDto.Id,
                Date = presentationDto.StartTime, 
                MoviePicUrl = movieDto.Image,
                MovieTitle = movieDto.Title, 
                Room = seatDto.RoomId,
                Seat = seatDto.Id,
            });

            return new TicketDto
            {
                Id = ticket.Id,
                SeatId = ticket.SeatId,
                PresentationId = ticket.PresentationId,
                CustomerName = ticket.CustomerName,
            };
        }

        public TicketDto UpdateTicket(int id, TicketDto ticketDto)
        {
            if (!IsSeatAvailable(ticketDto.PresentationId, ticketDto.SeatId)) throw new Exception("Seat is already taken!");

            var ticketToUpdate = _ticketDbContext.Tickets.Find(id);

            ticketToUpdate.SeatId = ticketDto.SeatId;
            ticketToUpdate.PresentationId = ticketDto.PresentationId;
            ticketToUpdate.CustomerName = ticketDto.CustomerName;

            _ticketDbContext.SaveChanges();

            return new TicketDto
            {
                Id = ticketToUpdate.Id,
                SeatId = ticketToUpdate.SeatId,
                PresentationId = ticketToUpdate.PresentationId,
                CustomerName = ticketToUpdate.CustomerName,
            };
        }

        private bool IsSeatAvailable(int presentationId, int seatId) =>
            _ticketDbContext.Tickets.All(x => !(x.PresentationId == presentationId && x.SeatId == seatId));
    }
}
