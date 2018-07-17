using System;
using System.IO;
using System.Net;

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
			var request = WebRequest.Create(uri);
			using (var response = request.GetResponse())
			using (var stream = response.GetResponseStream())
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}
}