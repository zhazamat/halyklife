using System;

using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hbk.Data;
using hbk.Models;
using hbk.Models.Requests.ThanksBoard;
using hbk.Models.Requests.Market;

namespace hbk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly HbkApiDbContext _context;

        public AdminController(HbkApiDbContext context)
        {
            _context = context;
        }

        // GET: api/Admin
        [HttpGet("/get_all_messages")]
        public async Task<ActionResult<IEnumerable<ThanksBoard>>> GetThanksBoards()
        {
          if (_context.ThanksBoards == null)
          {
              return NotFound("Таблица не существует");
          }
            return await _context.ThanksBoards.ToListAsync();
        }

        // GET: api/Admin/5
        [HttpGet("/get_message_by/{id}")]
        public async Task<ActionResult<ThanksBoard>> GetThanksBoard(int id)
        {
          if (_context.ThanksBoards == null)
          {
              return NotFound("Таблица не существует");
          }
            var thanksBoard = await _context.ThanksBoards.FindAsync(id);

            if (thanksBoard == null)
            {
                return NotFound("Сообщение не найдено");
            }

            return thanksBoard;
        }

        // PUT: api/Admin/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("/update_message/{id}")]
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
                    return NotFound("Сообщение не найдено");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

            // POST: api/Admin
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPost("/add_message")]
        public async Task<ActionResult<ThanksBoard>> PostThanksBoard(SendMessageRequest sendMessageRequest)
        {
          if (_context.ThanksBoards == null)
          {
              return Problem("Entity set 'HbkApiDbContext.ThanksBoards'  is null.");
          }
            ThanksBoard thanksBoard = new ThanksBoard()
            {
                

                Message = sendMessageRequest.Message,
                SenderId = sendMessageRequest.SenderId,
                ReceiverId = sendMessageRequest.ReceiverId,
                DateReceived = sendMessageRequest.SendTime,
                CategoryId=sendMessageRequest.CategoryId

            };
            _context.ThanksBoards.Add(thanksBoard);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetThanksBoard", new { id = thanksBoard.Id }, thanksBoard);
        }

        // DELETE: api/Admin/5
        [HttpDelete("/delete_message_by/{id}")]
        public async Task<IActionResult> DeleteThanksBoard(int id)
        {
            if (_context.ThanksBoards == null)
            {
                return NotFound("Таблица не существует");
            }
            var thanksBoard = await _context.ThanksBoards.FindAsync(id);
            if (thanksBoard == null)
            {
                return NotFound("Сообщение не найдено");
            }

            _context.ThanksBoards.Remove(thanksBoard);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // DELETE: api/Admin/5
        [HttpDelete("/delete_message_by_date")]
        public async Task<IActionResult> DeleteMesage(int receiverId,int senderId,DateTime dateTime)
        {
            if (_context.ThanksBoards == null)
            {
                return NotFound("Таблица не найдено");
            }
            
           
          
            int thanksBoardId =await (from q1 in _context.Employees
                                     join q2 in _context.ThanksBoards on q1.Id equals q2.SenderId
                                     where q2.ReceiverId.Equals(receiverId) && q2.SenderId.Equals(senderId) && q2.DateReceived.Equals(dateTime)
                                     select q1.Id).FirstOrDefaultAsync();
            var thanksBoard = await _context.ThanksBoards.FindAsync(thanksBoardId);
            _context.ThanksBoards.Remove(thanksBoard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ThanksBoardExists(int id)
        {
            return (_context.ThanksBoards?.Any(e => e.Id == id)).GetValueOrDefault();
        }
/*
        // GET: api/Markets
        [HttpGet("/get_all_products_in_market")]
        public async Task<ActionResult<IEnumerable<Market>>> GetMarkets()
        {
            if (_context.Markets == null)
            {
                return NotFound("Таблица не найдено");
            }
            return await _context.Markets.ToListAsync();
        }

        // GET: api/Markets/5
        [HttpGet("/get_product_by/{id}")]
        public async Task<ActionResult<Market>> GetMarket(int? id)
        {
            if (_context.Markets == null)
            {
                return NotFound();
            }
            var market = await _context.Markets.FindAsync(id);

            if (market == null)
            {
                return NotFound();
            }

            return market;
        }

        // PUT: api/Markets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("/update_product_in_market/{id}")]
        public async Task<IActionResult> PutMarket(int? id, UpdateMarketRequest updateMarketRequest)
        {
            var market = await _context.Markets.FindAsync(id);

            if (id !=market.Id)
            {
                return BadRequest();
            }

            _context.Entry(market).State = EntityState.Modified;

            try
            {
                market.Name = updateMarketRequest.Name;
                market.Image = updateMarketRequest.Image;
                market.Description = updateMarketRequest.Description;
                market.Quantity = updateMarketRequest.Quantity;
                market.Price = updateMarketRequest.Price;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarketExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Markets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("/add_product_for_market")]
        public async Task<ActionResult<Market>> PostMarket(UpdateMarketRequest updateMarketRequest)
        {
            if (_context.Markets == null)
            {
                return Problem("Entity set 'HbkApiDbContext.Markets'  is null.");
            }
            Market market = new Market();

            market.Name = updateMarketRequest.Name;
            market.Image = updateMarketRequest.Image;
            market.Description = updateMarketRequest.Description;
            market.Quantity = updateMarketRequest.Quantity;
            market.Price = updateMarketRequest.Price;
            _context.Markets.Add(market);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMarket", new { id = market.Id }, market);
        }

        // DELETE: api/Markets/5
        [HttpDelete("/delete_product_in_market/{id}")]
        public async Task<IActionResult> DeleteMarket(int? id)
        {
            if (_context.Markets == null)
            {
                return NotFound("Таблица не найдено");
            }
            var market = await _context.Markets.FindAsync(id);
            if (market == null)
            {
                return NotFound(" ");
            }

            _context.Markets.Remove(market);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MarketExists(int? id)
        {
            return (_context.Markets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
*/
        [HttpGet("get_all_categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
          //  var categories= await (from c in _context.Categories select new { id=c.Id, categoryImg=c.CategoryImg, categoryName=c.CategoryName, Description=c.Description})
          var categories=await _context.Categories
                .Include(e=>e.Messages)

                .ToListAsync();
           
            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("get_category_by/{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update_category/{id}")]
        public async Task<IActionResult> PutCategory(int id, UpdateCategory updateCategory)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                category.CategoryName = updateCategory.Name;
                category.CategoryImg = updateCategory.Img;
                category.Description = updateCategory.Description;
                return Ok(category);
            }

            _context.Entry(updateCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("save_category")]
        public async Task<ActionResult<Category>> PostCategory(AddCategory addCategory)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'HbkApiDbContext.Categories'  is null.");
            }
            Category c = new Category()
            {

                CategoryName = addCategory.Name,
                CategoryImg=addCategory.Img,
                Description=addCategory.Description

            };
            _context.Categories.Add(c);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = c.Id }, c);
        }

        // DELETE: api/Categories/5
        [HttpDelete("category_delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

   
    }

