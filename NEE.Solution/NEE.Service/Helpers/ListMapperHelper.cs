using NEE.Core.BO;
using NEE.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Service.Helpers
{
    public static class ListMapperHelper
    {
        public static List<ZipCode> MapList(this List<DIC_ZipCodes> srcList)
        {
            List<ZipCode> destList = new List<ZipCode>();

            foreach (DIC_ZipCodes src in srcList)
            {
                destList.Add(src.Map());
            }

            return destList;
        }
    }
}
