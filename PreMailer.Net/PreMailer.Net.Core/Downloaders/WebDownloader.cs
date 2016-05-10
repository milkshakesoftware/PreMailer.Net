using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace PreMailer.Net.Downloaders
{
	public class WebDownloader : IWebDownloader
	{
		private static IWebDownloader _sharedDownloader;

		public static IWebDownloader SharedDownloader
		{
			get
			{
				if (_sharedDownloader == null)
				{
					_sharedDownloader = new WebDownloader();
				}

				return _sharedDownloader;
			}
			set
			{
				_sharedDownloader = value;
			}
		}

		public string DownloadString(Uri uri)
		{
            using (var client = new HttpClient())
            {
                var data = client.GetStringAsync(uri).Result;
                return data;
            }
		}
	}
}