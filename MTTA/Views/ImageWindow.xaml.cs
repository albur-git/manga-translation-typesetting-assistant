using System.Windows;
using System.Windows.Media.Imaging;

namespace MTTA.Views
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        private BitmapSource _originalImage;

        public ImageWindow(BitmapSource imageSource)
        {
            InitializeComponent();
            _originalImage = imageSource;
            ImageViewer.Source = _originalImage;
        }

        // This method is called when the window has been fully loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
