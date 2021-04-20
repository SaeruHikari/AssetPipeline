using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using System.Numerics;
using AssetPipeline;
using AssetPipeline.Core;
using AssetPipeline.Pipeline;
using System.Collections.Concurrent;

namespace AssetSandbox
{
    public class AssetBrowser : ImGuiWindow
    {
        public Guid CurrentFocusedOn;
        //HashSet<string> SelectedFilenames = new HashSet<string>();
        protected class FileTreeElement : TreeElement
        {
            public bool Open { get; set; }
            public string ShortName => Name.Length == 0 ? "Pipeline Root" : System.IO.Path.GetFileName(Name);
        }
        FileTreeElement CacheTree = new FileTreeElement() {
            Name = ""
        };

        protected override void OnImGui()
        {
            base.OnImGui();
            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
            ImGui.SetNextWindowPos(new Vector2(0.0f, 0.0f), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new Vector2(window.Width, window.Height));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            windowFlags |= ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize |
                           ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus |
                           ImGuiWindowFlags.NoTitleBar;
            var pOpen = true;
            ImGui.Begin("Asset Browser", ref pOpen, windowFlags);
            ImGui.PopStyleVar(2);
            // Menu
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open..", "Ctrl+O")) { /* Do stuff */ }
                    if (ImGui.MenuItem("Save", "Ctrl+S")) { /* Do stuff */ }
                    if (ImGui.MenuItem("Close", "Ctrl+W")) { /* Do stuff */ }
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }
            // DockSpace
            ImGui.DockSpace(ImGui.GetID("DockSpace"));
            ImGui.Columns(2, "DockSpace");
            if (ImGui.GetColumnWidth(0) >= ImGui.GetColumnWidth(1))
                ImGui.SetColumnWidth(
                    0, (int)(0.45 * (ImGui.GetColumnWidth(1) + ImGui.GetColumnWidth(0)))
                );
            // Left
            {
                var Height = ImGui.GetFrameHeightWithSpacing();
                ImGui.BeginChild("FilePicker", new Vector2(0, (int)(-4.4 * Height)), false);
                DisplayFileSystemTree();
                ImGui.EndChild();
                DisplayAssetSketchy(CurrentFocusedOn);
            }
            ImGui.NextColumn();
            // Right
            {
                ImGui.BeginGroup();
                // Leave room for 1 line below us
                var Height = ImGui.GetFrameHeightWithSpacing();
                ImGui.BeginChild("DetailViewer", new Vector2(0, -Height));
                ImGui.Text("GUID: " + CurrentFocusedOn);

                ImGui.EndChild();
                ImGui.EndGroup();
            }
        }

        protected void RecursiveClosePath(FileTreeElement Element)
        {
            foreach(var Child in Element.Children)
            {
                RecursiveClosePath(Child as FileTreeElement);
            }
            Element.Open = false;
            Element.Children.Clear();
        }

        protected void OpenPath(FileTreeElement Element)
        {
            if(!Element.Open)
            {
                Element.Open = true;
                var directories = System.IO.Directory.GetDirectories(
                    System.IO.Path.Combine(PipelineInstance.Instance.Root, Element.Name)
                );
                foreach(var subdir in directories)
                {
                    Element.Children.Add(new FileTreeElement()
                    {
                        Name = System.IO.Path.GetRelativePath(PipelineInstance.Instance.Root, subdir)
                    });
                }
            }
        }

        protected void DisplayAssetSketchy(Guid AssetGuid)
        {
            var Height = ImGui.GetFrameHeightWithSpacing();
            ImGui.BeginChild("MetaSketchy", new Vector2(0, 0), false);
            ImGui.Separator();
            if (CurrentFocusedOn != Guid.Empty)
            {
                var E = PipelineInstance.AllMetas.TryGetValue(AssetGuid, out var Asset);
                if(E)
                {
                    ImGui.Columns(2, "DockSpace");
                    ImGui.Image(IntPtr.Zero, new Vector2(4 * Height, 4 * Height));
                    ImGui.SetColumnWidth(0, 4 * Height);
                    ImGui.NextColumn();
                    ImGui.Text("AssetType: " + Asset.GetType().Name);
                    ImGui.Text("Version: " + Asset.VersionNumber);
                    ImGui.Text("LastWrite: " + Asset.LastWriteTime);
                    ImGui.Text("References: " + Asset.References.Length);
                    ImGui.Button("Visualize References");
                }
            }
            ImGui.EndChild();
        }

        protected bool DisplayAssetTreeElement(string MetaName, AssetMetaFile MetaFile)
        {
            var Flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet |
                ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.SpanFullWidth |
                ImGuiTreeNodeFlags.NoAutoOpenOnLog;
            if (CurrentFocusedOn == MetaFile.Guid)
            {
               Flags |= ImGuiTreeNodeFlags.Selected;
            }
            var Result = ImGui.TreeNodeEx(MetaFile.Name, Flags);
            if(ImGui.IsItemClicked())
            {
                CurrentFocusedOn = MetaFile.Guid;
            }
            return Result;
        }

        protected bool DisplayFileSystemTreeElement(FileTreeElement Element)
        {
            bool Open = ImGui.TreeNodeEx(Element.ShortName, ImGuiTreeNodeFlags.SpanFullWidth);
            if (Open)
            {
                OpenPath(Element);
                foreach (var Child in Element.Children)
                {
                    DisplayFileSystemTreeElement(Child as FileTreeElement);
                }
                // Display Assets
                var Assets = LiveScanner.ScanUnderPath(Element.Name, false);
                foreach(var Asset in Assets)
                {
                    if(PipelineInstance.AllMetasPath.TryGetValue(Asset, out var GUID))
                    {
                        if(PipelineInstance.AllMetas.TryGetValue(GUID, out var AssetMeta))
                        {
                            DisplayAssetTreeElement(Asset, AssetMeta);
                        }
                    }
                }
                ImGui.TreePop();
            }
            else if (Element.Open)
            {
                RecursiveClosePath(Element);
            }
            return Open;
        }

        protected void DisplayFileSystemTree()
        {
            ImGui.PushID("FileTree");
            DisplayFileSystemTreeElement(CacheTree);
            ImGui.PopID();
        }
    }
}
