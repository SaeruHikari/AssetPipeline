using System;
using System.Collections.Generic;

namespace AssetPipeline.Core
{
    public interface IDBData
    {
        byte[] ToBytes();
        void FromBytes(Span<byte> bytes);
    }

    public interface IDBKey : IDBData
    {

    }
    
    public interface IDBValue : IDBData
    {

    }

    public class PipelineDBAttribute : Attribute
    {
        public PipelineDBAttribute(string Name)
        {
            name = Name;
        }
        public void Commit<T>()
        {

        }
        public void Load<T>()
        {

        }
        string name;
    }
}
