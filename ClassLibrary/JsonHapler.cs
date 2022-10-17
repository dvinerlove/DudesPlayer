using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DudesPlayer.Library
{
    public static class JsonHapler
    {
        public static string ToJson(this object source)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(source);
        }
        public static object? ToObject(this string source, Type type)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(source, type);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        public static T? ToObject<T>(this string source)
        {
            try
            {
                return (T?)Newtonsoft.Json.JsonConvert.DeserializeObject(source, typeof(T));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return default(T);
            }
        }
    }
}
