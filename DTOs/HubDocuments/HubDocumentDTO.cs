using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.HubDocuments
{
    public class HubDocumentDTO
    {
        public Guid Id { get; set; }
        public string Location { get; set; }
        public List<TranslatedValue> Summarization { get; set; }
        public List<string> KeyPhrases { get; set; }
        public List<string> Entities { get; set; }
        public List<string> Keywords { get; set; }
        public string Text { get; set; }
    }
    public class TranslatedValue
    {
        public string Language { get; set; }
        public string Text { get; set; }
    }
}
