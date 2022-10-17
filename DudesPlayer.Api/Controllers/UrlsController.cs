using DudesPlayer.Api.Models;
using DudesPlayer.Library;
using DudesPlayer.Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DudesPlayer.Api.Controllers
{
    [Route("api/urls")]
    [ApiController]
    public class UrlsController : ControllerBase
    {
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

        [HttpPost("{id}")]
        public Responce Post(int id, [FromBody] URLModel value)
        {
            value.RoomId = id.ToString();
            return ServerEventHandler.Action(value.ToJson(), JsonType.Urls, ActionType.Add, id.ToString());
        }

        [HttpDelete("{id}")]
        public Responce Delete(int id, [FromBody] URLModel value)
        {
            value.RoomId = id.ToString();
            return ServerEventHandler.Action(value.ToJson(), JsonType.Urls, ActionType.Remove, id.ToString());
        }
    }
}
