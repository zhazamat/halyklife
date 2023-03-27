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
                Category = sendMessageRequest.Category

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
                    return Ok("Вам отправили "+ count+ " Благадарность!");
                }
        
        [HttpGet("/receivered_messsages")]
        public async Task<ActionResult> ReceiveredMessages(int Id)
        {
            var receivered = await (from q1 in _context.Employees
                               join q2 in _context.ThanksBoards on q1.Id equals q2.SenderId
                               where q2.ReceiverId == Id
                               select q2).ToListAsync();
            return Ok("Полученные:"+receivered);
        }

        [HttpGet("/sent_messsages")]
        public async Task<ActionResult> SentMessages(int Id)
        {
            var count = await (from q1 in _context.Employees
                               join q2 in _context.ThanksBoards on q1.Id equals q2.ReceiverId
                               where q2.SenderId==Id
                               select q2).ToListAsync();
            return Ok("Отправленные"+count);
        }
       
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

      /*  [HttpGet("/all_category_messages")]
        public async Task<IActionResult> Respon()
        {
            var map = new Dictionary<Category, int>();
            foreach (var line in _context.ThanksBoards.GroupBy(info => info.Category)
                        .Select(group => new
                        {
                            Metric = group.Key,
                            Count = group.Count()
                        })
                        .OrderBy(x => x.Metric))
                map.Add(line.Metric,line.Count);


                return Ok(map);
           

        }*/
        
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
        
        [HttpGet("/get_confidence_top")]
        public async Task<IActionResult> GetConfidence()
        {
            var Respon = await (from q2 in _context.ThanksBoards
                                where q2.Category == Category.Confidence
                                select q2).ToListAsync();
              return Ok(CountReceiverRating(Respon));
        }

        [HttpGet("/get_responsibility_top")]
        public async Task<IActionResult> GetResponsibility()
        {
            var Responsibility = await (from q2 in _context.ThanksBoards
                                where q2.Category == Category.Responsibility
                                select q2).ToListAsync();
            return Ok(CountReceiverRating(Responsibility));

        }

        [HttpGet("/get_progressorism_top")]
        public async Task<IActionResult> GetProgressorism()
        {var Respon = await (from q2 in _context.ThanksBoards
                                where q2.Category == Category.Progressorism
                                select q2).ToListAsync();
            return Ok(CountReceiverRating(Respon));
        }

        [HttpGet("/get_efficiency_top")]
        public async Task<IActionResult> GetEfficiency()
        {
            var Respon = await (from q2 in _context.ThanksBoards
                                where q2.Category == Category.Efficiency
                                select q2).ToListAsync();
            return Ok(CountReceiverRating(Respon));
        }

        [HttpGet("/get_openness_top")]
        public async Task<IActionResult> GetOpenness()
        {
            var Respon = await (from q2 in _context.ThanksBoards
                                where q2.Category == Category.Openness
                                select q2).ToListAsync();
            return Ok(CountReceiverRating(Respon));
        }

        [HttpGet("/get_innovation_top")]
        public async Task<IActionResult> GetInnovation()
        {
            var Respon = await (from q2 in _context.ThanksBoards
                                where q2.Category == Category.Innovation
                                select q2).ToListAsync();
            return Ok(CountReceiverRating(Respon));
        }

        [HttpGet("/get_manifold_top")]
        public async Task<IActionResult> GetManifold()
        {
            var Respon = await (from q2 in _context.ThanksBoards
                                where q2.Category == Category.Manifold
                                select q2).ToListAsync();
            return Ok(CountReceiverRating(Respon));
        }

        [HttpGet("/get_month_of_category_rating")]
        public async Task<IActionResult> CountMonthOfCategoryMesseges()
        {
            var ConfidenceMessages = await (from q2 in _context.ThanksBoards 
                                       where q2.Category==Category.Confidence 
                                       select q2.DateReceived ).ToListAsync();
            int Confidence = GetMonthOfRating(ConfidenceMessages);

            var EfficiencyMessages = await (from q2 in _context.ThanksBoards
                                            where q2.Category == Category.Efficiency
                                            select q2.DateReceived).ToListAsync();
            int Efficiency = GetMonthOfRating(EfficiencyMessages);

            var ProgressorismMessages = await (from q2 in _context.ThanksBoards
                                            where q2.Category == Category.Progressorism
                                            select q2.DateReceived).ToListAsync();
            int Progressorism = GetMonthOfRating(ProgressorismMessages);

            var OpennessMessages = await (from q2 in _context.ThanksBoards
                                               where q2.Category == Category.Openness
                                               select q2.DateReceived).ToListAsync();
            int Openness = GetMonthOfRating(OpennessMessages);

            var InnovationMessages = await (from q2 in _context.ThanksBoards
                                          where q2.Category == Category.Innovation
                                          select q2.DateReceived).ToListAsync();
            int Innovation = GetMonthOfRating(InnovationMessages);

            var ResponsibilityMessages = await (from q2 in _context.ThanksBoards
                                            where q2.Category == Category.Responsibility
                                            select q2.DateReceived).ToListAsync();
            int Responsibility = GetMonthOfRating(ResponsibilityMessages);

            var ManifoldMessages = await (from q2 in _context.ThanksBoards
                                                where q2.Category == Category.Manifold
                                                select q2.DateReceived).ToListAsync();
            int Manifold = GetMonthOfRating(ManifoldMessages);

            var map = new Dictionary<string, int>();
            map.Add("Kонфиденциальность", Confidence);
            map.Add("Результативность", Efficiency);
            map.Add("Прогрессивизм", Progressorism);
            map.Add("Открытость", Openness);
            map.Add("Новаторство", Innovation);
            map.Add("Ответственность", Responsibility);
            map.Add("Многообразие", Manifold);

            var map2 = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> map1 in map.OrderBy(key => key.Value))
            {
                map2.Add(map1.Key, map1.Value);
                continue;
            }
            var map3 = new Dictionary<string, int>(map2.Reverse());
            return Ok(map3);
         }

        [HttpGet("/get_years_of_category_rating")]
        public async Task<IActionResult> CountYearsOfCategoryMesseges()
        {

            var ConfidenceMessages = await (from q2 in _context.ThanksBoards
                                            where q2.Category == Category.Confidence
                                            select q2.DateReceived).ToListAsync();
            int Confidence = GetYearOfRating(ConfidenceMessages);

            var EfficiencyMessages = await (from q2 in _context.ThanksBoards
                                            where q2.Category == Category.Efficiency
                                            select q2.DateReceived).ToListAsync();
            int Efficiency = GetYearOfRating(EfficiencyMessages);

            var ProgressorismMessages = await (from q2 in _context.ThanksBoards
                                               where q2.Category == Category.Progressorism
                                               select q2.DateReceived).ToListAsync();
            int Progressorism = GetYearOfRating(ProgressorismMessages);

            var OpennessMessages = await (from q2 in _context.ThanksBoards
                                          where q2.Category == Category.Openness
                                          select q2.DateReceived).ToListAsync();
            int Openness = GetYearOfRating(OpennessMessages);

            var InnovationMessages = await (from q2 in _context.ThanksBoards
                                            where q2.Category == Category.Innovation
                                            select q2.DateReceived).ToListAsync();
            int Innovation = GetYearOfRating(InnovationMessages);

            var ResponsibilityMessages = await (from q2 in _context.ThanksBoards
                                                where q2.Category == Category.Responsibility
                                                select q2.DateReceived).ToListAsync();
            int Responsibility = GetYearOfRating(ResponsibilityMessages);

            var ManifoldMessages = await (from q2 in _context.ThanksBoards
                                          where q2.Category == Category.Manifold
                                          select q2.DateReceived).ToListAsync();
            int Manifold = GetYearOfRating(ManifoldMessages);

            var map = new Dictionary<string, int>();
            map.Add("Kонфиденциальность", Confidence);
            map.Add("Результативность", Efficiency);
            map.Add("Прогрессивизм", Progressorism);
            map.Add("Открытость", Openness);
            map.Add("Новаторство", Innovation);
            map.Add("Ответственность", Responsibility);
            map.Add("Многообразие", Manifold);

            var map2 = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> map1 in map.OrderBy(key => key.Value))
            {
                map2.Add(map1.Key, map1.Value);
                continue;
            }
            var map3 = new Dictionary<string, int>(map2.Reverse());
            return Ok(map3);
           
        }

        [HttpGet("/get_halfyears_of_category_rating")]
        public async Task<IActionResult> CountHalfAYearsOfCategoryMesseges()
        {

            var ConfidenceMessages = await (from q2 in _context.ThanksBoards
                                            where q2.Category == Category.Confidence
                                            select q2.DateReceived).ToListAsync();
            int Confidence = GetHalfYearOfRating(ConfidenceMessages);

            var EfficiencyMessages = await (from q2 in _context.ThanksBoards
                                            where q2.Category == Category.Efficiency
                                            select q2.DateReceived).ToListAsync();
            int Efficiency = GetHalfYearOfRating(EfficiencyMessages);

            var ProgressorismMessages = await (from q2 in _context.ThanksBoards
                                               where q2.Category == Category.Progressorism
                                               select q2.DateReceived).ToListAsync();
            int Progressorism = GetHalfYearOfRating(ProgressorismMessages);

            var OpennessMessages = await (from q2 in _context.ThanksBoards
                                          where q2.Category == Category.Openness
                                          select q2.DateReceived).ToListAsync();
            int Openness = GetHalfYearOfRating(OpennessMessages);

            var InnovationMessages = await (from q2 in _context.ThanksBoards
                                            where q2.Category == Category.Innovation
                                            select q2.DateReceived).ToListAsync();
            int Innovation = GetHalfYearOfRating(InnovationMessages);

            var ResponsibilityMessages = await (from q2 in _context.ThanksBoards
                                                where q2.Category == Category.Responsibility
                                                select q2.DateReceived).ToListAsync();
            int Responsibility = GetHalfYearOfRating(ResponsibilityMessages);

            var ManifoldMessages = await (from q2 in _context.ThanksBoards
                                          where q2.Category == Category.Manifold
                                          select q2.DateReceived).ToListAsync();
            int Manifold = GetHalfYearOfRating(ManifoldMessages);

            var map = new Dictionary<string, int>();
            map.Add("Kонфиденциальность", Confidence);
            map.Add("Результативность", Efficiency);
            map.Add("Прогрессивизм", Progressorism);
            map.Add("Открытость", Openness);
            map.Add("Новаторство", Innovation);
            map.Add("Ответственность", Responsibility);
            map.Add("Многообразие", Manifold);

            var map2 = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> map1 in map.OrderBy(key => key.Value))
            {
                map2.Add(map1.Key, map1.Value);
                continue;
            }
            var map3 = new Dictionary<string, int>(map2.Reverse());
            return Ok(map3);

        }


    }








    }
    

