using System.Linq;
using System.Collections.Specialized;
using System.Web;

namespace MakelaarsList.Helper
{
    public static class QueryStringHelper
    {
        public static string ToQueryString(NameValueCollection collection)
        {
            if (collection == null) return "";

            var itemsAsString = (
                    from key in collection.AllKeys
                    from value in collection.GetValues(key)
                    select string.Format(
                        "{0}={1}",
                        HttpUtility.UrlEncode(key),
                        HttpUtility.UrlEncode(value))).ToArray();

            return "?" + string.Join("&", itemsAsString);
        }
    }
}
