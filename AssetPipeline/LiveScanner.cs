using System;
using System.Collections.Generic;
using System.Text;
using AssetPipeline.Scanner;
using AssetPipeline.Pipeline;
using AssetPipeline.Core;

namespace AssetPipeline
{
    public static class LiveScanner
    {
        public static void ScanUpdateAll()
        {

        }

        static LiveScanner()
        {
            watcher = new Scanner.PipelineWatcher(PipelineInstance.Instance.Root);
        }

        public static Scanner.PipelineWatcher watcher;
        public static Dictionary<Guid, string> allAssets = new Dictionary<Guid, string>();
    }
}
