using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprudelsuche.Portable.Model
{
    public class GasQueryDownloadResult
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
        public GasQueryResult Result { get; set; } 
    }
}
