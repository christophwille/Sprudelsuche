using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.WP.Models;

namespace Sprudelsuche.WP.Services
{
    public interface ILocationService
    {
        Task<LocationResult> GetCurrentPositionAsync();
    }
}
