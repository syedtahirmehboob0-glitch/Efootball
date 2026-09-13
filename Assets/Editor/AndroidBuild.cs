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

        PlayerSettings.productName = "Pakistan Street Football";
        PlayerSettings.companyName = "Pakistan Street Football Studio";
        PlayerSettings.applicationIdentifier = "com.pakistan.streetfootball";
        PlayerSettings.bundleVersion = "0.2.0";
        PlayerSettings.Android.bundleVersionCode = 2;

        // Prefer the most conservative Android runtime configuration for this first
        // device-tested build. Mono avoids an IL2CPP startup failure masking the game
        // itself, while OpenGLES3 avoids device-specific Vulkan initialization crashes.
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel35;
        PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.Activity;
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
        PlayerSettings.stripEngineCode = false;
        PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Low);

        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.allowUnsafeCode = false;

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
