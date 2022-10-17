using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace DudesPlayer.Api.Models
{

    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public List<string> Get()
        {
            List<string> strs = new List<string>();
            try
            {
                var result = WSServer.Start();
                strs.AddRange(result);

            }
            catch (Exception ex)
            {
                strs.Add(ex.Message);

            }
            strs.Add("");
            strs.Add("");

            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                strs.Add("Name: " + netInterface.Name);
                strs.Add("Description: " + netInterface.Description);
                strs.Add("Addresses: ");
                IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                {
                    strs.Add(" " + addr.Address.ToString());
                }
                strs.Add("");
            }

            return strs;
        }
    }
}
