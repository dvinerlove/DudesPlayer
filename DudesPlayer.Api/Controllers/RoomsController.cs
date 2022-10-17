using DudesPlayer.Library;
using DudesPlayer.Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DudesPlayer.Api.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(ILogger<RoomsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}")]
        public IEnumerable<RoomInfo> Get(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var rooms = db.Rooms.Where(x => x.Name == id.ToString()).ToList();
                return rooms;
            }
        }

        //get rooms
        [HttpGet]
        public IEnumerable<RoomInfo> Get()
        {

            using (ApplicationContext db = new ApplicationContext())
            {
                var users = db.Rooms.Include(x => x.Settings).Include(x => x.URLs).ToList();
                return users;
            }
        }



        //create room
        [HttpPost]
        public Responce Post()
        {
            var rng = new Random();
            var room = new RoomInfo();
            var settings = new RoomSettings();
            string id = rng.Next(1000, 9999).ToString();
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    while (db.Rooms.Where(x => x.Name == id).FirstOrDefault() != null)
                    {
                        id = rng.Next(1000, 9999).ToString();
                    }
                    room.Name = id;
                    settings.RoomId = id;
                    settings.Speed = 1.0;
                    settings.CurrentTime = 0;
                    settings.CurrentURL = null;
                    settings.State = RoomState.Pause;
                    db.Settings.Add(settings);
                    room.Settings = settings;
                    room.URLs = new List<URLModel>();
                    db.Rooms.Add(room);
                    db.SaveChanges();
                }
                return new Responce() { Reason = room.ToJson(), State = ResponceStateCode.Success };
            }
            catch (Exception ex)
            {
                return new Responce() { Reason = ex.Message, State = ResponceStateCode.ErrorUnknown };
            }
        }
    }
}