using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using AssetPipeline.Core;
using AssetPipeline.Pipeline;
using AssetPipeline.Pipeline.SourceControl;
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
            var guid = classType.GetCustomAttributes(typeof(AssetMetaAttribute), false)[0] as AssetMetaAttribute;
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

        public static void ConnectToPerforce(string ServerIp, string UserName, string Workspace = "")
        {
            _instance.sourceControl = new PerfoceConnection(ServerIp, UserName, Workspace);
        }

        public PipelineInstance(string RootURL)
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
        public static readonly string MetaAfterFix = ".meta";
        public readonly Dictionary<Guid, Type> TypedMetaFileDictionary = new Dictionary<Guid, Type>();
        public readonly Dictionary<string, Type> ExtNameMetaTypeDictionary = new Dictionary<string, Type>();

        public static SourceControlBase SourceContrrol => Instance.sourceControl;
        protected SourceControlBase sourceControl = new SourceControlBase();

        public static ConcurrentDictionary<Guid, AssetMetaFile> AllMetas = new ConcurrentDictionary<Guid, AssetMetaFile>();
        public static ConcurrentDictionary<string, Guid> AllMetasPath = new ConcurrentDictionary<string, Guid>();
        public static ConcurrentDictionary<Guid, string> AllMetasDir = new ConcurrentDictionary<Guid, string>();
    }
}
