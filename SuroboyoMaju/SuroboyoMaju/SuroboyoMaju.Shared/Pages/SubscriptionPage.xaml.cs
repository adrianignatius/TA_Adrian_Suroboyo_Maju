using Newtonsoft.Json.Linq;
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

        private void validateInput(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private async void confirmUpgrade(object sender, RoutedEventArgs e)
        {
            if (cbExpiredYear.SelectedIndex == -1 || cbExpiredMonth.SelectedIndex == 1||txtCardNumber.Text.Length==0||txtCvvNumber.Text.Length==0)
            {
                var message = new MessageDialog("Pastikan Semua Data Kartu Terisi");
                await message.ShowAsync();
            }
            else
            {
                var content = new FormUrlEncodedContent(new[]{
                    new KeyValuePair<string, string>("card_number", txtCardNumber.Text),
                    new KeyValuePair<string, string>("card_exp_month", cbExpiredMonth.SelectedItem.ToString()),
                    new KeyValuePair<string, string>("card_exp_year", "20"+cbExpiredYear.SelectedItem.ToString())
                });
                string responseData = await httpObject.PostRequestUrlEncodedWithAuthorization("user/registerCard/"+userLogin.id_user, content, session.getTokenAuthorization());
                JObject json = JObject.Parse(responseData);
                if (json["status"].ToString() == "1")
                {
                    content = new FormUrlEncodedContent(new[]{
                        new KeyValuePair<string, string>("tanggal_charge", DateTime.Now.ToString("yyyy-MM-dd"))
                    });
                    responseData = await httpObject.PostRequestUrlEncodedWithAuthorization("user/chargeUser/"+userLogin.id_user, content, session.getTokenAuthorization());
                    json = JObject.Parse(responseData);
                    if (json["status"].ToString() == "1")
                    {
                        await new MessageDialog("Berhasil Melakukan Pembayaran, Akun anda sekarang terdaftar sebagai akun premium").ShowAsync();
                        userLogin.status_user = 1;
                        userLogin.premium_available_until = json["premium_available_until"].ToString();
                        this.Frame.GoBack();
                    }else{
                        await new MessageDialog(json["message"].ToString()).ShowAsync();
                    }
                }
                else
                {
                    await new MessageDialog(json["message"].ToString()).ShowAsync();
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
