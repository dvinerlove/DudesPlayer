using AspWebServer.Models;
using ClassLibrary;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspWebServer.Controllers
{
    [Route("api/commands")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        [HttpPost("{token}/set")]
        public Responce Set(string token, [FromBody] URLModel url)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Settings.Where(x => x.RoomId == new UserModel(token).RoomName).FirstOrDefault()!.CurrentURL = url;
                db.SaveChanges();
            }
            return CreateEvent(token, url.ToJson(), PacketType.Set);
        }

        [HttpPost("{token}/play")]
        public Responce Play(string token)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Settings.Where(x => x.RoomId == new UserModel(token).RoomName).FirstOrDefault()!.State = RoomState.Play;
                db.SaveChanges();
            }
            return CreateEvent(token, null, PacketType.Play);
        }
        [HttpPost("{token}/pause")]
        public Responce Pause(string token)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Settings.Where(x => x.RoomId == new UserModel(token).RoomName).FirstOrDefault()!.State = RoomState.Pause;
                db.SaveChanges();
            }

            return CreateEvent(token, null, PacketType.Pause);
        }

        [HttpPost("{token}/time")]
        public Responce Time(string token, [FromBody] string value)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Settings.Where(x => x.RoomId == new UserModel(token).RoomName).FirstOrDefault()!.CurrentTime = long.Parse(value);
                db.SaveChanges();
            }
            return CreateEvent(token, value, PacketType.Time);
        }


        [HttpPost("{token}/joke")]
        public Responce Joke(string token, [FromBody] string value)
        {
            return CreateEvent(token, value, PacketType.Joke);
        }

        [HttpPost("{token}/shake")]
        public Responce Shake(string token)
        {
            return CreateEvent(token, null, PacketType.Shake);
        }

        [HttpGet("{token}/shake")]
        public Responce ShakeGet(string token)
        {
            return CreateEvent(token, null, PacketType.Shake);
        }

        public Responce CreateEvent(string token, string? data, PacketType type)
        {
            var user = new UserModel(token);
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var room = db.Rooms.Include(x => x.URLs).Include(x => x.Settings).Where(x => x.Name == user.RoomName).First();
                    if (room != null)
                    {
                        SSEEvent? sse = new SSEEvent();
                        foreach (var item in room.GetUsers())
                        {
                            var @event = new EventInfo()
                            {
                                Token = item.ToString(),
                                Type = type,
                                DataJson = data ?? "",
                                DateCreated = DateTime.Now,
                            };
                            db.Events.Add(@event);

                            sse = new SSEEvent(@event.Type, @event.DataJson)
                            {
                                Id = @event.Id.ToString(),
                                Retry = 5
                            };
                        }
                        if (sse != null)
                        {
                            SocketServer.Send(user.Username, $"/{room.Name}", sse.ToJson());
                            //WSServer.Send($"/{room.Name}", sse.ToJson());
                        }
                        db.SaveChanges();



                    }

                }



                return new Responce() { State = ResponceStateCode.Success };
            }
            catch (Exception ex)
            {
                return new Responce() { State = ResponceStateCode.ErrorUnknown, Reason = ex.ToMessageString() };
            }
        }
    }
}
