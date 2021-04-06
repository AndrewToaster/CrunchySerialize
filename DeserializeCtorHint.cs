using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize
{
    public enum DeserializeConstructorHint
    {
        Ignore = 0,
        DefaultPreInit = 1,
        DefaultPostInit = 2
    }
}
