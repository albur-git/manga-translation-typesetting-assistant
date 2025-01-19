using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTTA.Models
{
    public class TranslationEntry
    {

        public TranslationEntry()
        {
            
        }

        public string? Original { get; set; }
        public string? Initial { get; set; }
        public float Confidence { get; set; }
    }
}
