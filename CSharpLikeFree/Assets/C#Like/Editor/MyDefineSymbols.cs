/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System.Collections.Generic;
using UnityEditor;

namespace CSharpLikeEditor
{
    public class MyDefineSymbols
    {
        /// <summary>
        /// Get all script define symbols which can use in hot update script.
        /// I want the function like ScriptCompilationSettings.extraScriptingDefines(https://docs.unity3d.com/ScriptReference/Build.Player.ScriptCompilationSettings-extraScriptingDefines.html),but didn't find how to use it.
        /// So I write this function here.
        /// "UNITY_EDITOR/UNITY_EDITOR_WIN/UNITY_EDITOR_OSX/UNITY_EDITOR_LINUX" not exist here.
        /// https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
        /// </summary>
        public static List<string> GetAllScriptDefineSymbols()
        {
            List<string> defines = GetBuiltInScriptDefineSymbols();//include the define symbols in "Edit" -> "Project Settings" -> "Player" -> "Scripting Define Symbols"
            //add your custom define symbols here,
            //but we recommend set it in UnityEditor "Edit" -> "Project Settings" -> "Player" -> "Scripting Define Symbols".
            //you can modify the GetBuiltInScriptDefineSymbols too.


            //remove the duplicate define symbols(make sure all define symbols is unique)
            Dictionary<string, int> temps = new Dictionary<string, int>();
            foreach (string define in defines)
            {
                if (!temps.ContainsKey(define))
                    temps.Add(define, 0);
            }
            //force remove "UNITY_EDITOR/UNITY_EDITOR_WIN/UNITY_EDITOR_OSX/UNITY_EDITOR_LINUX" symbols,because it should not exist while compiling the hot update script.
            temps.Remove("UNITY_EDITOR");
            temps.Remove("UNITY_EDITOR_WIN");
            temps.Remove("UNITY_EDITOR_OSX");
            temps.Remove("UNITY_EDITOR_LINUX");
            defines.Clear();
            foreach (string define in temps.Keys)
                defines.Add(define);
            return defines;
        }
        static List<string> GetBuiltInScriptDefineSymbols()
        {
            List<string> defines = new List<string>();
            //Platform scripting symbols
#if UNITY_STANDALONE
            defines.Add("UNITY_STANDALONE");
#endif
#if UNITY_STANDALONE_OSX
            defines.Add("UNITY_STANDALONE_OSX");
#endif
#if UNITY_STANDALONE_WIN
            defines.Add("UNITY_STANDALONE_WIN");
#endif
#if UNITY_STANDALONE_LINUX
            defines.Add("UNITY_STANDALONE_LINUX");
#endif
#if UNITY_WII
            defines.Add("UNITY_WII");
#endif
#if UNITY_IOS
            defines.Add("UNITY_IOS");
#endif
#if UNITY_IPHONE
            defines.Add("UNITY_IPHONE");
#endif
#if UNITY_ANDROID
            defines.Add("UNITY_ANDROID");
#endif
#if UNITY_PS4
            defines.Add("UNITY_PS4");
#endif
#if UNITY_SAMSUNGTV
            defines.Add("UNITY_SAMSUNGTV");
#endif
#if UNITY_XBOXONE
            defines.Add("UNITY_XBOXONE");
#endif
#if UNITY_FACEBOOK
            defines.Add("UNITY_FACEBOOK");
#endif
#if PLATFORM_LUMIN
            defines.Add("PLATFORM_LUMIN");
#endif
#if UNITY_LUMIN
            defines.Add("UNITY_LUMIN");
#endif
#if UNITY_TIZEN
            defines.Add("UNITY_TIZEN");
#endif
#if UNITY_TVOS
            defines.Add("UNITY_TVOS");
#endif
#if UNITY_WP_8_1
            defines.Add("UNITY_WP_8_1");
#endif
#if UNITY_WSA
            defines.Add("UNITY_WSA");
#endif
#if UNITY_WSA_8_1
            defines.Add("UNITY_WSA_8_1");
#endif
#if UNITY_WSA_10_0
            defines.Add("UNITY_WSA_10_0");
#endif
#if UNITY_WINRT
            defines.Add("UNITY_WINRT");
#endif
#if UNITY_WINRT_8_1
            defines.Add("UNITY_WINRT_8_1");
#endif
#if UNITY_WINRT_10_0
            defines.Add("UNITY_WINRT_10_0");
#endif
#if UNITY_ANALYTICS
            defines.Add("UNITY_ANALYTICS");
#endif
#if UNITY_ADS
            defines.Add("UNITY_ADS");
#endif
#if UNITY_WEBGL
            defines.Add("UNITY_WEBGL");
#endif
#if UNITY_ASSERTIONS
            defines.Add("UNITY_ASSERTIONS");
#endif
#if UNITY_64
            defines.Add("UNITY_64");
#endif

            //Editor version Scripting symbols
            /// Given a version number X.Y.Z (for example, 2019.4.14), Unity exposes three global scripting symbols in the following formats: UNITY_X, UNITY_X_Y.
            /// Here is a sample of scripting symbols exposed in Unity 2019.4.14:
            /// UNITY_2019	    Scripting symbol for the release version of Unity 2019, exposed in every 2019.Y.Z release.
            /// UNITY_2019_4	Scripting symbol for the release version of Unity 2019.4, exposed in every 2019.4.Z release.
            /// Version number just give the X.Y,not the X.Y.Z,because is to much defines and I'm lazy.
            /// You can add here for yourself if you need it.
#if UNITY_5
            defines.Add("UNITY_5");
#if UNITY_5_0
            defines.Add("UNITY_5_0");
#elif UNITY_5_1
            defines.Add("UNITY_5_1");
#elif UNITY_5_2
            defines.Add("UNITY_5_2");
#elif UNITY_5_3
            defines.Add("UNITY_5_3");
#elif UNITY_5_4
            defines.Add("UNITY_5_4");
#elif UNITY_5_5
            defines.Add("UNITY_5_5");
#elif UNITY_5_6
            defines.Add("UNITY_5_6");
#endif
#elif UNITY_2017
            defines.Add("UNITY_2017");
#if UNITY_2017_1
            defines.Add("UNITY_2017_1");
#elif UNITY_2017_2
            defines.Add("UNITY_2017_2");
#elif UNITY_2017_3
            defines.Add("UNITY_2017_3");
#elif UNITY_2017_4
            defines.Add("UNITY_2017_4");
#endif
#elif UNITY_2018
            defines.Add("UNITY_2018");
#if UNITY_2018_1
            defines.Add("UNITY_2018_1");
#elif UNITY_2018_2
            defines.Add("UNITY_2018_2");
#elif UNITY_2018_3
            defines.Add("UNITY_2018_3");
#elif UNITY_2018_4
            defines.Add("UNITY_2018_4");
#endif
#elif UNITY_2019
            defines.Add("UNITY_2019");
#if UNITY_2019_1
            defines.Add("UNITY_2019_1");
#elif UNITY_2019_2
            defines.Add("UNITY_2019_2");
#elif UNITY_2019_3
            defines.Add("UNITY_2019_3");
#elif UNITY_2019_4
            defines.Add("UNITY_2019_4");
#endif
#elif UNITY_2020
            defines.Add("UNITY_2020");
#if UNITY_2020_1
            defines.Add("UNITY_2020_1");
#elif UNITY_2020_2
            defines.Add("UNITY_2020_2");
#elif UNITY_2020_3
            defines.Add("UNITY_2020_3");
#elif UNITY_2020_4
            defines.Add("UNITY_2020_4");
#endif
#elif UNITY_2021
            defines.Add("UNITY_2021");
#if UNITY_2021_1
            defines.Add("UNITY_2021_1");
#elif UNITY_2021_2
            defines.Add("UNITY_2021_2");
#elif UNITY_2021_3
            defines.Add("UNITY_2021_3");
#elif UNITY_2021_4
            defines.Add("UNITY_2021_4");
#endif
#elif UNITY_2022
            defines.Add("UNITY_2022");
#if UNITY_2022_1
            defines.Add("UNITY_2022_1");
#elif UNITY_2022_2
            defines.Add("UNITY_2022_2");
#elif UNITY_2022_3
            defines.Add("UNITY_2022_3");
#elif UNITY_2022_4
            defines.Add("UNITY_2022_4");
#endif
#elif UNITY_2023
            defines.Add("UNITY_2023");
#if UNITY_2023_1
            defines.Add("UNITY_2023_1");
#elif UNITY_2023_2
            defines.Add("UNITY_2023_2");
#elif UNITY_2023_3
            defines.Add("UNITY_2023_3");
#elif UNITY_2023_4
            defines.Add("UNITY_2023_4");
#endif
#elif UNITY_2024
            defines.Add("UNITY_2024");
#if UNITY_2024_1
            defines.Add("UNITY_2024_1");
#elif UNITY_2024_2
            defines.Add("UNITY_2024_2");
#elif UNITY_2024_3
            defines.Add("UNITY_2024_3");
#elif UNITY_2024_4
            defines.Add("UNITY_2024_4");
#endif
#elif UNITY_2025
            defines.Add("UNITY_2025");
#if UNITY_2025_1
            defines.Add("UNITY_2025_1");
#elif UNITY_2025_2
            defines.Add("UNITY_2025_2");
#elif UNITY_2025_3
            defines.Add("UNITY_2025_3");
#elif UNITY_2025_4
            defines.Add("UNITY_2025_4");
#endif
#endif

#if UNITY_5_1_OR_NEWER
            defines.Add("UNITY_5_1_OR_NEWER");
#endif
#if UNITY_5_2_OR_NEWER
            defines.Add("UNITY_5_2_OR_NEWER");
#endif
#if UNITY_5_3_OR_NEWER
            defines.Add("UNITY_5_3_OR_NEWER");
#endif
#if UNITY_5_4_OR_NEWER
            defines.Add("UNITY_5_4_OR_NEWER");
#endif
#if UNITY_5_5_OR_NEWER
            defines.Add("UNITY_5_5_OR_NEWER");
#endif
#if UNITY_5_6_OR_NEWER
            defines.Add("UNITY_5_6_OR_NEWER");
#endif
#if UNITY_2017_1_OR_NEWER
            defines.Add("UNITY_2017_1_OR_NEWER");
#endif
#if UNITY_2017_2_OR_NEWER
            defines.Add("UNITY_2017_2_OR_NEWER");
#endif
#if UNITY_2017_3_OR_NEWER
            defines.Add("UNITY_2017_3_OR_NEWER");
#endif
#if UNITY_2017_4_OR_NEWER
            defines.Add("UNITY_2017_4_OR_NEWER");
#endif
#if UNITY_2018_1_OR_NEWER
            defines.Add("UNITY_2018_1_OR_NEWER");
#endif
#if UNITY_2018_2_OR_NEWER
            defines.Add("UNITY_2018_2_OR_NEWER");
#endif
#if UNITY_2018_3_OR_NEWER
            defines.Add("UNITY_2018_3_OR_NEWER");
#endif
#if UNITY_2018_4_OR_NEWER
            defines.Add("UNITY_2018_4_OR_NEWER");
#endif
#if UNITY_2019_1_OR_NEWER
            defines.Add("UNITY_2019_1_OR_NEWER");
#endif
#if UNITY_2019_2_OR_NEWER
            defines.Add("UNITY_2019_2_OR_NEWER");
#endif
#if UNITY_2019_3_OR_NEWER
            defines.Add("UNITY_2019_3_OR_NEWER");
#endif
#if UNITY_2019_4_OR_NEWER
            defines.Add("UNITY_2019_4_OR_NEWER");
#endif
#if UNITY_2020_1_OR_NEWER
            defines.Add("UNITY_2020_1_OR_NEWER");
#endif
#if UNITY_2020_2_OR_NEWER
            defines.Add("UNITY_2020_2_OR_NEWER");
#endif
#if UNITY_2020_3_OR_NEWER
            defines.Add("UNITY_2020_3_OR_NEWER");
#endif
#if UNITY_2020_4_OR_NEWER
            defines.Add("UNITY_2020_4_OR_NEWER");
#endif
#if UNITY_2021_1_OR_NEWER
            defines.Add("UNITY_2021_1_OR_NEWER");
#endif
#if UNITY_2021_2_OR_NEWER
            defines.Add("UNITY_2021_2_OR_NEWER");
#endif
#if UNITY_2021_3_OR_NEWER
            defines.Add("UNITY_2021_3_OR_NEWER");
#endif
#if UNITY_2021_4_OR_NEWER
            defines.Add("UNITY_2021_4_OR_NEWER");
#endif
#if UNITY_2022_1_OR_NEWER
            defines.Add("UNITY_2022_1_OR_NEWER");
#endif
#if UNITY_2022_2_OR_NEWER
            defines.Add("UNITY_2022_2_OR_NEWER");
#endif
#if UNITY_2022_3_OR_NEWER
            defines.Add("UNITY_2022_3_OR_NEWER");
#endif
#if UNITY_2022_4_OR_NEWER
            defines.Add("UNITY_2022_4_OR_NEWER");
#endif
#if UNITY_2023_1_OR_NEWER
            defines.Add("UNITY_2023_1_OR_NEWER");
#endif
#if UNITY_2023_2_OR_NEWER
            defines.Add("UNITY_2023_2_OR_NEWER");
#endif
#if UNITY_2023_3_OR_NEWER
            defines.Add("UNITY_2023_3_OR_NEWER");
#endif
#if UNITY_2023_4_OR_NEWER
            defines.Add("UNITY_2023_4_OR_NEWER");
#endif
#if UNITY_2024_1_OR_NEWER
            defines.Add("UNITY_2024_1_OR_NEWER");
#endif
#if UNITY_2024_2_OR_NEWER
            defines.Add("UNITY_2024_2_OR_NEWER");
#endif
#if UNITY_2024_3_OR_NEWER
            defines.Add("UNITY_2024_3_OR_NEWER");
#endif
#if UNITY_2024_4_OR_NEWER
            defines.Add("UNITY_2024_4_OR_NEWER");
#endif
#if UNITY_2025_1_OR_NEWER
            defines.Add("UNITY_2025_1_OR_NEWER");
#endif
#if UNITY_2025_2_OR_NEWER
            defines.Add("UNITY_2025_2_OR_NEWER");
#endif
#if UNITY_2025_3_OR_NEWER
            defines.Add("UNITY_2025_3_OR_NEWER");
#endif
#if UNITY_2025_4_OR_NEWER
            defines.Add("UNITY_2025_4_OR_NEWER");
#endif

            //Other scripting symbols
#if CSHARP_7_3_OR_NEWER
            defines.Add("CSHARP_7_3_OR_NEWER");
#endif
#if ENABLE_MONO
            defines.Add("ENABLE_MONO");
#endif
#if ENABLE_MONO
            defines.Add("ENABLE_IL2CPP");
#endif
#if ENABLE_DOTNET
            defines.Add("ENABLE_DOTNET");
#endif
#if NET_2_0
            defines.Add("NET_2_0");
#endif
#if NET_2_0_SUBSET
            defines.Add("NET_2_0_SUBSET");
#endif
#if NET_LEGACY
            defines.Add("NET_LEGACY");
#endif
#if NET_4_6
            defines.Add("NET_4_6");
#endif
#if NET_STANDARD_2_0
            defines.Add("NET_STANDARD_2_0");
#endif
#if ENABLE_WINMD_SUPPORT
            defines.Add("ENABLE_WINMD_SUPPORT");
#endif
#if ENABLE_INPUT_SYSTEM
            defines.Add("ENABLE_INPUT_SYSTEM");
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            defines.Add("ENABLE_LEGACY_INPUT_MANAGER");
#endif
#if UNITY_SERVER
            defines.Add("UNITY_SERVER");
#endif
#if DEVELOPMENT_BUILD
            defines.Add("DEVELOPMENT_BUILD");
#endif

            //read from player settings
            string[] playerSettings;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out playerSettings);
            foreach (string define in playerSettings)
                defines.Add(define);

            return defines;
        }
    }
}

