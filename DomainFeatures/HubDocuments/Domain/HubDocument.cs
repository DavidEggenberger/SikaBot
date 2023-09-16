using DTOs.HubDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainFeatures.HubDocuments.Domain
{
    public class HubDocument
    {
        public Guid Id { get; set; }
        public string Location { get; set; }
        public string Summarization { get; set; }
        public List<string> KeyPhrases { get; set; }
        public List<string> Entities { get; set; }
        public List<string> Keywords { get; set; }
        public string Text { get; set; }

        public HubDocumentDTO ToDTO()
        {
            return new HubDocumentDTO
            {
                Entities = Entities,
                Id = Id,
                Location = Location,
                Summarization = Summarization,
                KeyPhrases = KeyPhrases,
                Text = Text,
                Keywords = Keywords
            };
        }
    }
}
