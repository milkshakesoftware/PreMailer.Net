using System;

namespace PreMailer.Parsing
{
	public interface ICssParser
	{
		void AddStyleSheet(string styleSheetContent);

		void AddStyleSheet(Uri url);
	}
}