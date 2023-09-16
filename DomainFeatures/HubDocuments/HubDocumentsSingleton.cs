using DomainFeatures.HubDocuments.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainFeatures.HubDocuments
{
    public class HubDocumentsSingleton
    {
        public HubDocumentsSingleton()
        {
            HubDocuments = new List<HubDocument>();
        }
        public List<HubDocument> HubDocuments { get; set; }
    }
}
