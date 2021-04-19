using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace AssetPipeline.Pipeline.InternalAssets
{
    [MetaSourceExt(".metameta")]
    [AssetMeta("45FE5416-39C1-44DA-8BF6-F101C333D501")]
    public class AssetMetaAsset : AssetMetaFile
    {
        [YamlMember] public Dictionary<string, string> AssetMeta = new Dictionary<string, string>();

        protected override void OnCreated(string File) 
        {
            var FileABSPath = System.IO.Path.Combine(PipelineInstance.Instance.Root, File);
            var FileABSDir = System.IO.Path.GetDirectoryName(FileABSPath);
            if(!System.IO.Directory.Exists(FileABSDir))
            {
                System.IO.Directory.CreateDirectory(FileABSDir);
            }
            var NewFile = System.IO.File.Create(FileABSPath);
            System.IO.TextWriter writer = new System.IO.StreamWriter(NewFile);
            writer.Write("TEST");
            writer.Flush();
            writer.Close();
            base.OnCreated(File);
        }
    }
}
