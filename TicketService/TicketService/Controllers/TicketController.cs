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

        public TicketController(Services.TicketService ticketService)
        {
            this._ticketService = ticketService;
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
            return Ok(await _ticketService.AddTicket(ticketDto));
        }

        [HttpPut("{id}")]
        public ActionResult<TicketDto> Put(int id, [FromBody] TicketDto ticketDto)
        {
            return Ok(_ticketService.UpdateTicket(id, ticketDto)); 
        }
    }
}
