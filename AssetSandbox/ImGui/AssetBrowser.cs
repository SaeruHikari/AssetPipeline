using ImGuiNET;
using System.Numerics;

namespace AssetSandbox
{
    public class AssetBrowser : ImGuiWindow
    {
        ImGuiTreeView.FileTreeElement CacheTree = new ImGuiTreeView.FileTreeElement()
        {
            Name = ""
        };

        protected void OnMenu()
        {
            /*
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open..", "Ctrl+O")) { }
                if (ImGui.MenuItem("Save", "Ctrl+S")) {}
                if (ImGui.MenuItem("Close", "Ctrl+W")) { }
                ImGui.EndMenu();
            }
            */
            ImGuiSourceControl.OnMenu();
        }

        protected override void OnImGui()
        {
            base.OnImGui();
            // Menu
            if (ImGui.BeginMenuBar())
            {
                OnMenu();
                ImGui.EndMenuBar();
            }
            // DockSpace
            ImGui.DockSpace(ImGui.GetID("DockSpace"));
            ImGui.Columns(2, "DockSpace");
            if (ImGui.GetColumnWidth(0) >= ImGui.GetColumnWidth(1))
                ImGui.SetColumnWidth(
                    0, (int)(0.45 * (ImGui.GetColumnWidth(1) + ImGui.GetColumnWidth(0)))
                );
            // Left: FileSystemTree
            {
                var Height = ImGui.GetFrameHeightWithSpacing();
                ImGui.BeginChild("FilePicker", new Vector2(0, (int)(-4.6 * Height)), false);
                DisplayFileSystemTree();
                ImGui.EndChild();
                ImGuiTreeView.DisplayAssetSketchy(ImGuiTreeView.CurrentFocusedOn);
            }
            ImGui.NextColumn();
            // Right
            {
                ImGui.BeginGroup();
                // Leave room for 1 line below us
                var Height = ImGui.GetFrameHeightWithSpacing();
                ImGui.BeginChild("DetailViewer", new Vector2(0, 0));
                ImGuiAssetInspector.OnWindow(ImGuiTreeView.CurrentFocusedOn);
                ImGui.EndChild();
                ImGui.EndGroup();
            }
            // Source Control.
            ImGuiSourceControl.OnWindow();
        }
        

        protected void DisplayFileSystemTree()
        {
            ImGui.PushID("FileTree");
            ImGuiTreeView.DisplayFileSystemTreeElement(CacheTree);
            ImGui.PopID();
        }
    }
}
