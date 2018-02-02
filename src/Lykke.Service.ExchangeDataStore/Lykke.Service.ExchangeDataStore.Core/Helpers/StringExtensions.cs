﻿using System.Linq;

namespace Lykke.Service.ExchangeDataStore.Core.Helpers
{
    public static class StringExtensions
    {
        public static string RemoveSpecialCharacters(this string str, params char[] additionalCharsAllowed)
        {
            return new string(str.Where(c => char.IsLetterOrDigit(c) ||
                                             additionalCharsAllowed.Contains(c)).ToArray());
        }
    }
}
