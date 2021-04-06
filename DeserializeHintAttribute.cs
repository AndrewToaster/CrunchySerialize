using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class SerializationHintAttribute : Attribute
    {
        public DeserializeConstructorHint Hint { get; set; }
    }
}
