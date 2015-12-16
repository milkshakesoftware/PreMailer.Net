using System;
using System.IO;
using System.Net;

namespace PreMailer.Net.Downloaders
{
	internal class WebDownloader : IWebDownloader
	{
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