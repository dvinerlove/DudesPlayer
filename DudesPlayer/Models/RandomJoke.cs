using ClassLibrary.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DudesPlayer.Models
{
    public static class RandomJoke
    {
        static Random Random = new Random();
        public static Joke Generate()
        {
            var file = GetResourceStream("jokes.json");
            string all = "";

            using (var reader = new StreamReader(file))
            {
                all = reader.ReadToEnd();
            }
            List<Joke> jokes = JsonConvert.DeserializeObject<List<Joke>>(all);
            int id = Random.Next(jokes.Count - 1);
            while (jokes[id].type == "программирование")
            {
                id = Random.Next(jokes.Count - 1);
            }
            return jokes[id];
        }
        static UnmanagedMemoryStream GetResourceStream(string resName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var strResources = assembly.GetName().Name + ".g.resources";
            var rStream = assembly.GetManifestResourceStream(strResources);
            var resourceReader = new System.Resources.ResourceReader(rStream);
            var items = resourceReader.OfType<System.Collections.DictionaryEntry>();
            var stream = items.First(x => (x.Key as string) == resName.ToLower()).Value;
            return (UnmanagedMemoryStream)stream;
        }
    }
}
