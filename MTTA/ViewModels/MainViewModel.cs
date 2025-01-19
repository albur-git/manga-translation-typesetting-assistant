using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MTTA.Commands;
using MTTA.Models;
using MTTA.Views;

namespace MTTA.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<TranslationEntry> TranslationEntries { get; set; }
        public ICommand ShowWindowCommand { get; set; }
        public ICommand ShowImageWindowCommand { get; set; }

        public MainViewModel()
        {
            TranslationEntries = TranslationEntryManager.GetTranslationEntries();
            ShowWindowCommand = new RelayCommand(ShowWindow, CanShowWindow);
            ShowImageWindowCommand = new RelayCommand(ShowImageWindow, CanShowImageWindow);
        }

        private bool CanShowImageWindow(object obj)
        {
            return true;
        }

        private void ShowImageWindow(object obj)
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

            var mainWindow = obj as Window;
            BitmapSource bitmapSource = PageImageManager.getFinalImage();
            if (bitmapSource != null)
            {
                ImageWindow imageWindow = new(bitmapSource);
                imageWindow.Owner = mainWindow;
                imageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                imageWindow.Show();
            }

        }

        private bool CanShowWindow(object obj)
        {
            return true;    // Return true --> Command will always be invoced.
        }

        private void ShowWindow(object obj)
        {
            var mainWindow = obj as Window;

            AddTranslationEntry addTranslationEntryWindow = new();
            addTranslationEntryWindow.Owner = mainWindow;
            addTranslationEntryWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addTranslationEntryWindow.Show();
        }
    }
}
