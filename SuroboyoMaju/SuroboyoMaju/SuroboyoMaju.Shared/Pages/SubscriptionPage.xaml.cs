﻿using Newtonsoft.Json.Linq;
using SuroboyoMaju.Shared.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class SubscriptionPage : Page
    {
        List<string> listMonthExpired, listYearExpired;
        Session session;
        User userLogin;
        HttpObject httpObject;
        public SubscriptionPage()
        {
            this.InitializeComponent();
            listMonthExpired = new List<string>();
            listYearExpired = new List<string>();
            for (int i = 1; i <= 12; i++)
            {
                string month = (i > 9) ? i.ToString() : month = "0" + i;
                listMonthExpired.Add(month);
            }
            for (int i = 20; i <= 40; i++)
            {
                listYearExpired.Add(i.ToString());
            }
            cbExpiredMonth.ItemsSource = listMonthExpired;
            cbExpiredYear.ItemsSource = listYearExpired;
            session = new Session();
            httpObject = new HttpObject();
            userLogin = session.getUserLogin();
        }

        private async void confirmUpgrade(object sender, RoutedEventArgs e)
        {
            if (cbExpiredYear.SelectedIndex == -1 || cbExpiredMonth.SelectedIndex == 1)
            {
                var message = new MessageDialog("Pastikan Semua Data Kartu Terisi");
                await message.ShowAsync();
            }
            else
            {
                using (var client = new HttpClient())
                {
                    string cardNumber, cardExpMonth, cardExpYear, clientKey;
                    clientKey = "SB-Mid-client-J4xpVwGv_HmNID-g";
                    cardNumber = "4811 1111 1111 1114";
                    cardExpMonth = cbExpiredMonth.SelectedItem.ToString();
                    cardExpYear = "20" + cbExpiredYear.SelectedItem.ToString();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", "Basic U0ItTWlkLXNlcnZlci1GQjRNSERieVhlcFc5OFNRWjY0SHhNeEU=");
                    string reqUri = "https://api.sandbox.midtrans.com/v2/card/register?card_number=" + cardNumber + "&card_exp_month=" + cardExpMonth + "&card_exp_year=" + cardExpYear + "&client_key=" + clientKey;
                    HttpResponseMessage response = await client.GetAsync(reqUri);
                    if (response.IsSuccessStatusCode)
                    {
                        string idUser = userLogin.id_user.ToString();
                        var hasil = response.Content.ReadAsStringAsync().Result;
                        JObject json = JObject.Parse(hasil);
                        string tokenId = json["saved_token_id"].ToString();
                        var content = new FormUrlEncodedContent(new[]{
                            new KeyValuePair<string, string>("id_user", idUser),
                            new KeyValuePair<string, string>("credit_card_token", tokenId),
                        });
                        string responseData = await httpObject.PutRequest("user/updateCreditCardToken", content, session.getTokenAuthorization());
                        json = JObject.Parse(responseData);
                        if (json["status"].ToString() == "1")
                        {
                            var content2 = new FormUrlEncodedContent(new[]{
                                new KeyValuePair<string, string>("id_user", idUser)
                            });
                            responseData = await httpObject.PostRequestUrlEncodedWithAuthorization("user/chargeUser", content2, session.getTokenAuthorization());
                            json = JObject.Parse(responseData);
                            var message = new MessageDialog(json["message"].ToString());
                            await message.ShowAsync();
                            if (json["status"].ToString() == "1")
                            {
                                userLogin.status_user = 1;
                                userLogin.premium_available_until = json["premium_available_until"].ToString();
                                this.Frame.GoBack();
                            }
                        }
                    }
                    else
                    {
                        var message = new MessageDialog(response.StatusCode.ToString());
                        await message.ShowAsync();
                    }
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        private bool On_BackRequested()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            return false;
        }
    }
}
