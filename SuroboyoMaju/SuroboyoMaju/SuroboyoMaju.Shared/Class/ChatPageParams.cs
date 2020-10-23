﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SuroboyoMaju.Shared.Class
{
    class ChatPageParams
    {
        public int id_chat { get; set; }
        public int id_user_pengirim { get; set; }
        public int id_user_penerima { get; set; }
        
        public string nama_user_penerima { get; set; }

        public ChatPageParams(int id_chat,int id_user_pengirim, int id_user_penerima, string nama_user_penerima)
        {
            this.id_chat = id_chat;
            this.id_user_pengirim = id_user_pengirim;
            this.id_user_penerima = id_user_penerima;
            this.nama_user_penerima = nama_user_penerima;
        }
    }
}
