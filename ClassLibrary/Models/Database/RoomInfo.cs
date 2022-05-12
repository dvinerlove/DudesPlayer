using ClassLibrary.Models;
using Newtonsoft.Json;

namespace ClassLibrary.Models
{
    public enum RoomState
    {
        Play,
        Pause
    }

    public class RoomSettings
    {
        public int Id { get; set; }
        public URLModel? CurrentURL { get; set; }
        public long? CurrentTime { get; set; }
        public string? RoomId { get; set; }
        public double? Speed { get; set; }
        public RoomState? State { get; set; }

        public bool IsEqual(RoomSettings settings)
        {
            if (settings == null) return true;
            try
            {
                return settings.CurrentURL == CurrentURL &&
               settings.Speed == Speed &&
               settings.State == State &&
               settings.CurrentTime == CurrentTime;
            }
            catch
            {
                return true;
            }
        }
    }

    public enum URLType
    {
        link,
        youtube
    }
    public class URLModel
    {
        public int Id { get; set; }
        public string? RoomId { get; set; }
        public string? Url { get; set; }
        public string? Name { get; set; }
        public string? YTSource { get; set; }
        public URLType URLType { get; set; } = URLType.link;

        public string GetTitle()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return (Url ?? "").Split('\"').Last().Split('/').Last().Split('\\').Last();
            }
            else
            {
                return Name;
            }
        }
    }

    public class RoomInfo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<URLModel>? URLs { get; set; }
        public string? UsersJson { get; set; }
        public RoomSettings? Settings { get; set; }


        public List<UserModel> GetUsers()
        {
            if (UsersJson != null && UsersJson.Length > 0)
            {
                List<UserModel>? users = JsonConvert.DeserializeObject<List<UserModel>>(UsersJson);
                if (users != null)
                {
                    return users;
                }
                else
                {
                    return new List<UserModel>();
                }
            }
            else
            {
                return new List<UserModel>();
            }
        }
        public List<URLModel> GetURls()
        {
            return URLs ?? new List<URLModel>();
        }

        public UserModel? GetUser(string username)
        {
            List<UserModel> users = GetUsers();
            if (users.Count() > 0)
            {
                return users.Where(x => x.Username == username).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public void DeleteUser(string userName)
        {
            List<UserModel>? users = GetUsers();
            if (users.Count() > 0)
            {
                var user = GetUser(userName);
                if (user != null)
                {
                    users.Remove(user);
                }
            }
            UsersJson = users.ToJson();
        }

        public int UsersCount()
        {
            List<UserModel> users = GetUsers();
            if (users.Count() > 0)
            {
                return users.Count();
            }
            else
            {
                return 0;
            }
        }
        public override string ToString()
        {
            string str = "";

            str += $"\n\tROOM: {Name}";
            str += $"\n\tURLS: ";

            foreach (var item in GetURls())
            {
                str += $"\n\t\t{item.Url}";
            }

            str += $"\n\tUSERS: ";

            foreach (var item in GetUsers())
            {
                str += $"\n\t\t{item}";
            }
            str += $"\n";

            if (Settings != null)
            {
                str += $"\n\tSETTINGS: ";
                str += $"\n\t\tSpeed -{Settings.Speed}";
                str += $"\n\t\tState -{Settings.State}";
                str += $"\n\t\tCurrentURL -{Settings.CurrentURL}";
                str += $"\n\t\tCurrentTime -{Settings.CurrentTime}";
            }

            return str;
        }

        public Responce Action(string value, JsonType jsonType, ActionType actionType)
        {
            try
            {
                switch (jsonType)
                {
                    case JsonType.Users:
                        var users = GetUsers();
                        var valueUser = new UserModel(value);
                        if (valueUser != null)
                        {
                            if (valueUser.RoomName != Name)
                            {
                                return new Responce() { State = ResponceStateCode.InvalidRoom, Reason = valueUser.RoomName };
                            }
                            switch (actionType)
                            {
                                case ActionType.Remove:
                                    users.Remove(users.Where(x => x.Username == valueUser.Username).FirstOrDefault()!);
                                    break;
                                case ActionType.Add:
                                    if (users.Count == 0 || users.Where(x => x.Username == valueUser.Username).FirstOrDefault() == null)
                                    {
                                        users.Add(valueUser);
                                    }
                                    else
                                    {
                                        return new Responce()
                                        {
                                            State = ResponceStateCode.AlreadyExists,
                                            Reason = users.Count.ToString() +
                                            " " + (users.Where(x => x.Username == valueUser.Username).FirstOrDefault() == null).ToString()
                                        };
                                    }
                                    break;
                            }
                        }
                        UsersJson = users.ToJson();
                        break;
                    case JsonType.Urls:
                        var urls = GetURls();
                        var newUrl = value.ToObject<URLModel>();
                        if (newUrl != null)
                        {
                            switch (actionType)
                            {
                                case ActionType.Remove:
                                    urls.Remove(urls.Where(x => x.Id == newUrl.Id).FirstOrDefault());
                                    break;
                                case ActionType.Add:
                                    urls.Add(newUrl);
                                    break;
                            }
                        }
                        else
                        {
                            return new Responce() { State = ResponceStateCode.ErrorUnknown, Reason = value };
                        }
                        URLs = urls;
                        break;
                    default:
                        break;
                }
                return new Responce() { State = ResponceStateCode.Success, Reason = this.ToJson() };

            }
            catch (Exception ex)
            {
                return new Responce() { State = ResponceStateCode.ErrorUnknown, Reason = value + " " + ex.Message };
            }
        }
    }
}
