using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Text;
using System.IO;

namespace ExtensionMethods
{
    public static class MyExtensionMethods
    {
        public static string ToDBString(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public static DateTime Zero(this DateTime date)
        {
            return new DateTime(1900, 10, 10, 1, 1, 1);
        }

        public static string ToShortDateDBString(this DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }

        public static DateTime GetNextWeekday(this DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static string ToRawString(this NameValueCollection collection)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var key in collection.AllKeys)
            {
                sb.Append(key);
                sb.Append("=");
                sb.Append(collection[key]);
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        public static string ToRawString(this HttpRequest request)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(request.Params.ToRawString());
            sb.Append("BODY:");

            string documentContents = request.GetFromBodyString();

            sb.Append(documentContents);

            return sb.ToString();
        }

        public static string GetFromBodyString(this HttpRequest request)
        {
            string result = string.Empty;

            if (request == null || request.InputStream == null)
                return result;

            request.InputStream.Position = 0;

            /*create a new thread in the memory to save the original 
            source form as may be required to read many of the 
            body of the current HTTP- request*/
            using (MemoryStream memoryStream = new MemoryStream())
            {
                request.InputStream.CopyToMemoryStream(memoryStream);
                using (StreamReader streamReader = new StreamReader(memoryStream))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            return result;
        }

        /*Copies bytes from the given stream MemoryStream and writes 
        them to another stream.*/
        public static void CopyToMemoryStream(this Stream source, MemoryStream destination)
        {
            if (source.CanSeek)
            {
                int pos = (int)destination.Position;
                int length = (int)(source.Length - source.Position) + pos;
                destination.SetLength(length);

                while (pos < length)
                    pos += source.Read(destination.GetBuffer(), pos, length - pos);
            }
            else
                source.CopyTo((Stream)destination);
        }

        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }


}
