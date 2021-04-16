using System;
using System.Collections.Generic;
using System.Text;
using AssetPipeline.Core;
using AssetPipeline.Pipeline;

using LightningDB;

namespace AssetPipeline
{
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

        public static void Initialize(PipelineURL RootURL)
        {
            _instance = new PipelineInstance(RootURL);

            RegisterAssetMetaFile<AssetMetaFile>();
        }

        PipelineInstance(PipelineURL RootURL)
        {
            Root = RootURL;
            // Connect to DB
            if(RootURL.virtualSource.Length != 0)
            {
                Console.WriteLine("ERROR: VirtualLink not supported now.");
            }
            DBEnv = new LightningEnvironment(RootURL.url);
            DBEnv.Open();
        }

        public readonly LightningEnvironment DBEnv;
        public static PipelineInstance Instance => _instance;
        protected static PipelineInstance _instance;

        public readonly PipelineURL Root;
        public readonly Dictionary<AssetGuid, Type> TypedMetaFileDictionary = new Dictionary<AssetGuid, Type>();
        public readonly Dictionary<string, Type> ExtNameMetaTypeDictionary = new Dictionary<string, Type>();
    }
}
