using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PreMailer.Net
{
	public class StyleClass
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StyleClass"/> class.
		/// </summary>
		public StyleClass()
		{
			this.Attributes = new SortedList<string, string>();
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the attributes.
		/// </summary>
		/// <value>The attributes.</value>
		public SortedList<string, string> Attributes { get; set; }

		/// <summary>
		/// Merges the specified style class, with this instance. Styles on this instance are not overriden by duplicates in the specified styleClass.
		/// </summary>
		/// <param name="styleClass">The style class.</param>
		/// <param name="canOverwrite">if set to <c>true</c> [can overwrite].</param>
		public void Merge(StyleClass styleClass, bool canOverwrite)
		{
			foreach (var item in styleClass.Attributes)
			{
				if (!this.Attributes.ContainsKey(item.Key))
				{
					this.Attributes.Add(item.Key, item.Value);
				}
				else if (canOverwrite)
				{
					this.Attributes[item.Key] = item.Value;
				}
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			foreach (var item in this.Attributes)
			{
				sb.AppendFormat("{0}: {1};", item.Key, item.Value);
			}

			return sb.ToString();
		}
	}
}