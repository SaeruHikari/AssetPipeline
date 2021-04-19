using System;
using System.Collections.Generic;
using System.Text;
using AssetPipeline.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AssetPipeline.Pipeline
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AssetMetaAttribute : Attribute
    {
        public AssetMetaAttribute(string value)
        {
            guid = new AssetGuid(value);
        }
        public AssetMetaAttribute(AssetGuid value)
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

    [MetaSourceExt("")]
    [AssetMeta("9bf72b35-a2e9-6b04-9a1d-2b51c0c3fdcc")]
    public class AssetMetaFile
    {
        [YamlMember] public Guid Guid { get; set; }
        [YamlMember] public GuidReferenceNode References { get; set; }
        [YamlMember] public int VersionNumber { get; set; }
        [YamlMember] public DateTime LastWriteTime { get; set; }
        [YamlMember] public string Source { get; set; }
        [YamlIgnore] public string ThisFile => Source + PipelineInstance.MetaAfterFix;

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
            var stream = System.IO.File.Create(System.IO.Path.Combine(
                PipelineInstance.Instance.Root, File + PipelineInstance.MetaAfterFix));
            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);
            writer.Write(yaml);
            writer.Flush();
            writer.Close();
        }

        virtual protected void OnLoaded(string MetaFile)
        {

        }

        public static AssetMetaFile LoadMetaFromDisk(string AssetFileName)
        {
            // Create a metafile memory-object of proper type.
            var ext = System.IO.Path.GetExtension(AssetFileName);
            if (!PipelineInstance.Instance.ExtNameMetaTypeDictionary.TryGetValue(ext, out Type T))
            {
                T = typeof(AssetMetaFile);
            }
            AssetMetaFile Loaded = null;
            {
                // Deserialize from disk.
                var stream = System.IO.File.OpenRead(
                    System.IO.Path.Combine(PipelineInstance.Instance.Root, AssetFileName + PipelineInstance.MetaAfterFix));
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                var yaml = reader.ReadToEnd();
                reader.Close();
                Loaded = deserializer.Deserialize(yaml, T) as AssetMetaFile;
            }
            if (Loaded is null) 
                return Loaded; 
            Loaded.OnLoaded(AssetFileName);
            return Loaded;
        }

        public static AssetMetaFile FindOnDisk(string AssetFileName)
        {
            var existed = System.IO.File.Exists(
                System.IO.Path.Combine(PipelineInstance.Instance.Root, AssetFileName + PipelineInstance.MetaAfterFix));
            return existed ? 
                LoadMetaFromDisk(AssetFileName) :
                null;
        }

        public static T FindOnDisk<T>(Guid Guid) where T : AssetMetaFile
        {
            var existed = PipelineInstance.AllMetas.TryGetValue(Guid, out AssetMetaFile F);
            if(!existed)
            {
                System.Console.WriteLine($"Asset {Guid} Not Existed!");
            }
            else if (F is null)
            {
                System.Console.WriteLine($"Asset {Guid} exist however null!");
            }
            else if(F.GetType() == typeof(T))
            {
                return F as T;
            }
            return null;
        }

        public static AssetMetaFile FindOnDisk(Guid Guid)
        {
            return FindOnDisk<AssetMetaFile>(Guid);
        }

        public static AssetMetaFile CreateOrLoadOnDisk(string AssetFileName)
        {
            var Existed = FindOnDisk(AssetFileName);
            if (Existed != null) 
                return Existed;

            var ext = System.IO.Path.GetExtension(AssetFileName);
            if (!PipelineInstance.Instance.ExtNameMetaTypeDictionary.TryGetValue(ext, out Type T))
            {
                T = typeof(AssetMetaFile);
            }
            var Result = System.Activator.CreateInstance(T) as AssetMetaFile;
            Result.OnCreated(AssetFileName);
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
