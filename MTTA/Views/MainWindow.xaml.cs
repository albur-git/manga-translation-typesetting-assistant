using System.Windows;
using MTTA.ViewModels;

namespace MTTA.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // For Information on which the MVVM implementation is based on, see: https://www.youtube.com/watch?v=4v8PobcZpqM
            MainViewModel mainViewModel = new();
            // Make data and commands from MainViewModel instance accessible in the MainWindow.xaml file, e.g through ItemsSource="{Binding TranslationEntries}" in a ListView or Command="{Binding ShowWindowCommand}" in a Button
            this.DataContext = mainViewModel;  
        }
    }
}