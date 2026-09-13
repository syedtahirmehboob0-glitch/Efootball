#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
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

        // Explicitly activate Android before changing Android-specific PlayerSettings.
        // Unity 6 can otherwise keep the Android architecture at None even when the
        // targetArchitectures property is assigned while another platform is active.
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            throw new System.Exception("Could not activate Android build target.");

        PlayerSettings.productName = "Pakistan Street Football";
        PlayerSettings.companyName = "Pakistan Street Football Studio";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.pakistan.streetfootball");
        PlayerSettings.bundleVersion = "0.2.2";
        PlayerSettings.Android.bundleVersionCode = 4;

        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        if (PlayerSettings.Android.targetArchitectures == AndroidArchitecture.None)
            throw new System.Exception("Android ARM64 architecture was not applied.");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel35;
        PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.Activity;
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
        PlayerSettings.stripEngineCode = false;
        PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Low);

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
