using System;
using System.Collections.Generic;
using System.Text;
using AssetPipeline.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
        [YamlMember, PipelineMeta] public Guid Guid { get; set; }
        [YamlMember, PipelineMeta] public GuidReferenceNode References { get; set; }
        [YamlMember, PipelineMeta] public int VersionNumber { get; set; }
        [YamlMember, PipelineMeta] public DateTime LastWriteTime { get; set; }
        [YamlMember, PipelineMeta] public string Source { get; set; }
        [YamlIgnore, PipelineMeta] public string ThisFile => Source + ".meta";

        virtual protected void OnCreated(string File)
        {
            // Initialize Data.
            var info = new System.IO.FileInfo(System.IO.Path.Combine(PipelineInstance.Instance.Root, File));
            Guid = Guid.NewGuid();
            References = new GuidReferenceNode();
            VersionNumber = PipelineInstance.VersionControl is null ? 0 : PipelineInstance.VersionControl.FileVersion(File);
            LastWriteTime = info.LastWriteTime;
            Source = System.IO.Path.GetFileName(File);

            // Serialize & Write At Once.
            var yaml = serializer.Serialize(this);
            var stream = System.IO.File.Create(System.IO.Path.Combine(PipelineInstance.Instance.Root, File + ".meta"));
            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);
            writer.Write(yaml);
            writer.Flush();
            writer.Close();
        }

        virtual protected void OnLoaded(string MetaFile)
        {

        }

        public static AssetMetaFile LoadMetaFromDisk(string AssetFile)
        {
            // Create a metafile memory-object of proper type.
            var ext = System.IO.Path.GetExtension(AssetFile);
            if (!PipelineInstance.Instance.ExtNameMetaTypeDictionary.TryGetValue(ext, out Type T))
            {
                T = typeof(AssetMetaFile);
            }
            AssetMetaFile Loaded = null;
            {
                // Deserialize from disk.
                var stream = System.IO.File.OpenRead(
                    System.IO.Path.Combine(PipelineInstance.Instance.Root, AssetFile + ".meta"));
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                var yaml = reader.ReadToEnd();
                reader.Close();
                Loaded = deserializer.Deserialize(yaml, T) as AssetMetaFile;
            }
            if (Loaded is null) 
                return Loaded; 
            Loaded.OnLoaded(AssetFile);
            return Loaded;
        }

        public static AssetMetaFile FindOnDisk(string AssetFile)
        {
            var existed = System.IO.File.Exists(System.IO.Path.Combine(PipelineInstance.Instance.Root, AssetFile + ".meta"));
            return existed ? 
                LoadMetaFromDisk(AssetFile) :
                null;
        }

        public static T FindOnDisk<T>(AssetGuid Guid) where T : AssetMetaFile
        {
            return null;
        }

        public static T CreateOrLoadOnDisk<T>(string File) where T : AssetMetaFile, new()
        {
            var Existed = FindOnDisk(File);
            if (Existed != null && Existed is T) 
                return Existed as T;

            var Result = new T();
            Result.OnCreated(File);
            return Result;
        }

        static AssetMetaFile()
        {
            serializer = new SerializerBuilder()
                .WithNamingConvention(NullNamingConvention.Instance)
                .Build();
            deserializer = new DeserializerBuilder()
                .WithNamingConvention(NullNamingConvention.Instance)
                .Build();
        }

        protected static YamlDotNet.Serialization.ISerializer serializer;
        protected static YamlDotNet.Serialization.IDeserializer deserializer;
    }
}
