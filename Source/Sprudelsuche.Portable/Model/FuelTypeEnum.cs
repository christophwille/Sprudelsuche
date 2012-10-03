using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprudelsuche.Portable.Model
{
    public enum FuelTypeEnum
    {
        Diesel,
        Super
    }

    public static class FuelTypeEnumExtension
    {
        public static List<string> ToList()
        {
            var names = Enum.GetNames(typeof(FuelTypeEnum));
            return names.ToList();
        }

        public static string ToSpritpreisrechnerString(this FuelTypeEnum c)
        {
            switch (c)
            {
                case FuelTypeEnum.Diesel:
                    return "DIE";
                case FuelTypeEnum.Super:
                    return "SUP";

            }

            throw new ArgumentException("Diese Art von Treibstoff wird nicht unterstützt");
        }

        public static FuelTypeEnum FromSpritpreisrechnerString(this string c)
        {
            switch (c)
            {
                case "DIE":
                    return FuelTypeEnum.Diesel;
                case "SUP":
                    return FuelTypeEnum.Super;

            }

            throw new ArgumentException("Diese Art von Treibstoff wird nicht unterstützt");
        }
    }

}
