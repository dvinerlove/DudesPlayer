using DudesPlayer.Library;
using DudesPlayer.Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DudesPlayer.Api.Models
{
    public static class Cleaner
    {
        public static void CreateUpdate(string roomName)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    var room = db.Rooms.Include(x => x.URLs).Include(x => x.Settings).Where(x => x.Name == roomName).FirstOrDefault();
                    if (room != null)
                    {
                        var users = room.GetUsers();
                        Debug.WriteLine($"CreateUpdate: Create update in room {room.Name} for {users.Count} users");
                        if (users != null && users.Count != 0)
                        {
                            foreach (UserModel user in users.ToList())
                            {
                                db.Events.Add(new EventInfo()
                                {
                                    Token = user.ToString(),
                                    Type = PacketType.UpdateAll,
                                    DataJson = room.ToJson(),
                                    DateCreated = DateTime.Now,
                                });
                            }
                            Debug.WriteLine($"CreateUpdate: update created in {roomName} for {users.Count} users");
                        }
                        else
                        {
                            Debug.WriteLine($"CreateUpdate: room {roomName} removed");
                            db.Rooms.Remove(db.Rooms.Where(x => x.Id == room.Id).FirstOrDefault()!);
                        }
                    }
                    db.SaveChanges();
                    Debug.WriteLine($"CreateUpdate: SaveChanges");
                }
                catch (Exception ex)
                {

                    Debug.WriteLine($"CreateUpdate: exception {ex.Message}");
                    Debug.WriteLine($"CreateUpdate: trace {ex.StackTrace}");
                }

            }
        }

        public static void StartPing()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        try
                        {
                            var rooms = db.Rooms.ToList();
                            if (rooms.Count > 0)
                                Debug.WriteLine($"CreatePing: active rooms count {rooms.Count}");
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
                                    Debug.WriteLine($"CreatePing: Ping {users.Count} users in room {room.Name}");
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
                                    Debug.WriteLine($"CreatePing: room {room.Name} removed");
                                    db.Rooms.Remove(db.Rooms.Where(x => x.Id == room.Id).FirstOrDefault()!);
                                    db.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"CreatePing: exception {ex.Message}");
                            Debug.WriteLine($"CreatePing: trace {ex.StackTrace}");
                        }

                        db.SaveChanges();
                    }
                    await Task.Delay(10000);
                }
            });
        }
        public static void StartCleanUsers()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000);
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        if (db.Events != null && db.Events.Count() > 0)
                        {
                            Debug.WriteLine($"CheckUsers: new afk events {db.Events.Count()}");
                            foreach (var lastEvent in db.Events.ToArray())
                            {
                                if (lastEvent!.DateCreated < DateTime.Now.AddSeconds(-20))
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

                                                Debug.WriteLine($"CheckUsers: user {token.Username} in {token.RoomName} removed for afk");

                                                var events = db.Events.ToArray().Where(x =>
                                                {
                                                    DudesPlayer.Library.Models.UserModel newToken = new UserModel(x.Token);
                                                    return newToken.Username == token.Username;
                                                });

                                                try
                                                {
                                                    //foreach (var item in events)
                                                    //{
                                                    //        db.Events.Remove(item);
                                                    //}
                                                }
                                                catch (Exception ex)
                                                {
                                                    Debug.WriteLine("CheckUsers: exception " + ex.Message);
                                                }

                                                if (usersList.Count == 0)
                                                {
                                                    Debug.WriteLine($"CheckUsers: room {room.Name} removed");

                                                    //db.Rooms.Remove(db.Rooms.Where(x => x.Id == room.Id).FirstOrDefault()!);
                                                }
                                                else
                                                {
                                                    db.Rooms.Update(room);
                                                }

                                                db.Events.Remove(lastEvent);
                                                db.SaveChanges();

                                                CreateUpdate(room.Name!);
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
                                        Debug.WriteLine($"CheckUsers: SaveChanges");

                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine($"CheckUsers: exception {ex.Message}");
                                        Debug.WriteLine("");
                                        Debug.WriteLine($"CheckUsers: trace {ex.StackTrace}");
                                        Debug.WriteLine("");
                                        Debug.WriteLine($"CheckUsers: trace {ex.InnerException}");
                                        Debug.WriteLine("");
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }
    }

}
