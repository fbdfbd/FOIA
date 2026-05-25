using OneMoreSpoon.Game.Definitions;
using UnityEditor;
using UnityEngine;

namespace OneMoreSpoon.Editor
{
    internal static class DefinitionCatalogRefresher
    {
        public static void Refresh()
        {
            ImportAssetUtility.EnsureGeneratedFolders();

            var catalog = AssetDatabase.LoadAssetAtPath<SO_DefinitionCatalog>(ImportPaths.CatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<SO_DefinitionCatalog>();
                AssetDatabase.CreateAsset(catalog, ImportPaths.CatalogPath);
            }

            var so = new SerializedObject(catalog);
            ImportAssetUtility.SetObjectArray(
                so,
                "nodeDefinitions",
                ImportAssetUtility.LoadGeneratedAssets<SO_NodeDefinition>(ImportPaths.NodeDir));
            ImportAssetUtility.SetObjectArray(
                so,
                "operationDefinitions",
                ImportAssetUtility.LoadGeneratedAssets<SO_OperationDefinition>(ImportPaths.OperationDir));
            ImportAssetUtility.SetObjectArray(
                so,
                "substanceDefinitions",
                ImportAssetUtility.LoadGeneratedAssets<SO_SubstanceDefinition>(ImportPaths.SubstanceDir));
            ImportAssetUtility.SetObjectArray(
                so,
                "outputRuleDefinitions",
                ImportAssetUtility.LoadGeneratedAssets<SO_OutputRuleDefinition>(ImportPaths.OutputRuleDir));
            ImportAssetUtility.SetObjectArray(
                so,
                "mergeRecipeDefinitions",
                ImportAssetUtility.LoadGeneratedAssets<SO_MergeRecipeDefinition>(ImportPaths.RecipeDir));
            ImportAssetUtility.SetObjectArray(
                so,
                "nodeInspectDefinitions",
                ImportAssetUtility.LoadGeneratedAssets<SO_NodeInspectDefinition>(ImportPaths.NodeInspectDir));
            ImportAssetUtility.SetObjectArray(
                so,
                "substanceInspectDefinitions",
                ImportAssetUtility.LoadGeneratedAssets<SO_SubstanceInspectDefinition>(ImportPaths.SubstanceInspectDir));
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(catalog);
            Debug.Log($"[CsvImporter] Catalog refreshed: {ImportPaths.CatalogPath}");
        }
    }
}
