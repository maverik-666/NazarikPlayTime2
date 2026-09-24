using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Editor.PlayMode
{
    /// <summary>
    /// Extends the workaround from UiGraphicSelectionPlayModeGuard to also handle
    /// manual domain reloads (Ctrl+R), which do not trigger playModeStateChanged
    /// or focusChanged events.
    /// 
    /// Temporary workaround for a UnityEditor bug affecting Unity 6.0.0f4 through 6.0.0f6.
    /// Place this file inside an Editor folder.
    /// </summary>
    [InitializeOnLoad]
    public static class UiGraphicSelectionReloadGuard
    {
        static UiGraphicSelectionReloadGuard()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        private static void OnBeforeAssemblyReload()
        {
            TryDeselectGraphicSelection();
        }

        private static void TryDeselectGraphicSelection()
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject == null)
            {
                return;
            }

            if (selectedGameObject.GetComponent<Graphic>() == null)
            {
                return;
            }

            Selection.objects = Array.Empty<UnityEngine.Object>();
        }
    }
}