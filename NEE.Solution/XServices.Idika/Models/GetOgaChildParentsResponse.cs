using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opeka.Contract;
using XS.Contract;

namespace XServices.Idika.Contract
{
    public class GetOgaChildParentsResponse : HBServiceResponseBase
	{

        public class ParenthoodInfo
        {
            public string ChildAMKA { get; set; }
            public string ParentAFM { get; set; }
            public string ParentSpouseAFM { get; set; }
            public int NumParents { get; set; }
        }
        

        // criteria (returned)

        public string ChildAMKA { get; set; }

        // results

        public List<ParenthoodInfo> ParenthoodInfos { get; set; }

        public List<string> ParentAFMs { get; set; }

    }
}
