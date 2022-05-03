using ClassLibrary;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AspWebServer.Models
{
    public static class ServerEventHandler
    {
        public static void CreateUpdateEvent(string roomName)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var room = db.Rooms.Include(x => x.URLs).Include(x => x.Settings).Where(x => x.Name == roomName).FirstOrDefault();
                if (room != null)
                {
                    var users = room.GetUsers();
                    if (users != null&&users.Count!=0)
                    {
                        foreach (UserModel user in users)
                        {
                            db.Events.Add(new EventInfo()
                            {
                                Token = user.ToString(),
                                Type = PacketType.UpdateAll,
                                DataJson = room.ToJson(),
                                DateCreated = DateTime.Now,
                            });
                        }
                    }
                    else
                    {
                        db.Rooms.Remove(db.Rooms.Where(x => x.Id == room.Id).FirstOrDefault()!);
                    }
                }
                db.SaveChanges();
            }
        }

        public static void Ping()
        {
            Task.Factory.StartNew(async () =>
           {
               while (true)
               {
                   using (ApplicationContext db = new ApplicationContext())
                   {
                       var rooms = db.Rooms.ToList();

                       foreach (var room in rooms)
                       {
                           List<UserModel> users;

                           if (room.UsersJson != null)
                           {
                               try
                               {
                                   users = room.UsersJson!.ToObject<List<UserModel>>()!;
                               }
                               catch
                               {
                                   users = new List<UserModel>();
                               }
                           }
                           else
                           {
                               users = new List<UserModel>();
                           }

                           if (users != null && users.Count > 0)
                           {
                               foreach (var user in users.ToList())
                               {
                                   db.Events.Add(new EventInfo()
                                   {
                                       Token = user.ToString(),
                                       Type = PacketType.Ping,
                                       DataJson = "",
                                       DateCreated = DateTime.Now,
                                   });
                               }
                           }
                           else
                           {
                               db.Rooms.Remove(db.Rooms.Where(x => x.Id == room.Id).FirstOrDefault()!);
                               db.SaveChanges();
                           }
                       }
                       db.SaveChanges();
                   }
                   await Task.Delay(10000);
               }
           });
        }

        internal static void CheckUsers()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(15000);
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        if (db.Events != null && db.Events.Count() > 0)
                        {
                            foreach (var lastEvent in db.Events.ToArray())
                            {
                                if (lastEvent!.DateCreated < DateTime.Now.AddMinutes(-1))
                                {
                                    try
                                    {
                                        var token = new UserModel(lastEvent.Token);
                                        if (db.Rooms != null)
                                        {
                                            var room = db.Rooms.Where(x => x.Name == token.RoomName).FirstOrDefault();
                                            if (room != null)
                                            {
                                                List<UserModel> usersList = room.UsersJson!.ToObject<List<UserModel>>()!;

                                                usersList.Remove(usersList.Where(x => x.ToString() == token.ToString()).FirstOrDefault()!);

                                                room.UsersJson = usersList.ToJson();

                                                if (usersList.Count == 0)
                                                {
                                                    db.Rooms.Remove(db.Rooms.Where(x=>x.Id == room.Id).FirstOrDefault()!);
                                                    db.SaveChanges();
                                                }

                                                db.Events.Remove(lastEvent);
                                                db.SaveChanges();

                                                db.Rooms.Update(room);
                                                db.SaveChanges();

                                                CreateUpdateEvent(room.Name!);
                                            }
                                            else
                                            {
                                                db.Events.Remove(lastEvent);
                                                db.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            db.Events.Remove(lastEvent);
                                            db.SaveChanges();
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
            });

        }

        public static Responce Action(string value, JsonType jsonType, ActionType actionType, string roomName = "")
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    UserModel valueUser = new UserModel();

                    if (roomName == "")
                    {
                        valueUser = new UserModel(value);
                    }
                    else
                    {
                        valueUser.RoomName = roomName;
                    }

                    if (valueUser != null)
                    {
                        RoomInfo? room = db.Rooms.Include(x => x.Settings).Include(x => x.URLs).Where(x => x.Name == valueUser.RoomName).FirstOrDefault();

                        if (room != null)
                        {
                            Responce responce;
                            responce = room.Action(value, jsonType, actionType);
                            db.Rooms.Update(room);
                            db.SaveChanges();
                            db.Dispose();
                            CreateUpdateEvent(room.Name!);
                            return responce;
                        }
                        else
                        {
                            return new Responce() { State = ResponceStateCode.RoomNotFound, Reason = valueUser.RoomName };
                        }
                    }
                    else
                    {
                        return new Responce() { State = ResponceStateCode.UserNull };
                    }

                }
            }
            catch (Exception ex)
            {
                return new Responce() { State = ResponceStateCode.ErrorUnknown, Reason = value + " " + ex.Message + " ||| " + (ex.InnerException ?? ex).Message };
            }
        }

    }
}
