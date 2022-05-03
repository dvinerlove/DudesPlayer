using System;
using System.Diagnostics;

namespace DudesPlayer.Models.Client
{
    public static class VDebug
    {
        static public event EventHandler DebugEvent;

        public static void WriteLine(object str)
        {
            if (str == null)
            {
                str = "null";
            }
            str = str.ToString().Replace("|", "\t");
            DebugEvent?.Invoke(str, EventArgs.Empty);
            Debug.WriteLine(str);
        }
    }
}
