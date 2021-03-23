using CSVProcessor.Business.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSVProcessor.Business.Helpers
{
    public static class StringExtensions
    {
        public static string GetBetween(this string source, string before, string after)
        {
            var start = source.LastIndexOf(before, StringComparison.Ordinal) + before.Length;
            var end = source.LastIndexOf(after, StringComparison.Ordinal);
            return start < end ? source.Substring(start, end - start) : "";
        }

        public static string GetBefore(this string source, string before)
        {
            var position = source.LastIndexOf(before, StringComparison.Ordinal);
            return source.Substring(0, position);
        }

        public static string GetAfter(this string source, string after)
        {
            var position = source.LastIndexOf(after, StringComparison.Ordinal);
            var adjPos = position + after.Length;
            return source.Substring(adjPos);
        }

        public static bool IsTopLevel(this string source) =>
            source.Length == 2 &&
            char.IsDigit(source, 0) &&
            char.IsPunctuation(source, 1);

        public static bool ContainsHtmlTag(this string text) =>
            Regex.IsMatch(text, "<html>", RegexOptions.IgnoreCase);

        public static bool HasNoRecordForCommunity(this string text)
        {
            var hasNoRecord = Regex.IsMatch(text, "ThirdPartyCallRecord_no", RegexOptions.IgnoreCase);
            var isEmpty = string.IsNullOrEmpty(text);
            return isEmpty || hasNoRecord;
        }

        public static List<AdministrativeDomain> Deserialize(this string source) =>
            JsonConvert.DeserializeObject<List<AdministrativeDomain>>(source);

    }
}
