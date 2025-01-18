using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Tesseract;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MTTA.ViewModels;

namespace MTTA.Views
{
    public class OcrResult
    {
        public System.Windows.Rect BoundingBox { get; set; }
        public string Text { get; set; }
        public float Confidence { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double MINIMUM_CONFIDENCE_THRESHOLD = 50.0;

        public MainWindow()
        {
            InitializeComponent();
            // For Information on which the MVVM implementation is based on, see: https://www.youtube.com/watch?v=4v8PobcZpqM
            MainViewModel mainViewModel = new();
            // Make data and commands from MainViewModel instance accessible in the MainWindow.xaml file, e.g through ItemsSource="{Binding TranslationEntries}" in a ListView or Command="{Binding ShowWindowCommand}" in a Button
            this.DataContext = mainViewModel;  
        }




        //--------------------------- Old Stuff --> Do not use events in MVVM
        // Todo: Refactor

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
                var finalImage = DrawRectanglesOnImage(
                    bitmapImage,
                    ocrResults,
                    Brushes.Red,
                    2.0,
                    new Typeface("Arial"),
                    24,
                    Brushes.Blue
                );
                ImageWindow window = new ImageWindow(finalImage);
                window.Show();

                // show text results
                foreach (OcrResult ocrResult in ocrResults)
                {
                    //MessageBox.Show($"OCR Result:\n\nText: {ocrResult.Text}",
                    //"OCR Result",
                    //MessageBoxButton.OK,
                    //MessageBoxImage.Information);
                    Debug.WriteLine(ocrResult.Text);
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

                using var engine = new TesseractEngine(@"./tessdata", "jpn_vert", EngineMode.Default);
                engine.DefaultPageSegMode = PageSegMode.SparseTextOsd; // Use SparseText for scattered entries
                using var img = Pix.LoadFromFile(imagePath);
                using var result = engine.Process(img);

                var itr = result.GetIterator();
                itr.Begin();
                do
                {
                    string ocrText = itr.GetText(PageIteratorLevel.Block);
                    float confidence = itr.GetConfidence(PageIteratorLevel.Block);
                    if (itr.TryGetBoundingBox(PageIteratorLevel.Block, out var boundingBox))
                    {
                        if (confidence > MINIMUM_CONFIDENCE_THRESHOLD)
                        {
                            results.Add(new OcrResult
                            {
                                Text = ocrText,
                                Confidence = confidence,
                                BoundingBox = new System.Windows.Rect(boundingBox.X1, boundingBox.Y1, boundingBox.Width, boundingBox.Height)
                            });
                        }
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

        private BitmapSource DrawRectanglesOnImage(
            BitmapImage originalImage,
            List<OcrResult> ocrResults,
            Brush rectangleBrush,
            double rectangleThickness,
            Typeface textTypeface,
            double fontSize,
            Brush textBrush)
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

                    // Create the text to display
                    string confidenceValue = ocrResult.Confidence.ToString();
                    if (!string.IsNullOrEmpty(confidenceValue))
                    {
                        var formattedText = new FormattedText(
                            confidenceValue,
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Windows.FlowDirection.LeftToRight,
                            textTypeface,
                            fontSize,
                            textBrush,
                            VisualTreeHelper.GetDpi(visual).PixelsPerDip
                        );

                        // Position the text inside or next to the rectangle
                        var textPosition = new System.Windows.Point(
                            ocrResult.BoundingBox.Left + (ocrResult.BoundingBox.Width - formattedText.Width) / 2.0,
                            ocrResult.BoundingBox.Top + (ocrResult.BoundingBox.Height - formattedText.Height) / 2.0
                        );

                        drawingContext.DrawText(formattedText, textPosition);
                    }
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