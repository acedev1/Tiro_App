using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Model;

namespace TiroApp
{
    public class DataGate
    {
        public const string SERVER_URL = "http://tiro.flexible-solutions.com.ua";
        private const string API_URL = "http://tiro.flexible-solutions.com.ua/api/";
        //private const string API_URL = "http://192.168.1.10/Tiro/api/";

        private const string M_CUSTOMER_LOGIN = "customer/login";
        private const string M_CUSTOMER_SIGNUP = "customer/signup";
        private const string M_MUA_SEARCH = "mua/search";
        private const string M_MUA_GET = "mua/getmua";
        private const string M_MUA_LOGIN = "mua/login";
        private const string M_MUA_SIGNUP = "mua/signup";

        public static void VerifyPhoneNumber(string phone, Action<ResponseDataJson> callback)
        {
            var body = $"\"{phone}\"";
            MakeRequestJson("helper/verifyphonenumber", body, callback);
        }

        public static void CustomerLoginJson(string email, string password, string fbId, Action<ResponseDataJson> callback)
        {
            var data = new Dictionary<string, object>()
            {
                { "Email", email },
                { "FbId", fbId },
                { "Password", password }
            };
            var body = BiuldJson(data);
            MakeRequestJson(M_CUSTOMER_LOGIN, body.ToString(), callback);
        }
        
        public static void CustomerSignupJson(Dictionary<string, object> data, Action<ResponseDataJson> callback)
        {
            var body = BiuldJson(data);
            MakeRequestJson(M_CUSTOMER_SIGNUP, body.ToString(), callback);
        }

        public static void MuaLoginJson(string email, string password, Action<ResponseDataJson> callback)
        {
            var data = new JObject();
            data["Email"] = email;
            data["Password"] = password;
            MakeRequestJson(M_MUA_LOGIN, data.ToString(), callback);
        }

        public static void MuaSignupJson(Dictionary<string, object> data, Action<ResponseDataJson> callback)
        {
            try
            {
                var jobj = BiuldJson(data);
                MakeRequestJson(M_MUA_SIGNUP, jobj.ToString(), callback);
            }
            catch (Exception e)
            {
                callback.Invoke(new ResponseDataJson(ResponseCode.Fail, null, e));
            }
        }

        public static void UploadImage(bool isMua, System.IO.Stream imgStream, Action<ResponseData> callback)
        {
            var buffer = new byte[imgStream.Length];
            imgStream.Read(buffer, 0, buffer.Length);
            imgStream.Dispose();
            var imgBase64 = System.Convert.ToBase64String(buffer);
            var sb = new StringBuilder();
            sb.Append("<base64Binary xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">");
            sb.Append(imgBase64);
            sb.Append("</base64Binary>");
            if (isMua)
            {
                MakeRequest("mua/uploadimage?muaId=" + GlobalStorage.Settings.MuaId, sb.ToString(), callback);
            }
            else
            {
                MakeRequest("customer/uploadimage?customerId=" + GlobalStorage.Settings.CustomerId, sb.ToString(), callback);
            }
        }

        public static void GetMua(string id, Action<ResponseDataJson> callback)
        {
            var body = $"\"{id}\"";
            MakeRequestJson(M_MUA_GET, body, callback);
        }

        public static void DoMUASearchJson(List<string> categories, Pages.LocationFilter lFilter, Availibility aFilter, Action<ResponseDataJson> callback)
        {
            var body = new JObject();
            var categoryArr = new JArray();
            body["CategoryFilter"] = categoryArr;
            foreach (var c in categories)
            {
                categoryArr.Add(c);
            }
            if (lFilter != null)
            {
                var obj = new JObject();
                obj["Lat"] = lFilter.Lat;
                obj["Lon"] = lFilter.Lon;
                obj["Radius"] = lFilter.Radius;
                body["LocationFilter"] = obj;
            }
            if (aFilter != null)
            {
                var obj = new JObject();
                obj["From"] = aFilter.DatesFrom[0].ToUniversalTime();
                obj["To"] = aFilter.DatesTo[0].ToUniversalTime();
                body["AvailibilityFilter"] = obj;
            }
            MakeRequestJson(M_MUA_SEARCH, body.ToString(), callback);
        }

        public static void CustomerSetInfo(Customer customer, Action<ResponseDataJson> callback)
        {
            var jObj = new JObject();
            jObj["Id"] = customer.Id;
            jObj["FirstName"] = customer.FirstName;
            jObj["LastName"] = customer.LastName;
            jObj["Phone"] = customer.PhoneNumber;
            MakeRequestJson("customer/setinfo", jObj.ToString(), callback);
        }

        public static void MuaSetInfo(MuaArtist mua,  Action<ResponseDataJson> callback)
        {
            var jObj = new JObject();
            jObj["Id"] = mua.Id;
            jObj["FirstName"] = mua.FirstName;
            jObj["LastName"] = mua.LastName;
            jObj["BusinessName"] = mua.BusinessName;
            jObj["Phone"] = mua.PhoneNumber;
            jObj["Address"] = mua.Address;
            jObj["LocationLat"] = mua.Lat;
            jObj["LocationLon"] = mua.Lon;
            jObj["AboutMe"] = mua.AboutMe;
            MakeRequestJson("mua/setinfo", jObj.ToString(), callback);
        }

        public static void MuaSearchFree(List<string> categories, Pages.LocationFilter lFilter, Availibility aFilter, Action<ResponseDataJson> callback)
        {
            var body = new JObject();
            var categoryArr = new JArray();
            body["CategoryFilter"] = categoryArr;
            foreach (var c in categories)
            {
                categoryArr.Add(c);
            }
            if (lFilter != null)
            {
                var obj = new JObject();
                obj["Lat"] = lFilter.Lat;
                obj["Lon"] = lFilter.Lon;
                obj["Radius"] = lFilter.Radius;
                body["LocationFilter"] = obj;
            }
            if (aFilter != null)
            {
                var obj = new JObject();
                obj["From"] = aFilter.DatesFrom[0].ToUniversalTime();
                obj["To"] = aFilter.DatesTo[0].ToUniversalTime();
                body["AvailibilityFilter"] = obj;
            }
            MakeRequestJson("mua/searchfree", body.ToString(), callback);
        }

        public static void MuaSetAvailability(string muaId, Availibility a, Action<ResponseDataJson> callback)
        {
            var jObj = new JObject();
            jObj["MuaId"] = muaId;
            jObj["Mode"] = (int)a.Mode;
            if (a.DaysOfWeek != null)
            {
                var sb = new StringBuilder();
                foreach (var d in a.DaysOfWeek)
                {
                    sb.Append(d);
                    sb.Append(",");
                }
                sb.Length--;
                jObj["DaysOfWeek"] = sb.ToString();
            }
            var fromArray = new JArray();
            var toArray = new JArray();
            foreach (var dt in a.DatesFrom)
            {
                fromArray.Add(dt.ToUniversalTime());
            }
            foreach (var dt in a.DatesTo)
            {
                toArray.Add(dt.ToUniversalTime());
            }
            jObj["DatesFrom"] = fromArray;
            jObj["DatesTo"] = toArray;
            MakeRequestJson("mua/setavailability", jObj.ToString(), callback);
        }

        public static void MuaGetAvailability(string muaId, DateTime from, DateTime to, bool convertToDates, Action<ResponseDataJson> callback)
        {
            var jObj = new JObject();
            jObj["MuaId"] = muaId;
            jObj["From"] = from.ToString("yyyy-MM-dd hh:mm");
            jObj["To"] = to.ToString("yyyy-MM-dd hh:mm");
            jObj["ConvertToDates"] = convertToDates;
            MakeRequestJson("mua/getavailability", jObj.ToString(), callback);
        }

        public static void SendReview(string muaId, string customerId, int rating, string description, Action<ResponseDataJson> callback)
        {
            var jObj = new JObject();
            jObj["MuaId"] = muaId;
            jObj["CustomerId"] = customerId;
            jObj["Rating"] = rating;
            jObj["Description"] = description;
            MakeRequestJson("mua/sendreview", jObj.ToString(), callback);
        }

        public static void CanWriteReview(string muaId, string customerId, Action<ResponseDataJson> callback)
        {
            MakeRequestJson($"mua/canwritereview?muaId={muaId}&customerId={customerId}", "", callback);
        }

        public static void ServiceAdd(string muaId, string name, double price, int length, string category, Action<ResponseDataJson> callback)
        {
            var jObj = new JObject();
            jObj["MuaId"] = muaId;
            jObj["Name"] = name;
            jObj["Price"] = price.ToString(Utils.EnCulture);
            jObj["Length"] = length;
            jObj["Category"] = category;
            MakeRequestJson("mua/addservice", jObj.ToString(), callback);
        }

        public static void ServiceUpdate(string serviceId, string name, double price, int length, string category, Action<ResponseDataJson> callback)
        {
            var jObj = new JObject();
            jObj["Id"] = serviceId;
            jObj["Name"] = name;
            jObj["Price"] = price.ToString(Utils.EnCulture);
            jObj["Length"] = length;
            jObj["Category"] = category;
            MakeRequestJson("mua/UpdateService", jObj.ToString(), callback);
        }

        public static void ServiceDelete(string serviceId, Action<ResponseDataJson> callback)
        {
            MakeRequestJson($"mua/DeleteService?serviceId={serviceId}", string.Empty, callback);
        }

        public static void AddAppointment(string id, Order order, Action<ResponseDataJson> callback)
        {
            var jObj = new JObject();
            jObj["Id"] = id;
            jObj["MuaId"] = order.Mua.Id;
            jObj["CustomerId"] = GlobalStorage.Settings.CustomerId;
            jObj["Time"] = order.DateTime.ToUniversalTime();
            jObj["PaymentType"] = (int)order.PaymentType;
            jObj["Message"] = order.Note;
            jObj["Services"] = new JArray(order.Basket.Select(s =>
                new JObject {
                    new JProperty("ServiceId", s.Service.Id),
                    new JProperty("Count", s.Count)
                }));
            jObj["IsFree"] = order.IsFree;
            MakeRequestJson("appointment/add", jObj.ToString(), callback);
        }

        public static void GetAppointmentsByMua(string id, Action<ResponseDataJson> callback)
        {
            MakeRequestJson($"appointment/getbymua?id={id}", string.Empty, callback);
        }

        public static void GetAppointmentsByCustomer(string id, Action<ResponseDataJson> callback)
        {
            MakeRequestJson($"appointment/getbycustomer?id={id}", string.Empty, callback);
        }

        public static void GetAppointmentById(string id, Action<ResponseDataJson> callback)
        {
            MakeRequestJson($"appointment/get?id={id}", string.Empty, callback);
        }

        public static void GetCustomerInfo(string id, Action<ResponseDataJson> callback)
        {
            MakeRequestJson($"customer/getinfo?id={id}", string.Empty, callback);
        }

        public static void GetMuaInfo(string id, Action<ResponseDataJson> callback)
        {
            MakeRequestJson($"mua/getinfo?id={id}", string.Empty, callback);
        }

        public static void SetAppointmentStatus(string appointmentId, int status, Action<ResponseDataJson> callback)
        {
            var jObj = new JObject();
            jObj["AppointmentId"] = appointmentId;
            jObj["Status"] = status;
            MakeRequestJson("appointment/setstatus", jObj.ToString(), callback);
        }

        public static void SendCrash(string text, Action<ResponseData> callback)
        {
            var str = $"<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\"><![CDATA[{text}]]></string>";
            MakeRequest("helper/sendcrash", str, callback);
        }

        public static void PNRegister(string muaId, string customerId, string token)
        {
            if (token == null)
                return;
            var obj = new JObject();
            obj["Token"] = token;
            if (!string.IsNullOrEmpty(muaId))
            {
                obj["MuaId"] = muaId;
            }
            else if (!string.IsNullOrEmpty(customerId))
            {
                obj["CustomerId"] = customerId;
            }
            else
            {
                return;
            }
            MakeRequestJson("appointment/registerpn", obj.ToString(), r => { });
        }

        public static void PNUnregister(string muaId, string customerId)
        {
            var obj = new JObject();
            if (!string.IsNullOrEmpty(muaId))
            {
                obj["MuaId"] = muaId;
            }
            else if (!string.IsNullOrEmpty(customerId))
            {
                obj["CustomerId"] = customerId;
            }
            else
            {
                return;
            }
            MakeRequestJson("appointment/unregisterpn", obj.ToString(), r => { });
        }

        private static JObject BiuldJson(Dictionary<string, object> data)
        {
            var o = new JObject();
            foreach (var pair in data)
            {
                if (pair.Value == null)
                {
                    continue;
                }
                if (pair.Value is Dictionary<string, object>)
                {
                    var iobj = BiuldJson(pair.Value as Dictionary<string, object>);
                    o.Add(pair.Key, iobj);
                    continue;
                }
                if (pair.Value is List<string>)
                {
                    var jarr = new JArray();
                    foreach (var s in (pair.Value as List<string>))
                    {
                        jarr.Add(s);
                    }
                    o.Add(pair.Key, jarr);
                    continue;
                }
                o[pair.Key] = pair.Value.ToString();
            }
            return o;
        }

        private static string BuildXml(string headTag, Dictionary<string, object> data, bool includeTopNS)
        {
            var sb = new StringBuilder();
            sb.Append("<");
            sb.Append(headTag);
            if (includeTopNS)
            {
                sb.Append(" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/TiroWeb.Models\" ");
            }
            sb.Append(">");

            foreach (var item in data)
            {
                if (item.Value != null)
                {
                    sb.Append("<");
                    sb.Append(item.Key);
                    sb.Append(">");
                    sb.Append(item.Value.ToString().Replace(',', '.'));
                    sb.Append("</");
                    sb.Append(item.Key);
                    sb.Append(">");
                }
            }

            sb.Append("</");
            sb.Append(headTag);
            sb.Append(">");
            return sb.ToString();
        }

        private static string BuildXmlForSearchRequest(List<string> categories)
        {
            var sb = new StringBuilder();
            sb.Append("<");
            sb.Append("MuaSearchRequest");
            sb.Append(" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/TiroWeb.Models\" ");
            sb.Append(">");

            sb.Append("<AvailibilityFilter />");

            sb.Append("<CategoryFilter xmlns:d2p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">");

            foreach (var category in categories)
            {
                sb.Append("<d2p1:string>");
                sb.Append(category);
                sb.Append("</d2p1:string>");
            }
            sb.Append("</CategoryFilter>");

            sb.Append("<LocationFilter>");
            sb.Append("<Lat>1.1</Lat>");
            sb.Append("<Lon>2.1</Lon>");
            sb.Append("</LocationFilter>");

            sb.Append("</MuaSearchRequest>");
            return sb.ToString();
        }

        private static void MakeRequest(string method, string body, Action<ResponseData> callback)
        {
            System.Net.HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp(API_URL + method);
            request.ContentType = "text/xml";
            request.Accept = "text/xml";
            request.Method = "POST";
            request.BeginGetRequestStream((iar) =>
            {
                try
                {
                    var reqStream = ((System.Net.HttpWebRequest)iar.AsyncState).EndGetRequestStream(iar);
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(reqStream);
                    writer.Write(body);
                    writer.Flush();
                    request.BeginGetResponse((iar2) =>
                    {
                        var isOk = false;
                        try
                        {
                            var response = ((System.Net.HttpWebRequest)iar.AsyncState).EndGetResponse(iar2);
                            var responseStream = response.GetResponseStream();
                            var xmlDoc = System.Xml.Linq.XDocument.Load(responseStream);
                            responseStream.Dispose();
                            isOk = true;
                            callback.Invoke(new ResponseData(ResponseCode.OK, xmlDoc));
                        }
                        catch (Exception e)
                        {
                            if (!isOk)
                            {
                                callback.Invoke(new ResponseData(ResponseCode.Fail, null, e));
                            }
                        }
                    }, request);
                }
                catch (Exception e)
                {
                    callback.Invoke(new ResponseData(ResponseCode.Fail, null, e));
                }
            }, request);
        }

        private static void MakeRequestJson(string method, string body, Action<ResponseDataJson> callback)
        {
            System.Net.HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp(API_URL + method);
            request.ContentType = "text/json";
            request.Accept = "text/json";
            request.Method = "POST";
            request.BeginGetRequestStream((iar) =>
            {
                try
                {
                    var reqStream = ((System.Net.HttpWebRequest)iar.AsyncState).EndGetRequestStream(iar);
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(reqStream);
                    writer.Write(body);
                    writer.Flush();
                    request.BeginGetResponse((iar2) =>
                    {
                        var isOk = false;
                        try
                        {
                            var response = ((System.Net.HttpWebRequest)iar.AsyncState).EndGetResponse(iar2);
                            var responseStream = response.GetResponseStream();
                            var jsonStr = new System.IO.StreamReader(responseStream).ReadToEnd();
                            responseStream.Dispose();
                            isOk = true;
                            callback.Invoke(new ResponseDataJson(ResponseCode.OK, jsonStr));
                        }
                        catch (Exception e)
                        {
                            if (!isOk)
                            {
                                callback.Invoke(new ResponseDataJson(ResponseCode.Fail, null, e));
                            }
                        }
                    }, request);
                }
                catch (Exception e)
                {
                    callback.Invoke(new ResponseDataJson(ResponseCode.Fail, null, e));
                }
            }, request);
        }
    }

    public enum ResponseCode
    {
        OK, Fail
    }

    public enum PaymentType
    {
        Cash, Card
    }

    public class ResponseData
    {
        public ResponseData(ResponseCode code, System.Xml.Linq.XDocument result)
        {
            Code = code;
            Result = result;
        }
        public ResponseData(ResponseCode code, System.Xml.Linq.XDocument result, Exception error)
            : this(code, result)
        {
            this.Error = error;
        }
        public System.Xml.Linq.XDocument Result { get; set; }
        public ResponseCode Code { get; set; }
        public Exception Error { get; set; }
    }

    public class ResponseDataJson
    {
        public ResponseDataJson(ResponseCode code, string result)
        {
            Code = code;
            Result = result;
        }
        public ResponseDataJson(ResponseCode code, string result, Exception error)
            : this(code, result)
        {
            this.Error = error;
        }
        public string Result { get; set; }
        public ResponseCode Code { get; set; }
        public Exception Error { get; set; }
    }
}
