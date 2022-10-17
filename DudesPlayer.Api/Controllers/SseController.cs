using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DudesPlayer.Api.Models;
using DudesPlayer.Library;
using DudesPlayer.Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DudesPlayer.Api.Controllers
{
    [Route("sse/room")]
    [ApiController]
    public class SseController : ControllerBase
    {
        [HttpGet]
        public string GetEmpty()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Events.ToList().ToJson();
            }
        }

        [HttpGet("{token}")]
        public async Task Get(string token)
        {
            int counter = 0;
            UserModel myToken = new(token);

            using (ApplicationContext db = new ApplicationContext())
            {
                var sse = db.Events.Where(x => x.Token == token).FirstOrDefault();
                await HttpContext.SSEInitAsync();
                while (sse == null)
                {
                    await Task.Delay(2);
                    sse = db.Events.Where(x => x.Token == token).FirstOrDefault();
                    counter++;
                    if (counter > 100000)
                    {
                        return;
                    }
                }
                db.Events.Remove(sse);
                db.SaveChanges();
                await HttpContext.SSESendEventAsync(
                new SSEEvent(sse.Type, sse.DataJson)
                {
                    Id = sse.Id.ToString(),
                    Retry = 5
                }
                );
            }
        }

        private void CheckDisconnectedUsers(ApplicationContext db)
        {

        }
    }
}
