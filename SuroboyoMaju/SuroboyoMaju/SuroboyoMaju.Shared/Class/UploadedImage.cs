using System;
using System.Collections.Generic;
using System.Text;

namespace SuroboyoMaju.Shared.Class
{
    class UploadedImage
    {

        public string file_name { get; set; }
        public byte[] image { get; set; }

        public UploadedImage(string file_name,byte[] image)
        {
            this.file_name = file_name;
            this.image = image;
        }
    }
}
