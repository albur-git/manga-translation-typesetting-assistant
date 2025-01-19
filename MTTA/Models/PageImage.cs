using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTTA.Views;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using Tesseract;

namespace MTTA.Models
{
    class PageImage
    {
        public List<System.Windows.Rect> BoundingBoxes { get; set; }

        public PageImage()
        {
            
        }
    }
}
