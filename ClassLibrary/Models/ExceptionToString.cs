using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public static class ExceptionToString
    {
        public static  string ToMessageString(this Exception ex)
        {
            return ex.Message + " | " + ex.InnerException ?? "";
        }
    }
}
