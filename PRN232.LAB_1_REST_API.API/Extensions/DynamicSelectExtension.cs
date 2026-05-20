using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace PRN232.LAB_1_REST_API.API.Extensions
{
    public static class DynamicSelectExtension
    {
        public static object ShapeData<T>(this T source, string? fields)
        {
            if (source == null) return null;
            if (string.IsNullOrWhiteSpace(fields)) return source;

            var expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fieldsList = fields.Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                                   .Select(f => f.Trim().ToLowerInvariant())
                                   .ToList();

            foreach (var propertyInfo in propertyInfos)
            {
                if (fieldsList.Contains(propertyInfo.Name.ToLowerInvariant()))
                {
                    var propertyValue = propertyInfo.GetValue(source);
                    dictionary.Add(propertyInfo.Name, propertyValue);
                }
            }

            return expando;
        }

        public static IEnumerable<object> ShapeData<T>(this IEnumerable<T> source, string? fields)
        {
            if (string.IsNullOrWhiteSpace(fields)) return source.Cast<object>();

            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fieldsList = fields.Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                                   .Select(f => f.Trim().ToLowerInvariant())
                                   .ToList();

            var validProperties = propertyInfos
                .Where(p => fieldsList.Contains(p.Name.ToLowerInvariant()))
                .ToList();

            var shapedData = new List<object>();

            foreach (var item in source)
            {
                var expando = new ExpandoObject();
                var dictionary = (IDictionary<string, object>)expando;

                foreach (var propertyInfo in validProperties)
                {
                    var propertyValue = propertyInfo.GetValue(item);
                    // Dữ liệu JSON luôn trả về camelCase nên ta định dạng sẵn key
                    string keyName = char.ToLowerInvariant(propertyInfo.Name[0]) + propertyInfo.Name.Substring(1);
                    dictionary.Add(keyName, propertyValue);
                }

                shapedData.Add(expando);
            }

            return shapedData;
        }
    }
}
