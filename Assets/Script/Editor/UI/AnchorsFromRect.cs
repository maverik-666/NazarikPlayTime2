using UnityEditor;
using UnityEngine;

public static class AnchorsFromRect
{
    [MenuItem("Tools/UI Anchors From Rect/Selected Only", priority = 10)]
    private static void ApplySelected()
    {
        Apply(Selection.gameObjects, recursive: false);
    }

    [MenuItem("Tools/UI Anchors From Rect/Selected + Children", priority = 11)]
    private static void ApplySelectedRecursive()
    {
        Apply(Selection.gameObjects, recursive: true);
    }

    [MenuItem("Tools/UI Anchors From Rect/Selected Only", validate = true)]
    [MenuItem("Tools/UI Anchors From Rect/Selected + Children", validate = true)]
    private static bool ValidateSelection()
    {
        return Selection.activeGameObject != null;
    }

    private static void Apply(GameObject[] roots, bool recursive)
    {
        if (roots == null || roots.Length == 0) 
            return;

        Undo.SetCurrentGroupName("Anchors From Rect");
        int undoGroup = Undo.GetCurrentGroup();

        int count = 0;
        foreach (var go in roots)
        {
            if (recursive)
            {
                foreach (var rt in go.GetComponentsInChildren<RectTransform>(true))
                {
                    if (BakeAnchors(rt))
                    {
                        count++;
                    }
                }
            }
            else
            {
                var rt = go.GetComponent<RectTransform>();
                if (rt != null && BakeAnchors(rt))
                {
                    count++;
                }
            }
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log($"[AnchorsFromRect] Applied to {count} RectTransform(s).");
    }

    private static bool BakeAnchors(RectTransform rt)
    {
        var parent = rt.parent as RectTransform;
        if (parent == null) 
            return false;

        Undo.RecordObject(rt, "Bake Anchors");

        var worldCorners = new Vector3[4];
        rt.GetWorldCorners(worldCorners);

        Vector2 bl = parent.InverseTransformPoint(worldCorners[0]);
        Vector2 tr = parent.InverseTransformPoint(worldCorners[2]);

        var parentRect = parent.rect;

        var newAnchorMin = new Vector2(
            (bl.x - parentRect.xMin) / parentRect.width,
            (bl.y - parentRect.yMin) / parentRect.height
        );
        var newAnchorMax = new Vector2(
            (tr.x - parentRect.xMin) / parentRect.width,
            (tr.y - parentRect.yMin) / parentRect.height
        );

        rt.anchorMin = newAnchorMin;
        rt.anchorMax = newAnchorMax;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;

        return true;
    }
}
