using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize.Attributes
{
    /// <summary>
    /// Attribute used indicate how correctly serialize/deserialize types
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class SerializationHintAttribute : Attribute
    {
        /// <summary>
        /// Hint used for calling constructors
        /// </summary>
        public ConstructorHint Hint { get; set; }
    }
}
