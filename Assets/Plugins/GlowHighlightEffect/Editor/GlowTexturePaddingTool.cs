using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GlowTexturePaddingTool : EditorWindow
{
    private List<Sprite> _sprites = new List<Sprite>();
    private int _padding = 25;
    private Vector2 _scroll;

    [MenuItem("Tools/Glow Highlight Preparation")]
    public static void ShowWindow()
    {
        GetWindow<GlowTexturePaddingTool>("Glow Texture Padding");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Glow Texture Padding", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _padding = EditorGUILayout.IntField(
            new GUIContent("Padding (px)", "Transparent pixels added on each side"),
            _padding);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sprites");

        _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.MaxHeight(200));
        for (int i = 0; i < _sprites.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _sprites[i] = (Sprite)EditorGUILayout.ObjectField(_sprites[i], typeof(Sprite), false);
            if (GUILayout.Button("-", GUILayout.Width(24)))
            {
                _sprites.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        // Drop zone
        Rect dropRect = GUILayoutUtility.GetRect(0, 40, GUILayout.ExpandWidth(true));
        GUI.Box(dropRect, "Drop sprites / textures here", EditorStyles.helpBox);
        HandleDrop(dropRect);

        if (GUILayout.Button("+ Add Sprite"))
            _sprites.Add(null);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Adds transparent pixels on all sides and sets Mesh Type to Full Rect. " +
            "Overwrites original files — use version control.",
            MessageType.Info);
        EditorGUILayout.Space();

        bool hasValid = _sprites.Exists(s => s != null);
        EditorGUI.BeginDisabledGroup(!hasValid || _padding <= 0);
        if (GUILayout.Button($"Apply Padding to {_sprites.FindAll(s => s != null).Count} Sprite(s)", GUILayout.Height(32)))
            ApplyAll();
        EditorGUI.EndDisabledGroup();
    }

    private void HandleDrop(Rect dropRect)
    {
        Event evt = Event.current;
        if (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform)
            return;
        if (!dropRect.Contains(evt.mousePosition))
            return;

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        if (evt.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();

            foreach (Object obj in DragAndDrop.objectReferences)
            {
                Sprite sprite = obj as Sprite;
                if (sprite != null)
                {
                    AddUnique(sprite);
                    continue;
                }

                // Texture2D dropped — load its sprite sub-asset
                Texture2D tex = obj as Texture2D;
                if (tex != null)
                {
                    string texPath = AssetDatabase.GetAssetPath(tex);
                    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(texPath);
                    foreach (Object a in assets)
                    {
                        if (a is Sprite s)
                            AddUnique(s);
                    }
                }
            }

            Repaint();
        }

        evt.Use();
    }

    private void AddUnique(Sprite sprite)
    {
        if (!_sprites.Contains(sprite))
            _sprites.Add(sprite);
    }

    private void ApplyAll()
    {
        int success = 0;
        int failed = 0;

        for (int i = 0; i < _sprites.Count; i++)
        {
            if (_sprites[i] == null) continue;

            EditorUtility.DisplayProgressBar("Glow Texture Padding",
                $"Processing {_sprites[i].name}...", (float)i / _sprites.Count);

            if (ApplyToSprite(_sprites[i]))
                success++;
            else
                failed++;
        }

        EditorUtility.ClearProgressBar();

        string msg = $"Done: {success} padded.";
        if (failed > 0) msg += $"\n{failed} skipped (see Console).";
        EditorUtility.DisplayDialog("Glow Texture Padding", msg, "OK");
    }

    private bool ApplyToSprite(Sprite sprite)
    {
        string path = AssetDatabase.GetAssetPath(sprite);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);

        if (importer == null)
        {
            Debug.LogWarning($"[GlowPadding] Could not get TextureImporter for '{sprite.name}', skipped.");
            return false;
        }

        if (importer.spriteImportMode == SpriteImportMode.Multiple)
        {
            Debug.LogWarning($"[GlowPadding] '{sprite.name}' is part of a multi-sprite atlas, skipped.");
            return false;
        }

        bool wasReadable = importer.isReadable;
        if (!wasReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        Texture2D src = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

        int sx = Mathf.RoundToInt(sprite.rect.x);
        int sy = Mathf.RoundToInt(sprite.rect.y);
        int sw = Mathf.RoundToInt(sprite.rect.width);
        int sh = Mathf.RoundToInt(sprite.rect.height);

        Color[] pixels = src.GetPixels(sx, sy, sw, sh);

        int newW = sw + _padding * 2;
        int newH = sh + _padding * 2;

        Texture2D padded = new Texture2D(newW, newH, TextureFormat.RGBA32, false);
        padded.SetPixels(new Color[newW * newH]);
        padded.SetPixels(_padding, _padding, sw, sh, pixels);
        padded.Apply();

        File.WriteAllBytes(path, padded.EncodeToPNG());
        DestroyImmediate(padded);

        Vector2 oldPivot = importer.spritePivot;
        float newPivotX = (_padding + oldPivot.x * sw) / newW;
        float newPivotY = (_padding + oldPivot.y * sh) / newH;

        Vector4 oldBorder = importer.spriteBorder;
        Vector4 newBorder = oldBorder + new Vector4(_padding, _padding, _padding, _padding);

        importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.isReadable = wasReadable;
        importer.spritePivot = new Vector2(newPivotX, newPivotY);
        importer.spriteBorder = newBorder;

        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteAlignment = (int)SpriteAlignment.Custom;
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);

        importer.SaveAndReimport();

        Debug.Log($"[GlowPadding] '{path}' padded to {newW}x{newH} (+{_padding}px each side).");
        return true;
    }
}
