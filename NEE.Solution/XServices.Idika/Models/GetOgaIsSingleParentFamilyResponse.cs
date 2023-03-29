using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.Idika
{
    public class GetOgaIsSingleParentFamilyResponse : HBServiceResponseBase
	{

        public class ParenthoodInfo
        {
            public string ChildAMKA { get; set; }
            public string ParentAFM { get; set; }
            public string ParentSpouseAFM { get; set; }
            public int NumParents { get; set; }
        }

        // criteria (returned)

        public string ParentAFM { get; set; }

        // results

        public List<ParenthoodInfo> ParenthoodInfos { get; set; }

        public bool? IsSingleParentFamily { get; set; }

    }
}
