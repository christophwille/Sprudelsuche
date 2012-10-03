using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprudelsuche.Portable.Model
{
    public class GasQueryRefreshResult
    {
        public bool Succeeded { get; set; }
        public List<GasQueryResult> Results { get; set; } 
    }
}
