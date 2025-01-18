using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTTA.Models
{
    public class TranslationEntryManager
    {
        // This class is meant to manage e.g. database accesses to get data fromn the disk

        //public ObservableCollection<TranslationEntry> _TranslationEntries { get; set; };
        // methods are startic so they can be called without instantiating a class of the TranslationEntryManager and passing it through the various instances that are using it.
        public static ObservableCollection<TranslationEntry> _TranslationEntries = new ObservableCollection<TranslationEntry>() { new TranslationEntry { Original = "おはよう", Initial = "Good Morning" }, new TranslationEntry { Original = "おやすみ", Initial = "Good Night" } };

        public static ObservableCollection<TranslationEntry> GetTranslationEntries() { return _TranslationEntries; }

        public static void AddTranslationEntry(TranslationEntry translationEntry)
        {
            _TranslationEntries.Add(translationEntry);
        }
    }
}
