/*******************************************************************
This file is part of WP7DropBox.

WP7DropBox is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 2 of the License, or
(at your option) any later version.

WP7DropBox is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with WP7DropBox.  If not, see <http://www.gnu.org/licenses/>.
*******************************************************************/
namespace l3v5y.Dropbox
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Text;
    public class Helper
    {
        /// <summary>
        /// Deserializes a Json string to an object
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="jsonString">json string</param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }
        /*
        public static void Download(DownloadStringCompletedEventHandler ondownload, string url)
        {
           
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += ondownload;
            wc.DownloadStringAsync(new Uri(url));
        }*/
        public static void HttpWebRequestGet(AsyncCallback result, string url)
        {
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.Method = "GET";
            wr.BeginGetResponse(result, wr);
        }
        public static void HttpWebRequestPost(AsyncCallback result, string url)
        {
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.Method = "POST";
            wr.BeginGetRequestStream(result, wr);            
        }
    }
}
