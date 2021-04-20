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
            guid = new Guid(value);
        }
        public AssetMetaAttribute(Guid value)
        {
            guid = value;
        }
        public readonly Guid guid;
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
        [YamlIgnore] public string Name => System.IO.Path.GetFileNameWithoutExtension(Source);
        [YamlIgnore] public string ThisFile => Source + PipelineInstance.MetaAfterFix;
        [YamlIgnore] public string AssetFilePath
            => PipelineInstance.AllMetasDir.ContainsKey(Guid) ?
            System.IO.Path.Combine(PipelineInstance.Instance.Root, PipelineInstance.AllMetasDir[Guid], Source) :
            null;
        [YamlIgnore] public string MetaFilePath 
            => AssetFilePath is null ? null : AssetFilePath + PipelineInstance.MetaAfterFix;

        virtual protected void OnLoaded(string MetaFile) { }
        virtual protected void OnCreated(string File)
        {
            // Initialize Data.
            var info = new System.IO.FileInfo(System.IO.Path.Combine(PipelineInstance.Instance.Root, File));
            Guid = Guid.NewGuid();
            References = new GuidReferenceNode();
            VersionNumber = PipelineInstance.VersionControl is null ? 0 : PipelineInstance.VersionControl.FileVersion(File);
            LastWriteTime = info.LastWriteTime;
            Source = System.IO.Path.GetFileName(File);
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

        public static AssetMetaFile FindAndTryOpen(string AssetFileName)
        {
            var existed = System.IO.File.Exists(
                System.IO.Path.Combine(PipelineInstance.Instance.Root, AssetFileName + PipelineInstance.MetaAfterFix));
            return existed ? 
                LoadMetaFromDisk(AssetFileName) :
                null;
        }

        public static T FindAndTryOpen<T>(Guid Guid) where T : AssetMetaFile
        {
            // Find in Pipeline Cache.
            var existed = PipelineInstance.AllMetas.TryGetValue(Guid, out AssetMetaFile F);
            if(!existed)
            {
                System.Console.WriteLine($"Asset {Guid} Not Existed in PipelineCache!");
                //TODO: Find On Disk.
            }
            else if (F is null)
            {
                System.Console.WriteLine($"Asset {Guid} exist however null!");
            }
            else if(F.GetType() == typeof(T))
            {
                return F as T;
            } 
            else
            {
                System.Console.WriteLine($"Asset {Guid} exist however is {F.GetType()}, not of Type {typeof(T)}!");
                return null;
            }
            System.Console.WriteLine("Unknown Error!");
            return null;
        }

        public static AssetMetaFile FindAndTryOpen(Guid Guid)
        {
            return FindAndTryOpen<AssetMetaFile>(Guid);
        }
        
        public static void FindAndTryDelete(string AssetPath, string MetaPath)
        {
            var E0 = AssetPath is null ? false :System.IO.File.Exists(MetaPath);
            var E1 = MetaPath is null ? false : System.IO.File.Exists(MetaPath);
            if(E0) System.IO.File.Delete(AssetPath);
            if(E1) System.IO.File.Delete(MetaPath);
        }

        public static void FindAndTryDelete(Guid Guid)
        {
            // Find in Pipeline Cache.
            var ExistedInCache = PipelineInstance.AllMetas.TryGetValue(Guid, out AssetMetaFile F);
            //TODO: Find On Disk.
            if (ExistedInCache && F != null)
            {
                // Delete AssetFile & MetaFile.
                FindAndTryDelete(F.AssetFilePath, F.MetaFilePath);
                // Remove Related Cache.
                var R0 = PipelineInstance.AllMetas.TryRemove(Guid, out var RemovedMeta);
                var R1 = PipelineInstance.AllMetasDir.TryRemove(Guid, out var RemovedPath);
                if(!R0 || !R1)
                {
                    Console.WriteLine("Error: Failed to Remove Meta Cache!");
                }
            }
            return;
        }

        public static AssetMetaFile CreateOrLoadOnDisk(string AssetFileName)
        {
            var Existed = FindAndTryOpen(AssetFileName);
            if (Existed != null) 
                return Existed;
            var ext = System.IO.Path.GetExtension(AssetFileName);
            if (!PipelineInstance.Instance.ExtNameMetaTypeDictionary.TryGetValue(ext, out Type T))
            {
                T = typeof(AssetMetaFile);
            }
            var Result = System.Activator.CreateInstance(T) as AssetMetaFile;
            Result.OnCreated(AssetFileName);
            // Serialize & Write At Once.
            var yaml = serializer.Serialize(Result);
            var ThisPath = AssetFileName + PipelineInstance.MetaAfterFix;
            var stream = System.IO.File.Create(ThisPath);
            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);
            writer.Write(yaml);
            writer.Flush();
            writer.Close();
            LiveScanner.ScanMetaAt(ThisPath);
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
