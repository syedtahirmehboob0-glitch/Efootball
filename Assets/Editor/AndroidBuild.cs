#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public static class AndroidBuild
{
    public static void Build()
    {
        BuildAPK();
    }

    public static void BuildAPK()
    {
        StreetFootballBootstrap.BuildDemo();
        Directory.CreateDirectory("Builds");
        PlayerSettings.productName = "Pakistan Street Football";
        PlayerSettings.companyName = "Pakistan Street Football Studio";
        PlayerSettings.applicationIdentifier = "com.pakistan.streetfootball";
        PlayerSettings.bundleVersion = "0.1.0";
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel35;
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.development = false;

        var scene = "Assets/Scenes/StreetMatch.unity";
        var opts = new BuildPlayerOptions
        {
            scenes = new[] { scene },
            locationPathName = "Builds/PakistanStreetFootball.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(opts);
        if (report.summary.result != BuildResult.Succeeded)
            throw new System.Exception("Android build failed: " + report.summary.result);

        Debug.Log("APK READY: " + Path.GetFullPath(opts.locationPathName));
    }
}
#endif
