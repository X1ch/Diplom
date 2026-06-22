using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uchet_Oborudovania.AppData
{
    public static class EquipmentStatus
    {
        public const string InUse = "В эксплуатации";
        public const string InStock = "На складе";
        public const string InRepair = "В ремонте";
        public const string WrittenOff = "Списано";

        public static string[] GetAll()
        {
            return new[] { InUse, InStock, InRepair, WrittenOff };
        }

    }
}
