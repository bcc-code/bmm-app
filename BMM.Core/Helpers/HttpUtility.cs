namespace BMM.Core.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public sealed class HttpUtility
    {
        public static HttpValueCollection ParseQueryString(string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if ((query.Length > 0) && (query[0] == '?'))
            {
                query = query.Substring(1);
            }

            return new HttpValueCollection(query, true);
        }
    }

    public class HttpValueCollection : Dictionary<string, string>
    {
        public HttpValueCollection()
        {
        }

        public HttpValueCollection(string query)
            : this(query, true)
        {
        }

        public HttpValueCollection(string query, bool urlencoded)
        {
            if (!string.IsNullOrEmpty(query))
            {
                this.FillFromString(query, urlencoded);
            }
        }

        public override string ToString()
        {
            return this.ToString(true);
        }

        public virtual string ToString(bool urlencoded)
        {
            return this.ToString(urlencoded, null);
        }

        public virtual string ToString(bool urlencoded, IDictionary excludeKeys)
        {
            if (this.Count == 0)
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();

            foreach (var item in this)
            {
                var key = item.Key;

                if ((excludeKeys == null) || !excludeKeys.Contains(key))
                {
                    var value = item.Value;

                    if (urlencoded)
                    {
                        key = Uri.EscapeDataString(key);
                    }

                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append('&');
                    }

                    stringBuilder.Append((key != null) ? (key + "=") : string.Empty);

                    if (!string.IsNullOrEmpty(value))
                    {
                        if (urlencoded)
                        {
                            value = Uri.EscapeDataString(value);
                        }

                        stringBuilder.Append(value);
                    }
                }
            }

            return stringBuilder.ToString();
        }

        private void FillFromString(string query, bool urlencoded)
        {
            var num = query != null ? query.Length : 0;

            for (var i = 0; i < num; i++)
            {
                var startIndex = i;
                var num4 = -1;

                while (i < num)
                {
                    var ch = query[i];

                    if (ch == '=')
                    {
                        if (num4 < 0)
                        {
                            num4 = i;
                        }
                    }
                    else if (ch == '&')
                    {
                        break;
                    }

                    i++;
                }

                string str = null;
                string str2 = null;

                if (num4 >= 0)
                {
                    str = query.Substring(startIndex, num4 - startIndex);
                    str2 = query.Substring(num4 + 1, (i - num4) - 1);
                }
                else
                {
                    str2 = query.Substring(startIndex, i - startIndex);
                }

                if (urlencoded)
                {
                    this.Add(Uri.UnescapeDataString(str), Uri.UnescapeDataString(str2));
                }
                else
                {
                    this.Add(str, str2);
                }

                if ((i == (num - 1)) && (query[i] == '&'))
                {
                    this.Add(null, string.Empty);
                }
            }
        }
    }
}