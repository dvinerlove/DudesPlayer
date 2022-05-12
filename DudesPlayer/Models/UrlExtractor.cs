using ClassLibrary;
using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.Grabbed;
using DudesPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DudesPlayer.Models
{
    public static class UrlExtractor
    {
        private static IMultiGrabber GetGrabber()
        {
            return GrabberBuilder.New()
            .UseDefaultServices()
            .AddYouTube().AddVimeo()
            .Build();
        }
        public static Uri GetVideo(string url, string quality)
        {
            IMultiGrabber grabber = GetGrabber();

            var result = grabber.GrabAsync(new Uri(url));

            GrabbedMedia item = result.Result.Resources<GrabbedMedia>()
            .Where(m => m.Channels == MediaChannels.Video).Where(x => x.Resolution.Contains(quality))
            .ToList().FirstOrDefault();

            if (item != null)
            {
                VDebug.WriteLine(item.ResourceUri + "\n" 
                    + item.FormatTitle + "\n" 
                    + item.Container + "\n" 
                    + item.BitRateString + "\n" 
                    + item.Resolution + "\n" 
                    + item.GetBitRate() + "\n" 
                    + item.Length);
                VDebug.WriteLine("");
            }
            return item.ResourceUri;
        }
        public static Uri GetAudio(string url)
        {
            IMultiGrabber grabber = GetGrabber();

            var result = grabber.GrabAsync(new Uri(url));

            

            int bitrate = 0;
            GrabbedMedia media= new GrabbedMedia();
            foreach (var item in result.Result.Resources<GrabbedMedia>()
            .Where(m => m.Channels == MediaChannels.Audio).ToList())
            {
                int newbitrate = int.Parse(item.BitRateString.Replace("k", ""));
                if (newbitrate > bitrate)
                {
                    bitrate = newbitrate;
                    media = item;
                }
            }
            if (media != null)
            {
                VDebug.WriteLine(media.ResourceUri + "\n" 
                    + media.FormatTitle + "\n" + media.Container + "\n" 
                    + media.BitRateString + "\n" + media.Resolution + "\n" 
                    + media.Length);
                VDebug.WriteLine("");
            }
            return media.ResourceUri;
        }
    }
}
