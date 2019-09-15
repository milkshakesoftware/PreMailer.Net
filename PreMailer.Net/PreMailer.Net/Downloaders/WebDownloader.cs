using System;
using System.IO;
using System.Net;
using System.Text;

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
			{
				switch (response)
				{
					case HttpWebResponse httpWebResponse:
					{
						var charset = httpWebResponse.CharacterSet;
						var encoding = Encoding.GetEncoding(charset);
						using (var stream = httpWebResponse.GetResponseStream())
						using (var reader = new StreamReader(stream, encoding))
						{
							return reader.ReadToEnd();
						}
					}

					case FileWebResponse fileWebResponse:
					{
						using (var stream = fileWebResponse.GetResponseStream())
						using (var reader = new StreamReader(stream))
						{
							return reader.ReadToEnd();
						}
					}

					default:
						throw new NotSupportedException($"The Uri type is giving a response in unsupported type '{response.GetType()}'.");
				}
			}
		}
	}
}
