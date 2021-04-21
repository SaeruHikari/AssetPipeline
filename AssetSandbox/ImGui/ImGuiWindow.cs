using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using System.Numerics;

namespace AssetSandbox
{
    public class ImGuiWindow
    {
        public static Veldrid.Sdl2.Sdl2Window window;
        public static Veldrid.GraphicsDevice gd;
        public static Veldrid.CommandList cl;
        public static Veldrid.ImGuiRenderer controller;

        public ImGuiWindow()
        {
            Veldrid.StartupUtilities.VeldridStartup.CreateWindowAndGraphicsDevice(
              new Veldrid.StartupUtilities.WindowCreateInfo(
                148, 148, 780, 432, Veldrid.WindowState.Normal, "Asset Browser"
              ),
              new Veldrid.GraphicsDeviceOptions(true, null, true),
              out window,
              out gd
            );
            //window.BorderVisible = false;
            window.Resized += () =>
            {
                gd.MainSwapchain.Resize((uint)window.Width, (uint)window.Height);
                controller.WindowResized(window.Width, window.Height);
            };
            cl = gd.ResourceFactory.CreateCommandList();
            controller = new Veldrid.ImGuiRenderer(gd, gd.MainSwapchain.Framebuffer.OutputDescription, window.Width, window.Height);
            ImGuiIOPtr io = ImGui.GetIO();
            io.WantSaveIniSettings = true;
        }

        ~ImGuiWindow()
        {
            gd.WaitForIdle();
            controller.Dispose();
            cl.Dispose();
            gd.Dispose();
        }

        public void Update()
        {
            Veldrid.InputSnapshot snapshot = window.PumpEvents();
            if (!window.Exists) { return; }
            controller.Update(1f / 60f, snapshot);

            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
            ImGui.SetNextWindowPos(new Vector2(0.0f, 0.0f), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new Vector2(window.Width, window.Height));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            windowFlags |= ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize |
                           ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus |
                           ImGuiWindowFlags.NoTitleBar;
            var pOpen = true;
            //ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.22f, 0.22f, 0.22f, 1.0f));
            ImGui.Begin("Asset Browser", ref pOpen, windowFlags);
            ImGui.PopStyleVar(2);
            //ImGui.PopStyleColor(1);
            OnImGui();
            ImGui.End();

            cl.Begin();
            cl.SetFramebuffer(gd.MainSwapchain.Framebuffer);
            cl.ClearColorTarget(0, new Veldrid.RgbaFloat(160f / 255, 160f / 255, 192f / 255, 255f));
            controller.Render(gd, cl);
            cl.End();
            gd.SubmitCommands(cl);
            gd.SwapBuffers(gd.MainSwapchain);
        }

        public bool ShouldQuit()
        {
            return !window.Exists;
        }

        protected virtual void OnImGui()
        {
            
        }
    }
}
