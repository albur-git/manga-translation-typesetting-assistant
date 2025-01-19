using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MTTA.Commands;
using MTTA.Models;
using MTTA.Views;

namespace MTTA.ViewModels
{
    public class MainViewModel : ObservableViewModelBase
    {
        public ObservableCollection<TranslationEntry> TranslationEntries { get; set; }
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

        public ICommand ShowAddTranslationWindowCommand { get; set; }
        public ICommand LoadPageImageCommand { get; set; }

        public MainViewModel()
        {
            TranslationEntries = TranslationEntryManager.GetTranslationEntries();
            ShowAddTranslationWindowCommand = new RelayCommand(ShowAddTranslationWindow, CanShowAddTranslationWindow);
            LoadPageImageCommand = new RelayCommand(LoadPageImage, CanLoadPageImage);
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
    }
}
