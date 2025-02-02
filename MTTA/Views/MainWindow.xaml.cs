using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using MTTA.ViewModels;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace MTTA.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel _mainViewModel;
        private Point _startPoint;
        private Rectangle _currentRectangle;
        public MainWindow()
        {
            InitializeComponent();
            // For Information on which the MVVM implementation is based on, see: https://www.youtube.com/watch?v=4v8PobcZpqM
            _mainViewModel = new();
            // Make data and commands from MainViewModel instance accessible in the MainWindow.xaml file, e.g through ItemsSource="{Binding TranslationEntries}" in a ListView or Command="{Binding ShowWindowCommand}" in a Button
            this.DataContext = _mainViewModel;
            // Add key press event handler to delete rectangle on "Delete" key press
            this.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && _mainViewModel.SelectedOcrArea != null)
            {
                _mainViewModel.DeleteOcrArea();
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(PageCanvas);
            _startPoint = position;

            
            if (_mainViewModel.ocrAreaExistsAtPositionAndIsSelected(position))
            {
                return;
            }

            _currentRectangle = new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft(_currentRectangle, _startPoint.X);
            Canvas.SetTop(_currentRectangle, _startPoint.Y);
            PageCanvas.Children.Add(_currentRectangle);

        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_currentRectangle != null)
            {
                var position = e.GetPosition(PageCanvas);
                var width = Math.Abs(position.X - _startPoint.X);
                var height = Math.Abs(position.Y - _startPoint.Y);
                _currentRectangle.Width = width;
                _currentRectangle.Height = height;
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentRectangle != null)
            {
                _mainViewModel.AddOcrArea(_currentRectangle);
                PageCanvas.Children.Remove(_currentRectangle);
                _currentRectangle = null; 
            }
        }
    }
}