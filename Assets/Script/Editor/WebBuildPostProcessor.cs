namespace Project
{
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    public class WebBuildPostProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.WebGL)
                return;

            string indexPath = Path.Combine(report.summary.outputPath, "index.html");

            if (!File.Exists(indexPath))
            {
                Debug.LogWarning("[WebBuildPostProcessor] index.html not found at: " + indexPath);
                return;
            }

            string html = File.ReadAllText(indexPath);
            string original = html;

            // Add overflow:hidden to body
            html = Regex.Replace(
                html,
                @"(<body\b[^>]*?style=""[^""]*?)("")",
                m => m.Groups[1].Value.TrimEnd(';') + "; overflow: hidden;" + m.Groups[2].Value);

            // Replace canvas style: inject width+display, remove any inline height so CSS controls it
            html = Regex.Replace(
                html,
                @"(<canvas\b[^>]*?\bstyle="")([^""]*)("")",
                m =>
                {
                    string style = m.Groups[2].Value;
                    // strip existing width/display/height entries to avoid duplicates
                    style = Regex.Replace(style, @"\b(width|display|height)\s*:[^;]+;?\s*", "").Trim().TrimEnd(';');
                    style = "width: 100%; height: 100vh; display: block; " + style;
                    return m.Groups[1].Value + style + m.Groups[3].Value;
                });

            if (html == original)
                Debug.LogWarning("[WebBuildPostProcessor] No changes made — template may have changed. Check index.html manually.");
            else
            {
                File.WriteAllText(indexPath, html);
                Debug.Log("[WebBuildPostProcessor] Patched index.html for itch.io embed.");
            }
        }
    }
}
