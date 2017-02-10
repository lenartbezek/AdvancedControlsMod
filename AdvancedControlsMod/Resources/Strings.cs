using System;
using System.IO;
using System.Reflection;
using SimpleJSON;
using UnityEngine;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable InconsistentNaming

namespace Lench.AdvancedControls.Resources
{
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    internal class Strings
    {
        private static JSONNode _strings;

        private static bool LoadLanguage(string language)
        {
            // Read from external resource
            var file = $"{Application.dataPath}/Mods/Resources/AdvancedControls/{language}.json";
            if (File.Exists(file))
            {
                try
                {
                    var content = File.ReadAllText(file);
                    _strings = JSON.Parse(content);
                    return true;
                }
                catch
                {
                    // Continue to embedded resource
                }
            }

            // Fallback to embedded resource
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Lench.AdvancedControls.Resources.{language}.json";
            try
            {

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(stream))
                {
                    var content = reader.ReadToEnd();
                    _strings = JSON.Parse(content);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static string GetString(string key)
        {
            try
            {
                return _strings[key].Value;
            }
            catch
            {
                return $"<color=red>{key}</color>";
            }
        }

        /// <summary>
        ///   Overrides the Language property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        internal static string Language {
            get { return _language; }
            set
            {
                if (_language == value) return;

                if (LoadLanguage(value))
                    _language = value;
                else
                    throw new InvalidOperationException($"Language {value} is not supported.");
            }
         }

        private static string _language;

        /// <summary>
        ///   Looks up a localized string similar to Chain Axis.
        /// </summary>
        internal static string AxisEditorWindow_ButtonText_ChainAxis => GetString("AxisEditorWindow_ButtonText_ChainAxis");

        /// <summary>
        ///   Looks up a localized string similar to Controller Axis.
        /// </summary>
        internal static string AxisEditorWindow_ButtonText_ControllerAxis => GetString("AxisEditorWindow_ButtonText_ControllerAxis");

        /// <summary>
        ///   Looks up a localized string similar to Custom Axis.
        /// </summary>
        internal static string AxisEditorWindow_ButtonText_CustomAxis => GetString("AxisEditorWindow_ButtonText_CustomAxis");

        /// <summary>
        ///   Looks up a localized string similar to Key Axis.
        /// </summary>
        internal static string AxisEditorWindow_ButtonText_KeyAxis => GetString("AxisEditorWindow_ButtonText_KeyAxis");

        /// <summary>
        ///   Looks up a localized string similar to Mouse Axis.
        /// </summary>
        internal static string AxisEditorWindow_ButtonText_MouseAxis => GetString("AxisEditorWindow_ButtonText_MouseAxis");

        /// <summary>
        ///   Looks up a localized string similar to new chain axis.
        /// </summary>
        internal static string AxisEditorWindow_DefaultAxisName_NewChainAxis => GetString("AxisEditorWindow_DefaultAxisName_NewChainAxis");

        /// <summary>
        ///   Looks up a localized string similar to new controller axis.
        /// </summary>
        internal static string AxisEditorWindow_DefaultAxisName_NewControllerAxis => GetString("AxisEditorWindow_DefaultAxisName_NewControllerAxis");

        /// <summary>
        ///   Looks up a localized string similar to new custom axis.
        /// </summary>
        internal static string AxisEditorWindow_DefaultAxisName_NewCustomAxis => GetString("AxisEditorWindow_DefaultAxisName_NewCustomAxis");

        /// <summary>
        ///   Looks up a localized string similar to new key axis.
        /// </summary>
        internal static string AxisEditorWindow_DefaultAxisName_NewKeyAxis => GetString("AxisEditorWindow_DefaultAxisName_NewKeyAxis");

        /// <summary>
        ///   Looks up a localized string similar to new mouse axis.
        /// </summary>
        internal static string AxisEditorWindow_DefaultAxisName_NewMouseAxis => GetString("AxisEditorWindow_DefaultAxisName_NewMouseAxis");

        /// <summary>
        ///   Looks up a localized string similar to Create new axis.
        /// </summary>
        internal static string AxisEditorWindow_WindowTitle_CreateNewAxis => GetString("AxisEditorWindow_WindowTitle_CreateNewAxis");

        /// <summary>
        ///   Looks up a localized string similar to Create new chain axis.
        /// </summary>
        internal static string AxisEditorWindow_WindowTitle_CreateNewChainAxis => GetString("AxisEditorWindow_WindowTitle_CreateNewChainAxis");

        /// <summary>
        ///   Looks up a localized string similar to Create new controller axis.
        /// </summary>
        internal static string AxisEditorWindow_WindowTitle_CreateNewControllerAxis => GetString("AxisEditorWindow_WindowTitle_CreateNewControllerAxis");

        /// <summary>
        ///   Looks up a localized string similar to Create new custom axis.
        /// </summary>
        internal static string AxisEditorWindow_WindowTitle_CreateNewCustomAxis => GetString("AxisEditorWindow_WindowTitle_CreateNewCustomAxis");

        /// <summary>
        ///   Looks up a localized string similar to Create new key axis.
        /// </summary>
        internal static string AxisEditorWindow_WindowTitle_CreateNewKeyAxis => GetString("AxisEditorWindow_WindowTitle_CreateNewKeyAxis");

        /// <summary>
        ///   Looks up a localized string similar to Create new mouse axis.
        /// </summary>
        internal static string AxisEditorWindow_WindowTitle_CreateNewMouseAxis => GetString("AxisEditorWindow_WindowTitle_CreateNewMouseAxis");

        /// <summary>
        ///   Looks up a localized string similar to Edit {0}.
        /// </summary>
        internal static string AxisEditorWindow_WindowTitle_Edit => GetString("AxisEditorWindow_WindowTitle_Edit");

        /// <summary>
        ///   Looks up a localized string similar to LOCALLY SAVED AXES.
        /// </summary>
        internal static string AxisSelector_Label_LocallySavedAxes => GetString("AxisSelector_Label_LocallySavedAxes");

        /// <summary>
        ///   Looks up a localized string similar to MACHINE EMBEDDED AXES.
        /// </summary>
        internal static string AxisSelector_Label_MachineEmbeddedAxes => GetString("AxisSelector_Label_MachineEmbeddedAxes");

        /// <summary>
        ///   Looks up a localized string similar to Select axis.
        /// </summary>
        internal static string AxisSelector_WindowTitle_SelectAxis => GetString("AxisSelector_WindowTitle_SelectAxis");

        /// <summary>
        ///   Looks up a localized string similar to ERROR.
        /// </summary>
        internal static string AxisStatus_Error => GetString("AxisStatus_Error");

        /// <summary>
        ///   Looks up a localized string similar to NO LINK.
        /// </summary>
        internal static string AxisStatus_NoLink => GetString("AxisStatus_NoLink");

        /// <summary>
        ///   Looks up a localized string similar to NOT AVAILABLE.
        /// </summary>
        internal static string AxisStatus_NotAvailable => GetString("AxisStatus_NotAvailable");

        /// <summary>
        ///   Looks up a localized string similar to NOT CONNECTED.
        /// </summary>
        internal static string AxisStatus_NotConnected => GetString("AxisStatus_NotConnected");

        /// <summary>
        ///   Looks up a localized string similar to NOT FOUND.
        /// </summary>
        internal static string AxisStatus_NotFound => GetString("AxisStatus_NotFound");

        /// <summary>
        ///   Looks up a localized string similar to NOT RUNNING.
        /// </summary>
        internal static string AxisStatus_NotRunning => GetString("AxisStatus_NotRunning");

        /// <summary>
        ///   Looks up a localized string similar to OK.
        /// </summary>
        internal static string AxisStatus_Ok => GetString("AxisStatus_Ok");

        /// <summary>
        ///   Looks up a localized string similar to UNKNOWN.
        /// </summary>
        internal static string AxisStatus_Unknown => GetString("AxisStatus_Unknown");

        /// <summary>
        ///   Looks up a localized string similar to &gt;.
        /// </summary>
        internal static string ButtonText_ArrowNext => GetString("ButtonText_ArrowNext");

        /// <summary>
        ///   Looks up a localized string similar to &lt;.
        /// </summary>
        internal static string ButtonText_ArrowPrevious => GetString("ButtonText_ArrowPrevious");

        /// <summary>
        ///   Looks up a localized string similar to ×.
        /// </summary>
        internal static string ButtonText_Close => GetString("ButtonText_Close");

        /// <summary>
        ///   Looks up a localized string similar to ✎.
        /// </summary>
        internal static string ButtonText_EditAxis => GetString("ButtonText_EditAxis");

        /// <summary>
        ///   Looks up a localized string similar to ?.
        /// </summary>
        internal static string ButtonText_Help => GetString("ButtonText_Help");

        /// <summary>
        ///   Looks up a localized string similar to Save.
        /// </summary>
        internal static string ButtonText_Save => GetString("ButtonText_Save");

        /// <summary>
        ///   Looks up a localized string similar to Select Input Axis.
        /// </summary>
        internal static string ButtonText_SelectInputAxis => GetString("ButtonText_SelectInputAxis");

        /// <summary>
        ///   Looks up a localized string similar to https://github.com/lench4991/AdvancedControlsMod/wiki/Chain-Axis.
        /// </summary>
        internal static string ChainAxisEditor_HelpURL => GetString("ChainAxisEditor_HelpURL");

        /// <summary>
        ///   Looks up a localized string similar to Chain cycle error.
        /// </summary>
        internal static string ChainAxisEditor_Message_ChainCycleError => GetString("ChainAxisEditor_Message_ChainCycleError");

        /// <summary>
        ///   Looks up a localized string similar to Renaming this axis formed a cycle in the chain.
        ///.
        /// </summary>
        internal static string ChainAxisEditor_Message_ChainCycleErrorDetail => GetString("ChainAxisEditor_Message_ChainCycleErrorDetail");

        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is already in the axis chain.
        ///Linking it here would create a cycle..
        /// </summary>
        internal static string ChainAxisEditor_Message_ChainCycleErrorDetail2 => GetString("ChainAxisEditor_Message_ChainCycleErrorDetail2");

        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; has been unlinked. .
        /// </summary>
        internal static string ChainAxisEditor_Message_ChainCycleErrorEffect => GetString("ChainAxisEditor_Message_ChainCycleErrorEffect");

        /// <summary>
        ///   Looks up a localized string similar to Average.
        /// </summary>
        internal static string ChainMethod_Average => GetString("ChainMethod_Average");

        /// <summary>
        ///   Looks up a localized string similar to Maximum.
        /// </summary>
        internal static string ChainMethod_Maximum => GetString("ChainMethod_Maximum");

        /// <summary>
        ///   Looks up a localized string similar to Minimum.
        /// </summary>
        internal static string ChainMethod_Minimum => GetString("ChainMethod_Minimum");

        /// <summary>
        ///   Looks up a localized string similar to Multiply.
        /// </summary>
        internal static string ChainMethod_Multiply => GetString("ChainMethod_Multiply");

        /// <summary>
        ///   Looks up a localized string similar to Subtract.
        /// </summary>
        internal static string ChainMethod_Subtract => GetString("ChainMethod_Subtract");

        /// <summary>
        ///   Looks up a localized string similar to Sum.
        /// </summary>
        internal static string ChainMethod_Sum => GetString("ChainMethod_Sum");

        /// <summary>
        ///   Looks up a localized string similar to Enter &apos;acm&apos; for all available configuration commands..
        /// </summary>
        internal static string Console_Acm_AllAvailable => GetString("Console_Acm_AllAvailable");

        /// <summary>
        ///   Looks up a localized string similar to Available commands:
        ///  acm modupdate check  	 Checks for mod update.
        ///  acm modupdate enable 	 Enables update checker.
        ///  acm modupdate disable	 Disables update checker.
        ///  acm dbupdate check   	 Checks for controller database update.
        ///  acm dbupdate enable  	 Enables automatic controller database updates.
        ///  acm dbupdate disable 	 Disables automatic controller database updates.
        ///.
        /// </summary>
        internal static string Console_Acm_AllAvailableList => GetString("Console_Acm_AllAvailableList");

        /// <summary>
        ///   Looks up a localized string similar to Checking for controller DB updates ....
        /// </summary>
        internal static string Console_Acm_CheckingForControllerDBUpdates => GetString("Console_Acm_CheckingForControllerDBUpdates");

        /// <summary>
        ///   Looks up a localized string similar to Checking for mod updates ....
        /// </summary>
        internal static string Console_Acm_CheckingForModUpdates => GetString("Console_Acm_CheckingForModUpdates");

        /// <summary>
        ///   Looks up a localized string similar to Controller DB update checker disabled..
        /// </summary>
        internal static string Console_Acm_ControllerDBUpdateCheckerDisabled => GetString("Console_Acm_ControllerDBUpdateCheckerDisabled");

        /// <summary>
        ///   Looks up a localized string similar to Controller DB update checker enabled..
        /// </summary>
        internal static string Console_Acm_ControllerDBUpdateCheckerEnabled => GetString("Console_Acm_ControllerDBUpdateCheckerEnabled");

        /// <summary>
        ///   Looks up a localized string similar to Invalid command. Enter &apos;acm&apos; for all available commands..
        /// </summary>
        internal static string Console_Acm_InvalidCommand => GetString("Console_Acm_InvalidCommand");

        /// <summary>
        ///   Looks up a localized string similar to Mod update checker disabled..
        /// </summary>
        internal static string Console_Acm_ModUpdateCheckerDisabled => GetString("Console_Acm_ModUpdateCheckerDisabled");

        /// <summary>
        ///   Looks up a localized string similar to Mod update checker enabled..
        /// </summary>
        internal static string Console_Acm_ModUpdateCheckerEnabled => GetString("Console_Acm_ModUpdateCheckerEnabled");

        /// <summary>
        ///   Looks up a localized string similar to Invalid argument [check/enable/disable]. Enter &apos;acm&apos; for all available commands..
        /// </summary>
        internal static string Console_Acm_UpdateInvalidArgument => GetString("Console_Acm_UpdateInvalidArgument");

        /// <summary>
        ///   Looks up a localized string similar to Missing argument [check/enable/disable]. Enter &apos;acm&apos; for all available commands..
        /// </summary>
        internal static string Console_Acm_UpdateMissingArgument => GetString("Console_Acm_UpdateMissingArgument");

        /// <summary>
        ///   Looks up a localized string similar to Enter &apos;controller&apos; for all available controller commands..
        /// </summary>
        internal static string Console_Controller_AllAvailable => GetString("Console_Controller_AllAvailable");

        /// <summary>
        ///   Looks up a localized string similar to Available commands:
        ///  controller list		List all connected devices.
        ///  controller info [index]	Show info of a device at index.
        ///.
        /// </summary>
        internal static string Console_Controller_AllAvailableList => GetString("Console_Controller_AllAvailableList");

        /// <summary>
        ///   Looks up a localized string similar to Controller list:
        ///.
        /// </summary>
        internal static string Console_Controller_ControllerList => GetString("Console_Controller_ControllerList");

        /// <summary>
        ///   Looks up a localized string similar to Controller.
        /// </summary>
        internal static string Console_Controller_ControllerTag => GetString("Console_Controller_ControllerTag");

        /// <summary>
        ///   Looks up a localized string similar to GUID: .
        /// </summary>
        internal static string Console_Controller_GUID => GetString("Console_Controller_GUID");

        /// <summary>
        ///   Looks up a localized string similar to Invalid command. Enter &apos;controller&apos; for all available commands..
        /// </summary>
        internal static string Console_Controller_InvalidCommand => GetString("Console_Controller_InvalidCommand");

        /// <summary>
        ///   Looks up a localized string similar to Joystick.
        /// </summary>
        internal static string Console_Controller_JoystickTag => GetString("Console_Controller_JoystickTag");

        /// <summary>
        ///   Looks up a localized string similar to No devices connected..
        /// </summary>
        internal static string Console_Controller_NoDevicesConnected => GetString("Console_Controller_NoDevicesConnected");

        /// <summary>
        ///   Looks up a localized string similar to Axis {0}.
        /// </summary>
        internal static string Controller_AxisName_Default => GetString("Controller_AxisName_Default");

        /// <summary>
        ///   Looks up a localized string similar to Left Trigger.
        /// </summary>
        internal static string Controller_AxisName_LeftTrigger => GetString("Controller_AxisName_LeftTrigger");

        /// <summary>
        ///   Looks up a localized string similar to Left X.
        /// </summary>
        internal static string Controller_AxisName_LeftX => GetString("Controller_AxisName_LeftX");

        /// <summary>
        ///   Looks up a localized string similar to Left Y.
        /// </summary>
        internal static string Controller_AxisName_LeftY => GetString("Controller_AxisName_LeftY");

        /// <summary>
        ///   Looks up a localized string similar to Right Trigger.
        /// </summary>
        internal static string Controller_AxisName_RightTrigger => GetString("Controller_AxisName_RightTrigger");

        /// <summary>
        ///   Looks up a localized string similar to Right X.
        /// </summary>
        internal static string Controller_AxisName_RightX => GetString("Controller_AxisName_RightX");

        /// <summary>
        ///   Looks up a localized string similar to Right Y.
        /// </summary>
        internal static string Controller_AxisName_RightY => GetString("Controller_AxisName_RightY");

        /// <summary>
        ///   Looks up a localized string similar to Unknown axis.
        /// </summary>
        internal static string Controller_AxisName_UnknownAxis => GetString("Controller_AxisName_UnknownAxis");

        /// <summary>
        ///   Looks up a localized string similar to X Axis.
        /// </summary>
        internal static string Controller_AxisName_XAxis => GetString("Controller_AxisName_XAxis");

        /// <summary>
        ///   Looks up a localized string similar to Y Axis.
        /// </summary>
        internal static string Controller_AxisName_YAxis => GetString("Controller_AxisName_YAxis");

        /// <summary>
        ///   Looks up a localized string similar to Ball {0}.
        /// </summary>
        internal static string Controller_BallName_Default => GetString("Controller_BallName_Default");

        /// <summary>
        ///   Looks up a localized string similar to Unknown ball.
        /// </summary>
        internal static string Controller_BallName_UnknownBall => GetString("Controller_BallName_UnknownBall");

        /// <summary>
        ///   Looks up a localized string similar to A Button.
        /// </summary>
        internal static string Controller_ButtonName_AButton => GetString("Controller_ButtonName_AButton");

        /// <summary>
        ///   Looks up a localized string similar to Back Button.
        /// </summary>
        internal static string Controller_ButtonName_BackButton => GetString("Controller_ButtonName_BackButton");

        /// <summary>
        ///   Looks up a localized string similar to B Button.
        /// </summary>
        internal static string Controller_ButtonName_BButton => GetString("Controller_ButtonName_BButton");

        /// <summary>
        ///   Looks up a localized string similar to Button {0}.
        /// </summary>
        internal static string Controller_ButtonName_Default => GetString("Controller_ButtonName_Default");

        /// <summary>
        ///   Looks up a localized string similar to Guide Button.
        /// </summary>
        internal static string Controller_ButtonName_GuideButton => GetString("Controller_ButtonName_GuideButton");

        /// <summary>
        ///   Looks up a localized string similar to Left Shoulder.
        /// </summary>
        internal static string Controller_ButtonName_LeftShoulder => GetString("Controller_ButtonName_LeftShoulder");

        /// <summary>
        ///   Looks up a localized string similar to Left Stick.
        /// </summary>
        internal static string Controller_ButtonName_LeftStick => GetString("Controller_ButtonName_LeftStick");

        /// <summary>
        ///   Looks up a localized string similar to Right Shoulder.
        /// </summary>
        internal static string Controller_ButtonName_RightShoulder => GetString("Controller_ButtonName_RightShoulder");

        /// <summary>
        ///   Looks up a localized string similar to Right Stick.
        /// </summary>
        internal static string Controller_ButtonName_RightStick => GetString("Controller_ButtonName_RightStick");

        /// <summary>
        ///   Looks up a localized string similar to Start Button.
        /// </summary>
        internal static string Controller_ButtonName_StartButton => GetString("Controller_ButtonName_StartButton");

        /// <summary>
        ///   Looks up a localized string similar to Unknown button.
        /// </summary>
        internal static string Controller_ButtonName_UnknownButton => GetString("Controller_ButtonName_UnknownButton");

        /// <summary>
        ///   Looks up a localized string similar to X Button.
        /// </summary>
        internal static string Controller_ButtonName_XButton => GetString("Controller_ButtonName_XButton");

        /// <summary>
        ///   Looks up a localized string similar to Y Button.
        /// </summary>
        internal static string Controller_ButtonName_YButton => GetString("Controller_ButtonName_YButton");

        /// <summary>
        ///   Looks up a localized string similar to Game controller connected:.
        /// </summary>
        internal static string Controller_GameControllerConnected => GetString("Controller_GameControllerConnected");

        /// <summary>
        ///   Looks up a localized string similar to GUID:.
        /// </summary>
        internal static string Controller_GUID => GetString("Controller_GUID");

        /// <summary>
        ///   Looks up a localized string similar to Hat {0}.
        /// </summary>
        internal static string Controller_HatName_Default => GetString("Controller_HatName_Default");

        /// <summary>
        ///   Looks up a localized string similar to DPAD.
        /// </summary>
        internal static string Controller_HatName_DPAD => GetString("Controller_HatName_DPAD");

        /// <summary>
        ///   Looks up a localized string similar to Unknown hat.
        /// </summary>
        internal static string Controller_HatName_UnknownHat => GetString("Controller_HatName_UnknownHat");

        /// <summary>
        ///   Looks up a localized string similar to Joystick connected:.
        /// </summary>
        internal static string Controller_JoystickConnected => GetString("Controller_JoystickConnected");

        /// <summary>
        ///   Looks up a localized string similar to X.
        /// </summary>
        internal static string ControllerAxisEditor_AxisX => GetString("ControllerAxisEditor_AxisX");

        /// <summary>
        ///   Looks up a localized string similar to Y.
        /// </summary>
        internal static string ControllerAxisEditor_AxisY => GetString("ControllerAxisEditor_AxisY");

        /// <summary>
        ///   Looks up a localized string similar to Use another controller.
        /// </summary>
        internal static string ControllerAxisEditor_ButtonText_UseAnotherController => GetString("ControllerAxisEditor_ButtonText_UseAnotherController");

        /// <summary>
        ///   Looks up a localized string similar to Controller axis requires SDL2 library to work.
        ///Press download to install it automatically.
        ///
        ///.
        /// </summary>
        internal static string ControllerAxisEditor_DrawAxis_RequireSDL2Windows => GetString("ControllerAxisEditor_DrawAxis_RequireSDL2Windows");

        /// <summary>
        ///   Looks up a localized string similar to DRAG TO SET OFFSET.
        /// </summary>
        internal static string ControllerAxisEditor_Graph_DragToSetOffset => GetString("ControllerAxisEditor_Graph_DragToSetOffset");

        /// <summary>
        ///   Looks up a localized string similar to RESET OFFSET.
        /// </summary>
        internal static string ControllerAxisEditor_Graph_ResetOffset => GetString("ControllerAxisEditor_Graph_ResetOffset");

        /// <summary>
        ///   Looks up a localized string similar to https://github.com/lench4991/AdvancedControlsMod/wiki/Controller-Axis.
        /// </summary>
        internal static string ControllerAxisEditor_HelpURL => GetString("ControllerAxisEditor_HelpURL");

        /// <summary>
        ///   Looks up a localized string similar to Additional library needed.
        /// </summary>
        internal static string ControllerAxisEditor_Message_AdditionalLibraryNeeded => GetString("ControllerAxisEditor_Message_AdditionalLibraryNeeded");

        /// <summary>
        ///   Looks up a localized string similar to Associated controller not connected..
        /// </summary>
        internal static string ControllerAxisEditor_Message_AssociatedControllerNotConnected => GetString("ControllerAxisEditor_Message_AssociatedControllerNotConnected");

        /// <summary>
        ///   Looks up a localized string similar to The device this axis is bound to is not found.
        ///
        ///&lt;b&gt;Device GUID&lt;/b&gt;
        ///.
        /// </summary>
        internal static string ControllerAxisEditor_Message_AssociatedControllerNotConnectedDetail => GetString("ControllerAxisEditor_Message_AssociatedControllerNotConnectedDetail");

        /// <summary>
        ///   Looks up a localized string similar to No controllers connected..
        /// </summary>
        internal static string ControllerAxisEditor_Message_NoControllersConnected => GetString("ControllerAxisEditor_Message_NoControllersConnected");

        /// <summary>
        ///   Looks up a localized string similar to Connect a joystick or controller to use this axis..
        /// </summary>
        internal static string ControllerAxisEditor_Message_NoControllersConnectedDetail => GetString("ControllerAxisEditor_Message_NoControllersConnectedDetail");

        /// <summary>
        ///   Looks up a localized string similar to Platform.
        /// </summary>
        internal static string ControllerAxisEditor_Message_Platform => GetString("ControllerAxisEditor_Message_Platform");

        /// <summary>
        ///   Looks up a localized string similar to If you are using some other operating system,
        ///download the correct version of the mod..
        /// </summary>
        internal static string ControllerAxisEditor_Message_PlatformCorrect => GetString("ControllerAxisEditor_Message_PlatformCorrect");

        /// <summary>
        ///   Looks up a localized string similar to You are using Linux version of ACM.
        ///.
        /// </summary>
        internal static string ControllerAxisEditor_Message_PlatformLinux => GetString("ControllerAxisEditor_Message_PlatformLinux");

        /// <summary>
        ///   Looks up a localized string similar to You are using OSX version of ACM.
        ///.
        /// </summary>
        internal static string ControllerAxisEditor_Message_PlatformOSX => GetString("ControllerAxisEditor_Message_PlatformOSX");

        /// <summary>
        ///   Looks up a localized string similar to You are using Windows version of ACM.
        ///.
        /// </summary>
        internal static string ControllerAxisEditor_Message_PlatformWindows => GetString("ControllerAxisEditor_Message_PlatformWindows");

        /// <summary>
        ///   Looks up a localized string similar to Controller axis requires SDL2 library to work.
        ///Run the command below to install it.
        ///
        ///.
        /// </summary>
        internal static string ControllerAxisEditor_Message_RequireSDL2Linux => GetString("ControllerAxisEditor_Message_RequireSDL2Linux");

        /// <summary>
        ///   Looks up a localized string similar to Controller axis requires SDL2 library to work.
        ///Download it at the link below.
        ///
        ///.
        /// </summary>
        internal static string ControllerAxisEditor_Message_RequireSDL2OSX => GetString("ControllerAxisEditor_Message_RequireSDL2OSX");

        /// <summary>
        ///   Looks up a localized string similar to OVERVIEW.
        /// </summary>
        internal static string ControlMapper_ButtonText_Overview => GetString("ControlMapper_ButtonText_Overview");

        /// <summary>
        ///   Looks up a localized string similar to This block has no available controls..
        /// </summary>
        internal static string ControlMapper_Message_NoAvailableControls => GetString("ControlMapper_Message_NoAvailableControls");

        /// <summary>
        ///   Looks up a localized string similar to Advanced Controls.
        /// </summary>
        internal static string ControlMapper_WindowTitle_AdvancedControls => GetString("ControlMapper_WindowTitle_AdvancedControls");

        /// <summary>
        ///   Looks up a localized string similar to ANGLE.
        /// </summary>
        internal static string ControlName_Angle => GetString("ControlName_Angle");

        /// <summary>
        ///   Looks up a localized string similar to CONTROL.
        /// </summary>
        internal static string ControlName_Default => GetString("ControlName_Default");

        /// <summary>
        ///   Looks up a localized string similar to INPUT.
        /// </summary>
        internal static string ControlName_Input => GetString("ControlName_Input");

        /// <summary>
        ///   Looks up a localized string similar to POSITION.
        /// </summary>
        internal static string ControlName_Position => GetString("ControlName_Position");

        /// <summary>
        ///   Looks up a localized string similar to https://github.com/lench4991/AdvancedControlsMod/wiki/Sharing.
        /// </summary>
        internal static string ControlOverview_HelpURL => GetString("ControlOverview_HelpURL");

        /// <summary>
        ///   Looks up a localized string similar to &lt;b&gt;{0}&lt;/b&gt; uses no advanced controls..
        /// </summary>
        internal static string ControlOverview_Label_NoControls => GetString("ControlOverview_Label_NoControls");

        /// <summary>
        ///   Looks up a localized string similar to &lt;b&gt;{0}&lt;/b&gt; for {1}.
        /// </summary>
        internal static string ControlOverview_List_Entry => GetString("ControlOverview_List_Entry");

        /// <summary>
        ///   Looks up a localized string similar to To use this machine as intended,
        ///make sure all axes report no problems.
        ///
        ///&lt;b&gt;{0}&lt;/b&gt; uses these input axes:.
        /// </summary>
        internal static string ControlOverview_Message_IntroNote => GetString("ControlOverview_Message_IntroNote");

        /// <summary>
        ///   Looks up a localized string similar to Overview.
        /// </summary>
        internal static string ControlOverview_WindowTitle => GetString("ControlOverview_WindowTitle");

        /// <summary>
        ///   Looks up a localized string similar to START.
        /// </summary>
        internal static string CustomAxisEditor_ButtonText_Start => GetString("CustomAxisEditor_ButtonText_Start");

        /// <summary>
        ///   Looks up a localized string similar to STOP.
        /// </summary>
        internal static string CustomAxisEditor_ButtonText_Stop => GetString("CustomAxisEditor_ButtonText_Stop");

        /// <summary>
        ///   Looks up a localized string similar to File downloaded: .
        /// </summary>
        internal static string CustomAxisEditor_InstallIronPython_FileDownloaded => GetString("CustomAxisEditor_InstallIronPython_FileDownloaded");

        /// <summary>
        ///   Looks up a localized string similar to &lt;b&gt;Additional library needed&lt;/b&gt;
        ///Custom axis runs on IronPython engine.
        ///Press download to install it automatically..
        /// </summary>
        internal static string CustomAxisEditor_Message_MissingIronPython => GetString("CustomAxisEditor_Message_MissingIronPython");

        /// <summary>
        ///   Looks up a localized string similar to Python error.
        /// </summary>
        internal static string CustomAxisEditor_Message_PythonError => GetString("CustomAxisEditor_Message_PythonError");

        /// <summary>
        ///   Looks up a localized string similar to Update code does not return a value..
        /// </summary>
        internal static string CustomAxisEditor_Message_UpdateCodeDoesNotReturnAValue => GetString("CustomAxisEditor_Message_UpdateCodeDoesNotReturnAValue");

        /// <summary>
        ///   Looks up a localized string similar to Update code returns {0}
        ///instead of number..
        /// </summary>
        internal static string CustomAxisEditor_Message_UpdateCodeReturnsWrongType => GetString("CustomAxisEditor_Message_UpdateCodeReturnsWrongType");

        /// <summary>
        ///   Looks up a localized string similar to Complete.
        /// </summary>
        internal static string DownloadButtonText_Complete => GetString("DownloadButtonText_Complete");

        /// <summary>
        ///   Looks up a localized string similar to Download.
        /// </summary>
        internal static string DownloadButtonText_Download => GetString("DownloadButtonText_Download");

        /// <summary>
        ///   Looks up a localized string similar to Error.
        /// </summary>
        internal static string DownloadButtonText_Error => GetString("DownloadButtonText_Error");

        /// <summary>
        ///   Looks up a localized string similar to Please restart Besiege.
        /// </summary>
        internal static string DownloadButtonText_Restart => GetString("DownloadButtonText_Restart");

        /// <summary>
        ///   Looks up a localized string similar to DOWN.
        /// </summary>
        internal static string HatButton_Direction_DOWN => GetString("HatButton_Direction_DOWN");

        /// <summary>
        ///   Looks up a localized string similar to LEFT.
        /// </summary>
        internal static string HatButton_Direction_LEFT => GetString("HatButton_Direction_LEFT");

        /// <summary>
        ///   Looks up a localized string similar to RIGHT.
        /// </summary>
        internal static string HatButton_Direction_RIGHT => GetString("HatButton_Direction_RIGHT");

        /// <summary>
        ///   Looks up a localized string similar to UP.
        /// </summary>
        internal static string HatButton_Direction_UP => GetString("HatButton_Direction_UP");

        /// <summary>
        ///   Looks up a localized string similar to Error while loading a button:.
        /// </summary>
        internal static string KeyAxis_ParseButtonID_ErrorWhileLoadingAButton => GetString("KeyAxis_ParseButtonID_ErrorWhileLoadingAButton");

        /// <summary>
        ///   Looks up a localized string similar to https://github.com/lench4991/AdvancedControlsMod/wiki/Key-Axes.
        /// </summary>
        internal static string KeyAxisEditor_HelpURL => GetString("KeyAxisEditor_HelpURL");

        /// <summary>
        ///   Looks up a localized string similar to Device disconnected.
        /// </summary>
        internal static string KeyAxisEditor_Message_DeviceDisconnected => GetString("KeyAxisEditor_Message_DeviceDisconnected");

        /// <summary>
        ///   Looks up a localized string similar to 
        ///&apos;{0}&apos; is not connected..
        /// </summary>
        internal static string KeyAxisEditor_Message_DeviceDisconnectedDetail => GetString("KeyAxisEditor_Message_DeviceDisconnectedDetail");

        /// <summary>
        ///   Looks up a localized string similar to None.
        /// </summary>
        internal static string KeyAxisEditor_NoBind => GetString("KeyAxisEditor_NoBind");

        /// <summary>
        ///   Looks up a localized string similar to Center.
        /// </summary>
        internal static string Label_Center => GetString("Label_Center");

        /// <summary>
        ///   Looks up a localized string similar to Curvaure.
        /// </summary>
        internal static string Label_Curvaure => GetString("Label_Curvaure");

        /// <summary>
        ///   Looks up a localized string similar to Deadzone.
        /// </summary>
        internal static string Label_Deadzone => GetString("Label_Deadzone");

        /// <summary>
        ///   Looks up a localized string similar to Gravity.
        /// </summary>
        internal static string Label_Gravity => GetString("Label_Gravity");

        /// <summary>
        ///   Looks up a localized string similar to Initialisation code.
        /// </summary>
        internal static string Label_InitialisationCode => GetString("Label_InitialisationCode");

        /// <summary>
        ///   Looks up a localized string similar to Invert.
        /// </summary>
        internal static string Label_Invert => GetString("Label_Invert");

        /// <summary>
        ///   Looks up a localized string similar to Linked axes.
        /// </summary>
        internal static string Label_LinkedAxes => GetString("Label_LinkedAxes");

        /// <summary>
        ///   Looks up a localized string similar to Momentum.
        /// </summary>
        internal static string Label_Momentum => GetString("Label_Momentum");

        /// <summary>
        ///   Looks up a localized string similar to Range.
        /// </summary>
        internal static string Label_Range => GetString("Label_Range");

        /// <summary>
        ///   Looks up a localized string similar to Raw.
        /// </summary>
        internal static string Label_Raw => GetString("Label_Raw");

        /// <summary>
        ///   Looks up a localized string similar to Run in global scope.
        /// </summary>
        internal static string Label_RunInGlobalScope => GetString("Label_RunInGlobalScope");

        /// <summary>
        ///   Looks up a localized string similar to Sensitivity.
        /// </summary>
        internal static string Label_Sensitivity => GetString("Label_Sensitivity");

        /// <summary>
        ///   Looks up a localized string similar to Smooth.
        /// </summary>
        internal static string Label_Smooth => GetString("Label_Smooth");

        /// <summary>
        ///   Looks up a localized string similar to Snap.
        /// </summary>
        internal static string Label_Snap => GetString("Label_Snap");

        /// <summary>
        ///   Looks up a localized string similar to Update code.
        /// </summary>
        internal static string Label_UpdateCode => GetString("Label_UpdateCode");

        /// <summary>
        ///   Looks up a localized string similar to Error loading saved axes:.
        /// </summary>
        internal static string Log_AxisLoadingError => GetString("Log_AxisLoadingError");

        /// <summary>
        ///   Looks up a localized string similar to Error saving axes:.
        /// </summary>
        internal static string Log_AxisSavingError => GetString("Log_AxisSavingError");

        /// <summary>
        ///   Looks up a localized string similar to Error loading machine&apos;s controls:.
        /// </summary>
        internal static string Log_ControlLoadingError => GetString("Log_ControlLoadingError");

        /// <summary>
        ///   Looks up a localized string similar to Error saving machine&apos;s controls..
        /// </summary>
        internal static string Log_ControlSavingError => GetString("Log_ControlSavingError");

        /// <summary>
        ///   Looks up a localized string similar to SDL_GAMECONTROLLERCONFIG environment variable not retrieved..
        /// </summary>
        internal static string Log_EnvVarError => GetString("Log_EnvVarError");

        /// <summary>
        ///   Looks up a localized string similar to Successfully read SDL_GAMECONTROLLERCONFIG environment variable..
        /// </summary>
        internal static string Log_EnvVarRead => GetString("Log_EnvVarRead");

        /// <summary>
        ///   Looks up a localized string similar to SDL_GAMECONTROLLERCONFIG environment variable not set..
        /// </summary>
        internal static string Log_EnvVarSet => GetString("Log_EnvVarSet");

        /// <summary>
        ///   Looks up a localized string similar to Error downloading file:.
        /// </summary>
        internal static string Log_ErrorDownloadingFile => GetString("Log_ErrorDownloadingFile");

        /// <summary>
        ///   Looks up a localized string similar to Game Controller DB downloaded..
        /// </summary>
        internal static string Log_GameControllerDBDownloaded => GetString("Log_GameControllerDBDownloaded");

        /// <summary>
        ///   Looks up a localized string similar to Game Controller DB is up to date..
        /// </summary>
        internal static string Log_GameControllerDBIsUpToDate => GetString("Log_GameControllerDBIsUpToDate");

        /// <summary>
        ///   Looks up a localized string similar to Game Controller DB update successfull..
        /// </summary>
        internal static string Log_GameControllerDBUpdateSuccessfull => GetString("Log_GameControllerDBUpdateSuccessfull");

        /// <summary>
        ///   Looks up a localized string similar to Mod is up to date..
        /// </summary>
        internal static string Log_ModIsUpToDate => GetString("Log_ModIsUpToDate");

        /// <summary>
        ///   Looks up a localized string similar to File downloaded:.
        /// </summary>
        internal static string Log_SuccessDownloadingFile => GetString("Log_SuccessDownloadingFile");

        /// <summary>
        ///   Looks up a localized string similar to Unable to connect..
        /// </summary>
        internal static string Log_UnableToConnect => GetString("Log_UnableToConnect");

        /// <summary>
        ///   Looks up a localized string similar to Update available:.
        /// </summary>
        internal static string Log_UpdateAvailable => GetString("Log_UpdateAvailable");

        /// <summary>
        ///   Looks up a localized string similar to {0} was saved with mod version {1}.
        ///	It may not be compatible with some newer features..
        /// </summary>
        internal static string Log_VersionWarning => GetString("Log_VersionWarning");

        /// <summary>
        ///   Looks up a localized string similar to https://github.com/lench4991/AdvancedControlsMod/wiki/Mouse-axis.
        /// </summary>
        internal static string MouseAxisEditor_HelpURL => GetString("MouseAxisEditor_HelpURL");

        /// <summary>
        ///   Looks up a localized string similar to BUOYANCY.
        /// </summary>
        internal static string SliderName_Buoyancy => GetString("SliderName_Buoyancy");

        /// <summary>
        ///   Looks up a localized string similar to DISTANCE.
        /// </summary>
        internal static string SliderName_Distance => GetString("SliderName_Distance");

        /// <summary>
        ///   Looks up a localized string similar to EXPLOSIVE CHARGE.
        /// </summary>
        internal static string SliderName_ExplosiveCharge => GetString("SliderName_ExplosiveCharge");

        /// <summary>
        ///   Looks up a localized string similar to FLIGHT DURATION.
        /// </summary>
        internal static string SliderName_FlightDuration => GetString("SliderName_FlightDuration");

        /// <summary>
        ///   Looks up a localized string similar to FLYING SPEED.
        /// </summary>
        internal static string SliderName_FlyingSpeed => GetString("SliderName_FlyingSpeed");

        /// <summary>
        ///   Looks up a localized string similar to HEIGHT.
        /// </summary>
        internal static string SliderName_Height => GetString("SliderName_Height");

        /// <summary>
        ///   Looks up a localized string similar to MASS.
        /// </summary>
        internal static string SliderName_Mass => GetString("SliderName_Mass");

        /// <summary>
        ///   Looks up a localized string similar to POWER.
        /// </summary>
        internal static string SliderName_Power => GetString("SliderName_Power");

        /// <summary>
        ///   Looks up a localized string similar to RANGE.
        /// </summary>
        internal static string SliderName_Range => GetString("SliderName_Range");

        /// <summary>
        ///   Looks up a localized string similar to ROTATION.
        /// </summary>
        internal static string SliderName_Rotation => GetString("SliderName_Rotation");

        /// <summary>
        ///   Looks up a localized string similar to ROTATION SPEED.
        /// </summary>
        internal static string SliderName_RotationSpeed => GetString("SliderName_RotationSpeed");

        /// <summary>
        ///   Looks up a localized string similar to SPEED.
        /// </summary>
        internal static string SliderName_Speed => GetString("SliderName_Speed");

        /// <summary>
        ///   Looks up a localized string similar to SPRING.
        /// </summary>
        internal static string SliderName_Spring => GetString("SliderName_Spring");

        /// <summary>
        ///   Looks up a localized string similar to STRENGTH.
        /// </summary>
        internal static string SliderName_Strength => GetString("SliderName_Strength");

        /// <summary>
        ///   Looks up a localized string similar to STRING LENGTH.
        /// </summary>
        internal static string SliderName_StringLength => GetString("SliderName_StringLength");

        /// <summary>
        ///   Looks up a localized string similar to THRUST.
        /// </summary>
        internal static string SliderName_Thrust => GetString("SliderName_Thrust");

        /// <summary>
        ///   Looks up a localized string similar to GitHub release page.
        /// </summary>
        internal static string Updater_GithubReleasePageLink => GetString("Updater_GithubReleasePageLink");

        /// <summary>
        ///   Looks up a localized string similar to New update available.
        /// </summary>
        internal static string Updater_NewUpdateAvailable => GetString("Updater_NewUpdateAvailable");

        /// <summary>
        ///   Looks up a localized string similar to Spiderling forum page.
        /// </summary>
        internal static string Updater_SpiderlingForumLink => GetString("Updater_SpiderlingForumLink");

        /// <summary>
        ///   Looks up a localized string similar to Advanced Controls Mod.
        /// </summary>
        internal static string Updater_WindowTitle => GetString("Updater_WindowTitle");

        /// <summary>
        ///   Looks up a localized string similar to HORIZONTAL.
        /// </summary>
        internal static string VectorControl_AxisHorizontal => GetString("VectorControl_AxisHorizontal");

        /// <summary>
        ///   Looks up a localized string similar to POWER.
        /// </summary>
        internal static string VectorControl_AxisPower => GetString("VectorControl_AxisPower");

        /// <summary>
        ///   Looks up a localized string similar to VERTICAL.
        /// </summary>
        internal static string VectorControl_AxisVertical => GetString("VectorControl_AxisVertical");

        /// <summary>
        ///   Looks up a localized string similar to Language {0} set.
        /// </summary>
        internal static string Console_Acm_LanguageSet => GetString("Console_Acm_LanguageSet");

        /// <summary>
        ///   Looks up a localized string similar to Language {0} not found.
        /// </summary>
        internal static string Console_Acm_LanguageNotFound => GetString("Console_Acm_LanguageNotFound");
    }
}
