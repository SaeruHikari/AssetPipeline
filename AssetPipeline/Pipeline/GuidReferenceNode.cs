using System;
using System.Collections.Generic;
using System.Text;
using AssetPipeline.Core;

namespace AssetPipeline.Pipeline
{
    public struct GuidReferenceNode : IDBData
    {
        public AssetGuid[] refs;

        public void FromBytes(Span<byte> bytes)
        {
            var Refs = new List<AssetGuid>();
            for (int Offset = 0; Offset < bytes.Length; Offset += AssetGuid.SizeInBytes)
            {
                var Ref = new AssetGuid();
                Ref.FromBytes(bytes.Slice(Offset, AssetGuid.SizeInBytes));
                Refs.Add(Ref);
            }
            refs = Refs.ToArray();
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            foreach(AssetGuid G in refs)
            {
                bytes.AddRange(G.ToBytes());
            }
            return bytes.ToArray();
        }
    }
}
