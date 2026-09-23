namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class AutoExpander : MonoBehaviour
    {
        [SerializeField]
        private bool _isAutoExpanded = true;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_isAutoExpanded) return;

            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null) return;

                var type = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
                var windows = Resources.FindObjectsOfTypeAll(type);

                if (windows.Length > 0)
                {
                    var window = windows[0] as UnityEditor.EditorWindow;
                    var sceneHierarchyField = type.GetField("m_SceneHierarchy", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    var sceneHierarchy = sceneHierarchyField?.GetValue(window);

                    if (sceneHierarchy != null)
                    {
                        var method = sceneHierarchy.GetType().GetMethod("ExpandTreeViewItem", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        method?.Invoke(sceneHierarchy, new object[] { gameObject.GetEntityId(), true });
                    }
                }
            };
        }
#endif
    }
}