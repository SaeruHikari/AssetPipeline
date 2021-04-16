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
			PipelineURL Root = new PipelineURL() { 
				virtualSource = "",
				url = "Assets"
			};
			PipelineInstance.Initialize(Root);
			var pipeline = PipelineInstance.Instance;
			PipelineURL File = Root;
			File.url = "data.mdb";
			var meta = AssetMetaFile.FindOrCreate<AssetMetaFile>(File);

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
				using (var tx = env.BeginTransaction(TransactionBeginFlags.ReadOnly))
				using (var db = tx.OpenDatabase("custom"))
				{
					var (resultCode, key, value) = tx.Get(db, Encoding.UTF8.GetBytes("hello"));
					var arr = value.CopyToNewArray().ToString();
					var cd = Encoding.UTF8.GetBytes("world").ToString();
					if (arr.Equals(cd))
                    {
						System.Console.WriteLine("Yes!");
                    }
				}
			}
		}
    }
}
