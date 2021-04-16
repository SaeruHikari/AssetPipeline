using System;
using System.Collections.Generic;
using System.Text;

namespace AssetPipeline.Pipeline
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    class AssetAttribute : Attribute
    {
        bool bImportCaching = false;
    }
}
