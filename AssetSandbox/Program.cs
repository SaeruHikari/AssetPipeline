using System;
using System.Text;
using LightningDB;
using AssetPipeline.Core;
using AssetPipeline.Pipeline;
using AssetPipeline;

namespace AssetSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
			DBInit();

			PipelineInstance.Initialize("Assets");
			LiveScanner.ScanUpdateAll();

			var pipeline = PipelineInstance.Instance;
			var meta = AssetMetaFile.CreateOrLoadOnDisk<AssetMetaFile>("data.mdb");

			Console.WriteLine("Scanner keeps working, Press enter to exit...");
			Console.ReadLine();
		}


		static void DBInit()
        {
			using (var env = new LightningEnvironment("Assets"))
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
