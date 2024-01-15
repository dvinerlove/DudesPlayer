using DudesPlayer.Api.Models;
using DudesPlayer.Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DudesPlayer.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        [HttpGet("{id}")]
        public List<UserModel>? Get(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                RoomInfo? room = db.Rooms.Include(x => x.URLs).Include(x => x.Settings).Where(x => x.Name == id.ToString()).FirstOrDefault();
                if (room != null && room.UsersJson != null)
                {
                    return room.GetUsers();
                }
                else
                {
                    return new List<UserModel>();
                }

            }
        }

        // PUT api/<ValuesController>/5
        [HttpPost]
        public Responce Put([FromBody] string value)
        {
            return ServerEventHandler.Action(value, JsonType.Users, ActionType.Add);
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete]
        public Responce Delete([FromBody] string value)
        {
            return ServerEventHandler.Action(value, JsonType.Users, ActionType.Remove);
        } 
    }
}
