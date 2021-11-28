// ReSharper disable StringLiteralTypo

namespace Loki.Runtime.Utility
{
	public static class RandomString
	{
		private static readonly string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

		public static string Get(int length)
		{
			char[] charArray = new char[length];
			var rand = new System.Random();
			int numChars = AllowedChars.Length;
			for (var i = 0; i < length; i++)
			{
				charArray[i] = AllowedChars[rand.Next(numChars)];
			}

			return new string(charArray);
		}
	}
}
