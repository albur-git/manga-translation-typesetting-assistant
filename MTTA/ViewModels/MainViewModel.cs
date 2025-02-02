using System;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MTTA.Commands;
using MTTA.Models;
using MTTA.Views;
using System.Windows.Media;
using System.Windows.Controls;

namespace MTTA.ViewModels
{
    public class MainViewModel : ObservableViewModelBase
    {
        public ObservableCollection<TranslationEntry> TranslationEntries { get; set; }
        public ObservableCollection<Rectangle> OcrAreas { get; set; }
        private BitmapImage _pageImage;
        public BitmapImage PageImage
        {
            get => _pageImage;
            set
            {
                _pageImage = value;
                OnPropertyChanged();
            }
        }

        private Rectangle _selectedOcrArea;
        public Rectangle SelectedOcrArea
        {
            get => _selectedOcrArea;
            set {
                // clear visual cue of old selection
                if (_selectedOcrArea != null)
                {
                    _selectedOcrArea.Stroke = Brushes.Blue;
                }
                if (value != null)
                {
                    _selectedOcrArea = value;
                    _selectedOcrArea.Stroke = Brushes.Red;
                }
            }
        }

        public ICommand ShowAddTranslationWindowCommand { get; set; }
        public ICommand LoadPageImageCommand { get; set; }
        public ICommand AddOcrAreaCommand { get; }
        public ICommand DeleteSelectedOcrAreaCommand { get; }

        public MainViewModel()
        {
            TranslationEntries = TranslationEntryManager.GetTranslationEntries();
            ShowAddTranslationWindowCommand = new RelayCommand(ShowAddTranslationWindow, CanShowAddTranslationWindow);
            LoadPageImageCommand = new RelayCommand(LoadPageImage, CanLoadPageImage);
            
            OcrAreas = new ObservableCollection<Rectangle>();
        }

        private bool CanLoadPageImage(object obj)
        {
            return true;
        }

        private void LoadPageImage(object obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*",
                Title = "Select an Image"
            };

            // Show the dialog and check if the user selects a file
            if (openFileDialog.ShowDialog() == true)
            {
                PageImageManager.processImage(openFileDialog.FileName);
            }

            PageImage = PageImageManager.getFinalImage(); 
        }

        private bool CanShowAddTranslationWindow(object obj)
        {
            return true; 
        }

        private void ShowAddTranslationWindow(object obj)
        {
            var mainWindow = obj as Window;

            AddTranslationEntry addTranslationEntryWindow = new();
            addTranslationEntryWindow.Owner = mainWindow;
            addTranslationEntryWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addTranslationEntryWindow.Show();
        }

        public void AddOcrArea(Rectangle ocrArea)
        {
            ocrArea.Stroke = Brushes.Blue;
            OcrAreas.Add(ocrArea);
        }

        public void DeleteOcrArea()
        {
            if (SelectedOcrArea != null)
            {
                OcrAreas.Remove(SelectedOcrArea);
                SelectedOcrArea = null;
            }
        }

        public bool ocrAreaExistsAtPositionAndIsSelected(Point position)
        {
            foreach (var ocrArea in OcrAreas)
            {
                // Check if the click is inside the bounds of the rectangle (ocrArea)
                if (position.X >= Canvas.GetLeft(ocrArea) && position.X <= Canvas.GetLeft(ocrArea) + ocrArea.Width &&
                    position.Y >= Canvas.GetTop(ocrArea) && position.Y <= Canvas.GetTop(ocrArea) + ocrArea.Height)
                {
                    // Set the clicked OCR area as the selected one
                    SelectedOcrArea = ocrArea;

                    return true; // Exit the loop after finding the first match
                }
            }
            return false;
        }
    }
}
