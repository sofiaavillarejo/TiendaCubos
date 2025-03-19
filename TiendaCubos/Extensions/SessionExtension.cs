﻿using Newtonsoft.Json;

namespace TiendaCubos.Extensions
{
    public static class SessionExtension
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            string json = JsonConvert.SerializeObject(value);
            session.SetString(key, json); //guardamos lo q tenemos
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            string data = session.GetString(key);
            if (data == null)
            {
                return default(T);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(data); //deserializamos el objeto que tenemos para mostrarlo
            }
        }
    }
}
