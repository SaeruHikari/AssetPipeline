using System;
using System.Collections.Generic;
using System.Text;
using AssetPipeline.Core;

namespace AssetPipeline.Pipeline
{
    public struct GuidReferenceNode : IDBData
    {
        [YamlDotNet.Serialization.YamlIgnore] public int Length => refs is null ? 0 : refs.Length;
        
        public Guid[] refs;

        public void FromBytes(Span<byte> bytes)
        {
            var Refs = new List<Guid>();
            for (int Offset = 0; Offset < bytes.Length; Offset += 12)
            {
                var Ref = new Guid(bytes.Slice(Offset, 12));
                var arr = Ref.ToByteArray();
                Refs.Add(Ref);
            }
            refs = Refs.ToArray();
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            foreach(var G in refs)
            {
                bytes.AddRange(G.ToByteArray());
            }
            return bytes.ToArray();
        }
    }
}
