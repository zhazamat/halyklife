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

namespace hbk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : Controller
    {
        private readonly HbkApiDbContext _context;

        public EmployeesController(HbkApiDbContext context)
        {
            _context = context;
        }

        
        // GET: api/Employees
        [HttpGet("/get_all_receiver_messages")]
        public async Task<ActionResult<IList<Employee>>> GetMessageReceived()
        {
            var employee = await _context.Employees
                 .Include(e => e.Messages)
                 .Include(e => e.EmployeeMarkets)
                 .ThenInclude(em => em.Market)
                 .ToListAsync();
            if (employee == null)
            {
                return NotFound("Пользователи не найдены ");
            }
            return Ok(employee);
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
        public async Task<IActionResult> deleteEmloyee(int id)
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
