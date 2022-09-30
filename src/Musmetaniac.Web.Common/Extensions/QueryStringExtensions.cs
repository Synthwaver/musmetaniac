using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Musmetaniac.Web.Common.Extensions
{
    public static class QueryStringExtensions
    {
        public static QueryString ToQueryString<T>(this T? self) where T : class
        {
            var queryParameters = new Dictionary<string, string?>();

            foreach (var property in typeof(T).GetProperties())
            {
                var propertyType = property.PropertyType;
                if (!propertyType.IsValueType && propertyType != typeof(string))
                    throw new NotSupportedException();

                var propertyValue = property.GetValue(self);
                if (propertyValue == null)
                    continue;

                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propertyType = Nullable.GetUnderlyingType(propertyType);

                var parameterValue = propertyType == typeof(DateTime)
                    ? ((DateTime)propertyValue).ToUniversalTime().ToString(QueryStringConst.DateTimeFormat)
                    : propertyValue.ToString();

                queryParameters.Add(property.Name, parameterValue);
            }

            return new QueryBuilder(queryParameters).ToQueryString();
        }
    }
}
