using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hbk.Data;
using hbk.Models;
using hbk.Models.Requests.ThanksBoard;
//using Microsoft.AspNetCore.Http;
//using System.Collections.Generic;
using hbk.Models.Requests.Employees;
//using NuGet.Packaging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;
using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.ComponentModel;
using System.Collections.Specialized;
using hbk.Converters;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NuGet.Packaging;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hbk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanksBoardsController : ControllerBase
    {
        private readonly HbkApiDbContext _context;
        //  private readonly IWebHostEnvironment _environment;
        //  private readonly EmployeesController _employees;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        public ThanksBoardsController(HbkApiDbContext context, IWebHostEnvironment environment, IConfiguration config)
        {

            _context = context;
            _webHostEnvironment = environment;
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
        // GET: api/ThanksBoards

       

        [HttpGet("/get_info_list")]
        public async Task<ActionResult<ThanksBoard>> GetInfoList(int page,int limit)
        {

            var infoList = await (from i in _context.ThanksBoards where i.IsActive==true
                                  select new { id = i.Id,
                                      senderName = i.Sender.FullName,

                                      receiverName = i.Receiver.FullName,
                                      message = i.Message,
                                     
                                      categoryName = i.Category.CategoryName,
                                      date = i.DateReceived })
                                      .OrderByDescending(e => e.date)
                                  .ToListAsync();

            var infoLists = new List<GetInfoListResponse>();
            foreach (var i in infoList) {

                GetInfoListResponse getInfoListResponse = new GetInfoListResponse();
                getInfoListResponse.Id = i.id;
                getInfoListResponse.SenderName = i.senderName;
                getInfoListResponse.SenderImgUrl = Get(i.senderName);
                getInfoListResponse.ReceiverName = i.receiverName;
                getInfoListResponse.ReceiverImgUrl = Get(i.receiverName);
                getInfoListResponse.Message = i.message;
              //  getInfoListResponse.IsActive = i.isActive;
                getInfoListResponse.CategoryName = i.categoryName;
                getInfoListResponse.CategoryImgUrl = Get(i.categoryName);
                getInfoListResponse.DateReceived = i.date.ToString("dd.MM.yyyy");
                
                infoLists.Add(getInfoListResponse);
            }
            if (page != 0 && limit != 0)
            {
                return Ok(infoLists.Skip((page - 1) * limit).Take(limit));
            }
            return Ok(infoLists);

        }

        [HttpGet("/searchEmployee")]
        public async Task<IEnumerable<Employee>> Search(string search)
        {
            IQueryable<Employee> query =   _context.Employees;

            if (!string.IsNullOrEmpty(search))
            {

                query = query.Where(e =>
                e.PersonnelNumber.ToUpper().Contains(search.ToUpper()) ||
                e.FullName.ToUpper().Contains(search.ToUpper()));

            }
            List<Employee> list = new List<Employee>();

            foreach (var k in query)
            {
                if (k.Id != 2)
                {
                    Employee empl = new Employee();
                    empl.Id = k.Id;
                    empl.PersonnelNumber = k.PersonnelNumber;
                    empl.FullName = k.FullName;
                    empl.Department = k.Department;
                    empl.Company = k.Company;
                    empl.PositionTitle = k.PositionTitle;
                    empl.Mobile = k.Mobile;
                    empl.WhatsAppMobile = k.WhatsAppMobile;
                    empl.linkImg = Get(k.FullName);
                    empl.Email = k.Email;
                    empl.Phone = k.Phone;
                    empl.DirectPhone = k.DirectPhone;
                    list.Add(empl);
                }

            }
            return list;

        }

   
        [HttpGet("/count_opportunity")]
        public int CountSenderOpportunity(int id)
        {
           
            List<SenderOpportunityResponse> slist = new List<SenderOpportunityResponse>();

            var count = this._context.ThanksBoards
  .Where(x => x.Id > 0 && x.SenderId == id)
  .GroupBy(s => new { sender = s.Sender.FullName, month = s.DateReceived.Month })
  .Select(x => new { count = x.Count(), sender = x.Key.sender, month = x.Key.month })
  .ToList();

            foreach (var c in count)
            {
                var senderOpportunityResponse = new SenderOpportunityResponse();
                senderOpportunityResponse.Count = c.count;
                senderOpportunityResponse.Name = c.sender;
                senderOpportunityResponse.Month = c.month;
                slist.Add(senderOpportunityResponse);
            }
            var k = 0;
            foreach (var s in slist)
            {
               
                    DateTime now = DateTime.Now;
                    if (s.Month == now.Month)
                    {
                        k = s.Count++;
                    }
                    else
                    {
                        break;
                    }
                
            }
          
            return k;
        }


        [HttpGet("/count")]
        public int Count()
        {
            // string personnelNumber = "23456781";
            List<SenderOpportunityResponse> slist = new List<SenderOpportunityResponse>();

            var count = this._context.ThanksBoards
  .Where(x => x.Id > 0 && x.SenderId == 2)
  .GroupBy(s => new { sender = s.Sender.FullName, month = s.DateReceived.Month })
  .Select(x => new { count = x.Count(), sender = x.Key.sender, month = x.Key.month })
  .ToList();

            foreach (var c in count)
            {
                var senderOpportunityResponse = new SenderOpportunityResponse();
                senderOpportunityResponse.Count = c.count;
                senderOpportunityResponse.Name = c.sender;
                senderOpportunityResponse.Month = c.month;
                slist.Add(senderOpportunityResponse);
            }
            var k = 0;
            foreach (var s in slist)
            {
               
                    if (s.Month == DateTime.Now.Month)
                    {

                        k = s.Count;
                    }
                }
            

            int j = 3 - k;
            if (j < 0 || j > 4)
            {
                return 0;
            }
            return (j);

        }

        [HttpGet("/count_day")]
        public EmployeeWithCount CountDay()
        {  EmployeeWithCount empl = new EmployeeWithCount();
             
                empl.Day = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day;
      
            return empl;

        }

        // POST: api/ThanksBoards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("/sending_message")]
        [AllowAnonymous]
        public async Task<IActionResult> AddEmployee(SendMessageRequest sendMessageRequest)
        {


            ThanksBoard thanksBoard = new ThanksBoard();
            // thanksBoard.Id = sendMessageRequest.id;
            var receiverId = await (from e in _context.Employees where e.FullName == sendMessageRequest.receiverName select e.Id).FirstOrDefaultAsync();
            if (receiverId != 0)
            {
                thanksBoard.ReceiverId = receiverId;
               
            }
            else
            {
                return NotFound("Not found receiver Id");
            }
            var senderId = await (from e in _context.Employees where e.FullName == sendMessageRequest.senderName select e.Id).FirstOrDefaultAsync();


            if (senderId != 0)
            {
                    thanksBoard.SenderId = senderId;
            }
           else
            {
                return NotFound("Not found sender Id");
            }

            var categoryId = await (from e in _context.Categories where e.CategoryName == sendMessageRequest.achiv select e.Id).FirstOrDefaultAsync();
            if (categoryId != 0)
            {
                thanksBoard.CategoryId = categoryId;
            }
            else
            {
                return NotFound("Not found this category of Id");
            }

            if (senderId != receiverId)
            {
                thanksBoard.DateReceived = DateTime.UtcNow;
                thanksBoard.Message = sendMessageRequest.message;
                thanksBoard.IsActive = true;

            }
            else
            {
                return BadRequest("You couldn't send message on yourself");
            }



            await _context.ThanksBoards.AddAsync(thanksBoard);
            await _context.SaveChangesAsync();

            return Ok(thanksBoard);

        }
        
        private bool ThanksBoardExists(int id)
        {
            return _context.ThanksBoards.Any(e => e.Id == id);
        }
      
        [HttpGet("/employee_messsages_history")]
        public async Task<IActionResult> SentMessages(string input)
        {
            string PersonnelNumber = "23456781";
            var SentMessageList = new List<EmployeeMessagesResponse>();
            // string PersonnelNumber = "23456781";
            if (input.ToUpper().Equals("Отправленные".ToUpper()))
            {
                var category = (from q1 in _context.Categories select q1.Id).ToList();
                foreach (var c in category)
                {
                    var sentMessages = await (from q2 in _context.ThanksBoards
                                      where q2.Sender.PersonnelNumber == PersonnelNumber && q2.CategoryId == c 
                                      select new
                                      {
                                          id = q2.Id,
                                          sentDate = q2.DateReceived,
                                          receiverName = q2.Receiver.FullName,
                                          receiverImg = q2.Receiver.linkImg,

                                          categoryImg = q2.Category.CategoryImg,
                                          categoryName = q2.Category.CategoryName,
                                          message = q2.Message
                                      }).ToListAsync();
                    foreach (var sent in sentMessages)
                    {
                      
                            EmployeeMessagesResponse r = new EmployeeMessagesResponse();
                            r.Id = sent.id;
                            r.DateReceived = sent.sentDate.ToString("dd.MM.yyyy");
                            r.EmployeeImgUrl = Get(sent.receiverName);
                            r.EmployeeName = sent.receiverName;
                            r.CategoryImg = Get(sent.categoryName);
                            r.CategoryName = sent.categoryName;
                            r.Message = sent.message;
                            SentMessageList.Add(r);
                       
                    }

                }

            }
            else if (input.ToUpper().Equals("Полученные".ToUpper()))
            {
                var category = (from q1 in _context.Categories select q1.Id).ToList();
                foreach (var c in category)
                {
                    var receiveredMessages = await (from q2 in _context.ThanksBoards
                                            where q2.Receiver.PersonnelNumber == PersonnelNumber && q2.CategoryId == c &&q2.IsActive==true
                                            select new
                                            {
                                                id = q2.Id,
                                                date = q2.DateReceived,
                                                receiverImg = q2.Receiver.linkImg,
                                                receiverName = q2.Receiver.FullName,
                                                categoryImg = q2.Category.CategoryImg,
                                                categoryName = q2.Category.CategoryName,
                                                message = q2.Message
                                            })
                                            .ToListAsync();

                    foreach(var receivered in receiveredMessages) { 
                        EmployeeMessagesResponse s = new EmployeeMessagesResponse();
                        s.Id = receivered.id;
                        s.DateReceived = receivered.date.ToString("dd.MM.yyyy");
                        s.EmployeeImgUrl = Get(receivered.receiverName);
                        s.EmployeeName = receivered.receiverName;
                        s.CategoryImg = Get(receivered.categoryName);
                        s.CategoryName = receivered.categoryName;
                        s.Message = receivered.message;

                        SentMessageList.Add(s);
                    }
                }
            }
            return Ok(SentMessageList);
        }

        /*
        [HttpGet("/get_top_of_statistic")]

        public List<TopReceiverStatisticResponse> GetTopReceiverWithCategories()
        {
            var tops = new List<TopReceiverStatisticResponse>();

            var category = (from c in _context.Categories select c.CategoryName).ToList();
            foreach (var item in category) {

                TopReceiverStatisticResponse topReceiverStatisticResponse = new TopReceiverStatisticResponse();
                List<StatisticResponse> s = new List<StatisticResponse>(CountStatistic(item));

                topReceiverStatisticResponse.Employee = GetTopReceiver(item);
                topReceiverStatisticResponse.Statistics = s;
                tops.Add(topReceiverStatisticResponse);

            }
            return tops;
        }


        private Employee GetTopReceiver(string categoryName)
        {
            var l = (from t in _context.Categories
                     where t.CategoryName.ToLower().Equals(categoryName.ToLower())
                     select t.Id)
                         .FirstOrDefault();

            int key = CountReceiverMessagesByCategory(l).MaxBy(v => v.Value).Key;

            var top = (from e in _context.Employees
                       where e.Id == key
                       select e)
                .FirstOrDefault();

            Employee empl = new Employee();
            empl = top;

            empl.linkImg = Get(top.FullName);

            return empl;
        }

        private List<StatisticResponse> CountStatistic(string categoryName)
        {

            var category = (from q1 in _context.Categories select q1.Id).ToList();
            var StatisticList = new List<StatisticResponse>();
            var l = (from t in _context.Categories
                     where t.CategoryName.ToLower().Equals(categoryName.ToLower())
                     select t.Id)
                         .FirstOrDefault();

            int key = CountReceiverMessagesByCategory(l).MaxBy(v => v.Value).Key;

            var top = (from e in _context.Employees
                       where e.Id == key
                       select e.Id)
                .FirstOrDefault();
            foreach (var c in category)
            {

                var statistic = (from q2 in _context.ThanksBoards
                                 where q2.ReceiverId == top && q2.CategoryId == c
                                 select new
                                 {
                                     CategoryId = q2.Id,
                                     categoryImg = q2.Category.CategoryImg,
                                     categoryName = q2.Category.CategoryName,
                                 }).FirstOrDefault();

                var count = (from q2 in _context.ThanksBoards
                             where q2.ReceiverId == top && q2.CategoryId == c
                             select q2).Count();
                if (statistic != null)
                {
                    StatisticResponse r = new StatisticResponse();
                    r.CategoryImg = Get(statistic.categoryName);
                    r.CategoryName = statistic.categoryName;
                    r.Id = statistic.CategoryId;
                    r.Quantity = count;
                    StatisticList.Add(r);
                }
            }

            return StatisticList;
        }*/
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
        /*
                [HttpGet("/count_messages_by_category")]
                public async Task<IActionResult> CountMessagesByCategory()
                {
                    var map = new Dictionary<string, int>();

                    foreach (var line in _context.ThanksBoards.GroupBy(info => info.Category.CategoryName)
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
        */
        static private Dictionary<int, int> CountReceiverRating(List<ThanksBoard> list)
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

            var map1 = new Dictionary<int, int>();
            if (map.MaxBy(kvp => kvp.Value).Key <= 0 && map.MaxBy(kvp => kvp.Value).Value <= 0)
            {
                map1.Add(0, 0);
            }
            else
            {
                var key = map.MaxBy(kvp => kvp.Value).Key;
                var value = map.MaxBy(kvp => kvp.Value).Value;


                // var map1 = new Dictionary<int, int>();
                map1.Add(key, value);
            }

            return map1;

        }

        private int GetReceiverMessagesByCategoryId(int categoryId)
        {
            var Respon = new List<ThanksBoard>();
            try
            {
                Respon = (from q2 in _context.ThanksBoards
                          join q1 in _context.Categories on q2.CategoryId equals q1.Id
                          where q2.CategoryId == categoryId &&q2.IsActive==true
                          select q2).ToList();


                var map = CountReceiverRating(Respon).Keys.FirstOrDefault();
                return map;

            } catch
            {
                return 0;
            }

        }

        [HttpGet("/count_top_of_receiver_messages_by_category")]
        public List<CountReceiverResponse> CountTopOfReceiverMessagesByCategory()
        {
            List<CountReceiverResponse> list = new List<CountReceiverResponse>();
            var map = new Dictionary<int, int>();
            List<int> categories = (from e in _context.Categories select e.Id).ToList();
            foreach (var k in categories)
            {
                int key = CountReceiverMessagesByCategory(k).MaxBy(v => v.Value).Key;
                CountReceiverResponse c = new CountReceiverResponse();
                var emplImg = (from e in _context.Employees
                               where e.Id == key
                               select e)
                    .ToList();
                foreach (var empl in emplImg)
                {
                    c.EmployeeImg = Get(empl.FullName);

                }
                var categoryName = (from t in _context.ThanksBoards
                                    where t.CategoryId == k && t.ReceiverId == key && t.IsActive == true
                                    select t.Category.CategoryName)
                      .FirstOrDefault();
                if (categoryName != null)
                {
                    var count = (from t in _context.ThanksBoards
                                 where t.CategoryId == k && t.ReceiverId == key && t.IsActive == true
                                 select t.Category.CategoryName)
                         .Count();
                    c.Id = k;
                    c.CategoryName = categoryName;
                    c.Count = count;
                    list.Add(c);
                }
            }
            var list1 = (from l in list select l).OrderByDescending(x => x.Count).ToList();
            return list1;


        }

        private Dictionary<int, int> CountReceiverMessagesByCategory(int categoryId)
        {
            var map = new Dictionary<int, int>();
            var Respon = new List<ThanksBoard>();
            try
            {
                Respon = (from q2 in _context.ThanksBoards
                          join q1 in _context.Categories on q2.CategoryId equals q1.Id
                          where q2.CategoryId == categoryId
                          select q2).ToList();
                map = CountReceiverRating(Respon);



                return map;
            }
            catch
            {
                var d = new Dictionary<int, int>();
                d.Add(0, 0);
                return d;
            }
        }

        [HttpGet("/get_top_of_employee")]
        public async Task<IActionResult> CountReceiverResAsync(int input)
        {
           // var categoryId = (from c in _context.Categories where c.CategoryName == input select c.Id).FirstOrDefault();
            List<TopReceiver> tr = new List<TopReceiver>();
                var list = (from q2 in _context.ThanksBoards where q2.IsActive==true
                            select q2).ToList();
                var map = new Dictionary<int, int>();
                foreach (var line in list.GroupBy(info => info.ReceiverId)
                                 .Select(group => new
                                 {
                                     ReceiverId = group.Key,
                                     Count = group.Count()
                                 }))
                    map.Add((int)line.ReceiverId, line.Count);
                var sorted = map.OrderByDescending(r => r.Value)
                     .ThenBy(r => r.Key)
                     .Take(10);
                var topList = new List<object>();
                var categories = (from cat in _context.Categories select cat.Id).ToList();
                foreach (var item in sorted)
                {
                    TopReceiver t = new TopReceiver();
                    var d = new Dictionary<int, int>();
                    var employees = await _context.Employees.Where(e => e.Id == item.Key).FirstOrDefaultAsync();
                    t.Id = employees.Id;
                    t.FullName = employees.FullName;
                    t.PersonnelNumber = employees.PersonnelNumber;
                    t.OfficeNumber = employees.OfficeNumber;
                    t.DirectPhone = employees.DirectPhone;
                    t.Phone = employees.Phone;
                    t.Company = employees.Company;
                    t.Department = employees.Department;
                    t.PositionTitle = employees.PositionTitle;
                    t.Company = employees.Company;
                    t.WhatsAppMobile = employees.WhatsAppMobile;
                    t.Mobile = employees.Mobile;
                    t.Email = employees.Email;
                    t.Status = employees.Status;
                    t.linkImg = Get(employees.FullName);
                t.IsLocal = employees.IsLocal;
                    foreach (var k in categories)
                    {
                        var count = await (from th in _context.ThanksBoards
                                           where th.ReceiverId == t.Id && th.CategoryId == k && th.IsActive==true
                                           select th)
                                                     .CountAsync();
                        d.Add(k, count);
                    }
                    t.Categories = d;

                    tr.Add(t);
                }
            List<TopReceiver> tr1 = new List<TopReceiver>();
            if (input!=0)
            {var dic = new Dictionary<int, int>();
                foreach (var r in tr)
                {
                    foreach (var c in r.Categories)
                    {
                        if (c.Key == input)
                        {
                            dic.Add(r.Id, c.Value);
                        }
                    }
                }
                var max = dic.OrderByDescending(r => r.Value)
                     .ThenBy(r => r.Key)
                     .Take(10);
                foreach (var m in max)
                {  var r = (from t in tr where t.Id == m.Key select t).FirstOrDefault();
                    tr1.Add(r);
                }
            }
            else { tr1 = tr; }
            
           // tr1 = tr;
            return Ok(tr1);

    }
}

   
}
    

