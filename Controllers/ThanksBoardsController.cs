using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hbk.Data;
using hbk.Models;
using hbk.Models.Requests.ThanksBoard;
using Microsoft.AspNetCore.Http;
//using System.Collections.Generic;
using hbk.Models.Requests.Employees;
//using NuGet.Packaging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;
using System;

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
        // GET: api/ThanksBoards
        [HttpGet("/get_info_list")]
        public async Task<ActionResult<ThanksBoard>> GetInfoList()
        {
            var infoList = await (from i in _context.ThanksBoards
                                  select new { id = i.Id,
                                      senderImgUrl = i.Sender.linkImg,
                                      senderName = i.Sender.FullName,
                                      receiverImgUrl = i.Receiver.linkImg,
                                      receiverName = i.Receiver.FullName,
                                      message = i.Message,
                                      categoryImgUrl = i.Category.CategoryImg,
                                      categoryName = i.Category.CategoryName,
                                      date = i.DateReceived })
                                  .ToListAsync();
            return Ok(infoList);

        }

        [HttpGet("/search")]
        public async Task<IEnumerable<Employee>> Search(string name)
        {
            IQueryable<Employee> query = _context.Employees;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.FullName.Contains(name) || e.PositionTitle.Contains(name));

            }


            return await query.ToListAsync();
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

           
            ThanksBoard thanksBoard = new ThanksBoard();

           // var k = Search(sendMessageRequest.ReceiverName);

            

                thanksBoard.SenderId = sendMessageRequest.SenderId;
            thanksBoard.ReceiverId = sendMessageRequest.ReceiverId;
            thanksBoard.DateReceived = sendMessageRequest.SendTime;
            thanksBoard.CategoryId = sendMessageRequest.CategoryId;
            thanksBoard.Message = sendMessageRequest.Message;


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
        /*     [HttpGet("/count_sender_opportunity")]
            private async Task<IActionResult>  CountSenderOpportunity(string PersonnelNumber)
           {
               int senderId = (from q1 in _context.Employees
                                   // join q2 in _context.ThanksBoards on q1.Id equals q2.SenderId
                               where q1.PersonnelNumber == PersonnelNumber
                               select q1.Id).FirstOrDefault();

               int count = 0;
               foreach (var th in _context.ThanksBoards)
               {
                   if (th.SenderId==senderId && DateTime.Now.Day - th.DateReceived.Day <= 30)
                   {
                       count++;

                   }
                   else
                   {

                       BadRequest("У вас не осталось возможности ");
                       // break;
                   }
               }

               return Ok(count);
           }*/

        /*     [HttpGet("/count_sender_opportunity")]
             public async Task<IActionResult> CountOpportunity()
             {
                 var list = new List<int>();

                 foreach (var e in _context.Employees)
                 {
                     list.Add(await CountSenderOpportunity(e.PersonnelNumber));


                 }




                     return Ok();

                 // continue;

         }*/

        [HttpGet("/receivered_messsages_history")]
        public async Task<IActionResult> ReceiveredMessages(string PersonnelNumber)
        {
            var category = (from q1 in _context.Categories select q1.Id).ToList();
            var ReceiveredMessageList = new List<ReceiveredMessagesResponse>();

            foreach (var c in category)
            {
                var receivered = await (from q2 in _context.ThanksBoards
                                        where q2.Receiver.PersonnelNumber == PersonnelNumber && q2.CategoryId == c
                                        select new { id = q2.Id, date = q2.DateReceived,
                                            receiverImg = q2.Receiver.linkImg,
                                            receiverName = q2.Receiver.FullName,
                                            categoryImg = q2.Category.CategoryImg,
                                            categoryName = q2.Category.CategoryName,
                                            message = q2.Message }).FirstOrDefaultAsync();

                if (receivered != null)
                {
                    ReceiveredMessagesResponse r = new ReceiveredMessagesResponse();
                    r.Id = receivered.id;
                    r.DateReceived = receivered.date.ToUniversalTime();

                    r.ReceiverImgUrl = receivered.receiverImg;
                    r.ReceiverName = receivered.receiverName;
                    r.CategoryImg = receivered.categoryImg;
                    r.CategoryName = receivered.categoryName;
                    r.Message = receivered.message;

                    ReceiveredMessageList.Add(r);
                }
            }

            return Ok(ReceiveredMessageList);
        }

        [HttpGet("/sent_messsages_history")]
        public async Task<IActionResult> SentMessages(string PersonnelNumber)
        {
            var category = (from q1 in _context.Categories select q1.Id).ToList();
            var SentMessageList = new List<SentMessagesResponse>();

            foreach (var c in category)
            {
                var sent = await (from q2 in _context.ThanksBoards
                                  where q2.Receiver.PersonnelNumber == PersonnelNumber && q2.CategoryId == c
                                  select new
                                  {
                                      id = q2.Id,
                                      sentDate = q2.DateReceived,
                                      senderName = q2.Sender.FullName,
                                      senderImg = q2.Sender.linkImg,
                                      // receiverName = q2.Receiver.FullName,
                                      categoryImg = q2.Category.CategoryImg,
                                      categoryName = q2.Category.CategoryName,
                                      message = q2.Message
                                  }).FirstOrDefaultAsync();
                if (sent != null)
                {
                    SentMessagesResponse r = new SentMessagesResponse();
                    r.Id = sent.id;
                    r.DateReceived = sent.sentDate;
                    r.SenderImgUrl = sent.senderImg;
                    r.SenderName = sent.senderName;
                    r.CategoryImg = sent.categoryImg;
                    r.CategoryName = sent.categoryName;
                    r.Message = sent.message;
                    SentMessageList.Add(r);
                }

            }
            return Ok(SentMessageList);
        }

        [HttpGet("/get_top_of_receiver")]
        public async Task<IActionResult> GetReceiver(string category)
        {
            var l = await (from t in _context.Categories
                           where t.CategoryName.ToLower().Equals(category.ToLower())
                           select t.Id)
                          .FirstOrDefaultAsync();

            int key = CountReceiverMessagesByCategory(l).MaxBy(v => v.Value).Key;
           
            var top = await (from e in _context.Employees

                                 where e.Id == key
                                 select e)
                .FirstOrDefaultAsync();
        
            return Ok(top);

    } 

        [HttpGet("/get_top_of_statistic")]
        public async Task<IActionResult> CountStatistic(string categoryName)
        {
            
            var category = (from q1 in _context.Categories select q1.Id).ToList();
            var StatisticList = new List<StatisticResponse>();
            var l = await (from t in _context.Categories
                           where t.CategoryName.ToLower().Equals(categoryName.ToLower())
                           select t.Id)
                         .FirstOrDefaultAsync();

            int key = CountReceiverMessagesByCategory(l).MaxBy(v => v.Value).Key;

            var top = await (from e in _context.Employees
                             where e.Id == key
                             select e.Id)
                .FirstOrDefaultAsync();
            foreach (var c in category)
            {
              
                var statistic = await (from q2 in _context.ThanksBoards
                                       where q2.ReceiverId==top&& q2.CategoryId == c
                                       select new
                                       {
                                           categoryImg = q2.Category.CategoryImg,
                                           categoryName = q2.Category.CategoryName,
                                       }).FirstOrDefaultAsync();

                var count = await (from q2 in _context.ThanksBoards
                                   where q2.ReceiverId == top && q2.CategoryId == c
                                   select q2).CountAsync();
                if (statistic != null)
                {
                    StatisticResponse r = new StatisticResponse();
                    r.CategoryImg = statistic.categoryImg;
                    r.CategoryName = statistic.categoryName;

                    r.Quantity = count;
                    StatisticList.Add(r);
                }
            }
            return Ok(StatisticList);
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
            if (map.MaxBy(kvp => kvp.Value).Key <= null && map.MaxBy(kvp => kvp.Value).Value <= null)
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
                                where q2.CategoryId == categoryId
                                select q2).ToList();


                var map = CountReceiverRating(Respon).Keys.FirstOrDefault();
                return map;
                
            }catch 
            {
                return 0;
            }
            
            }
        
        [HttpGet("/count_top_of_receiver_messages_by_category")]
        public async Task<ActionResult<IList<CountReceiverResponse>>> CountTopOfReceiverMessagesByCategory()
        {
            List<CountReceiverResponse> list = new List<CountReceiverResponse>();


            var map = new Dictionary<int, int>();

            // var list = new List<int>();
            List<int> categories = (from e in _context.Categories select e.Id).ToList();

            foreach (var k in categories)
            {
                int key = CountReceiverMessagesByCategory(k).MaxBy(v => v.Value).Key;
                CountReceiverResponse c = new CountReceiverResponse();
                var emplImg = await (from e in _context.Employees
                                     where e.Id == key
                                     select e.linkImg)
                    .FirstOrDefaultAsync();
                var categoryName = await (from t in _context.ThanksBoards
                                          where t.CategoryId == k
                                          select t.Category.CategoryName)
                      .FirstOrDefaultAsync();
                if (emplImg != null && categoryName != null)
                {

                    c.EmployeeImg = emplImg;
                    c.CategoryName = categoryName;
                    c.Count = CountReceiverMessagesByCategory(k).MaxBy(v => v.Value).Value;
                    c.Id = k;
                    list.Add(c);
                }
                else { break; }

            }
            return Ok(list);


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
                return new Dictionary<int, int>(d);
            }
        }

        /*
                [HttpGet("/get_top_of_receiver_messages_by_categoryi")]
                public async Task<ActionResult<IList<HistoryOfTopReceiverMessages>>> GetReceiverMessagesByCategoryi()
                {
                    List<int> categories = await (from e in _context.Categories select e.Id).ToListAsync();
                        List<HistoryOfTopReceiverMessages> historyList = new List<HistoryOfTopReceiverMessages>();
                    List<HistoryForThanksBoard> historyThanksBoards = new List<HistoryForThanksBoard>();
                    foreach (var c in categories)
                    {
                        var employee = await (from e in _context.Employees
                                              join th in _context.ThanksBoards on e.Id equals th.ReceiverId
                                              where e.Id == GetReceiverMessagesByCategoryId(c) && th.CategoryId == c
                                              select new
                                              {
                                                  eImg = e.linkImg,
                                                  eFullname = e.FullName,
                                                  ePosition = e.PositionTitle,
                                                  eEmail = e.Email,
                                                  ePhone = e.Phone,
                                                  Message = new
                                                  {
                                                      ReceiverId = th.Receiver.Id,
                                                      senderName = th.Sender.FullName,
                                                      Category = new { categoryName = th.Category.CategoryName },
                                                      Messages = th.Message,
                                                      sendTime = th.DateReceived
                                                  }
                                              })
                                           .FirstOrDefaultAsync();
                        if (employee != null)
                        {
                            HistoryOfTopReceiverMessages h = new HistoryOfTopReceiverMessages();
                            h.EId = employee.Message.ReceiverId;
                            h.EImg = employee.eImg;
                            h.EFullName = employee.eFullname;
                            h.EPositionTitle = employee.ePosition;
                            h.EPhone = employee.ePhone;
                            h.Email = employee.eEmail;
                            historyList.Add(h);

                                h.Messages = historyThanksBoards;
                                HistoryForThanksBoard t = new HistoryForThanksBoard();
                                t.SenderName = employee.Message.senderName;
                                t.CategoryName = employee.Message.Category.categoryName;
                                t.Message = employee.Message.Messages;
                                t.DataReceived = employee.Message.sendTime;
                                t.ReceiverId = employee.Message.ReceiverId;


                            foreach (var his in historyList)
                            {
                                if (his.EId == t.ReceiverId)
                                {
                                    t.HistoryOfTopReceiverMessages = h;
                                    historyThanksBoards.Add(t);
                                    h.Messages = historyThanksBoards;
                                }
                            }
                           // t.HistoryOfTopReceiverMessages = h;

                            historyList.Add(h);
                        }
                    }

                  return Ok(historyList);

                }

             */

       
    }

   
}
    

