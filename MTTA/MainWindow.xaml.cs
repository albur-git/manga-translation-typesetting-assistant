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
using System.Windows.Media.Media3D;

namespace MTTA
{
    public class OcrResult
    {
        public System.Windows.Rect BoundingBox { get; set; }
        public string Text { get; set; }
    }

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

                List<OcrResult> ocrResults = PerformOCR(selectedFilePath);

                // Create a new window and display the image
                BitmapImage bitmapImage = new BitmapImage(new Uri(selectedFilePath));
                BitmapSource finalImage = DrawRectanglesOnImage(bitmapImage, ocrResults, Brushes.Red, 2);
                ImageWindow window = new ImageWindow(finalImage);
                window.Show();

                // show text results
                foreach (OcrResult ocrResult in ocrResults)
                {
                    MessageBox.Show($"OCR Result:\n\nText: {ocrResult.Text}",
                    "OCR Result",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                }
            }
        }

        private List<OcrResult> PerformOCR(string imagePath)
        {
            var results = new List<OcrResult>();

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

                using var engine = new TesseractEngine(@"./tessdata", "jpn", EngineMode.Default);
                engine.DefaultPageSegMode = PageSegMode.SparseTextOsd; // Use SparseText for scattered entries
                using var img = Pix.LoadFromFile(imagePath);
                using var result = engine.Process(img);

                var itr = result.GetIterator();
                itr.Begin();
                do
                {
                    string ocrText = itr.GetText(PageIteratorLevel.Block);
                    if (itr.TryGetBoundingBox(PageIteratorLevel.Block, out var boundingBox))
                    {
                        results.Add(new OcrResult
                        {
                            Text = ocrText,
                            BoundingBox = new System.Windows.Rect(boundingBox.X1, boundingBox.Y1, boundingBox.Width, boundingBox.Height)
                        });
                    }
                    else
                    {
                        MessageBox.Show("No bounding box found for this block.", "OCR Result", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                } while (itr.Next(PageIteratorLevel.Block));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during OCR process: {ex.Message}", "OCR Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return results;
        }

        private BitmapSource DrawRectanglesOnImage(BitmapImage originalImage, List<OcrResult> ocrResults, Brush rectangleBrush, double rectangleThickness)
        {
            // Convert BitmapImage to a DrawingVisual for rendering
            var visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                // Draw the original image
                drawingContext.DrawImage(originalImage, new System.Windows.Rect(0, 0, originalImage.PixelWidth, originalImage.PixelHeight));

                // Draw each rectangle
                foreach (var ocrResult in ocrResults)
                {
                    var rectanglePen = new Pen(rectangleBrush, rectangleThickness);
                    drawingContext.DrawRectangle(null, rectanglePen, ocrResult.BoundingBox);
                }
            }

            // Render the visual into a new BitmapSource
            var renderTarget = new RenderTargetBitmap(
                originalImage.PixelWidth,
                originalImage.PixelHeight,
                originalImage.DpiX,
                originalImage.DpiY,
                PixelFormats.Pbgra32
            );
            renderTarget.Render(visual);

            return renderTarget;
        }
    }
}