namespace Project
{
    using UnityEditor;

    public class ScriptCreator
    {
        [MenuItem(itemName: "Assets/Create/Namespaced Class", isValidateFunction: false, priority: -1000)]
        public static void CreateNamespacedClass()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Script/Editor/ScriptCreator/NamespacedClass.cs.txt", "NameablePoco.cs");
        }

        [MenuItem(itemName: "Assets/Create/Namespaced Editor", isValidateFunction: false, priority: -1000)]
        public static void CreateNamespacedEditor()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Script/Editor/ScriptCreator/NamespacedEditor.cs.txt", "NameableEditor.cs");
        }

        [MenuItem(itemName: "Assets/Create/Namespaced Interface", isValidateFunction: false, priority: -1000)]
        public static void CreateNamespacedInterface()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Script/Editor/ScriptCreator/NamespacedInterface.cs.txt", "INameable.cs");
        }

        [MenuItem(itemName: "Assets/Create/Namespaced MoBe", isValidateFunction: false, priority: -1000)]
        public static void CreateNamespacedMoBe()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Script/Editor/ScriptCreator/NamespacedMoBe.cs.txt", "NameableMoBe.cs");
        }

        [MenuItem(itemName: "Assets/Create/Namespaced ScOb", isValidateFunction: false, priority: -1000)]
        public static void CreateNamespacedScOb()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Script/Editor/ScriptCreator/NamespacedScOb.cs.txt", "NameableScOb.cs");
        }
    }
}