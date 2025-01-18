using System.Windows;
using MTTA.ViewModels;

namespace MTTA.Views
{
    /// <summary>
    /// Interaction logic for AddTranslationEntry.xaml
    /// </summary>
    public partial class AddTranslationEntry : Window
    {
        public AddTranslationEntry()
        {
            InitializeComponent();
            AddTranslationEntryViewModel addTranslationEntryViewModel = new AddTranslationEntryViewModel();
            this.DataContext = addTranslationEntryViewModel;
        }
    }
}
