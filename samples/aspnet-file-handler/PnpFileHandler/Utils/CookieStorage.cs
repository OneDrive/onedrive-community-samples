/*
 * PnP File Handler - Sample Code
 * Copyright (c) Microsoft Corporation
 * All rights reserved. 
 * 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the ""Software""), to deal in 
 * the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace PnpFileHandler
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.Script.Serialization;


    public class CookieStorage
    {
        private const string CookieName = "FileHandlerActivationParameters";


        public static NameValueCollection Load(Microsoft.Owin.RequestCookieCollection cookies)
        {
            var cookie = cookies[CookieName];
            if (cookie != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new JavaScriptConverter[] { new NameValueCollectionConverter() });
                return serializer.Deserialize<NameValueCollection>(cookie);
            }
            else
            {
                return null;
            }
        }

        public static NameValueCollection Load(HttpRequestBase request)
        {
            HttpCookie cookie = request.Cookies[CookieName];

            if (cookie != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new JavaScriptConverter[] { new NameValueCollectionConverter() });
                return serializer.Deserialize<NameValueCollection>(cookie.Value);
            }
            else
            {
                return null;
            }
        }

        public static void Save(NameValueCollection collection, HttpResponse response)
        {
            HttpCookie cookie = new HttpCookie(CookieName);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new NameValueCollectionConverter() });

            cookie.Value = serializer.Serialize(collection);
            cookie.Expires = DateTime.Now.AddMinutes(10);

            response.Cookies.Add(cookie);
        }

        public static void Clear(HttpRequestBase request, HttpResponseBase response)
        {
            HttpCookie cookie = request.Cookies[CookieName];

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                response.Cookies.Add(cookie);
            }
        }

        private class NameValueCollectionConverter : JavaScriptConverter
        {
            public override IEnumerable<Type> SupportedTypes
            {
                get
                {
                    return new List<Type>
                    {
                        typeof(NameValueCollection)
                    };
                }
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                NameValueCollection collection = obj as NameValueCollection;
                Dictionary<string, object> result = new Dictionary<string, object>();

                if (collection != null)
                {
                    foreach (string key in collection.AllKeys)
                    {
                        result.Add(key, collection[key]);
                    }
                }

                return result;
            }

            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException("dictionary");
                }

                if (type == typeof(NameValueCollection))
                {
                    NameValueCollection collection = new NameValueCollection();

                    foreach (string key in dictionary.Keys)
                    {
                        collection.Add(key, dictionary[key] as string);
                    }

                    return collection;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}