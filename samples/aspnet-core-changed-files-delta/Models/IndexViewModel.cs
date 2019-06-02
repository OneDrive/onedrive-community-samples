using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCore_Changed_Files.Models
{
    public class IndexViewModel
    {
        public IReadOnlyList<DriveItem> Items { get; set; }
        public string DeltaToken { get; internal set; }
    }
}
