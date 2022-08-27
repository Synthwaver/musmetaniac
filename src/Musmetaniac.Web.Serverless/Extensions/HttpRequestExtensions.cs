using System;
using System.ComponentModel;
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
            var nullabilityInfoContext = new NullabilityInfoContext();

            foreach (var property in typeof(T).GetProperties())
            {
                var propertyType = property.PropertyType;
                if (!propertyType.IsValueType && propertyType != typeof(string))
                    throw new NotSupportedException();

                var isPropertyProvided = self.Query.TryGetValue(property.Name, out var stringValues);
                if (!isPropertyProvided)
                {
                    var nullabilityInfo = nullabilityInfoContext.Create(property);
                    var isNullable = nullabilityInfo.ReadState == NullabilityState.Nullable || nullabilityInfo.WriteState == NullabilityState.Nullable;

                    if (!isNullable)
                        throw new BusinessException($"Property '{property.Name}' is required.");

                    continue;
                }

                if (stringValues.Count > 1)
                    throw new BusinessException($"Property '{property.Name}' is presented several times.");

                var typeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                object propertyValue;

                try
                {
                    propertyValue = typeConverter.ConvertFromInvariantString(stringValues)!;
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
