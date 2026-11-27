using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Futsal27.EditorTools
{
    /// <summary>Gera o APK (menu Futsal27 ou linha de comando / GitHub Actions).</summary>
    public static class BuildScript
    {
        [MenuItem("Futsal27/2 - Gerar APK Android")]
        public static void BuildAndroid()
        {
            ProjectSetup.Configure();

            string output = Path.Combine("build", "Android", "Futsal27.apk");
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "-customBuildPath")
                {
                    string p = args[i + 1];
                    output = p.EndsWith(".apk", StringComparison.OrdinalIgnoreCase) ? p : Path.Combine(p, "Futsal27.apk");
                }
            }
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ProjectSetup.ScenePath },
                locationPathName = output,
                target = BuildTarget.Android,
                options = BuildOptions.None
            });

            Debug.Log("Resultado do build: " + report.summary.result + " -> " + output);
            if (report.summary.result != BuildResult.Succeeded && Application.isBatchMode)
                EditorApplication.Exit(1);
        }
    }
}
