using System;

namespace PreMailer.Net.Downloaders
{
	public interface IWebDownloader
	{
		string DownloadString(Uri uri);
	}
}
