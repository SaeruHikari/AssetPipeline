using AssetPipeline.Core;
using System;
using ImGuiNET;
using System.Numerics;
using AssetPipeline;
using AssetPipeline.Pipeline;
using AssetPipeline.Pipeline.SourceControl;
using System.Collections.Generic;

namespace AssetSandbox
{
    public static class ImGuiSourceControl
    {
        private static bool ConnectionWindowOpen = false;
        private static bool OpenConnectionWindowOpen = false;
        public static string[] AvalableSourceControls = { "None", "Perforce" , "Git" };
        public static int SelectedSourceControlProvider = 0;

        public static void OnMenu()
        {
            if (ImGui.BeginMenu("SourceControl"))
            {
                if (ImGui.MenuItem("About Connection", "..."))
                {
                    ConnectionWindowOpen = true;
                }
                if (ImGui.MenuItem("Open Connection", "..."))
                {
                    OpenConnectionWindowOpen = true;
                }
                ImGui.EndMenu();
            }
        }

        static string ServerIp = "";
        static string UserName = "";
        static string Workspace = "";
        static IList<Perforce.P4.Client> AvalableWorkspaces;
        public static int SelectedWorkspace = 0;
        public static void ShowPerforceConnectConfig()
        {
            ImGui.InputText("Server", ref ServerIp, 50);
            ImGui.InputText("UserName", ref UserName, 50);
            if (PipelineInstance.SourceContrrol is not PerfoceConnection)
            {
                ImGui.InputText("Workspace", ref Workspace, 50);
            } 
            else
            {
                var P4 = PipelineInstance.SourceContrrol as PerfoceConnection;
                AvalableWorkspaces = P4.WorkSpaces;
                if (AvalableWorkspaces != null && 
                    ImGui.BeginCombo("Workspace", AvalableWorkspaces[SelectedWorkspace].Name))
                {
                    for (int i = 0; i < AvalableWorkspaces.Count; i++)
                    {
                        bool Selected = (i == SelectedWorkspace);
                        if (ImGui.Selectable(AvalableWorkspaces[i].Name, Selected))
                            SelectedSourceControlProvider = i;
                        if (Selected)
                            ImGui.SetItemDefaultFocus();
                    }
                    ImGui.EndCombo();
                }
                ImGui.Text($"CharacterSet: {P4.CharacterSet}");
            }
            if (ImGui.Button("Connect"))
            {
                PipelineInstance.ConnectToPerforce(ServerIp, UserName, Workspace);
                ConnectionWindowOpen = true;
            }
        }

        public static void OnWindow()
        {
            if (ConnectionWindowOpen &&
                ImGui.Begin("SourceControl: " + PipelineInstance.SourceContrrol?.SolutionName, ref ConnectionWindowOpen)
            )
            {
                var P4 = PipelineInstance.SourceContrrol as PerfoceConnection;
                if (P4 is not null)
                {
                    
                }
            }
            if (OpenConnectionWindowOpen &&
                ImGui.Begin("Open Connection", ref OpenConnectionWindowOpen)
            )
            {
                if (ImGui.BeginCombo("Provider", AvalableSourceControls[SelectedSourceControlProvider]))
                {
                    for(int i = 0; i < AvalableSourceControls.Length; i++)
                    {
                        bool Selected = (i == SelectedSourceControlProvider);
                        if(ImGui.Selectable(AvalableSourceControls[i], Selected))
                            SelectedSourceControlProvider = i;
                        if (Selected)
                            ImGui.SetItemDefaultFocus();
                    }
                    ImGui.EndCombo();
                }
                if(SelectedSourceControlProvider == 1)
                {
                    ShowPerforceConnectConfig();
                }
            }
        }
    }
}
