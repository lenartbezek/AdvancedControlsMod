using UnityEngine;
using Lench.AdvancedControls.Axes;
using spaar.ModLoader.UI;
using System.IO;
using System.Net;
using System;
using System.Linq;
using Lench.AdvancedControls.Resources;

namespace Lench.AdvancedControls.UI
{
    internal class CustomAxisEditor : IAxisEditor
    {
        internal static bool DownloadingInProgress;
        internal static string DownloadButtonText = Strings.DownloadButtonText_Download;

        internal CustomAxisEditor(InputAxis axis)
        {
            _axis = axis as CustomAxis;
        }

        private readonly CustomAxis _axis;

        internal string Note;
        internal string Error;

        private Vector2 _initialisationScrollPosition = Vector2.zero;
        private Vector2 _updateScrollPosition = Vector2.zero;

        public void Open() { }
        public void Close() { }

        public void DrawAxis(Rect windowRect)
        {
            if (!PythonEnvironment.Loaded)
            {
                GUILayout.Label(Strings.CustomAxisEditor_Message_MissingIronPython);
                if (GUILayout.Button(DownloadButtonText) && !DownloadingInProgress)
                    InstallIronPython();
            }
            else
            {
                Error = null;
                Note = null;

                // Draw graph
                GUILayout.BeginHorizontal();

                var text = _axis.Status == AxisStatus.OK 
                    ? _axis.OutputValue.ToString("0.00") 
                    : InputAxis.GetStatusString(_axis.Status);

                GUILayout.Label($"  <color=#808080><b>{text}</b></color>",
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

                if (_axis.Status == AxisStatus.OK)
                    Util.FillRect(new Rect(
                                          graphRect.x + graphRect.width / 2 + graphRect.width / 2 * _axis.OutputValue,
                                          graphRect.y,
                                          1,
                                          graphRect.height),
                                 Color.yellow);

                // Draw start/stop button
                if (GUILayout.Button(_axis.Running ? Strings.CustomAxisEditor_ButtonText_Stop : Strings.CustomAxisEditor_ButtonText_Start, 
                        new GUIStyle(Elements.Buttons.Red) { margin = new RectOffset(8, 8, 0, 0) }, GUILayout.Width(80)))
                    _axis.Running = !_axis.Running;

                GUILayout.EndHorizontal();

                // Draw initialisation code text area
                GUILayout.Label(Strings.Label_InitialisationCode,
                    Util.LabelStyle);

                _initialisationScrollPosition = GUILayout.BeginScrollView(_initialisationScrollPosition,
                    GUILayout.Height(100));
                _axis.InitialisationCode = GUILayout.TextArea(_axis.InitialisationCode);
                GUILayout.EndScrollView();

                // Draw update code text area
                GUILayout.Label(Strings.Label_UpdateCode,
                    Util.LabelStyle);

                _updateScrollPosition = GUILayout.BeginScrollView(_updateScrollPosition,
                    GUILayout.Height(200));
                _axis.UpdateCode = GUILayout.TextArea(_axis.UpdateCode);
                GUILayout.EndScrollView();

                // Draw Global toggle
                GUILayout.BeginHorizontal();

                _axis.GlobalScope = GUILayout.Toggle(_axis.GlobalScope, "",
                    Util.ToggleStyle,
                    GUILayout.Width(20),
                    GUILayout.Height(20));

                GUILayout.Label(Strings.Label_RunInGlobalScope,
                    new GUIStyle(Elements.Labels.Default) { margin = new RectOffset(0, 0, 14, 0) });

                GUILayout.EndHorizontal();

                GUILayout.Box(GUIContent.none, GUILayout.Height(8));

                // Display linked axes
                if (_axis.LinkedAxes.Count > 0)
                {
                    GUILayout.Label(Strings.Label_LinkedAxes, new GUIStyle(Elements.Labels.Title) { alignment = TextAnchor.MiddleCenter });
                    foreach (var name in _axis.LinkedAxes)
                        GUILayout.Label(name, new GUIStyle(Elements.Labels.Default) { alignment = TextAnchor.MiddleCenter });
                }

                // Display error
                if (_axis.Error != null)
                {
                    Error = $"<color=#FF0000><b>{Strings.CustomAxisEditor_Message_PythonError}</b></color>\n" +
                            _axis.Error;
                }
            }
        }

        public string GetHelpURL()
        {
            return "https://github.com/lench4991/AdvancedControlsMod/wiki/Custom-Axis";
        }

        public string GetNote()
        {
            return Note;
        }

        public string GetError()
        {
            return Error;
        }

        private static void InstallIronPython()
        {
            if (PythonEnvironment.LoadPythonAssembly())
            {
                DownloadButtonText = Strings.DownloadButtonText_Complete;
                return;
            }

            DownloadingInProgress = true;
            DownloadButtonText = "0.0 %";
            if (!Directory.Exists(PythonEnvironment.LibPath))
                Directory.CreateDirectory(PythonEnvironment.LibPath);
            try
            {
                for (int fileIndex = 0; fileIndex < FilesRequired; fileIndex++)
                {
                    using (var client = new WebClient())
                    {
                        var i = fileIndex;

                        // delete existing file
                        if (File.Exists(PythonEnvironment.LibPath + FileNames[i]))
                            File.Delete(PythonEnvironment.LibPath + FileNames[i]);

                        // progress handler
                        client.DownloadProgressChanged += (sender, e) =>
                        {
                            ReceivedSize[i] = e.BytesReceived;
                            float progress = (Convert.ToSingle(ReceivedSize.Sum()) / Convert.ToSingle(TotalSize.Sum()) * 100f);
                            DownloadButtonText = progress.ToString("0.0") + " %";
                        };

                        // completion handler
                        client.DownloadFileCompleted += (sender, e) =>
                        {
                            if (e.Error != null)
                            {
                                // set error messages
                                spaar.ModLoader.ModConsole.AddMessage(LogType.Log, $"[ACM]: {Strings.Log_ErrorDownloadingFile}" + FileNames[i]);
                                spaar.ModLoader.ModConsole.AddMessage(LogType.Error, "\t" + e.Error.Message);

                                DownloadingInProgress = false;
                                DownloadButtonText = Strings.DownloadButtonText_Error;

                                // delete failed file
                                if (File.Exists(PythonEnvironment.LibPath + FileNames[i]))
                                    File.Delete(PythonEnvironment.LibPath + FileNames[i]);
                            }
                            else
                            {
                                spaar.ModLoader.ModConsole.AddMessage(LogType.Log, $"[ACM]: {Strings.CustomAxisEditor_InstallIronPython_FileDownloaded}" + FileNames[i]);
                                _filesDownloaded++;
                                if (_filesDownloaded == FilesRequired)
                                {
                                    // finish download and load assemblies
                                    DownloadButtonText = PythonEnvironment.LoadPythonAssembly() 
                                        ? Strings.DownloadButtonText_Complete 
                                        : Strings.DownloadButtonText_Error;
                                }
                            }
                        };

                        // start download
                        client.DownloadFileAsync(
                            new Uri(BaseUri + FileNames[i]),
                            PythonEnvironment.LibPath + FileNames[i]);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log($"[ACM]: {Strings.Log_ErrorDownloadingFile}");
                Debug.LogException(e);
                DownloadingInProgress = false;
                DownloadButtonText = Strings.DownloadButtonText_Error;
            }
        }

        private static int _filesDownloaded;
        private const int FilesRequired = 5;

        private static readonly long[] ReceivedSize = new long[FilesRequired];
        private static readonly long[] TotalSize = {
            1805824,
            727040,
            1033728,
            142848,
            383488
        };

        private static readonly string BaseUri = "http://lench4991.github.io/AdvancedControlsMod/files/ironpython2.7";
        private static readonly string[] FileNames = {
            "IronPython.dll",
            "IronPython.Modules.dll",
            "Microsoft.Dynamic.dll",
            "Microsoft.Scripting.dll",
            "Microsoft.Scripting.Core.dll"
        };
    }
}
