using System;
using System.Collections.Generic;
using System.Text;

namespace AssetPipeline.Core
{
    public class TreeElement
    {
        public string Name { get; set; }
        public IList<TreeElement> Children { get; set; } = new List<TreeElement>();
    }
}
