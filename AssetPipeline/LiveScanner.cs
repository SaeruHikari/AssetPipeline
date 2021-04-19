using System;
using System.Collections.Generic;
using System.Text;
using AssetPipeline.Scanner;
using AssetPipeline.Pipeline;
using AssetPipeline.Core;
using System.IO;

namespace AssetPipeline
{
    public static class LiveScanner
    {
        public static void ScanMetaAt(string MetaPath)
        {
            var Root = PipelineInstance.Instance.Root;
            var RMetaPath = Path.GetRelativePath(Root, MetaPath);
            string AssetPath = RMetaPath.Substring(0, RMetaPath.Length - PipelineInstance.MetaAfterFix.Length);
            var MetaLoaded = AssetMetaFile.LoadMetaFromDisk(AssetPath);
            PipelineInstance.AllMetas[MetaLoaded.Guid] = MetaLoaded;
            PipelineInstance.AllMetasPath[MetaLoaded.Guid] = Path.GetDirectoryName(RMetaPath);
        }

        public static void ScanUpdateAllMeta()
        {
            var Root = PipelineInstance.Instance.Root;
            string[] MetaPaths = Directory.GetFiles(Root, 
                "*" + PipelineInstance.MetaAfterFix, SearchOption.AllDirectories
            );
            foreach(var MetaPath in MetaPaths)
            {
                ScanMetaAt(MetaPath);
            }
        }

        static LiveScanner()
        {
            watcher = new Scanner.PipelineWatcher(PipelineInstance.Instance.Root);
        }

        public static Scanner.PipelineWatcher watcher;
    }
}
