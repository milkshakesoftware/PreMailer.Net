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
			using (var response = (HttpWebResponse)request.GetResponse()) {
				string Charset = response.CharacterSet;
				Encoding encoding = Encoding.GetEncoding( Charset );
				using( var stream = response.GetResponseStream( ) )
				using( var reader = new StreamReader( stream, encoding ) ) {
					return reader.ReadToEnd( );
				}
			}
		}
	}
}
