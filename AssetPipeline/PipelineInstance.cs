using System;
using System.Collections.Generic;
using System.Text;
using AssetPipeline.Core;
using AssetPipeline.Pipeline;

using LightningDB;

namespace AssetPipeline
{
    public interface IVersionControlConnection
    {
        public int FileVersion(string File);
    }

    //[PipelineDB("AssetPipeline")]
    public class PipelineInstance 
    {
        static public void RegisterAssetMetaFile<T>()
        {
            Type classType = typeof(T);
            var guid = classType.GetCustomAttributes(typeof(MetaGuidAttribute), false)[0] as MetaGuidAttribute;
            Instance.TypedMetaFileDictionary[guid.guid] = classType;

            var exts = classType.GetCustomAttributes(typeof(MetaSourceExtAttribute), false);
            foreach(MetaSourceExtAttribute ext in exts)
            {
                Instance.ExtNameMetaTypeDictionary[ext.Ext] = classType;
            }
        }

        public static void Initialize(string RootURL)
        {
            _instance = new PipelineInstance(RootURL);

            RegisterAssetMetaFile<AssetMetaFile>();
        }

        PipelineInstance(string RootURL)
        {
            Root = RootURL;
            DBEnv = new LightningEnvironment(RootURL);
            DBEnv.Open();
        }

        public readonly LightningEnvironment DBEnv;
        public static PipelineInstance Instance => _instance;
        protected static PipelineInstance _instance;
        public static IVersionControlConnection VersionControl => null;

        public readonly string Root;
        public readonly Dictionary<AssetGuid, Type> TypedMetaFileDictionary = new Dictionary<AssetGuid, Type>();
        public readonly Dictionary<string, Type> ExtNameMetaTypeDictionary = new Dictionary<string, Type>();
    }
}
