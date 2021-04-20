using AssetPipeline;
using AssetPipeline.Pipeline;
using AssetPipeline.Pipeline.InternalAssets;
using LightningDB;
using System;
using System.Text;
using ImGuiNET;


namespace AssetSandbox
{
    public class MainProgram
    {
        static void Main(string[] args)
        {
            PipelineInstance.Initialize("Assets");
            LiveScanner.ScanUpdateAllMeta();

            var pipeline = PipelineInstance.Instance;
            //var metameta = AssetMetaFile.CreateOrLoadOnDisk("custom/custom.metameta");
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