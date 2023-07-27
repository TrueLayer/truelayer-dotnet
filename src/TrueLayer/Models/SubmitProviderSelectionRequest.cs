using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Models
{
    /// <summary>
    /// Submit the provider details selected by the PSU
    /// </summary>
    /// <param name="ProviderId"></param>
    internal record SubmitProviderSelectionRequest(string ProviderId);
}
