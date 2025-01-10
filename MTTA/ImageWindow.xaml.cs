using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MTTA
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        private string _filePath;
        private BitmapImage _originalImage;

        public ImageWindow(string filePath)
        {
            InitializeComponent();

            _filePath = filePath;

            ResizeImage();
        }

        // This method is called when the window has been fully loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the image (you can replace the path with a file picker or any logic)
            _originalImage = new BitmapImage(new Uri(_filePath));
            ImageViewer.Source = _originalImage;

            // Resize the image once the window is loaded
            ResizeImage();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeImage();
        }

        private void ResizeImage()
        {
            if (_originalImage != null)
            {
                // Maintain the image's aspect ratio (width and height)
                double aspectRatio = _originalImage.PixelWidth / (double)_originalImage.PixelHeight;

                // Set the width to the window's width (minus a small margin), and calculate the height based on the aspect ratio
                double newWidth = this.ActualWidth - 20; // Subtracting 20 for margin (adjust as needed)
                double newHeight = newWidth / aspectRatio;

                // Apply the calculated width and height
                ImageViewer.Width = newWidth;
                ImageViewer.Height = newHeight;
            }
        }
    }
}