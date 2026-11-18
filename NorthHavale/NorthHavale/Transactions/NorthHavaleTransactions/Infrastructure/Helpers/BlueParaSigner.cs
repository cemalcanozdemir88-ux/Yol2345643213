using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NorthHavaleTransactions.Infrastructure.Helpers
{
    /// <summary>
    /// BluePara imzası: HMAC-SHA256(JSON.stringify(sortedPayload), key).
    /// BluePara tarafı Node.js olduğu için JSON metni JavaScript'in JSON.stringify çıktısıyla birebir aynı üretilir:
    /// üst seviye anahtarlar alfabetik sıralı, boşluk yok, sayılar JS formatında (100.00 -> 100), Türkçe karakterler kaçışsız.
    /// </summary>
    public static class BlueParaSigner
    {
        public static string Stringify(IDictionary<string, object> payload)
        {
            var sb = new StringBuilder();
            WriteObject(sb, payload.OrderBy(k => k.Key, StringComparer.Ordinal).Select(k => (k.Key, k.Value)));
            return sb.ToString();
        }

        public static string StringifySorted(JsonElement payload)
        {
            var sb = new StringBuilder();
            WriteObject(sb, payload.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal).Select(p => (p.Name, (object)p.Value)));
            return sb.ToString();
        }

        public static string Sign(string json, string key)
        {
            var hash = HMACSHA256.HashData(Encoding.UTF8.GetBytes((key ?? string.Empty).Trim()), Encoding.UTF8.GetBytes(json));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private static void WriteObject(StringBuilder sb, IEnumerable<(string Key, object Value)> items)
        {
            sb.Append('{');
            var first = true;
            foreach (var (key, value) in items)
            {
                if (!first) sb.Append(',');
                first = false;
                WriteString(sb, key);
                sb.Append(':');
                WriteValue(sb, value);
            }
            sb.Append('}');
        }

        private static void WriteValue(StringBuilder sb, object value)
        {
            switch (value)
            {
                case null:
                    sb.Append("null");
                    break;
                case string s:
                    WriteString(sb, s);
                    break;
                case bool b:
                    sb.Append(b ? "true" : "false");
                    break;
                case decimal or double or float or int or long:
                    WriteNumber(sb, Convert.ToDouble(value, CultureInfo.InvariantCulture));
                    break;
                case IDictionary<string, object> dict:
                    // İç içe nesneler gönderildiği sırayla yazılır; bu yüzden bizim gönderdiklerimiz SortedDictionary olmalı.
                    WriteObject(sb, dict.Select(k => (k.Key, k.Value)));
                    break;
                case JsonElement e:
                    WriteJsonElement(sb, e);
                    break;
                default:
                    throw new NotSupportedException($"BluePara imzası için desteklenmeyen tip: {value.GetType().Name}");
            }
        }

        private static void WriteJsonElement(StringBuilder sb, JsonElement e)
        {
            switch (e.ValueKind)
            {
                case JsonValueKind.Object:
                    WriteObject(sb, e.EnumerateObject().Select(p => (p.Name, (object)p.Value)));
                    break;
                case JsonValueKind.Array:
                    sb.Append('[');
                    var first = true;
                    foreach (var item in e.EnumerateArray())
                    {
                        if (!first) sb.Append(',');
                        first = false;
                        WriteJsonElement(sb, item);
                    }
                    sb.Append(']');
                    break;
                case JsonValueKind.String:
                    WriteString(sb, e.GetString());
                    break;
                case JsonValueKind.Number:
                    WriteNumber(sb, e.GetDouble());
                    break;
                case JsonValueKind.True:
                    sb.Append("true");
                    break;
                case JsonValueKind.False:
                    sb.Append("false");
                    break;
                default:
                    sb.Append("null");
                    break;
            }
        }

        private static void WriteNumber(StringBuilder sb, double d)
        {
            if (d == Math.Floor(d) && Math.Abs(d) < 1e15)
                sb.Append(d.ToString("0", CultureInfo.InvariantCulture));
            else
                sb.Append(d.ToString("R", CultureInfo.InvariantCulture));
        }

        private static void WriteString(StringBuilder sb, string s)
        {
            sb.Append('"');
            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20)
                        {
                            sb.Append("\\u").Append(((int)c).ToString("x4"));
                        }
                        else if (char.IsHighSurrogate(c) && i + 1 < s.Length && char.IsLowSurrogate(s[i + 1]))
                        {
                            sb.Append(c).Append(s[i + 1]);
                            i++;
                        }
                        else if (char.IsSurrogate(c))
                        {
                            sb.Append("\\u").Append(((int)c).ToString("x4"));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append('"');
        }
    }
}
