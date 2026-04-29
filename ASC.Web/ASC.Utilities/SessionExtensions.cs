using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace ASC.Utilities
{
    public static class SessionExtensions
    {
        public static void SetSession<T>(this ISession session, string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            var bytes = Encoding.UTF8.GetBytes(json);

            session.Set(key, bytes);
        }

        public static T? GetSession<T>(this ISession session, string key)
        {
            if (session.TryGetValue(key, out var bytes))
            {
                var json = Encoding.UTF8.GetString(bytes);
                return JsonConvert.DeserializeObject<T>(json);
            }

            return default;
        }
    }
}