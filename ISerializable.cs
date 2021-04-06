﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize
{
    public interface ISerializable
    {
        void Serialize(BufferWriter writer);

        void Deserialize(ByteBuffer data);
    }
}
