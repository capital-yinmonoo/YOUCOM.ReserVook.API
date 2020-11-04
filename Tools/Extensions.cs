using Newtonsoft.Json;
using System;

namespace YOUCOM.ReserVook.API.Tools
{
    public static class Extensions
    {
        public static T ToObject<T>(this object obj)
        {
            var result = default(T);
            try
            {
                var str = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
                {
                    //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    //Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }); ;
                result = JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }
    }
}
