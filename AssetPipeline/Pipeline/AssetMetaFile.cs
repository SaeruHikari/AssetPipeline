using System;
using System.Collections.Generic;
using System.Text;
using AssetPipeline.Core;

namespace AssetPipeline.Pipeline
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MetaGuidAttribute : Attribute
    {
        public MetaGuidAttribute(string value)
        {
            guid = new AssetGuid(value);
        }
        public MetaGuidAttribute(AssetGuid value)
        {
            guid = value;
        }
        public readonly AssetGuid guid;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class MetaSourceExtAttribute : Attribute
    {
        public MetaSourceExtAttribute(string ext)
        {
            Ext = ext;
        }
        public readonly string Ext;
    }

    [PipelineFile()]
    [MetaSourceExt("")]
    [MetaGuid("9bf72b35-a2e9-6b04-9a1d-2b51c0c3fdcc")]
    public class AssetMetaFile
    {
        [PipelineMeta] protected AssetGuid guid;
        [PipelineMeta] protected GuidReferenceNode references;
        [PipelineMeta] protected int versionNumber;
        [PipelineMeta] protected DateTime lastWriteTime;
        [PipelineMeta] protected List<string> sources;

        public AssetGuid Guid => guid;
        public GuidReferenceNode References => references;
        public int VersionNumber => versionNumber;
        public DateTime LastWriteTime => lastWriteTime;
        public string[] Sources => sources.ToArray();

        protected void Setup(PipelineURL File)
        {
            PipelineURL MetaDst = AssetPipeline.PipelineInstance.Instance.Root;
            MetaDst.url = File.url;
            var filename = System.IO.Path.GetFileName(File.url);
            System.IO.FileInfo info = PipelineURL.FileInformation(File);
            guid = AssetGuid.Create();
        }

        public static T Find<T>(PipelineURL File) where T : AssetMetaFile
        {
            return null;
        }

        public static T FindOrCreate<T>(PipelineURL File) where T : AssetMetaFile, new()
        {
            var Finded = Find<T>(File);
            if (Finded != null)
            {
                return Finded;
            }
            else
            {
                var Result = new T();
                Result.Setup(File);
                return Result;
            }
        }
    }
}
