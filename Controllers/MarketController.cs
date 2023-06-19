using hbk.Data;
using hbk.Models;
using hbk.Models.Requests.EmployeeMarket;

using hbk.Models.Requests.Market;
using hbk.Models.Requests.ThanksBoard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.Drawing.Printing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hbk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        //private readonly    AdminController adminController;
        private readonly HbkApiDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        public MarketController(HbkApiDbContext context, IWebHostEnvironment environment, IConfiguration config)
        {

            _context = context;
            _webHostEnvironment = environment;
            _config = config;
          
        }



        private string Get(string category)
        {
            var provider = new PhysicalFileProvider(_webHostEnvironment.WebRootPath);
            var contents = provider.GetDirectoryContents(Path.Combine("Uploads", "Category"));
            var objFiles = contents.OrderBy(m => m.LastModified).ToArray();

            var obPng = objFiles.FirstOrDefault(x => x.Name.Contains(category + ".png"));
            var obJpg = objFiles.FirstOrDefault(x => x.Name.Contains(category + ".jpg"));
            var obJpeg = objFiles.FirstOrDefault(x => x.Name.Contains(category + ".jpeg"));
            if (obPng != null)
            {
                return _config.GetValue<string>("Kestrel:Endpoints:Http:Url") + "/Uploads/Category/" +
                       obPng.Name;
            }
            else if (obJpg != null)
            {
                return _config.GetValue<string>("Kestrel:Endpoints:Http:Url") + "/Uploads/Category/" +
                       obJpg.Name;
            }
            else if (obJpeg != null)
            {
                return _config.GetValue<string>("Kestrel:Endpoints:Http:Url") + "/Uploads/Category/" +
                       obJpeg.Name;
            }
            else
            {
                return _config.GetValue<string>("Kestrel:Endpoints:Http:Url") + "/Uploads/Category/loader.png";
            }
        }

        [HttpGet("/searchProduct")]
        public async Task<IEnumerable<Market>> SearchProduct(string search)
        {

            IQueryable<Market> query = _context.Markets;
            foreach (var m in query)
            {
                m.GiftImg = Get(m.Name);
            }
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e =>
                e.Name.ToUpper().Contains(search.ToUpper()));

            }
            return await query.Include(x=>x.EmployeeMarkets).ToListAsync();
        }
        [HttpGet("productCategoryList")]
        public List<string> CategoryProduct()
        {
            var category = new List<string>();
           // var market = _context.Markets.GroupBy(m => m.ProductCategory).Select(g => g.Key).ToList();
            var market = (from m in _context.Markets select m.ProductCategory).Distinct().ToList();
            category = market;
           return category;
        }
        [HttpGet("sortList")]
        public List<string> SortProduct()
        {
            var sort= new List<string>() {"Сначала новинки", "Сначала популярные", "Сначала дешевые" ,"Сначала дорогие"};
           
           
            return sort;
        }

        [HttpGet("productList")]
        public async Task<IActionResult> GetAllMarkets(int page, int limit, string? input, string? sort, string? search)
        {
            var obj = new List<Market>();
            var products = await (from m in _context.Markets select m).Include(x=>x.EmployeeMarkets)
                                     .ToListAsync();
            
            foreach (var p in products)
            {
                if (!string.IsNullOrEmpty(input) && p.ProductCategory.ToLower() == input.ToLower())
                {
                    p.GiftImg = Get(p.Name);
                    obj.Add(p);
                }
               else if (string.IsNullOrEmpty(input))
                {
                    p.GiftImg = Get(p.Name);
                    obj = products;
                }}
            var obj1 = new List<Market>();
            var obj2 = new List<Market>();
            if (string.IsNullOrEmpty(sort))
            {
                obj1 = obj;
            }
            else if (sort == "Сначала новинки")
            {
                obj1 = obj.OrderByDescending(x => x.Id)
                          .ThenBy(x => x.Name)
                         .ToList();
            }
            else if (sort == "Сначала популярные")
            {
                obj1 = obj.OrderByDescending(x => x.Quantity)
                          .ThenBy(x => x.Name)
                         .ToList();
            }
            else if (sort == "Сначала дешевые")
            {
                obj1 = obj.OrderBy(x => x.Price)
                          .ThenBy(x => x.Name)
                          .ToList();
            }
            else if (sort == "Сначала дорогие")
            { 
                obj1 = obj.OrderByDescending(x => x.Price)
                          .ThenBy(x => x.Name).ToList();

            }
          
            if (string.IsNullOrEmpty(search))
            {
                obj2 = obj1; // Возвращаем весь список без фильтрации
            }
            else
            {
                obj2 = obj1.Where(e => e.Name.ToUpper().Contains(search.ToUpper())).ToList(); // Фильтрация по имени только при наличии значения в поле search
            }

            List<GetMarketResponse> Markets = new List<GetMarketResponse>();
          GetProductResponse response=new GetProductResponse();
            var count=obj2.Count();
            obj2=obj2.Skip((page - 1) * limit).Take(limit).ToList();
            
            foreach (var o in obj2)
            {
                GetMarketResponse m = new GetMarketResponse();
                m.Id = (int)o.Id;
                m.Name = o.Name;
                m.Price = o.Price;
                m.Quantity = o.Quantity;
                m.Category = o.ProductCategory;
                m.Img = Get(o.Name);
                Markets.Add(m);
            }
            response.Markets = Markets;
            response.TotalProduct = count;





            //  return Ok(obj2.Skip((page - 1) * limit).Take(limit));
            return Ok(response);
        }
        
        // GET: api/Product/5
        [HttpGet("{id}")]
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

        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Market>> PutMarket(int? id, AddMarketRequest addMarketRequest)
        {
            var market = await _context.Markets.FindAsync(id);
            if (market != null)
            {
                market.ProductCategory = addMarketRequest.Category;
                market.Name = addMarketRequest.Name;
                market.Price = addMarketRequest.Price;
                market.Quantity = addMarketRequest.Quantity;
              //  _context.Markets.Add(market);
                _context.SaveChanges();
                return market;

            }
            return NotFound("id is not found");


        }

        [HttpDelete("/delete_emplgift_by/{id}")]
        public async Task<IActionResult> DeleteEmplGift(int id)
        {
            if (_context.EmployeeMarkets == null)
            {
                return NotFound("Таблица не существует");
            }
            var thanksBoard = await _context.EmployeeMarkets.FindAsync(id);
            if (thanksBoard == null)
            {
                return NotFound("Сообщение не найдено");
            }

            _context.EmployeeMarkets.Remove(thanksBoard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Market>> PostMarket(AddMarketRequest addMarketRequest)
        {
            if (_context.Markets == null)
            {
                return Problem("Entity set 'HbkApiDbContext.Markets'  is null.");
            }
            Market market = new Market();

            market.Name = addMarketRequest.Name;
            market.Price = addMarketRequest.Price;
            market.Quantity = addMarketRequest.Quantity;
            market.ProductCategory = addMarketRequest.Category;
            market.GiftImg = Get(addMarketRequest.Name);
            _context.Markets.Add(market);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMarket", new { id = market.Id }, market);
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarket(int? id)
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

            _context.Markets.Remove(market);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MarketExists(int? id)
        {
            return (_context.Markets?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPut("/update_th/{id}")]
        public async Task<ActionResult<ThanksBoard>> UpdateEmployee(int id,
                                                       UpdateThanksBoard updateThanks)
        {
            var thanksBoard = await _context.ThanksBoards.FindAsync(id);
            if (thanksBoard != null)
            {
                thanksBoard.IsActive = updateThanks.IsActive;
                await _context.SaveChangesAsync();
                return Ok(thanksBoard);
            }

            return NotFound("Пользователь не найден ");


        }

        [HttpPost("/buy_gift")]
        public async Task<IActionResult> AddEmployeeMarket(List<AddEmplProduct> addEmplMarkets)
                {
            var user = 2;
            int count = await (from q1 in _context.Employees
                               join q2 in _context.ThanksBoards on q1.Id equals q2.ReceiverId
                               where q2.ReceiverId == user && q2.IsActive==true
                               select q2).CountAsync();
            var emplMessages = await (from q1 in _context.Employees
                                      join q2 in _context.ThanksBoards on q1.Id equals q2.ReceiverId
                                      where q2.ReceiverId == user && q2.IsActive == true
                                      select q2).ToListAsync();
            int c = 0;
            foreach (var addEmplMarket in addEmplMarkets)
            {

                int  price = await (from m in _context.Markets where m.Id == addEmplMarket.MarketId select m.Price).FirstAsync();
                var savingMarket = await (from m in _context.Markets where m.Id == addEmplMarket.MarketId select m).FirstAsync();
                c += price*addEmplMarket.Quantity;
                var mg = emplMessages.Where(x => x.ReceiverId == user).OrderBy(e => e.Id).ToList().Take(c);
                var market1 = await _context.Markets.FindAsync(addEmplMarket.MarketId);
                if (count >= price && market1 != null && savingMarket.Quantity != 0)
                {
                    EmployeeMarket market = new EmployeeMarket();
                    market.MarketId = addEmplMarket.MarketId;
                    market.EmployeeId = user;
                    market.Date = DateTime.UtcNow;
                   market.Quantity = addEmplMarket.Quantity;
                    _context.EmployeeMarkets.Add(market);
                    _context.SaveChanges();
                    savingMarket.Quantity =market1.Quantity-addEmplMarket.Quantity;
                    market1.ProductCategory = savingMarket.ProductCategory;
                    market1.Name = savingMarket.Name;
                    market1.Price = savingMarket.Price;
                    market1.Quantity = savingMarket.Quantity;
                    _context.SaveChanges();
                    foreach (var t in mg)
                     {
                        var thanksBoard = await _context.ThanksBoards.FindAsync(t.Id);
                        if (thanksBoard != null)
                        {
                            thanksBoard.IsActive = false;
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                else { 
                return Ok("\"У вас не достаточное благодарности...\" или У нас не хватает продукты");
            }
            }
         
            return Ok(_context.EmployeeMarkets.ToList());
        }
         

    }
}

