using System;
using System.Numerics;
using AssetPipeline;
using AssetSandbox.Assets;
using ImGuiNET;

namespace AssetSandbox
{
    public static class ImGuiAssetInspector
    {
        public static Veldrid.ResourceFactory ResourceFactory => ImGuiWindow.gd.ResourceFactory;
        public static float DisplayScalar = 1.0f;
        
        public static void OnWindow(Guid GUID)
        {
            if (GUID != Guid.Empty)
            {
                ImGui.Text("GUID: " + GUID);
                if (PipelineInstance.AllMetas.TryGetValue(GUID, out var Asset))
                {
                    if(Asset.GetType() == typeof(TextureAsset))
                    {
                        ImGui.SameLine(ImGui.GetWindowWidth() - 40);
                        if (ImGui.Button("+"))
                        {
                            DisplayScalar += 0.035f;
                        }
                        ImGui.SameLine();
                        if(ImGui.Button("-"))
                        {
                            DisplayScalar -= 0.035f;
                        }
                        DisplayScalar = Math.Min(6.0f, Math.Max(0.15f, DisplayScalar));
                        ImGui.BeginChild("TextureViewer", new Vector2(0, 0), false, ImGuiWindowFlags.HorizontalScrollbar);
                        /*
                        var canvas_p0 = ImGui.GetCursorScreenPos();      // ImDrawList API uses screen coordinates!
                        var canvas_sz = ImGui.GetContentRegionAvail();   // Resize canvas to what's available.
                        if (canvas_sz.X < 50.0f) canvas_sz.X = 50.0f;
                        if (canvas_sz.Y < 50.0f) canvas_sz.Y = 50.0f;
                        var canvas_p1 = new Vector2(canvas_p0.X + canvas_sz.X, canvas_p0.Y + canvas_sz.Y);
                        ImGui.GetWindowDrawList().AddRectFilled(canvas_p0, canvas_p1, 255);
                        */
                        // Try Get.
                        if (MainProgram.PreviewTextures.TryGetValue(GUID, out var Texture))
                        {
                            IntPtr Binding = ImGuiWindow.controller.GetOrCreateImGuiBinding(ResourceFactory, Texture);
                            ImGui.Image(Binding, new System.Numerics.Vector2(
                                DisplayScalar * Math.Max(Texture.Width, ImGui.GetColumnWidth()),
                                DisplayScalar * Math.Max(Texture.Height, ImGui.GetWindowHeight()))
                            );
                        }
                        else
                        {
                            var CPUTexutre = new Veldrid.ImageSharp.ImageSharpTexture(Asset.AssetFilePath);
                            MainProgram.CPUTextures[GUID] = CPUTexutre;
                            MainProgram.PreviewTextures[GUID] = MainProgram.CPUTextures[GUID].CreateDeviceTexture(ImGuiWindow.gd, ResourceFactory);
                        }
                        ImGui.EndChild();
                    }
                }
            }
        }
    }
}
