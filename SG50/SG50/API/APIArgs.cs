using System;
using System.Collections.Generic;

namespace SG50
{
    public class APIArgs
    {
        public static APIArgs Empty = new APIArgs();

        public Dictionary<String, Object> Headers = new Dictionary<String, Object>();
        public Dictionary<String, Object> Parameters = new Dictionary<String, Object>();
    }
}