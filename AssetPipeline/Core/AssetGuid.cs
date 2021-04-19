using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace AssetPipeline.Core
{
    public struct AssetGuid : IDBKey, IDBValue
    {
        public AssetGuid(Guid value)
        {
            guid = value;
        }
        public AssetGuid(string str)
        {
            guid = Guid.Parse(str);
        }
        static AssetGuid()
        {
            SizeInBytes = Guid.NewGuid().ToByteArray().Length;
        }
        public static AssetGuid Create()
        {
            return new AssetGuid(Guid.NewGuid());
        }
        public bool Equals(Guid o)
        {
            return guid.Equals(o);
        }       
        public bool Equals(AssetGuid o)
        {
            return guid.Equals(o.guid);
        }

        public override string ToString()
        {
            return guid.ToString();
        }

        public byte[] ToBytes()
        {
            return guid.ToByteArray();
        }
        public void FromBytes(Span<byte> bytes)
        {
            guid = new Guid(bytes);
        }
        [YamlMember, PipelineMeta] public Guid guid;
        public static readonly int SizeInBytes; 
    }
}
