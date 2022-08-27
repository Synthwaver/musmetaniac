using Newtonsoft.Json;

namespace Musmetaniac.Common.Extensions
{
    public static class JsonExtensions
    {
        public static T? FromJson<T>(this string? self) where T : class
        {
            return self.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject<T>(self);
        }

        public static string ToJson(this object self)
        {
            return JsonConvert.SerializeObject(self);
        }
    }
}
