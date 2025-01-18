using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MTTA.Commands;
using MTTA.Models;
using MTTA.Views;

namespace MTTA.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<TranslationEntry> TranslationEntries { get; set; }
        public ICommand ShowWindowCommand { get; set; }

        public MainViewModel()
        {
            TranslationEntries = TranslationEntryManager.GetTranslationEntries();
            ShowWindowCommand = new RelayCommand(ShowWindow, CanShowWindow);
        }

        private bool CanShowWindow(object obj)
        {
            return true;    // Return true --> Command will always be invoced.
        }

        private void ShowWindow(object obj)
        {
            var mainWindow = obj as Window;

            AddTranslationEntry addTranslationEntryWindow = new AddTranslationEntry();
            addTranslationEntryWindow.Owner = mainWindow;
            addTranslationEntryWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addTranslationEntryWindow.Show();
        }
    }
}
