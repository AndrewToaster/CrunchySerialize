using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize.Attributes
{
    /// <summary>
    /// Attribute used to indicate that a member should not be used in serialization / deserialization
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IgnoreMemberAttribute : Attribute
    {
    }
}
