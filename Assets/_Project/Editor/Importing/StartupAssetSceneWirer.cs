using OneMoreSpoon.App.Config;
using OneMoreSpoon.App.LifetimeScopes;
using OneMoreSpoon.Game.Definitions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneMoreSpoon.Editor
{
    internal static class StartupAssetSceneWirer
    {
        public static void WireActiveScene()
        {
            var scope = FindActiveSceneGameLifetimeScope();
            var catalog = AssetDatabase.LoadAssetAtPath<SO_DefinitionCatalog>(ImportPaths.CatalogPath);
            var layout = AssetDatabase.LoadAssetAtPath<SO_InitialLevelLayout>(ImportPaths.InitialLayoutPath);
            if (scope == null || catalog == null || layout == null)
            {
                Debug.LogWarning("[CsvImporter] Could not wire startup assets. Scope, catalog, or initial layout is missing.");
                return;
            }

            var so = new SerializedObject(scope);
            so.FindProperty("definitionCatalog").objectReferenceValue = catalog;
            so.FindProperty("initialLevelLayout").objectReferenceValue = layout;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(scope);
            EditorSceneManager.MarkSceneDirty(scope.gameObject.scene);
            Debug.Log($"[CsvImporter] Wired startup assets to GameLifetimeScope: {scope.name}");
        }

        private static GameLifetimeScope FindActiveSceneGameLifetimeScope()
        {
            var activeScene = SceneManager.GetActiveScene();
            var scopes = Resources.FindObjectsOfTypeAll<GameLifetimeScope>();
            GameLifetimeScope fallback = null;

            foreach (var scope in scopes)
            {
                if (scope == null || EditorUtility.IsPersistent(scope))
                    continue;

                var scene = scope.gameObject.scene;
                if (!scene.IsValid() || !scene.isLoaded)
                    continue;

                fallback ??= scope;
                if (scene == activeScene)
                    return scope;
            }

            return fallback;
        }
    }
}
