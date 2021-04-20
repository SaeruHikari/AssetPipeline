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
            string AssetPath = MetaPath.Substring(0, MetaPath.Length - PipelineInstance.MetaAfterFix.Length);
            var MetaLoaded = AssetMetaFile.LoadMetaFromDisk(AssetPath);
            PipelineInstance.AllMetas[MetaLoaded.Guid] = MetaLoaded;
            PipelineInstance.AllMetasDir[MetaLoaded.Guid] = Path.GetDirectoryName(MetaPath);
            PipelineInstance.AllMetasPath[MetaPath] = MetaLoaded.Guid;
        }

        public static void ScanUpdateAllMeta()
        {
            var Root = PipelineInstance.Instance.Root;
            string[] MetaPaths = Directory.GetFiles(Root, 
                "*" + PipelineInstance.MetaAfterFix, SearchOption.AllDirectories
            );
            foreach(var MetaPath in MetaPaths)
            {
                var RMetaPath = Path.GetRelativePath(Root, MetaPath);
                ScanMetaAt(RMetaPath);
            }
        }

        public static string[] ScanUnderPath(string Path, bool Absolute = false, bool Recursive = false)
        {
            var Root = PipelineInstance.Instance.Root;
            string[] MetaPaths = Directory.GetFiles(
                Absolute?Path:System.IO.Path.Combine(Root, Path),
                "*" + PipelineInstance.MetaAfterFix,
                Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
            );
            for(int i = 0; i < MetaPaths.Length; i++)
            {
                MetaPaths[i] = Absolute ? MetaPaths[i] : System.IO.Path.GetRelativePath(Root, MetaPaths[i]);
            }
            return MetaPaths;
        }

        static LiveScanner()
        {
            watcher = new Scanner.PipelineWatcher(PipelineInstance.Instance.Root);
        }

        public static Scanner.PipelineWatcher watcher;
    }
}
