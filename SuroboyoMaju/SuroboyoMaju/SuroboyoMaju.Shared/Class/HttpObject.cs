﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace SuroboyoMaju.Shared.Class
{
    class HttpObject
    {
        public readonly static string API_URL = "http://adrian-webservice.ta-istts.com/";
        public readonly static string URL_DEBUG = "http://localhost:8080/";


        public async Task<string> GetRequestWithAuthorization(string url, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);
                //client.BaseAddress = new Uri(URL_DEBUG);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", token);
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        return responseData;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    var message = new MessageDialog("Koneksi Error");
                    await message.ShowAsync();
                    return null;
                }
            }
        }

        public async Task<string> GetRequest(string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);
                //client.BaseAddress = new Uri(URL_DEBUG);
                client.DefaultRequestHeaders.Accept.Clear();
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        return responseData;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    var message = new MessageDialog("Koneksi Error");
                    await message.ShowAsync();
                    return null;
                }
            }
        }

        public async Task<string> PostRequestWithMultipartFormData(string url, MultipartFormDataContent form, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);
                client.DefaultRequestHeaders.Add("Authorization", token);
                HttpResponseMessage response = await client.PostAsync(url, form);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    return responseData;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<string> PostRequestUrlEncodedWithAuthorization(string url, FormUrlEncodedContent form, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);
                client.DefaultRequestHeaders.Add("Authorization", token);
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.PostAsync(url, form);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    return responseData;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<string> PostRequestWithUrlEncoded(string url, FormUrlEncodedContent form)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.PostAsync(url, form);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    return responseData;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<string> PutRequest(string url, FormUrlEncodedContent form, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", token);
                HttpResponseMessage response = await client.PutAsync(url, form);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    return responseData;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<string> DeleteRequest(string url, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);
                client.DefaultRequestHeaders.Add("Authorization", token);
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    return responseData;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
