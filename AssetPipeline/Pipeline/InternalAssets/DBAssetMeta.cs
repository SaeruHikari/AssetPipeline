using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace AssetPipeline.Pipeline.InternalAssets
{
    [MetaSourceExt(".mdb")]
    [AssetMeta("0C2D58E0-E823-4AE4-AB5B-5F7083AC6DAF")]
    public class DBAssetMeta : AssetMetaFile
    {
        [YamlMember] public string encoding = "UTF8";


    }
}
