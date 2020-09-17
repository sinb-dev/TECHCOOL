using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;
using System.Text;

namespace TECHCOOL
{  
    /// <summary>class <c>Request</c> context returned by <c>HttpListener</c>. 
    /// and a <c>MatchCollection</c> containing information on how the Request was routed</summary>
    public class Request {
        public HttpListenerContext Context {get;set;}
        public MatchCollection Matches {get;set;}
        public RequestData Data {get;set;} = new RequestData();
        public string HttpMethod { get { return Context.Request.HttpMethod; }}
    }
    
    public class RequestData {
        public Dictionary<string,string> Post {get; internal set;} = new Dictionary<string, string>();
        public Dictionary<string,string> Get {get; internal set;} = new Dictionary<string, string>();
    }
    /// <summary>class <c>WebLet</c> is a small and basic webserver object. It listens for HTTP requests on a specified URI. 
    /// and responds with a string returned by a user specified method.</summary>
    public class WebLet {
        public class WebLetSettings {
            /// <summary>property <c>DocumentRoot</c> The document root will be used in case a request
            /// is not routed explicitly. Can be used to send stylesheets, javascripts and other content types to the user. Contains the current document root or current 
            /// working directory if no root was set </summary>
            public string DocumentRoot  {get;set;} = Directory.GetCurrentDirectory();
            /// <summary>property <c>DefaultIndexFile</c> The file to use for if no specific file was requested  </summary>
            public string DefaultIndexFile {get;set;} = "index.html";
            /// <summary>property <c>UseThreading</c> sets wether to prevent blocking of main thread and wait for new connections
            /// or to put the listening process in a thread.</summary>
            public bool UseThreading {get;set;} = false;
        }

        HttpListener httpListener;
        List<string> prefixes = new List<string>();
        bool running = false;
        public WebLetSettings Settings { get; protected set;} = new WebLetSettings();
         
        Thread thread;
        Dictionary<string,Func<Request,string>> routes = new Dictionary<string, Func<Request,string>>();
        public WebLet(string address) 
        {
            if (!string.IsNullOrWhiteSpace(address))
                prefixes.Add(address);
        }
        
        /// <summary>method <c>Start</c> creates a new HttpListener instance and either create a thread for listening
        /// or block main thread until a connection was established.</summary>
        public void Start()
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine ("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            if (prefixes.Count == 0) {
                throw new ArgumentException("Webserver is missing prefixes (The webserver must know the URI used to contact it with eg. http://localhost:8080/");
            }
            running = true;

            httpListener = new HttpListener();
            foreach (var s in prefixes) {
                httpListener.Prefixes.Add(s);
            }
            if (Settings.UseThreading) {
                thread = new Thread(new ThreadStart(listen));
                thread.Start();
            }
            else
                listen();
        }
        
        
        /// <summary>method <c>Route</c> pairs a URL together with a method. 
        /// Whenever a match is encountered the method passed is invoked  </summary>
        public void Route(string route, Func<Request,string> method) 
        {
            routes.Add(route,method);
        }
        void processRequest(HttpListenerContext context) 
        {
            HttpListenerRequest request = context.Request;
            Request webLetRequest = new Request();
            webLetRequest.Context = context;
            if (request.HttpMethod == "POST")
            {
                processPostRequest(webLetRequest);
            }

            //Find a route
            foreach (KeyValuePair<string,Func<Request,string>> kv in routes) {
                var pattern = kv.Key;
                var matches = Regex.Matches(context.Request.Url.AbsolutePath,pattern);
                
                if (matches.Count > 0) {
                    
                    webLetRequest.Matches = matches;
                    string response = kv.Value(webLetRequest);
                    respond(context,response,string.IsNullOrEmpty(response) ? 500 : 200);
                    return;
                }
            }

            //No route found. Examine document root
            string filename = context.Request.Url.AbsolutePath;
            if (!System.IO.Path.HasExtension(filename)) filename += Settings.DefaultIndexFile;

            //Check if file exists
            string testPath = Settings.DocumentRoot + filename;

            if (!IsSubDirectoryOf(testPath, Settings.DocumentRoot)) {
                //Cannot access this folder
                InternalServerError(context);
                return;
            }

            if (File.Exists(testPath)) {
                var contentType = mimeType(testPath);
                switch (contentType) {
                    case "application/octet-stream":
                        byte[] bytes = File.ReadAllBytes(testPath);
                        respond(context,bytes,200);
                        return;
                    case "text/html":
                    case "application/x-javascript":
                    case "text/javascript":
                    case "text/css":
                        string response = File.ReadAllText(testPath);
                        respond(context,response,200);
                        return;
                    default:
                        throw new Exception("Temp exception : unhandlede mime type "+contentType);
                }
            }

            PageNotFound(context);
            // Obtain a response object.
            //HttpListenerResponse response = context.Response;
        }
        void processPostRequest(Request webLetRequest) 
        {
            HttpListenerRequest request = webLetRequest.Context.Request;

            if (!request.HasEntityBody) return;

            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);

            string s = reader.ReadToEnd();
            body.Close();
            reader.Close();

            webLetRequest.Data.Post = handleRequestData(s);
        }
        Dictionary<string,string> handleRequestData(string data) 
        {
            var dict = new Dictionary<string,string>();
            var paramStrings = data.Split('&');
            foreach (string paramString in paramStrings) 
            {
                int idx = paramString.IndexOf("=");
                var key = System.Web.HttpUtility.UrlDecode(paramString.Substring(0,idx));
                var value = System.Web.HttpUtility.UrlDecode(paramString.Substring(idx+1));
                dict[key] = value;
            }
            return dict;
        }
        void listen() 
        {
            httpListener.Start();
            while (running) {
                var context = httpListener.GetContext();
                
                processRequest(context);
            }
        }
        void respond(HttpListenerContext context, string text, int code=200) 
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            respond(context, buffer, code);
            
        }
        void respond(HttpListenerContext context, byte[] buffer, int code=200)
        {
            try {
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.StatusCode = code;
                context.Response.Close();
            } catch (Exception) {}
        }
        public void PageNotFound(HttpListenerContext context) 
        {
            string response = "<html><head><title>404 Page not found</title><body><h1>404 Page not found</h1></body></html>";
            respond(context,response,404);
        }
        public void InternalServerError(HttpListenerContext context) 
        {
            string response = "<html><head><title>500 Internal server error</title><body><h1>500 Internal server error</h1></body></html>";
            respond(context,response,500);
        }

        static string mimeType(string filename) {
            string[] names = filename.Split('.');
            switch (names[names.Length-1]) {
                case "js":
                    return "application/javascript";
                case "css":
                    return "text/css";
                case "jpeg":
                case "jpg":
                    return "image/jpeg";
                case "png":
                    return "image/png";
                default:
                    return "application/octet-stream";
            }
        }
        static bool IsSubDirectoryOf(string candidate, string other)
        {
            var isChild = false;
            try
            {
                var candidateInfo = new DirectoryInfo(candidate);
                var otherInfo = new DirectoryInfo(other);

                while (candidateInfo.Parent != null)
                {
                    if (candidateInfo.Parent.FullName == otherInfo.FullName)
                    {
                        isChild = true;
                        break;
                    }
                    else candidateInfo = candidateInfo.Parent;
                }
            }
            catch (Exception error)
            {
                var message = String.Format("Unable to check directories {0} and {1}: {2}", candidate, other, error);
            }

            return isChild;
        }
    }
}