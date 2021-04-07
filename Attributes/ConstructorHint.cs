using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize.Attributes
{
    /// <summary>
    /// Enumeration class used to indicate how to deal with deserializing a object
    /// </summary>
    public enum ConstructorHint
    {
        /// <summary>
        /// Default behavior, Constructor is ignored
        /// </summary>
        None,

        /// <summary>
        /// Constructor called before assigning data
        /// </summary>
        BeforeAssignment,

        /// <summary>
        /// Constructor called after assigning data
        /// </summary>
        AfterAssignment
    }
}
