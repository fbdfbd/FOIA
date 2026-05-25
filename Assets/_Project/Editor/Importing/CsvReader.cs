using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace OneMoreSpoon.Editor
{
    internal static class CsvReader
    {
        public static List<Dictionary<string, string>> Read(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[CsvImporter] Not found: {path}");
                return null;
            }

            var lines = File.ReadAllLines(path, Encoding.UTF8);
            if (lines.Length < 2)
                return new List<Dictionary<string, string>>();

            var headers = ParseLine(lines[0]);
            var result = new List<Dictionary<string, string>>();

            for (var i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = ParseLine(lines[i]);
                var row = new Dictionary<string, string>();
                for (var j = 0; j < headers.Count && j < values.Count; j++)
                    row[headers[j].Trim()] = values[j].Trim();
                result.Add(row);
            }

            return result;
        }

        public static string Get(Dictionary<string, string> row, string key)
            => row.TryGetValue(key, out var value) ? value : string.Empty;

        public static List<string> SplitList(string value)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(value))
                return result;

            foreach (var item in value.Split('|'))
            {
                var trimmed = item.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    result.Add(trimmed);
            }

            return result;
        }

        private static List<string> ParseLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            foreach (var c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                current.Append(c);
            }

            result.Add(current.ToString());
            return result;
        }
    }
}
