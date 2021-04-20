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
        protected static Veldrid.Sdl2.Sdl2Window window;
        protected static Veldrid.GraphicsDevice gd;
        protected static Veldrid.CommandList cl;
        protected static Veldrid.ImGuiRenderer controller;

        public ImGuiWindow()
        {
            Veldrid.StartupUtilities.VeldridStartup.CreateWindowAndGraphicsDevice(
              new Veldrid.StartupUtilities.WindowCreateInfo(
                148, 148, 720, 410, Veldrid.WindowState.Normal, "Asset Browser"
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

            OnImGui();

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
