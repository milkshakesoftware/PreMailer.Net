using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PreMailer.Parsing
{
	public interface ICssParser
	{
		void AddStyleSheet(string styleSheetContent);

		void LoadStyleSheet(string filePath);
	}
}