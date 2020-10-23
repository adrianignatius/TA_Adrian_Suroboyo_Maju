﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SuroboyoMaju.Shared.Class
{
    class User
    {
        public int id_user { get; set; }

        public string nama_user { get; set; }

        public string telpon_user { get; set; }

        public int status_user { get; set; }

        public string premium_available_until { get; set; }

        public string lokasi_aktif_user { get; set; }

        public int? id_kecamatan_user { get; set; }

        public string kecamatan_user { get; set; }

        public int status_aktif_user { get; set; }

        public User(int id_user, string nama_user, string telpon_user, int status_user, string premium_available_until, string lokasi_aktif_user, int? id_kecamatan_user, string kecamatan_user, int status_aktif_user)
        {
            this.id_user = id_user;
            this.nama_user = nama_user;
            this.telpon_user = telpon_user;
            this.status_user = status_user;
            this.premium_available_until = premium_available_until;
            this.lokasi_aktif_user = lokasi_aktif_user;
            this.id_kecamatan_user = id_kecamatan_user;
            this.kecamatan_user = kecamatan_user;
            this.status_aktif_user = status_aktif_user;
        }
    }
}
