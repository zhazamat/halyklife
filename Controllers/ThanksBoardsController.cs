using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hbk.Data;
using hbk.Models;

using Azure.Identity;
using hbk.Models.Requests.ThanksBoard;

namespace hbk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanksBoardsController : ControllerBase
    {
        private readonly HbkApiDbContext _context;

        public ThanksBoardsController(HbkApiDbContext context)
        {
            _context = context;
        }

        // GET: api/ThanksBoards
        [HttpGet("/get_all")]
        public async Task<ActionResult<IEnumerable<ThanksBoard>>> GetThanksBoards()
        {
          
                return await _context.ThanksBoards.ToArrayAsync();
                    
        }

        // PUT: api/ThanksBoards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("/update/{id}")]
        public async Task<IActionResult> PutThanksBoard(int id, ThanksBoard thanksBoard)
        {
            if (id != thanksBoard.Id)
            {
                return BadRequest();
            }

            _context.Entry(thanksBoard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThanksBoardExists(id))
                {
                    return NotFound("Not Found this id"+id);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpGet("/notification")]
        public async Task<ActionResult<IEnumerable<ThanksBoard>>> GetNotification()
        {
            var employee = await (from e in _context.Employees
                                  where e.StartDate.Month == DateTime.Now.Month && e.StartDate.Day == DateTime.Now.Day && e.StartDate.Year > e.StartDate.Year
                                  select e.Id)
                                 .ToListAsync();

            return Ok(employee);

        }

        // POST: api/ThanksBoards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("/send_message")]
        public async Task<ActionResult<ThanksBoard>> AddEmployee(SendMessageRequest sendMessageRequest)
        {
            if (GetNotification != null)
            { 
                ThanksBoard thanksBoard = new ThanksBoard()
                {
                    Message = sendMessageRequest.Message,
                    SenderId = sendMessageRequest.SenderId,
                    ReceiverId = sendMessageRequest.ReceiverId,
                    DateReceived = sendMessageRequest.SendTime

                };

                await _context.ThanksBoards.AddAsync(thanksBoard);
                await _context.SaveChangesAsync();

                return Ok(thanksBoard);
            }
            return NotFound();

        }

        // DELETE: api/ThanksBoards/5
        [HttpDelete("/delete/{id}")]
        public async Task<IActionResult> DeleteThanksBoard(int id)
        {
            var thanksBoard = await _context.ThanksBoards.FindAsync(id);
            if (thanksBoard == null)
            {
                return NotFound("Сообщение не найдено");
            }

            _context.ThanksBoards.Remove(thanksBoard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ThanksBoardExists(int id)
        {
            return _context.ThanksBoards.Any(e => e.Id == id);
        }

        [HttpGet("/count_receiver_messsages")]
        public async Task<ActionResult<int>> CountReceiverMesseges(int Id)
        {
            int count = await (from q1 in _context.Employees
                               join q2 in _context.ThanksBoards on q1.Id equals q2.SenderId
                               where q2.ReceiverId == Id
                               select q1).CountAsync();
            return Ok(count);
        }

        [HttpGet("/rating")]
        public async Task<IActionResult> getRating()
        {
            //  var employees = await (from e in _context.Employees select e);
            var employee = await _context.Employees.ToListAsync();
            var thanksBoard = await _context.ThanksBoards.ToListAsync();
            
            
                
                    int count = await (from q1 in _context.Employees
                                       join q2 in _context.ThanksBoards on q1.Id equals q2.SenderId
                                      // where q2.ReceiverId == e.Id
                                       select q1).CountAsync();

                    return Ok(count);
                    
                }
            }



              

         //   return BadRequest();
            
           
        }
    

