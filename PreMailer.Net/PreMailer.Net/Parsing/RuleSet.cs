using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PreMailer.Parsing
{
	public class RuleSet
	{
		public RuleSet()
		{
			this.Selectors = new List<Selector>();
			this.Attributes = new SortedList<string, string>();
		}

		/// <summary>
		/// Gets or sets the selectors.
		/// </summary>
		/// <value>The selectors.</value>
		public ICollection<Selector> Selectors { get; private set; }

		/// <summary>
		/// Gets or sets the attributes.
		/// </summary>
		/// <value>The attributes.</value>
		public SortedList<string, string> Attributes { get; private set; }

		/// <summary>
		/// Merges the specified rule set, with this instance. Styles on this instance is overwritten, 
		/// only if the specificity of the mathcingSelector is greater than any of the selectors on this instance.
		/// </summary>
		/// <param name="ruleSet">The rule set.</param>
		/// <param name="matchingSelector">The selector that matched the element.</param>
		public virtual void Merge(RuleSet ruleSet, Selector matchingSelector)
		{
			foreach (var item in ruleSet.Attributes)
			{
				if (!this.Attributes.ContainsKey(item.Key))
				{
					this.Attributes.Add(item.Key, item.Value);
				}
				else if (matchingSelector.Specificity > this.Selectors.Max(s => s.Specificity))
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