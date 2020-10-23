using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuroboyoMaju.Shared.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class ChatListPage : Page
    {
        User userLogin;
        Session session;
        HttpObject httpObject;
        ObservableCollection<DisplayHeaderChat> listDisplayHeaderChat;
        public ChatListPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
        }

        public void pageLoaded(object sender, RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
            loadHeaderChat();
        }

        private async void loadHeaderChat()
        {
            listDisplayHeaderChat = new ObservableCollection<DisplayHeaderChat>();
            string responseData = await httpObject.GetRequestWithAuthorization("user/getHeaderChat/" + userLogin.id_user, session.getTokenAuthorization());
            ObservableCollection<HeaderChat> tempHeaderChat = JsonConvert.DeserializeObject<ObservableCollection<HeaderChat>>(responseData);
            for (int i = 0; i < tempHeaderChat.Count; i++)
            {
                int id_target_chat = 0;
                string namaDisplay = "";
                if (userLogin.id_user == tempHeaderChat[i].id_user_1)
                {
                    namaDisplay = tempHeaderChat[i].nama_user_2;
                    id_target_chat = tempHeaderChat[i].id_user_2;
                }
                else
                {
                    namaDisplay = tempHeaderChat[i].nama_user_1;
                    id_target_chat = tempHeaderChat[i].id_user_1;
                }
                responseData = await httpObject.GetRequestWithAuthorization("user/getLastMessage/" + tempHeaderChat[i].id_chat, session.getTokenAuthorization());
                JObject json = JObject.Parse(responseData);
                DisplayHeaderChat displayHeaderChat = new DisplayHeaderChat(tempHeaderChat[i].id_chat, id_target_chat, namaDisplay, null);
                if (json["status"].ToString() == "1")
                {
                    displayHeaderChat.waktu_chat = DateTime.ParseExact(json["data"]["waktu_chat"].ToString(), "yyyy-MM-dd HH:mm:ss", null);
                    displayHeaderChat.pesan_display = json["data"]["isi_chat"].ToString();
                }
                listDisplayHeaderChat.Add(displayHeaderChat);

            }
            listDisplayHeaderChat = new ObservableCollection<DisplayHeaderChat>(listDisplayHeaderChat.OrderByDescending(k => k.waktu_chat));
            if (listDisplayHeaderChat.Count == 0)
            {
                stackEmpty.Visibility = Visibility.Visible;
                lvDaftarChat.Visibility = Visibility.Collapsed;
            }
            else
            {
                lvDaftarChat.ItemsSource = listDisplayHeaderChat;
            }
        }

        private void goToChatPage(object sender, ItemClickEventArgs e)
        {
            DisplayHeaderChat selected = (DisplayHeaderChat)e.ClickedItem;
            ChatPageParams param = new ChatPageParams(selected.id_chat, userLogin.id_user, selected.id_target_chat, selected.nama_display);
            session.setChatPageParams(param);
            this.Frame.Navigate(typeof(PersonalChatPage));
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
