//using DudesPlayer.Api.Models;
//using ClassLibrary;
//using ClassLibrary.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace DudesPlayer.Api.Controllers
//{
//    [Route("api/files")]
//    [ApiController]
//    public class FilesController : ControllerBase
//    {
//        [HttpGet]
//        public List<URLModel>? Get(int id)
//        {
//            List<URLModel> urls = new List<URLModel>();
//            if (Directory.Exists(@"wwwroot\videos") == false)
//            {
//                return urls;
//            }
//            string[] filePaths = Directory.GetFiles(@"wwwroot\videos");
//            foreach (var item in filePaths)
//            {
//                urls.Add(new URLModel() { Url = item.Substring(7).Replace(@"\\", @"\") });
//            }
//            return urls;
//        }
//    }
//}
