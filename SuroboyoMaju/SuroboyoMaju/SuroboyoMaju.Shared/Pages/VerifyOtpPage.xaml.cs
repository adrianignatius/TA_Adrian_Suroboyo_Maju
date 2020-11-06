﻿using Newtonsoft.Json.Linq;
using SuroboyoMaju.Shared.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#if __ANDROID__
using Com.OneSignal;
using Com.OneSignal.Abstractions;
#endif


namespace SuroboyoMaju.Shared.Pages
{

    public sealed partial class VerifyOtpPage : Page
    {
        Session session;
        User userRegister;
        DispatcherTimer timer;
        HttpObject httpObject;
        int countdown = 30;
        public VerifyOtpPage()
        {
            this.InitializeComponent();
            txtOtp.Focus(FocusState.Keyboard);
            session = new Session();
            userRegister = session.getUserLogin();
            httpObject = new HttpObject();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            updateTxtTimer();
            timer.Start();
        }

        private void Back_Click(object sender,RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }

        private void updateTxtTimer()
        {
            if (countdown < 10)
            {
                txtTimer.Text = "00:0" + countdown;
            }
            else
            {
                txtTimer.Text = "00:" + countdown;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            if (countdown == 0)
            {
                timer.Stop();
                txtTimer.Visibility = Visibility.Collapsed;
            }
            else
            {
                countdown--;
                updateTxtTimer();
            }
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            
            sendOTP();
        }

        private async void sendOTP()
        {
            string responseData = await httpObject.PostRequestUrlEncodedWithAuthorization("user/sendOTP/"+userRegister.telpon_user, null, session.getTokenAuthorization());
            JObject json = JObject.Parse(responseData);
            var message = new MessageDialog(json["message"].ToString());
            await message.ShowAsync();
        }

        private async void sendOTP(object sender, RoutedEventArgs e)
        {
            if (countdown > 0)
            {
                var message = new MessageDialog("Tunggu beberapa saat lagi untuk request kode baru");
                await message.ShowAsync();
            }
            else
            {
                countdown = 30;
                timer.Start();
                txtTimer.Visibility = Visibility.Visible;
                sendOTP();
                updateTxtTimer();
            }
        }

        private async void confirmOTP(object sender, RoutedEventArgs e)
        {
            if (txtOtp.Text.Length != 0)
            {
                var content = new FormUrlEncodedContent(new[]{
                    new KeyValuePair<string, string>("number", userRegister.telpon_user),
                    new KeyValuePair<string, string>("otp_code", txtOtp.Text)
                });
                string responseData = await httpObject.PostRequestUrlEncodedWithAuthorization("user/verifyOTP", content, session.getTokenAuthorization());
                JObject json = JObject.Parse(responseData);
                var message = new MessageDialog(json["message"].ToString());
                await message.ShowAsync();
                if (json["status"].ToString() == "1")
                {
#if __ANDROID__
                        OneSignal.Current.SendTags(new Dictionary<string, string>() { {"no_handphone", userRegister.telpon_user}, {"tipe_user", userRegister.status_user.ToString()} });               
#endif
                    this.Frame.Navigate(typeof(HomeNavigationPage));
                }
                txtOtp.Text = "";
            }
            else
            {
                await new MessageDialog("Anda belum memasukkan kode").ShowAsync();
            }
        }

        private void txtOtpBeforeChangingEvent(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

    }
}
