using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Refraxion
{
    public class RxUriParser : GenericUriParser
    {
        static RxUriParser()
        {
            UriParser.Register(new RxUriParser(), "rx", 789789);
        }
        public RxUriParser()
            : base(GenericUriParserOptions.NoUserInfo | GenericUriParserOptions.NoQuery)
        {
        }
    }
}
