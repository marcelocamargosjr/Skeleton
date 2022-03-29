using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Skeleton.Domain.Core.Utilities
{
    public static class ObjectExtensions
    {
        /// <summary>
        ///     The string representation of null.
        /// </summary>
        private const string Null = "Null";

        /// <summary>
        ///     The string representation of exception.
        /// </summary>
        private const string Exception = "Exception";

        /// <summary>
        ///     To json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The Json of any object.</returns>
        public static string ToJson(this object? value)
        {
            if (value is null) return Null;

            try
            {
                return JsonConvert.SerializeObject(value, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
            catch (Exception)
            {
                return Exception;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescriptionFromEnumValue(this Enum value)
        {
            return value.GetType().GetField(value.ToString())
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() is not DescriptionAttribute attribute
                ? value.ToString()
                : attribute.Description;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T? GetEnumValueFromDescription<T>(this string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException(nameof(type));

            var fields = type.GetFields();
            var field = fields
                .SelectMany(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false), (f, a) => new { Field = f, Attribute = a })
                .SingleOrDefault(a => ((DescriptionAttribute)a.Attribute).Description.ToLower().Equals(description.ToLower()));

            return field is null ? default : (T)field.Field.GetRawConstantValue()!;
        }
    }
}