using Newtonsoft.Json;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;

namespace Km.AdExpressClientWeb.Helpers
{
    public class JsonWebApiClient : IDisposable
    {
        private readonly HttpClient _client;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public JsonWebApiClient()
        {
            //http://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
            _client = new HttpClient();
            //HttpClient(HttpMessageHandler handler);
            //HttpClient(HttpMessageHandler handler, bool disposeHandler);

            //clean supported Type
            _client.DefaultRequestHeaders.Accept.Clear();

            //Force to use JsonFormat
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region Envoie de message AVEC retour

        public Response<TOut> Post<TOut>(string uri, dynamic data)
            where TOut : class
        {
            Logger.Debug("Before sending to: {0}", uri);
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri(uri));
            return Send<TOut>(message, data);
        }

        public Response<TOut> Get<TOut>(string uri, dynamic data)
            where TOut : class
        {
            Logger.Debug("Before sending to: {0}", uri);
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
            return Send<TOut>(message, data);
        }

        public TOut GetTypedResult<TOut>(string uri, dynamic data)
            where TOut : class
        {
            Logger.Debug("Before sending to: {0}", uri);
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
            AddHeaders(message);
            if (data != null && message.Method == HttpMethod.Get)
                BindParametersToUrl(data, message);
            Logger.Debug("Sending Request to {0}", message.RequestUri.ToString());
            var response = _client.SendAsync(message).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<TOut>(response.Content.ReadAsStringAsync().Result,
                                                    new JsonSerializerSettings
                                                    {
                                                        TypeNameHandling = TypeNameHandling.Auto
                                                    });
                return result;
            }
            throw new HttpServiceException(response.ReasonPhrase, response.StatusCode, response.Content.ReadAsStringAsync().Result);
        }

        public Response<TOut> Put<TOut>(string uri, dynamic data)
            where TOut : class
        {
            Logger.Debug("Before sending to: {0}", uri);
            var message = new HttpRequestMessage(HttpMethod.Put, new Uri(uri));
            return Send<TOut>(message, data);
        }

        public Response<TOut> Delete<TOut>(string uri, dynamic data)
            where TOut : class
        {
            Logger.Debug("Before sending to: {0}", uri);
            var message = new HttpRequestMessage(HttpMethod.Delete, new Uri(uri));
            return Send<TOut>(message, data);
        }

        public FileDownloadInfo Download(string uri, dynamic data)
        {
            Logger.Debug("Before sending to: {0}", uri);
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
            AddHeaders(message);
            if (data != null)
            {
                BindParametersToUrl(data, message);
            }
            Logger.Debug("Sending Request to {0}", message.RequestUri.ToString());
            var response = _client.SendAsync(message).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsByteArrayAsync().Result;
                var fileinfo = new FileDownloadInfo { File = result };
                if (!string.IsNullOrWhiteSpace(response.Content.Headers.ContentDisposition.FileName))
                {
                    fileinfo.Filename = response.Content.Headers.ContentDisposition.FileName;
                }
                if (!string.IsNullOrEmpty(response.Content.Headers.ContentType.MediaType))
                {
                    fileinfo.ContentType = response.Content.Headers.ContentType.MediaType;
                }
                fileinfo.Parametres = response.Content.Headers.ContentDisposition.Parameters.ToDictionary(p => p.Name, p => p.Value);

                return fileinfo;
            }
            var content = response.ReasonPhrase;
            throw new HttpServiceException(content, response.StatusCode);
        }

        #endregion

        #region Envoie de message SANS valeur de retour (juste le status code)

        public bool Post(string uri, dynamic data, bool throwIfNotSuccess = false)
        {
            Logger.Debug("Before sending to: {0}", uri);
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri(uri));
            return Send(message, data, throwIfNotSuccess);
        }

        public bool Get(string uri, dynamic data, bool throwIfNotSuccess = false)
        {
            Logger.Debug("Before sending to: {0}", uri);
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
            return Send(message, data, throwIfNotSuccess);
        }

        public bool Put(string uri, dynamic data, bool throwIfNotSuccess = false)
        {
            Logger.Debug("Before sending to: {0}", uri);
            var message = new HttpRequestMessage(HttpMethod.Put, new Uri(uri));
            return Send(message, data, throwIfNotSuccess);
        }

        public bool Delete(string uri, dynamic data, bool throwIfNotSuccess = false)
        {
            Logger.Debug("Before sending to: {0}", uri);
            var message = new HttpRequestMessage(HttpMethod.Delete, new Uri(uri));
            return Send(message, data, throwIfNotSuccess);
        }
        #endregion;

        #region Envoie de message sans valeur en entrée

        public Response<TOut> Get<TOut>(string uri)
            where TOut : class
        {
            Logger.Debug("Before sending to: {0}", uri);
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
            return Send<TOut>(message);
        }

        #endregion;

        private Response<TOut> Send<TOut>(HttpRequestMessage message)
          where TOut : class
        {
            AddHeaders(message);

            return HandleResponse<TOut>(message);
        }

        private bool Send(HttpRequestMessage message, dynamic data, bool throwIfNotSuccess = false)
        {
            AddHeaders(message);

            if (data != null && message.Method != HttpMethod.Get)
            {
                message.Content = new StringContent(data.GetType(), data);
                    //new ObjectContent(data.GetType(), data, new JsonMediaTypeFormatter());
            }
            else if (data != null && message.Method == HttpMethod.Get)
            {
                BindParametersToUrl(data, message);
            }

            HttpResponseMessage response = null;
            try
            {
                response = _client.SendAsync(message).Result;
                Logger.Debug("{0} {1} {2}", response.RequestMessage.Method, response.StatusCode, response.RequestMessage.RequestUri);
                return true;
            }
            catch (AggregateException ex)
            {
                Logger.Error("{0} {1} {2}", message.Method, (response != null ? response.StatusCode.ToString() : "-"), message.RequestUri);
                Logger.Error(ex.InnerException);
                if (throwIfNotSuccess)
                {
                    throw ex.InnerException;
                }

                return false;
            }
        }

        private void BindParametersToUrl(dynamic data, HttpRequestMessage message)
        {
            //Get typeObject
            Type t = data.GetType();

            //Reflection on properties
            var props = t.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);

            var parameters = new StringBuilder();
            props.ToList().ForEach(p => parameters.Append("&").Append(GetParameter(p, data)));

            // remove starting &
            parameters.Remove(0, 1);

            // Concat param to the Uri
            message.RequestUri = new Uri(message.RequestUri + "?" + parameters);
        }

        private string GetParameter(PropertyInfo propertyInfo, dynamic data)
        {
            return HttpUtility.UrlEncode(propertyInfo.Name) + "=" +
                   HttpUtility.UrlEncode(GetValueParameter(propertyInfo.GetValue(data, null)));
        }

        private static string GetValueParameter(dynamic value)
        {
            if (value == null)
                return "";
            if (value is DateTime)
            {
                var date = (DateTime)value;
                return date.ToString(CultureInfo.InvariantCulture);
            }
            if (value is IList && value.GetType().IsGenericType)
            {
                var typeT = value.GetType().GetGenericArguments()[0];
                if (typeT.IsValueType)
                {
                    List<string> list = new List<string>();
                    foreach (var val in value)
                    {
                        list.Add(val.ToString());
                    }
                    return String.Join(",", list.ToArray());
                }
                throw new NotSupportedException("Impossible de faire passer une liste d'objet complexe par querystring");
            }
            return value.ToString();
        }

        private Response<TOut> Send<TOut>(HttpRequestMessage message, dynamic data)
            where TOut : class
        {
            AddHeaders(message);

            if (data != null && message.Method != HttpMethod.Get)
                message.Content = new StringContent(data.GetType(), data);
            //new ObjectContent(data.GetType(), data, new JsonMediaTypeFormatter());
            else if (data != null && message.Method == HttpMethod.Get)
                BindParametersToUrl(data, message);

            Logger.Debug("Sending Request to {0}", message.RequestUri.ToString());

            return HandleResponse<TOut>(message);
        }

        private Response<TOut> HandleResponse<TOut>(HttpRequestMessage message) where TOut : class
        {
            HttpResponseMessage response = null;
            try
            {
                response = _client.SendAsync(message).Result;
                TOut result = null;
                    //response.Content.ReadAsStringAsync().Result;
                    //ReadAsAsync<TOut>(new List<MediaTypeFormatter>() { new JsonMediaTypeFormatter() }).Result;
                Logger.Debug("{0} {1} {2}", response.RequestMessage.Method, response.StatusCode, response.RequestMessage.RequestUri);
                return new Response<TOut> { RawResult = response, Result = result };
            }
            catch (AggregateException ex)
            {
                Logger.Error("{0} {1} {2}", message.Method, (response != null ? response.StatusCode.ToString() : "-"), message.RequestUri);
                Logger.Error(ex.InnerException);
                throw ex.InnerException;
            }
        }

        private void AddHeaders(HttpRequestMessage message)
        {
            //message.Headers.Add("X-ApiKey", _apiKey);
            //if (!string.IsNullOrWhiteSpace(_origine))
            //{
            //    Logger.Debug("Adding Origine=" + _origine + " to request header.");
            //    message.Headers.Add(Constantes.XOrigine, _origine);
            //}
            //if (!string.IsNullOrWhiteSpace(_utilisateur))
            //{
            //    Logger.Debug("Adding Utilisateur=" + _utilisateur + " to request header.");
            //    message.Headers.Add(Constantes.XUser, _utilisateur);
            //}
        }

        public void Dispose()
        {
            if (_client != null)
                _client.Dispose();
        }
    }


    public class Response<T> where T : class
    {
        public T Result { get; set; }

        public HttpResponseMessage RawResult { get; set; }
    }

    public class FileDownloadInfo
    {
        /// <summary>
        /// Obtient ou définit le contenu du fichier téléchargé
        /// </summary>
        public byte[] File { get; set; }
        /// <summary>
        /// Obtient ou définit le nom du fichier
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// Obtient ou définit le content type du fichier
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// Obtient ou définit une liste de paramètres qui accompagne le fichier
        /// </summary>
        public Dictionary<string, string> Parametres { get; set; }
    }

    public class HttpServiceException : Exception
    {
        public string Reason { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }

        public HttpServiceException(string reason, HttpStatusCode statusCode)
            : base(reason)
        {
            StatusCode = statusCode;
            Reason = reason;
        }

        public HttpServiceException(string reason, HttpStatusCode statusCode, string message)
            : base(message)
        {
            Reason = reason;
            StatusCode = statusCode;
        }
    }
}
