using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketService.Events;
using TicketService.DTOs;

namespace TicketService.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly Services.TicketService _ticketService;
        private readonly IBus _bus; 

        public TicketController(Services.TicketService ticketService, IBus bus)
        {
            this._ticketService = ticketService;
            this._bus = bus;
        }

        [HttpGet]
        public ActionResult<List<TicketDto>> GetAll()
        {
            return Ok(_ticketService.GetAll());
        }
        
        [HttpGet]
        [Route("presentation/{id}")]
        public ActionResult<List<TicketDto>> GetSeats(int id)
        {
            return Ok(_ticketService.GetAllReservedSeats(id));
        }

        [HttpPost]
        public async Task<ActionResult<TicketDto>> Post([FromBody] TicketDto ticketDto)
        {
            await _bus.Publish(new TicketCreatedEvent
            {
                Firstname = ticketDto.CustomerName,
                Lastname = "Schlager",
                Address = "Kino Addresse",
                TicketId = 231223, 
                Date = "20.01.2022", 
                MoviePicUrl = "https://image.tmdb.org/t/p/w500/rjkmN1dniUHVYAtwuV3Tji7FsDO.jpg",
                MovieTitle = "MovieTitle", 
                Room = 2, 
                Seat = 23, 
            });
            Console.WriteLine("TicketCreated sent!"); 
            return Ok(_ticketService.AddTicket(ticketDto));
        }

        [HttpPut("{id}")]
        public ActionResult<TicketDto> Put(int id, [FromBody] TicketDto ticketDto)
        {
            return Ok(_ticketService.UpdateTicket(id, ticketDto)); 
        }
    }
}
