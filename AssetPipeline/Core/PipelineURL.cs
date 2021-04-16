using System;
using System.Collections.Generic;
using System.Text;

namespace AssetPipeline.Core
{
    public struct PipelineURL
    {
        public string virtualSource;
        public string url;
        public static System.IO.FileInfo FileInformation(PipelineURL URL)
        {
            if(URL.virtualSource.Length != 0)
            {
                Console.WriteLine("VirtualAsset is not supported now!");
                return null;
            }
            // Access on disk.
            var Result = new System.IO.FileInfo(System.IO.Path.Combine(PipelineInstance.Instance.Root.url, URL.url));
            return Result;
        }


    }
}
