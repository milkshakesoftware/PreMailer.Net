using System;
using System.Net;

namespace PreMailer.Net.Extensions
{
	public static class WebResponseExtensions
	{
		public static string ParseContentType(this WebResponse response)
		{
			if(response == null)
				throw new NullReferenceException("Malformed response detected when parsing WebResponse Content-Type");

			if(string.IsNullOrEmpty(response.ContentType))
				throw new NullReferenceException("Malformed Content-Type response detected when parsing WebResponse");

			var results = response.ContentType.Split(';');

			if(results.Length == 0)
				throw new FormatException("Malformed Content-Type response detected when parsing WebResponse");

			return results[0];
		}
	}
}
