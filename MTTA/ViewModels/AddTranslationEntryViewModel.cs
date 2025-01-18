using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MTTA.Commands;
using MTTA.Models;
using MTTA.Views;

namespace MTTA.ViewModels
{
    public class AddTranslationEntryViewModel
    {
        public string? Original { get; set; }
        public string? Initial { get; set; }
        public ICommand AddTranslationEntryCommand { get; set; }

        public AddTranslationEntryViewModel()
        {
            AddTranslationEntryCommand = new RelayCommand(AddTranslationEntryMethod, CanAddTranslationEntryMethod);
        }

        private bool CanAddTranslationEntryMethod(object obj)
        {
            return true;
        }

        private void AddTranslationEntryMethod(object obj)
        {
            TranslationEntryManager.AddTranslationEntry(new TranslationEntry() { Original = this.Original, Initial = this.Initial });
        }
    }
}
