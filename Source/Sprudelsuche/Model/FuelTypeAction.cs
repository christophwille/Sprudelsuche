using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable;
using Sprudelsuche.Portable.Model;

namespace Sprudelsuche.Model
{
    public class FuelTypeAction : BindableBase
    {
        private FuelTypeEnum _fuelType = FuelTypeEnum.Diesel;
        public FuelTypeEnum FuelType
        {
            get { return _fuelType; }
            set { SetProperty(ref _fuelType, value); }
        }

        public string FuelTypeName
        {
            get { return _fuelType.ToString(); }
        }
    }
}
