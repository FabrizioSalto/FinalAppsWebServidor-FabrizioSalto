using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationREST.Data;

namespace WebApplicationREST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly WebApplicationRESTContext _context;

        public ClientsController(WebApplicationRESTContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("HelloWorld")]
        public ActionResult<string> HelloWorld()
        {
            return Ok("Hello World");
        }

        [HttpGet]
        [Route("ReadAllClients")]
        public ActionResult<IEnumerable<Client>> ReadAllClients()
        {
            var clients = _context.Clients.ToList();
            return Ok(clients);
        }

        [HttpGet]
        [Route("ReadClient/{id}")]
        public ActionResult<Client> ReadClient(int id)
        {
            var client = _context.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        [HttpPost]
        [Route("CreateClient")]
        public ActionResult<Client> CreateClient([FromBody] Client client)
        {
            try
            {
                if (client == null)
                {
                    return BadRequest("Invalid data.");
                }

                _context.Clients.Add(client);
                _context.SaveChanges();

                return CreatedAtAction(nameof(ReadClient), new { id = client.Id }, client);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error saving client");
            }
        }

        [HttpPut]
        [Route("UpdateClient/{id}")]
        public IActionResult UpdateClient(int id, [FromBody] Client updatedClient)
        {
            if (updatedClient == null)
            {
                return BadRequest("Invalid data.");
            }

            var client = _context.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            client.Name = updatedClient.Name;
            client.Age = updatedClient.Age;
            client.Address = updatedClient.Address;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete]
        [Route("DeleteClient/{id}")]
        public IActionResult DeleteClient(int id)
        {
            var client = _context.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
