using AssetPipeline;
using LightningDB;
using System.Text;
using AssetSandbox.Assets;
using AssetPipeline.Pipeline;
using System.Collections.Concurrent;
using System;

namespace AssetSandbox
{
    public static class MainProgram
    {
        public static ConcurrentDictionary<Guid, Veldrid.Texture> PreviewTextures = new ConcurrentDictionary<Guid, Veldrid.Texture>();
        public static ConcurrentDictionary<Guid, Veldrid.ImageSharp.ImageSharpTexture> CPUTextures = new ConcurrentDictionary<Guid, Veldrid.ImageSharp.ImageSharpTexture>();

        static void Main(string[] args)
        {
            PipelineInstance.Initialize("Assets");
            PipelineInstance.RegisterAssetMetaFile<TextureAsset>();
            LiveScanner.ScanUpdateAllMeta();

            var pipeline = PipelineInstance.Instance;
            //var meta = AssetMetaFile.CreateOrLoadOnDisk("db/data.mdb");
            //var meta2 = AssetMetaFile.FindAndTryOpen<DBAssetMeta>(meta.Guid);
            //var metaSource = meta2.AssetFilePath;
            //AssetMetaFile.FindAndTryDelete(meta.Guid);
            //Console.WriteLine("Scanner keeps working...");

            var browser = new AssetBrowser();
            while(!browser.ShouldQuit())
            {
                browser.Update();
            }
        }

        static void DBInit()
        {
            using (var env = new LightningEnvironment("Assets/db"))
            {
                env.MaxDatabases = 2;
                env.Open();

                using (var tx = env.BeginTransaction())
                using (var db = tx.OpenDatabase("custom", new DatabaseConfiguration { Flags = DatabaseOpenFlags.Create }))
                {
                    tx.Put(db, Encoding.UTF8.GetBytes("hello"), Encoding.UTF8.GetBytes("world"));
                    tx.Commit();
                }
            }
        }
    }
}