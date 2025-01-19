using System.Windows.Input;
using MTTA.Commands;
using MTTA.Models;

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
