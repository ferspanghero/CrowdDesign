using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;

namespace CrowdDesign.Utils.AspNet.Mvc
{
    /// <summary>
    /// Represents an attribute that detects if an ASP.NET MVC controller action receives multiple requests from the same client.
    /// </summary>
    public class DetectMultipleRequestsAttribute : ActionFilterAttribute
    {
        public DetectMultipleRequestsAttribute()
        {
            DelayInMilliseconds = 1500;
            ViewDataKey = "MultipleRequests";
        }

        /// <summary>
        /// Gets or sets the delay between multiple requests from the same client for an operation.
        /// </summary>
        public int DelayInMilliseconds { get; set; }
        
        /// <summary>
        /// Gets or sets a key to identify a generated entry in the controller's view data when multiple requests are detected.
        /// </summary>
        public string ViewDataKey { get; set; }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var cache = filterContext.HttpContext.Cache;
            StringBuilder builder = new StringBuilder(string.Empty);

            // Creates a key to identify the client

            // Appends the client's IP address
            builder.Append(request.UserHostAddress);

            // Appends the client's User Agent
            builder.Append(request.UserAgent);

            // Appends the client's target URL
            builder.Append(request.RawUrl);
            builder.Append(request.QueryString);

            // Generates an hexadecimal hash value for the client's key
            var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(builder.ToString())).Select(s => s.ToString("x2")));

            // Verifies if the cache contains the client's key
            if (cache[hashValue] != null)
            {
                // Adds an entry to the controller's view data
                filterContext.Controller.ViewData.Add(ViewDataKey, true);
            }
            else
            {
                // Adds an empty object to the cache using the client's key
                cache.Add(hashValue, string.Empty, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMilliseconds(DelayInMilliseconds), CacheItemPriority.Default, null);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}