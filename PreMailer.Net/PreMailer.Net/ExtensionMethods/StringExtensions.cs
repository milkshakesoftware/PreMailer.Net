using System;

public static class StringExtensions
{
	public static bool IsNullOrWhiteSpace(this string value)
	{
		if (value == null) return true;

		return String.IsNullOrEmpty(value.Trim());
	}
}