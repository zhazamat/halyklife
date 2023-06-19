using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hbk.Data;
using hbk.Models;
using System.Diagnostics;
using hbk.Models.Requests.Employees;

using hbk.Models.Requests.EmployeeMarket;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace hbk.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class EmployeesController : Controller
    {
        private readonly HbkApiDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        public EmployeesController(HbkApiDbContext context, IWebHostEnvironment environment, IConfiguration config)
        {
            _context = context;
            _webHostEnvironment = environment;
            _config = config;
        }

        
        // GET: api/Employees
        [HttpGet("get_employees")]
        public async Task<IActionResult> GetMessageReceived()
        {
            var employees = await _context.Employees
                 .Include(e => e.Messages)
                 .Include(e=>e.EmployeeMarkets)
                 .ToListAsync();
            var provider = new PhysicalFileProvider(_webHostEnvironment.WebRootPath);
            var contents = provider.GetDirectoryContents(Path.Combine("Uploads", "User"));
            var objFiles = contents.OrderBy(m => m.LastModified).ToArray();


            foreach (var empl in employees)
            {
                var obPng = objFiles.FirstOrDefault(x => x.Name.Contains(empl.FullName + ".png"));
                var obJpg = objFiles.FirstOrDefault(x => x.Name.Contains(empl.FullName + ".jpg"));
                var obJpeg = objFiles.FirstOrDefault(x => x.Name.Contains(empl.FullName + ".jpeg"));
                if (obPng != null)
                {
                    empl.linkImg = _config.GetValue<string>("Kestrel:Endpoints:Http:Url") + "/Uploads/User/" +
                                   obPng.Name;
                }
                else if (obJpg != null)
                {
                    empl.linkImg = _config.GetValue<string>("Kestrel:Endpoints:Http:Url") + "/Uploads/User/" +
                                   obJpg.Name;
                }
                else if (obJpeg != null)
                {
                    empl.linkImg = _config.GetValue<string>("Kestrel:Endpoints:Http:Url") + "/Uploads/User/" +
                                   obJpeg.Name;
                }
                else
                {
                    empl.linkImg = _config.GetValue<string>("Kestrel:Endpoints:Http:Url") + "/Uploads/User/loader.png";
                }
            }

            return new JsonResult(new { Employees = employees });
        
           
        }

      
        [HttpPost("/add_employee")]
        public async Task<ActionResult<Employee>> AddEmployee(AddEmployeeRequest addEmployeeRequest)
        {
            Employee employee = new Employee()
            {

                Id = addEmployeeRequest.Id,
                FullName = addEmployeeRequest.FullName,
                Company = addEmployeeRequest.Company,
                Department = addEmployeeRequest.Department,
                Phone = addEmployeeRequest.Phone,
                IsLocal = addEmployeeRequest.IsLocal,
                linkImg = addEmployeeRequest.linkImg,
                DirectPhone = addEmployeeRequest.DirectPhone,
               
                Email = addEmployeeRequest.Email,
                Mobile = addEmployeeRequest.Mobile,
                OfficeNumber = addEmployeeRequest.OfficeNumber,
                PersonnelNumber = addEmployeeRequest.PersonnelNumber,
                PositionTitle = addEmployeeRequest.PositionTitle,
                Status = addEmployeeRequest.Status,
                WhatsAppMobile = addEmployeeRequest.WhatsAppMobile


            };
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return Ok(employee);

        }


        [HttpPut("/update_employee/{id}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(int id,
                                                        UpdateEmployeeRequest updateEmployeeRequest)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {


                employee.FullName = updateEmployeeRequest.FullName;
                employee.Department = updateEmployeeRequest.Department;
                employee.Phone = updateEmployeeRequest.Phone;
                employee.Company = updateEmployeeRequest.Company;
                employee.Email = updateEmployeeRequest.Email;
                employee.OfficeNumber = updateEmployeeRequest.OfficeNumber;
                employee.DirectPhone = updateEmployeeRequest.DirectPhone;
                
                employee.Mobile = updateEmployeeRequest.Mobile;
                employee.PersonnelNumber = updateEmployeeRequest.PersonnelNumber;
                employee.linkImg = updateEmployeeRequest.linkImg;
                employee.WhatsAppMobile = updateEmployeeRequest.WhatsAppMobile;
                employee.IsLocal = updateEmployeeRequest.IsLocal;
                employee.PositionTitle = updateEmployeeRequest.PositionTitle;
                employee.Status = updateEmployeeRequest.Status;
                await _context.SaveChangesAsync();
                return Ok(employee);


            }

            return NotFound("Пользователь не найден ");
          

        }



        [HttpDelete]
        [Route("/delete_employee/{id}")]
        public async Task<IActionResult> DeleteEmloyee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Remove(employee);
                await _context.SaveChangesAsync();
                return Ok(employee);
            }

            return NotFound(" Пользователь не найден ");


        }

     
    }
}
