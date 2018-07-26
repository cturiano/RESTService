using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

#pragma warning disable 1584,1711,1572,1581,1580

namespace Service.Extensions
{
    public static class JsonExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts the given JObject into a dictionary of strings.
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns>The given JObject as a dictionary of strings.</returns>
        /// <exception cref="ArgumentNullException">The key is null.</exception>
        public static async Task<IDictionary<string, string>> ConvertJObjectToDictionaryAsync(this JObject jObject)
        {
            if (jObject == null)
            {
                return null;
            }

            return await Task.Run(() =>
                                  {
                                      var realDictionary = new Dictionary<string, string>();

                                      foreach (var kvp in jObject)
                                      {
                                          realDictionary[kvp.Key] = kvp.Value.ToString();
                                      }

                                      return realDictionary;
                                  });
        }

        /// <summary>
        ///     Serializes the given object into a JSON string.
        /// </summary>
        public static string SerializeObject<T>(T data)
        {
            var jsonSerializer = new JsonSerializer
                                 {
                                     ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                     NullValueHandling = NullValueHandling.Ignore
                                 };


            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder))
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    jsonSerializer.Serialize(jsonWriter, data);
                }
            }

            return stringBuilder.ToString();
        }

        #endregion
    }
}