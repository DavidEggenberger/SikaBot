using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainFeatures.HubDocuments.Domain
{
    public class HubDocument
    {
        public string Location { get; set; }
        public List<string> Keywords { get; set; }
    }
}
