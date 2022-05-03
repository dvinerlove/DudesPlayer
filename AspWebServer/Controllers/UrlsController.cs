using AspWebServer.Models;
using ClassLibrary;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspWebServer.Controllers
{
    [Route("api/urls")]
    [ApiController]
    public class UrlsController : ControllerBase
    {
        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public List<URLModel>? Get(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                RoomInfo? room = db.Rooms.Include(x => x.URLs).Include(x => x.Settings).Where(x => x.Name == id.ToString()).FirstOrDefault();
                if (room != null && room.URLs != null)
                {
                    return room.URLs;
                }
                else
                {
                    return new List<URLModel>();
                }
            }
        }

        [HttpGet]
        public string? Get()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var users = db.Rooms.Include(x => x.Settings).Include(x => x.URLs).ToList();
                }
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        // PUT api/<ValuesController>/5
        [HttpPost("{id}")]
        public Responce Put(int id, [FromBody] URLModel value)
        {
            value.RoomId = id.ToString();
            return ServerEventHandler.Action(value.ToJson(), JsonType.Urls, ActionType.Add, id.ToString());
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public Responce Delete(int id, [FromBody] URLModel value)
        {
            value.RoomId = id.ToString();
            return ServerEventHandler.Action(value.ToJson(), JsonType.Urls, ActionType.Remove, id.ToString());
        }
    }
}
