using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Application.Common
{
    public class AlidayuMessageSender
    {
        private readonly string _appKey;
        private readonly string _appSecret;
        private readonly string _serverUrl;

        public AlidayuMessageSender(string url, string appKey, string appSecret)
        {
            _serverUrl = url; _appKey = appKey;
            _appSecret = appSecret;
        }

        public string SmsType { get; set; } = "normal";
        public string SmsFreeSignName { get; set; } = "生日提醒";
        public string SmsParam { get; set; }
        public string RecNum { get; set; }
        public string SmsTemplateCode { get; set; } = "SMS_24695114";

        public string GetApiName()
        {
            return "alibaba.aliqin.fc.sms.num.send";
        }
        
        public string SendMessage()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            //httpClient.DefaultRequestHeaders.Add("KeepAlive", "true"); 
            httpClient.DefaultRequestHeaders.Add("user-agent", "top-sdk-net");
            httpClient.DefaultRequestHeaders.Add("Method", "Post");
            httpClient.DefaultRequestHeaders.Add("Accept", "text/xml,text/javascript");
            httpClient.DefaultRequestHeaders.Add("Host", "gw.api.taobao.com");
            httpClient.Timeout = new TimeSpan(0, 0, 100);
            byte[] postData = GetPostData();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _serverUrl);
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded")); request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
            request.Content = new StreamContent(new MemoryStream(postData));
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"); HttpResponseMessage httpResponseMessage = httpClient.SendAsync(request).GetAwaiter().GetResult();
            string result = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult(); 
            return result;
        }
        public byte[] GetPostData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("rec_num", RecNum);
            dictionary.Add("sms_free_sign_name", SmsFreeSignName);
            dictionary.Add("sms_param", SmsParam);
            dictionary.Add("sms_template_code", SmsTemplateCode);
            dictionary.Add("sms_type", SmsType);
            dictionary.Add("method", "alibaba.aliqin.fc.sms.num.send");
            dictionary.Add("v", "2.0");
            dictionary.Add("sign_method", "hmac");
            dictionary.Add("app_key", _appKey); dictionary.Add("format", "xml");
            dictionary.Add("partner_id", "top-sdk-net-20160607");
            dictionary.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //dictionary.Add("target_app_key", null); 
            //dictionary.Add("session", null); 
            dictionary.Add("sign", SignTopRequest(dictionary, null, _appSecret, "hmac"));
            byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(dictionary)); 
            return postData;
        }
        public string SignTopRequest(IDictionary<string, string> parameters, string body, string secret, string signMethod)
        {
            // 第一步：把字典按Key的字母顺序排序 
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
            // 第二步：把所有参数名和参数值串在一起 
            StringBuilder query = new StringBuilder();
            if ("md5".Equals(signMethod)) { query.Append(secret); }
            foreach (KeyValuePair<string, string> kv in sortedParams)
            {
                if (!string.IsNullOrEmpty(kv.Key) && !string.IsNullOrEmpty(kv.Value))
                { query.Append(kv.Key).Append(kv.Value); }
            }
            // 第三步：把请求主体拼接在参数后面 
            if (!string.IsNullOrEmpty(body)) { query.Append(body); }
            // 第四步：使用MD5/HMAC加密 
            byte[] bytes;
            if ("hmac".Equals(signMethod))
            {
                HMACMD5 hmac = new HMACMD5(Encoding.UTF8.GetBytes(secret));
                bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));
            }
            else
            {
                query.Append(secret);
                MD5 md5 = MD5.Create();
                bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));
            }
            // 第五步：把二进制转化为大写的十六进制 
            StringBuilder result = new StringBuilder();
            foreach (byte t in bytes)
            { result.Append(t.ToString("X2")); }
            return result.ToString();
        }
        public string BuildQuery(IDictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return null;
            }
            StringBuilder query = new StringBuilder();
            bool hasParam = false;
            foreach (KeyValuePair<string, string> kv in parameters)
            {
                string name = kv.Key; string value = kv.Value;
                // 忽略参数名或参数值为空的参数 
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam) { query.Append("&"); }
                    query.Append(name); query.Append("="); query.Append(WebUtility.UrlEncode(value)); hasParam = true;
                }
            }
            return query.ToString();
        }
    }
}
