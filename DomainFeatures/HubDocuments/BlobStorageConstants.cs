using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainFeatures.HubDocuments
{
    public class BlobStorageConstants
    {
        public readonly string BlobContainer;
        public BlobStorageConstants()
        {
            BlobContainer = Guid.NewGuid().ToString();
        }
    }
}
