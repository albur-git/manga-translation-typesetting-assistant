using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Tesseract;

namespace MTTA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonLoadImage_Click(object sender, RoutedEventArgs e)
        {
            // Create and configure the OpenFileDialog
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*",
                Title = "Select an Image"
            };

            // Show the dialog and check if the user selects a file
            if (openFileDialog.ShowDialog() == true)
            {
                // Get the selected file path
                string selectedFilePath = openFileDialog.FileName;

                PerformOCR(selectedFilePath);

                // Create a new window and display the image
                var newWindow = new ImageWindow(selectedFilePath);
                newWindow.Show();
            }
        }

        private void PerformOCR(string imagePath)
        {
            try
            {
                // Initialize the Tesseract OCR engine
                // Note: Initializing the tesseract engine needs training data from the official project github:
                // --> https://github.com/tesseract-ocr/tessdoc/blob/main/Data-Files.md
                //      e.g. tessdata_fast
                // a tessdata folder needs to be created in the project folder, the contents of above above file) 
                // copied into and then the folder needs to be referenced in the constructor (see: @"./tessdata")
                // keep the following files and set their property to "copy always" (don't know about the scripts and config yet)
                // (Config just seems to point to the config folder, which is empty...)
                // - tessdata/eng.traineddata
                // - tessdata/equ.traineddata
                // - tessdata/jpn.traineddata
                // - tessdata/jpn_vert.traineddata
                // - tessdata/osd.traineddata
                // See https://github.com/charlesw/tesseract/ --> "Getting started quickly"
                // See https://stackoverflow.com/questions/38567100/failed-to-initialise-tesseract-engine-cant-find-correct-version for copy always setting

                // Todo: Add the Tesseract.Drawing NuGet package to support interop with System.Drawing in .NET Core, for instance to allow passing Bitmap to Tesseract
                using (var engine = new TesseractEngine(@"./tessdata", "jpn", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        // Perform OCR and get the text result
                        var result = engine.Process(img);
                        string ocrText = result.GetText();

                        // Show the OCR result in a MessageBox
                        MessageBox.Show($"OCR Result:\n\n{ocrText}", "OCR Result", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during OCR process: {ex.Message}", "OCR Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}