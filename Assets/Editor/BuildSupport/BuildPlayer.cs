using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public static class BuildPlayer
{
    private const string OutputPathStreamingAssets = "StreamingAssets/bb";
    private const string SplashScenePath = "Assets/GameTemplate/Scenes/Splash.unity";

    private static void IncreaseVersionNumbers()
    {
        PlayerSettings.bundleVersion = IncreaseVersion(PlayerSettings.bundleVersion);
        PlayerSettings.Android.bundleVersionCode++;
        PlayerSettings.iOS.buildNumber = IncreaseVersion(PlayerSettings.iOS.buildNumber);
    }

    private static string IncreaseVersion(string version)
    {
        string[] parts = version.Split('.');
        int lastPart = int.Parse(parts[parts.Length - 1]);
        lastPart++;
        parts[parts.Length - 1] = lastPart.ToString();
        return string.Join(".", parts);
    }

    private static void ClearBuildFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true);
        }
    }

    private static void ShowOutputFolder(string outputPath)
    {
#if UNITY_EDITOR_WIN
        EditorUtility.RevealInFinder(outputPath);
#else // MACOS
        EditorUtility.RevealInFinder(outputPath.Replace("/", "\\"));
#endif
    }

    private static void CopyDirectory(string sourceDir, string destDir)
    {
        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        string[] files = Directory.GetFiles(sourceDir);
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destDir, fileName);
            File.Copy(file, destFile, true);
        }

        string[] dirs = Directory.GetDirectories(sourceDir);
        foreach (string dir in dirs)
        {
            string dirName = Path.GetFileName(dir);
            string destSubDir = Path.Combine(destDir, dirName);
            CopyDirectory(dir, destSubDir);
        }
    }

    private static string GetBuildPathAddressable()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        string buildPath = "";
        if (settings.BuildRemoteCatalog)
        {
            buildPath = settings.RemoteCatalogBuildPath.GetValue(settings);
            ClearBuildFolder(buildPath);
        }

        return buildPath;
    }

    private static void BuildWeb(string outputPath, BuildTarget target, BuildOptions options = BuildOptions.None)
    {
        string[] scenes = {
            SplashScenePath,
        };

        PlayerSettings.SplashScreen.show = false;

        BuildPipeline.BuildPlayer(scenes, outputPath, target, options);
    }

    private static void BuildMobile(string outputPath, BuildTarget target, BuildTargetGroup targetGroup, BuildOptions options = BuildOptions.None | BuildOptions.CompressWithLz4HC)
    {
        string[] scenes = {
            SplashScenePath,
        };

        PlayerSettings.SplashScreen.show = false;

        BuildPlayerOptions buildOptions = new()
        {
            scenes = scenes,
            locationPathName = outputPath,
            targetGroup = targetGroup,
            target = target,
            options = options
        };

        BuildPipeline.BuildPlayer(buildOptions);
    }

    private static void PerformBuild(BuildTarget target, BuildTargetGroup targetGroup = BuildTargetGroup.WebGL, BuildOptions options = BuildOptions.None)
    {
        //string outputPath = networkMode == NetworkManager.Mode.LOCAL ? EditorUtility.SaveFolderPanel("Select Output Folder", "", "") : GetOutputPath(networkMode, target);
        string outputPath = GetOutputPath(target);

        if (string.IsNullOrEmpty(outputPath))
            return;

        ClearBuildFolder(outputPath);
        IncreaseVersionNumbers();

        if (targetGroup == BuildTargetGroup.WebGL)
        {
            BuildWeb(outputPath, target);
            CopyDirectory("ServerData/WebGL", Path.Combine(outputPath, OutputPathStreamingAssets, target.ToString()));
        }
        else if (targetGroup == BuildTargetGroup.Android)
        {
            BuildMobile(outputPath, target, targetGroup, BuildOptions.Development);
        }

        ShowOutputFolder(outputPath);
    }

    private static string GetOutputPath(BuildTarget target)
    {
        string outputPath = $"Build/{target}/GameTemplate";

        return outputPath;
    }

    [MenuItem("GameTemplate/Auto Build WebGL LOCAL #B")]
    public static void AutoBuildWebGL_LOCAL()
    {
        BuildAddressable.BuildAddressableWebBuildIn();

        BuildTarget target = BuildTarget.WebGL;
        string outputPath = GetOutputPath(target);
        BuildWeb(outputPath, target);
        ShowOutputFolder(outputPath);
    }
}