using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PreMailer.Net {
	public class CssAttributeCollection : IDictionary<string, CssAttribute> {
		private readonly IDictionary<string, CssValue> _attributes;
		private int _currentPosition;

		public CssAttributeCollection()
		{
			_attributes = new Dictionary<string, CssValue>(StringComparer.CurrentCultureIgnoreCase);
			_currentPosition = 0;
		}

		/// <summary>
		/// Add a CssAttribute and set it's position to overwrite all previous CssAttributes in the same Collection.
		/// </summary>
		public void Add(KeyValuePair<string, CssAttribute> item)
		{
			_attributes.Add(item.Key, new CssValue(position: ++_currentPosition, attribute: item.Value));
		}

		/// <summary>
		/// Add a CssAttribute and set it's position to overwrite all previous CssAttributes in the same Collection.
		/// </summary>
		public void Add(string key, CssAttribute value)
		{
			_attributes.Add(key, new CssValue(position: ++_currentPosition, attribute: value));
		}

		/// <summary>
		/// Add or Update a CssAttribute without changing it's position in the case of an update.
		/// </summary>
		public CssAttribute this[string key] {
			get => _attributes[key].Attribute;
			set {
				if (_attributes.TryGetValue(key, out var existing)) {
					_attributes[key] = new CssValue(position: existing.Position, attribute: value);
				}
				else {
					_attributes[key] = new CssValue(position: ++_currentPosition, attribute: value);
				}
			}
		}

		/// <summary>
		/// Add or Update a CssAttribute and set it's position to overwrite all previous CssAttributes in the same Collection.
		/// </summary>
		public void Merge(CssAttribute attribute)
		{
			var key = attribute.Style;
			var value = attribute;

			_attributes[key] = new CssValue(position: ++_currentPosition, attribute: value);
		}

		/// <summary>
		/// Copy the entries of this Collection, ordered by position, to the destination Array.
		/// </summary>
		public void CopyTo(KeyValuePair<string, CssAttribute>[] array, int arrayIndex)
		{
			var arr = _attributes.OrderBy(_ => _.Value.Position).Select(_ => new KeyValuePair<string, CssAttribute>(_.Key, _.Value.Attribute)).ToArray();
			Array.Copy(arr, 0, array, arrayIndex, arr.Length);
		}

		/// <summary>
		/// Gets an Enumerator of the attributes in this collection, ordered by position.
		/// </summary>
		public IEnumerator<KeyValuePair<string, CssAttribute>> GetEnumerator()
		{
			return _attributes
				.OrderBy(_ => _.Value.Position)
				.Select(pair => new KeyValuePair<string, CssAttribute>(pair.Key, pair.Value.Attribute)).GetEnumerator();
		}

		/// <summary>
		/// Gets an Enumerator of the attributes in this collection, ordered by position.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Gets all Keys in this Collection ordered by position.
		/// </summary>
		public ICollection<string> Keys => _attributes.OrderBy(_ => _.Value.Position).Select(_ => _.Key).ToList();

		/// <summary>
		/// Gets all Values in this Collection ordered by position.
		/// </summary>
		public ICollection<CssAttribute> Values => _attributes.Values.OrderBy(_ => _.Position).Select(_ => _.Attribute).ToList();

		/// <inheritdoc />
		public int Count => _attributes.Count;

		/// <inheritdoc />
		public bool IsReadOnly => _attributes.IsReadOnly;

		/// <inheritdoc />
		public bool TryGetValue(string key, out CssAttribute value)
		{
			if (_attributes.TryGetValue(key, out var data)) {
				value = data.Attribute;
				return true;
			}

			value = default;
			return false;
		}

		/// <inheritdoc />
		public bool Contains(KeyValuePair<string, CssAttribute> item)
		{
			return _attributes.TryGetValue(item.Key, out var value) && value.Attribute == item.Value;
		}

		/// <inheritdoc />
		public bool ContainsKey(string key)
		{
			return _attributes.ContainsKey(key);
		}

		/// <inheritdoc />
		public bool Remove(string key)
		{
			return _attributes.Remove(key);
		}

		/// <inheritdoc />
		public bool Remove(KeyValuePair<string, CssAttribute> item)
		{
			if (_attributes.TryGetValue(item.Key, out var value) && value.Attribute == item.Value) {
				return _attributes.Remove(new KeyValuePair<string, CssValue>(item.Key, value));
			}

			return false;
		}

		/// <inheritdoc />
		public void Clear()
		{
			_attributes.Clear();
		}


		private readonly struct CssValue {
			public int Position { get; }
			public CssAttribute Attribute { get; }

			public CssValue(int position, CssAttribute attribute)
			{
				Position = position;
				Attribute = attribute;
			}
		}
	}
}