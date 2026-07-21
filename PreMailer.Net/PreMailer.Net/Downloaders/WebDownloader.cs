using PreMailer.Net.Extensions;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace PreMailer.Net.Downloaders
{
	public class WebDownloader : IWebDownloader
	{
		private static IWebDownloader _sharedDownloader;
		private const string CssMimeType = "text/css";
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
			request.Headers.Add(HttpRequestHeader.Accept, CssMimeType);

			using (var response = request.GetResponse())
			{
				// We only support this operation for CSS file/content types coming back
				// from the response. If we get something different, throw with the unsupported
				// content type in the message
				if(response.ParseContentType() != CssMimeType)
					throw new NotSupportedException($"The Uri type is giving a response in unsupported content type '{response.ContentType}'.");

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
