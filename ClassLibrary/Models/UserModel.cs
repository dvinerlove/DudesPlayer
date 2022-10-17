using Newtonsoft.Json;
using System.Text;

namespace DudesPlayer.Library.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string RoomName { get; set; }
        public string AvatarId { get; set; }

        public UserModel()
        {
            Username = "";
            RoomName = "";
            AvatarId = ""; ;
        }

        public UserModel(string username, string room,string avatarId)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            RoomName = room ?? throw new ArgumentNullException(nameof(room));
            AvatarId = avatarId ?? throw new ArgumentNullException(nameof(avatarId));
        }

        public UserModel(string base64)
        {
            string tokenJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64 ?? throw new ArgumentNullException(nameof(base64))));
            var token = JsonConvert.DeserializeObject<UserModel>(tokenJson) ?? throw new ArgumentNullException(nameof(tokenJson));
            if (token != null)
            {
                Username = token.Username ?? throw new ArgumentNullException(nameof(token.Username));
                RoomName = token.RoomName ?? throw new ArgumentNullException(nameof(token.RoomName));
                AvatarId = token.AvatarId ?? throw new ArgumentNullException(nameof(token.AvatarId));
            }
            else
            {
                throw new ArgumentNullException(nameof(token));
            }
        }
        public override string ToString()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
        }
    }
}
