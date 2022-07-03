using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Musmetaniac.Common.Exceptions;

namespace Musmetaniac.Web.Serverless.Extensions
{
    public static class HttpRequestExtensions
    {
        public static T FromQueryString<T>(this HttpRequest self) where T : new()
        {
            var resultObject = new T();

            foreach (var property in typeof(T).GetProperties())
            {
                var propertyType = property.PropertyType;
                if (!propertyType.IsValueType && propertyType != typeof(string))
                    throw new NotSupportedException();

                var isPropertyProvided = self.Query.TryGetValue(property.Name, out var stringValue);
                if (!isPropertyProvided)
                {
                    var propertyCanBeNull = !propertyType.IsValueType || Nullable.GetUnderlyingType(propertyType) != null;
                    var isPropertyRequired = !propertyCanBeNull || property.GetCustomAttribute(typeof(RequiredAttribute)) != null;

                    if (isPropertyRequired)
                        throw new BusinessException($"Property '{property.Name}' is required.");

                    continue;
                }

                var typeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                object propertyValue;

                try
                {
                    propertyValue = typeConverter.ConvertFromInvariantString(stringValue);
                }
                catch (ArgumentException)
                {
                    throw new BusinessException($"Property '{property.Name}' is invalid.");
                }

                property.SetValue(resultObject, propertyValue);
            }

            return resultObject;
        }
    }
}
