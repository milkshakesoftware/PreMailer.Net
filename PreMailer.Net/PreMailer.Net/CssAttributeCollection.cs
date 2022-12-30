using System.Collections;
using System.Collections.Generic;

namespace PreMailer.Net {
	public class CssAttributeCollection : IEnumerable<CssAttribute> {
		private readonly List<CssAttribute> _attributes = new List<CssAttribute>();

		/// <summary>
		/// Add or Update a CssAttribute without changing it's position in the case of an update.
		/// </summary>
		public CssAttribute this[string key] {
			get {
				var index = IndexOfKey(key);
				return index == -1 ? null : _attributes[index];
			}
			set {
				var index = IndexOfKey(key);
				if (index == -1)
				{
					_attributes.Add(value);

				}
				else
				{
					_attributes[index] = value;
				}
			}
		}

		/// <summary>
		/// Add or Update a CssAttribute and set it's position to overwrite all previous CssAttributes in the same Collection.
		/// </summary>
		public void Merge(CssAttribute attribute)
		{
			var key = attribute.Style;

			// Remove previous to instead append at the end
			Remove(key);

			this[key] = attribute;
		}

		/// <summary>
		/// Gets an Enumerator of the attributes in this collection, ordered by position.
		/// </summary>
		public IEnumerator<CssAttribute> GetEnumerator()
		{
			return _attributes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _attributes.GetEnumerator();
		}

		public int Count => _attributes.Count;

		public bool TryGetValue(string key, out CssAttribute value)
		{
			var index = IndexOfKey(key);
			if (index != -1)
			{
				value = _attributes[index];
				return true;
			}

			value = default;
			return false;
		}

		private int IndexOfKey(string key)
		{
			for (int i = 0; i < _attributes.Count; i++)
			{
				var attribute = _attributes[i];
				if (string.Equals(attribute.Style, key, System.StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}

			return -1;
		}

		public bool ContainsKey(string key)
		{
			return IndexOfKey(key) != -1;
		}

		public void Remove(string key)
		{
			var index = IndexOfKey(key);
			if (index != -1)
			{
				_attributes.RemoveAt(index);
			}
		}
		
		public void Clear()
		{
			_attributes.Clear();
		}
	}
}