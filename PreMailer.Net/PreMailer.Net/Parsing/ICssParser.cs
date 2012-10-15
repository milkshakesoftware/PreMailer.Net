using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreMailerDotNet.Parsing
{
	public interface ICssParser
	{
		void AddStyleSheet(string styleSheetContent);

		void LoadStyleSheet(string filePath);
	}
}