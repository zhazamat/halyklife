using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;

using hbk.Models;
using hbk.Data;

namespace hbk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment;
       // private string host = "";

        private readonly IConfiguration _config;
      //  private readonly IObtainEmployeesFromOneS _oneSServices;
        private readonly HbkApiDbContext _context;

        public ImageController(IWebHostEnvironment webHostEnvironment, IConfiguration _config,
            HbkApiDbContext context)
        {
            this._config = _config;
            this._context = context;
            //this._oneSServices = oneSServices;
            this.webHostEnvironment = webHostEnvironment;
        }

        [Route("api/file")]
        [HttpPost]
        public string Post([FromForm] FileModel file)
        {
            string path;
            try
            {
                path = Path.Combine(
                    "wwwroot",
                    "Uploads",
                    "User",
                    file.FileName
                );
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    file.FormFile.CopyTo(stream);
                }

                return path;
            }
            catch (FormatException ex)
            {
                return Convert.ToString(ex.Message);
            }
        }


        [Route("api/getImg")]
        [HttpPost]
        public string Get([FromForm] string userName)
        {
            var provider = new PhysicalFileProvider(webHostEnvironment.WebRootPath);
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
    }
}
