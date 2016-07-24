using UnityEngine;
using Lench.Scripter;
using Lench.AdvancedControls.Axes;
using spaar.ModLoader.UI;
using System.IO;
using System.Net;
using System.ComponentModel;
using System;
using System.Linq;

namespace Lench.AdvancedControls.UI
{
    internal class CustomAxisEditor : AxisEditor
    {
        internal static bool downloading_in_progress = false;
        internal static string download_button_text = "Download";

        internal CustomAxisEditor(InputAxis axis)
        {
            Axis = axis as CustomAxis;
        }

        private CustomAxis Axis;

        internal string note;
        internal string error;

        private Vector2 initialisationScrollPosition = Vector2.zero;
        private Vector2 updateScrollPosition = Vector2.zero;

        public void Open() { }
        public void Close() { }

        public void DrawAxis(Rect windowRect)
        {
            if (!PythonEnvironment.Loaded)
            {
                GUILayout.Label("<b>Python engine not available</b>\n" +
                                "Press download to install it automatically.\n\n" +
                                "<b>Note</b>\n" +
                                "This will install full Lench Scripter Mod.\n"+
                                "Please read about it's features on the forum.");
                if (GUILayout.Button(download_button_text) && !downloading_in_progress)
                    InstallIronPython();
            }
            else
            {
                error = null;
                note = null;

                // Draw graph
                GUILayout.BeginHorizontal();

                string text;
                if (Axis.Status == AxisStatus.OK)
                    text = Axis.OutputValue.ToString("0.00");
                else
                    text = InputAxis.GetStatusString(Axis.Status);

                GUILayout.Label("  <color=#808080><b>" + text + "</b></color>",
                    new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft },
                    GUILayout.Height(28));

                var graphRect = GUILayoutUtility.GetLastRect();

                Util.DrawRect(graphRect, Color.gray);
                Util.FillRect(new Rect(
                        graphRect.x + graphRect.width / 2,
                        graphRect.y,
                        1,
                        graphRect.height),
                    Color.gray);

                if (Axis.Status == AxisStatus.OK)
                    Util.FillRect(new Rect(
                                          graphRect.x + graphRect.width / 2 + graphRect.width / 2 * Axis.OutputValue,
                                          graphRect.y,
                                          1,
                                          graphRect.height),
                                 Color.yellow);

                // Draw start/stop button
                if (GUILayout.Button(Axis.Running ? "STOP" : "START", new GUIStyle(Elements.Buttons.Red) { margin = new RectOffset(8, 8, 0, 0) }, GUILayout.Width(80)))
                    Axis.Running = !Axis.Running;

                GUILayout.EndHorizontal();

                // Draw initialisation code text area
                GUILayout.Label("Initialisation code",
                    Util.LabelStyle);

                initialisationScrollPosition = GUILayout.BeginScrollView(initialisationScrollPosition,
                    GUILayout.Height(100));
                Axis.InitialisationCode = GUILayout.TextArea(Axis.InitialisationCode);
                GUILayout.EndScrollView();

                // Draw update code text area
                GUILayout.Label("Update code",
                    Util.LabelStyle);

                updateScrollPosition = GUILayout.BeginScrollView(updateScrollPosition,
                    GUILayout.Height(200));
                Axis.UpdateCode = GUILayout.TextArea(Axis.UpdateCode);
                GUILayout.EndScrollView();

                // Draw Global toggle
                GUILayout.BeginHorizontal();

                Axis.GlobalScope = GUILayout.Toggle(Axis.GlobalScope, "",
                    Util.ToggleStyle,
                    GUILayout.Width(20),
                    GUILayout.Height(20));

                GUILayout.Label("Run in global scope",
                    new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

                GUILayout.EndHorizontal();

                GUILayout.Box(GUIContent.none, GUILayout.Height(20));

                // Draw notes
                if (!PythonEnvironment.Enabled && Axis.GlobalScope)
                {
                    note = "<color=#FFFF00><b>Script not enabled in settings menu</b></color>\n" +
                           "Axis code will be run in local scope.";
                }

                if (Axis.Error != null)
                {
                    error = "<color=#FF0000><b>Python error</b></color>\n" +
                            Axis.Error;
                }
            }
        }

        public string GetHelpURL()
        {
            return "https://github.com/lench4991/AdvancedControlsMod/wiki/Custom-Axis";
        }

        public string GetNote()
        {
            return note;
        }

        public string GetError()
        {
            return error;
        }

        private static void InstallIronPython()
        {
            downloading_in_progress = true;
            download_button_text = "0.00 %";
            if (!Directory.Exists(Application.dataPath + @"\Mods\Resources\LenchScripter\lib\"))
                Directory.CreateDirectory(Application.dataPath + @"\Mods\Resources\LenchScripter\lib\");
            try
            {
                for (int file_index = 0; file_index < files_required; file_index++)
                {
                    using (var client = new WebClient())
                    {
                        var i = file_index;

                        // delete existing file
                        if (File.Exists(Application.dataPath + file_paths[i]))
                            File.Delete(Application.dataPath + file_paths[i]);

                        // progress handler
                        client.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
                        {
                            received_size[i] = e.BytesReceived;
                            float progress = (Convert.ToSingle(received_size.Sum()) / Convert.ToSingle(total_size.Sum()) * 100f);
                            download_button_text = progress.ToString("0.00") + " %";
                        };

                        // completion handler
                        client.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                        {
                            if (e.Error != null)
                            {
                                // set error message
                                downloading_in_progress = false;
                                download_button_text = "Error: " + e.Error.GetType().Name;

                                // delete failed file
                                if (File.Exists(Application.dataPath + file_paths[i]))
                                    File.Delete(Application.dataPath + file_paths[i]);
                            }
                            else
                            {
                                spaar.ModLoader.ModConsole.AddMessage(LogType.Log, "File downloaded: " + file_paths[i]);
                                files_downloaded++;
                                if (files_downloaded == files_required)
                                {
                                    // finish download and load assemblies
                                    download_button_text = "Complete";
                                    if (ScripterMod.LoadPythonAssembly())
                                    {
                                        ScripterMod.LoadScripter();
                                        ScripterMod.UpdateCheckerEnabled = false;
                                    }
                                }
                            }
                        };

                        // start download
                        client.DownloadFileAsync(
                            file_uris[i],
                            Application.dataPath + file_paths[i]);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("[ACM]: Error while downloading:");
                Debug.LogException(e);
                downloading_in_progress = false;
                download_button_text = "Error";
            }
        }

        private static int files_downloaded = 0;
        private static int files_required = 5;

        private static long[] received_size = new long[files_required];
        private static long[] total_size = new long[]
        {
            1805824,
            727040,
            1033728,
            142848,
            383488
        };

        private static Uri[] file_uris = new Uri[]
        {
            new Uri("http://lench4991.github.io/AdvancedControlsMod/files/IronPython.dll"),
            new Uri("http://lench4991.github.io/AdvancedControlsMod/files/IronPython.Modules.dll"),
            new Uri("http://lench4991.github.io/AdvancedControlsMod/files/Microsoft.Dynamic.dll"),
            new Uri("http://lench4991.github.io/AdvancedControlsMod/files/Microsoft.Scripting.dll"),
            new Uri("http://lench4991.github.io/AdvancedControlsMod/files/Microsoft.Scripting.Core.dll")
        };
        private static string[] file_paths = new string[]
        {
            @"\Mods\Resources\LenchScripter\lib\IronPython.dll",
            @"\Mods\Resources\LenchScripter\lib\IronPython.Modules.dll",
            @"\Mods\Resources\LenchScripter\lib\Microsoft.Dynamic.dll",
            @"\Mods\Resources\LenchScripter\lib\Microsoft.Scripting.dll",
            @"\Mods\Resources\LenchScripter\lib\Microsoft.Scripting.Core.dll"
        };
    }
}