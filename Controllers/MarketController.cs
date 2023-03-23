using hbk.Data;
using hbk.Models;
using hbk.Models.Requests.EmployeeMarket;

using hbk.Models.Requests.Market;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hbk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        // GET: api/<MarketController>
        // GET: api/Employees
        private readonly HbkApiDbContext _context;
        private readonly ThanksBoardsController _thanksBoardsController;
        //private readonly ThanksBoardsController _boardsController;

        public MarketController(HbkApiDbContext context)
        {
            _context = context;
           
        }
        [HttpGet("/ get_all_gifts")]
        public async Task<ActionResult<IEnumerable<Market>>> GetMarkets()
        {

            using (var context = new HbkApiDbContext())
            {
                return await _context.Markets
                    .Include(m=>m.EmployeeMarkets)
                    .ThenInclude(em=>em.Employee)
                    .ToListAsync();
            }

        }
       
        [HttpGet("/get_employee_with_gift")]
        public async Task<ActionResult<IEnumerable<EmployeeMarket>>> GetEmployeeWithGift()
        {
            var employeeMarket = await _context.EmployeeMarkets.ToArrayAsync();

            return employeeMarket;
            

        }

        [HttpPost("/ add_gift")]
        public async Task<ActionResult<Market>> AddMarket(AddMarketRequest addMarketRequest)
        {
            Market market = new Market()
            {

                
                Name = addMarketRequest.Name,
                Description = addMarketRequest.Description,
                Image = addMarketRequest.Image,
                Quantity = addMarketRequest.Quantity,
                Price=addMarketRequest.Price


            };
            await _context.Markets.AddAsync(market);
            await _context.SaveChangesAsync();
            return Ok(market);

        }
        
        [HttpPut]
        [Route("/ update_gift/{id}")]
        public async Task<ActionResult<Market>> UpdateMarket(int id,
                                                       UpdateMarketRequest updateMarketRequest)
        {
           
            var market = await _context.Markets.FindAsync(id);
                if (market != null)
                {


                    market.Name = updateMarketRequest.Name;
                    market.Image = updateMarketRequest.Image;
                    market.Description = updateMarketRequest.Description;
                    market.Quantity = updateMarketRequest.Quantity;
                    market.Price = updateMarketRequest.Price;

                    await _context.SaveChangesAsync();
                    return Ok(market);
                }
            
            return NotFound();

        }
        [HttpPut]
        [Route("/updateEmployeeGift/{id}")]
        public async Task<ActionResult<EmployeeMarket>> UpdateEmployeeGift(int id,
                                                      UpdateEmployeeMarketRequest updateMarketRequest)
        {
            
            var market = await _context.EmployeeMarkets.FindAsync(id);
            if (market != null)
            {
                market.EmployeeId = updateMarketRequest.EmployeeId;
                market.MarketId = updateMarketRequest.MarketId;
                await _context.SaveChangesAsync();



                return Ok(market);

            }
            return NotFound();

        }



        [HttpDelete]
        [Route("/ delete/{id}")]
        public async Task<ActionResult<Market>> deleteEmloyee(int id)
        {
            var market = await _context.Markets.FindAsync(id);
            if (market != null)
            {
                _context.Remove(market);
                await _context.SaveChangesAsync();
                return Ok(market);
            }
            return NotFound();
        }

        
        [HttpPost("/add_gift_for_employee")]
        public async Task<ActionResult> AddEmployeeMarket(int Id, string gift)
        {
            int count = await (from q1 in _context.Employees
                               join q2 in _context.ThanksBoards on q1.Id equals q2.SenderId
                               where q2.ReceiverId == Id 
                               select q1).CountAsync();
           

            var receiver = await (from q1 in _context.Employees
                                  join q2 in _context.ThanksBoards on q1.Id equals q2.ReceiverId
                                  where q2.ReceiverId == Id
                                  select q1.Id).FirstAsync();

            var count1 = await (from q1 in _context.Employees
                                  join q2 in _context.ThanksBoards on q1.Id equals q2.ReceiverId
                                  where q2.ReceiverId == Id
                                  select q1.Id).CountAsync();

            var price = await (from m in _context.Markets where m.Name.Equals(gift) select m.Price).FirstAsync();

            var quantity = await (from m in _context.Markets where m.Name.Equals(gift) select m.Quantity).FirstAsync();
          
          
            var giftId = await (from m1 in _context.Markets where m1.Name.Equals(gift) select m1.Id).FirstAsync();
            if (quantity == 0) {
                return Ok("Извините, у нас " + gift + " не осталось"); }
                if(count >= price){
                EmployeeMarket market = new EmployeeMarket()
                {
                    MarketId = giftId,
                    EmployeeId = receiver
                };
                await _context.EmployeeMarkets.AddAsync(market);
                await _context.SaveChangesAsync();
                
                return Ok(market);
            }
          

            return BadRequest("У вас не достаточные поздравление");
           
           


        }

    }
}
