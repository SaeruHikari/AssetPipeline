using System;

namespace AssetPipeline
{
    public enum SerializedFieldTypes
    {
        GUID,
        FLOAT32, FLOAT64,
        UINT32, UINT64,
        INT32, INT64,
        STRING,
        NONE
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class PipelineMetaAttribute : Attribute
    {

    }
}
