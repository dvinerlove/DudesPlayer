using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DudesPlayer.Models
{
    public enum SoundType
    {
        chat,
        login,
        logout
    }
    public class SoundHandler
    {
        private static SoundPlayer player;
        public static void Play(SoundType type)
        {
            Task.Factory.StartNew(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    switch (type)
                    {
                        case SoundType.chat:
                            player = new SoundPlayer("Sounds\\ChatAlert.wav");
                            break;
                        case SoundType.login:
                            player = new SoundPlayer("Sounds\\LoginAlert.wav");
                            break;
                        case SoundType.logout:
                            player = new SoundPlayer("Sounds\\LogoutAlert.wav");
                            break;
                        default:
                            break;
                    }
                    player.Load();

                    player.Play();
                });
            });

        }
    }
}
