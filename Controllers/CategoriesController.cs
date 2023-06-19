using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hbk.Data;
using hbk.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace hbk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly HbkApiDbContext _context;
      //  private readonly IWebHostEnvironment _environment;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        public CategoriesController(HbkApiDbContext context, IWebHostEnvironment environment, IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _config = config;

        }


        private string Get(string userName)
        {
            var provider = new PhysicalFileProvider(_webHostEnvironment.WebRootPath);
            var contents = provider.GetDirectoryContents(Path.Combine("Uploads", "User"));
            var objFiles = contents.OrderBy(m => m.LastModified).ToArray();

            var obPng = objFiles.FirstOrDefault(x => x.Name.Contains(userName + ".png"));
            var obJpg = objFiles.FirstOrDefault(x => x.Name.Contains(userName + ".jpg"));
            var obJpeg = objFiles.FirstOrDefault(x => x.Name.Contains(userName + ".jpeg"));
            if (obPng != null)
            {
                return _config.GetValue<string>("Kestrel:Endpoints:Http:Url") + "/Uploads/User/" +
                       obPng.Name;
            }
            else if (obJpg != null)
            {
                return _config.GetValue<string>("Kestrel:Endpoints:Http:Url") + "/Uploads/User/" +
                       obJpg.Name;
            }
            else if (obJpeg != null)
            {
                return _config.GetValue<string>("Kestrel:Endpoints:Http:Url") + "/Uploads/User/" +
                       obJpeg.Name;
            }
            else
            {
                return _config.GetValue<string>("Kestrel:Endpoints:Http:Url") + "/Uploads/User/loader.png";
            }
        }
        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
          if (_context.Categories == null)
          {
              return NotFound();
          }
            var categories = new List<Category>();
          var categoriesList= await _context.Categories.Include(e => e.Messages).ToListAsync();
            if (categoriesList != null && categoriesList.Count > 0)
            {
                foreach (var c in categoriesList) {
                    Category category = new Category();
                    category.CategoryImg = Get(c.CategoryName);
                    category.CategoryName = c.CategoryName;
                    category.Id = c.Id;
                    category.Description = c.Description;
                    categories.Add(category);
                }
                
            }
            else
            {
               return new List<Category>();
            }
            return categories;
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

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
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
          if (_context.Categories == null)
          {
              return Problem("Entity set 'HbkApiDbContext.Categories'  is null.");
          }
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
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
/*
        [HttpPost("UploadImage")]
        public async Task<ActionResult> UploadImage()
        {
            bool Results = false;
            try
            {
                var _uploadedfiles = Request.Form.Files;
                foreach (IFormFile source in _uploadedfiles)
                {
                    string FileName = source.FileName;
                    string Filepath = GetFilePath(FileName);
                    if (!System.IO.Directory.Exists(Filepath))
                    {
                        System.IO.Directory.CreateDirectory(Filepath);

                    }
                    string imagepath = Filepath + "\\image.jpg";
                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }
                    using (FileStream stream = System.IO.File.Create(imagepath))
                    {
                        await source.CopyToAsync(stream);
                        Results = true;
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            return Ok(Results);
        }

        [NonAction]
        private string GetFilePath(string CategoryCode)
        {
            return this._environment.WebRootPath + "\\Uploads\\Category" + CategoryCode;
        }

        [NonAction]
        private string GetImagebyCategory(string CategoryCode)
        {
            string ImageUrl = string.Empty;
            string hostUrl = "https://localhost:5555/";
            string Filepath = GetFilePath(CategoryCode);
            string Imagepath = Filepath + "\\1.jpg";
            if (!System.IO.File.Exists(Imagepath))
            {
                ImageUrl = hostUrl + "/Uploads/Category/1.jpg";
            }
            else
            {
                ImageUrl = hostUrl + "/Uploads/Category/" + CategoryCode + "/1.jpg";
            }
            return ImageUrl;
        }
/*
        static HttpClient httpClient = new HttpClient();
        static async Task Main()
        {
            List<Category>? categories = await httpClient.GetFromJsonAsync<List<Category>>("https://localhost:5555/api/Categories");
            if (categories != null)
            {
                foreach (var category in categories)
                {
                    Console.WriteLine(category.CategoryName);
                }
            }
        }*/
    }

}

