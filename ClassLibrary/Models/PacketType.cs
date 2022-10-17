using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DudesPlayer.Library.Models
{
    public enum PacketType
    {
        Unknown,
        UpdateAll,
        Play,
        Pause,
        Set,
        Time,
        Joke,
        Ping,
        Shake,
        Chat,
        LoginNew,
        Logout,
    }
}