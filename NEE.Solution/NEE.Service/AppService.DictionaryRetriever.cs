using NEE.Core.BO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NEE.Service
{
    partial class AppService
    {
        public async Task<List<ZipCode>> ZipCodesRetrieverAsync()
        {
            return await repository.LoadZipCodes();
        }
    }
}
