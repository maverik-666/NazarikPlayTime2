namespace Project
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class MaterialReplacerWindow : EditorWindow
    {
        [MenuItem("Tools/Material Replacer")]
        public static void Open() => GetWindow<MaterialReplacerWindow>("Material Replacer");

        [SerializeField] private Material from;
        [SerializeField] private Material to;

        private void OnGUI()
        {
            EditorGUILayout.Space(4);
            from = (Material)EditorGUILayout.ObjectField("From", from, typeof(Material), false);
            to   = (Material)EditorGUILayout.ObjectField("To",   to,   typeof(Material), false);
            EditorGUILayout.Space(8);

            bool canSwap = from != null && to != null && from != to;
            EditorGUI.BeginDisabledGroup(!canSwap);
            if (GUILayout.Button("Replace All References", GUILayout.Height(32)))
                ReplaceAll();
            EditorGUI.EndDisabledGroup();

            if (!canSwap)
            {
                EditorGUILayout.HelpBox(
                    from == null || to == null
                        ? "Assign both materials."
                        : "Source and target are the same material.",
                    MessageType.Info);
            }
        }

        private void ReplaceAll()
        {
            string fromPath = AssetDatabase.GetAssetPath(from);
            string toPath   = AssetDatabase.GetAssetPath(to);
            string fromGuid = AssetDatabase.AssetPathToGUID(fromPath);
            string toGuid   = AssetDatabase.AssetPathToGUID(toPath);

            if (string.IsNullOrEmpty(fromGuid) || string.IsNullOrEmpty(toGuid))
            {
                EditorUtility.DisplayDialog("Error", "Could not resolve GUIDs.", "OK");
                return;
            }

            string[] extensions = { "*.prefab", "*.unity", "*.asset" };
            string root = Application.dataPath;

            var candidates = new List<string>();
            foreach (string ext in extensions)
                candidates.AddRange(Directory.GetFiles(root, ext, SearchOption.AllDirectories));

            var changed = new List<string>();
            try
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    string file = candidates[i];
                    EditorUtility.DisplayProgressBar(
                        "Material Replacer",
                        Path.GetFileName(file),
                        (float)i / candidates.Count);

                    string content = File.ReadAllText(file);
                    if (!content.Contains(fromGuid)) continue;

                    File.WriteAllText(file, content.Replace(fromGuid, toGuid));
                    changed.Add(file);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            if (changed.Count > 0)
            {
                AssetDatabase.Refresh();
                string list = string.Join("\n", changed.ConvertAll(f =>
                    "• " + f.Replace(root, "Assets").Replace('\\', '/')));
                EditorUtility.DisplayDialog(
                    "Done",
                    $"Replaced in {changed.Count} file(s):\n\n{list}",
                    "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Done", "No references found.", "OK");
            }
        }
    }
}
