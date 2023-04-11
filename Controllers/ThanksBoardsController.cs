using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hbk.Data;
using hbk.Models;
using hbk.Models.Requests.ThanksBoard;
using Microsoft.AspNetCore.Http;

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
                    return NotFound("Not Found this id" + id);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ThanksBoards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("/sending_message")]
        public async Task<ActionResult<ThanksBoard>> AddEmployee(SendMessageRequest sendMessageRequest)
        {

            ThanksBoard thanksBoard = new ThanksBoard()
            {
                Message = sendMessageRequest.Message,
                SenderId = sendMessageRequest.SenderId,
                ReceiverId = sendMessageRequest.ReceiverId,
                DateReceived = sendMessageRequest.SendTime,
              
                CategoryId = sendMessageRequest.CategoryId

            };

            await _context.ThanksBoards.AddAsync(thanksBoard);
            await _context.SaveChangesAsync();

            return Ok(thanksBoard);

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
        
        [HttpGet("/count_receivered_messsages")]
        public async Task<ActionResult<int>> CountReceiveredMesseges(int Id)
                {
                    int count = await (from q1 in _context.Employees
                                       join q2 in _context.ThanksBoards on q1.Id equals q2.SenderId
                                       where q2.ReceiverId == Id
                                       select q1).CountAsync();
                    return Ok("Вам отправили "+ (count)+ " Благадарность!");
                }
        
        [HttpGet("/receivered_messsages")]
        public async Task<ActionResult> ReceiveredMessages(int Id)
        {
            var receivered = await (from q1 in _context.Employees
                               join q2 in _context.ThanksBoards on q1.Id equals q2.SenderId
                               where q2.ReceiverId == Id
                               select q2).ToListAsync();
            return Ok(receivered);
        }

        [HttpGet("/sent_messsages")]
        public async Task<ActionResult> SentMessages(int Id)
        {
            var count = await (from q1 in _context.Employees
                               join q2 in _context.ThanksBoards on q1.Id equals q2.ReceiverId
                               where q2.SenderId==Id
                               select q2).ToListAsync();
            return Ok(count);
        }
      /* 
        private int GetMonthOfRating(List<DateTime> date)
        {
            var list = new List<int>();
            foreach (var dob in date)
            {
                var age = DateTime.Today.Subtract(dob.Date);
                var totalDays = age.TotalDays;
                if (totalDays <= 30)
                    list.Add((int)totalDays);

                continue;
            }
            return list.Count();
        }
      
        private int GetYearOfRating(List<DateTime> date)
        {
            var list = new List<int>();
            foreach (var dob in date)
            {
                DateTime zeroTime = new DateTime(1, 1, 1);
                DateTime currentDate = DateTime.Today;
                TimeSpan span = currentDate-dob;
                int years = (zeroTime + span).Year - 1;
                if(years<=1)
                    list.Add((int)years);

                continue;
            }
            return list.Count();
        }

        private int GetHalfYearOfRating(List<DateTime> date)
        {
            var list = new List<int>();
            foreach (var dob in date)
            {
                var currentDate = DateTime.Today.Subtract(dob.Date);
                int month = (int)(DateTime.Today.Subtract(dob).Days /(365.25 / 12));
              if(month<=6)
                    list.Add((int)month);

                continue;
            }
            return list.Count();
        }
        */
      
       [HttpGet("/count_messages_by_category")]
        public async Task<IActionResult> CountMessagesByCategory()
        {
            var map = new Dictionary<string, int>();
            
            foreach (var line in _context.ThanksBoards .GroupBy(info => info.Category.Name)
                        .Select(group => new
                        {
                            CategoryName = group.Key,
                            Count = group.Count()
                        })
                       .OrderBy(x => x.Count))
            {
             
                map.Add(line.CategoryName, line.Count);
            }
            var map2 = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> map1 in map.OrderBy(key => key.Value))
            {
                map2.Add(map1.Key, map1.Value);
                continue;
            }
            var map3 = new Dictionary<string, int>(map2.Reverse());
            return Ok(map3);

        }
        
        private Dictionary<int,int> CountReceiverRating(List<ThanksBoard> list)
        {
            var map = new Dictionary<int, int>();
           
            foreach (var line in list.GroupBy(info => info.ReceiverId)
                        .Select(group => new
                        {
                            ReceiverId = group.Key,
                            Count = group.Count()
                        })
                        .OrderByDescending(c => c.Count)
                        )


                map.Add((int)line.ReceiverId, line.Count);
            
                        var key = map.MaxBy(kvp => kvp.Value).Key;
                        var value = map.MaxBy(kvp => kvp.Value).Value;
                        var map1 = new Dictionary<int, int>();
                        map1.Add(key, value);

                        return map1 ;
            
        }

       
     
        [HttpGet("/get_top_receiver_messages_by_category")]
        public async Task<IActionResult> GetReceiverMessagesByCategory(int categoryId)
        {
            var Respon=new List<ThanksBoard>();
         
               Respon = await (from q2 in _context.ThanksBoards
                                    join q1 in _context.Categories on q2.CategoryId equals q1.Id
                               where q1.Id==categoryId
                                    select q2).ToListAsync();
                if (Respon == null)
                {
                    return BadRequest("В этой катагории сообщений не найдено ");
                }
           
                var map = CountReceiverRating(Respon).Keys.FirstOrDefault();
           
                var employee = await (from e in _context.Employees
                                      join th in _context.ThanksBoards on e.Id equals th.ReceiverId
                                      where th.CategoryId == categoryId && e.Id==map
                                      select new
                                      {
                                          eImg = e.linkImg,
                                          eFullname = e.FullName,
                                          ePosition = e.PositionTitle,
                                          eEmail = e.Email,
                                          ePhone = e.Phone,
                                          Message = new {senderName = th.Sender.FullName, categoryId = "за "+th.Category.Name, Messages=th.Message, sendTime="От "+th.DateReceived  }
                                      })
                                      .ToListAsync();
            return Ok(employee);
            if (employee == null)
            {
                return NotFound("Пользователь не найден");
            }
        }

        [HttpGet("/count_top_receiver_messages_by_category")]
        public async Task<IActionResult> CountReceiverMessagesByCategory(int categoryId)
        {
            var Respon = new List<ThanksBoard>();

            Respon = await (from q2 in _context.ThanksBoards
                            join q1 in _context.Categories on q2.CategoryId equals q1.Id
                           // where q1.Id == categoryId
                            select q2).ToListAsync();
            if (Respon == null)
            {
                return BadRequest("В этой катагории сообщений не найдено ");
            }

            var map = CountReceiverRating(Respon);

          
            return Ok(map);
        }
          

        }








    }
    

