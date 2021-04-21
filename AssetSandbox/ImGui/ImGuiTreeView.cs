using AssetPipeline.Core;
using System;
using ImGuiNET;
using System.Numerics;
using AssetPipeline;
using AssetPipeline.Pipeline;

namespace AssetSandbox
{
    public static class ImGuiTreeView
    {
        public class FileTreeElement : TreeElement
        {
            public bool Open { get; set; }
            public string ShortName => Name.Length == 0 ? "Pipeline Root" : System.IO.Path.GetFileName(Name);
        }
        public static Guid CurrentFocusedOn;

        private static void RecursiveClosePath(FileTreeElement Element)
        {
            foreach (var Child in Element.Children)
            {
                RecursiveClosePath(Child as FileTreeElement);
            }
            Element.Open = false;
            Element.Children.Clear();
        }

        private static void OpenPath(FileTreeElement Element)
        {
            if (!Element.Open)
            {
                Element.Open = true;
                var directories = System.IO.Directory.GetDirectories(
                    System.IO.Path.Combine(PipelineInstance.Instance.Root, Element.Name)
                );
                foreach (var subdir in directories)
                {
                    Element.Children.Add(new FileTreeElement()
                    {
                        Name = System.IO.Path.GetRelativePath(PipelineInstance.Instance.Root, subdir)
                    });
                }
            }
        }

        public static void DisplayAssetSketchy(Guid AssetGuid)
        {
            var Height = ImGui.GetFrameHeightWithSpacing();
            ImGui.BeginChild("MetaSketchy", new Vector2(0, 0), false);
            ImGui.Separator();
            if (CurrentFocusedOn != Guid.Empty)
            {
                var E = PipelineInstance.AllMetas.TryGetValue(AssetGuid, out var Asset);
                if (E)
                {
                    ImGui.Columns(2, "DockSpace");
                    // Try Get.
                    if (MainProgram.PreviewTextures.TryGetValue(AssetGuid, out var Texture))
                    {
                        IntPtr Binding = ImGuiWindow.controller.GetOrCreateImGuiBinding(
                            ImGuiWindow.gd.ResourceFactory, Texture);
                        ImGui.Image(Binding, new System.Numerics.Vector2(4 * Height, 4 * Height));
                    }
                    ImGui.SetColumnWidth(0, (int)(4.4 * Height));
                    ImGui.SetColumnOffset(0, 0.65f * Height);
                    ImGui.NextColumn();
                    ImGui.Text("AssetType: " + Asset.GetType().Name);
                    ImGui.Text("Version: " + Asset.VersionNumber);
                    ImGui.Text("LastWrite: " + Asset.LastWriteTime);
                    ImGui.Text("References: " + Asset.References.Length);
                    ImGui.Button("Reference Graph");
                    ImGui.SameLine();
                    ImGui.Button("ReCompile");
                }
            }
            ImGui.EndChild();
        }

        private static bool DisplayAssetTreeElement(string MetaName, AssetMetaFile MetaFile)
        {
            var Flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet |
                ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.SpanFullWidth |
                ImGuiTreeNodeFlags.NoAutoOpenOnLog;
            if (CurrentFocusedOn == MetaFile.Guid)
            {
                Flags |= ImGuiTreeNodeFlags.Selected;
            }
            var Result = ImGui.TreeNodeEx(MetaFile.Source, Flags);
            if (ImGui.IsItemClicked())
            {
                CurrentFocusedOn = MetaFile.Guid;
            }
            return Result;
        }

        static string SelectedPath = "";
        public static bool DisplayFileSystemTreeElement(FileTreeElement Element)
        {
            bool Open = ImGui.TreeNodeEx(Element.ShortName, ImGuiTreeNodeFlags.SpanFullWidth);

            if (ImGui.BeginPopupContextWindow())
            {
                if (ImGui.MenuItem("Import", true))
                {
                    SelectedPath = ImGuiAssetImporters.Import(
                        System.IO.Path.Combine(PipelineInstance.Instance.Root, Element.Name)
                    );
                    System.Console.WriteLine(SelectedPath);
                }
                ImGui.EndPopup();
            }

            if (Open)
            {
                OpenPath(Element);
                foreach (var Child in Element.Children)
                {
                    DisplayFileSystemTreeElement(Child as FileTreeElement);
                }
                // Display Assets
                var Assets = LiveScanner.ScanUnderPath(Element.Name, false);
                foreach (var Asset in Assets)
                {
                    if (PipelineInstance.AllMetasPath.TryGetValue(Asset, out var GUID))
                    {
                        if (PipelineInstance.AllMetas.TryGetValue(GUID, out var AssetMeta))
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
    }
}
