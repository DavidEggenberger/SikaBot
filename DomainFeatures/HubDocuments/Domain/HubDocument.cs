using DTOs.HubDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DomainFeatures.HubDocuments.Domain
{
    public class Image
    {
        public List<string> Tags { get; set; }
        public string uri { get; set; }
    }
    public class HubDocument
    {
        public Guid Id { get; set; }
        public int Retrievals { get; set; }
        public string Location { get; set; }
        public List<(string, string)> Summarization { get; set; }
        public List<string> KeyPhrases { get; set; }
        public List<string> Entities { get; set; }
        public List<string> Keywords { get; set; }
        public string Text { get; set; }
        public List<Image> Images { get; set; }

        public HubDocumentDTO ToDTO()
        {
            return new HubDocumentDTO
            {
                Entities = Entities,
                Id = Id,
                Location = Location,
                Summarization = Summarization.Select(x => new TranslatedValue { Language = x.Item1, Text = x.Item2}).ToList(),
                KeyPhrases = KeyPhrases,
                Text = Text,
                Keywords = Keywords
            };
        }
    }
}
