using Newtonsoft.Json;
using PlaylistNameSort.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace PlaylistNameSort.Domain
{
    public class SpotifyApi : ISpotifyApi
    {
        public string Token { get; set; }

        public SpotifyApi()
        {

        }

        public SpotifyApi(string token)
        {
            Token = token;
        }

        public T GetSpotifyType<T>(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Set("Authorization", "Bearer" + " " + Token);
                request.ContentType = "application/json; charset=utf-8";

                T type = default(T);

                using (WebResponse response = request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string responseFromServer = reader.ReadToEnd();
                            type = JsonConvert.DeserializeObject<T>(responseFromServer);
                        }
                    }
                }
                return type;
            }
            catch (WebException ex)
            {
                return default(T);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public T PostSpotifyType<T>(string url, string PToken, string json)
        {
            try
            {

                System.Net.ServicePointManager.Expect100Continue = false;
                var requestedURL = url;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestedURL);
                request.ContentType = "application/json;charset=utf-8";
                request.Method = "POST"; //Set content length to 0 
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }
                request.Headers.Set("Authorization", "Bearer" + " " + PToken);
                T type = default(T);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string responseFromServer = reader.ReadToEnd();
                            type = JsonConvert.DeserializeObject<T>(responseFromServer);
                        }
                    }
                }
                return type;
            }
            catch (WebException ex)
            {
                return default(T);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}