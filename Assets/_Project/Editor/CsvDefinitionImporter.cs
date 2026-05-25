using UnityEditor;
using UnityEngine;

namespace OneMoreSpoon.Editor
{
    public static class CsvDefinitionImporter
    {
        [MenuItem("OneMoreSpoon/Import Definitions from CSV")]
        public static void ImportAll()
        {
            ImportAssetUtility.EnsureGeneratedFolders();

            CsvDefinitionAssetImporter.ImportAll();
            DefinitionCatalogRefresher.Refresh();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[CsvImporter] Imported definitions and refreshed catalog.");
        }

        [MenuItem("OneMoreSpoon/Refresh Initial Level Layout References")]
        public static void RefreshInitialLevelLayout()
        {
            InitialLevelLayoutRefresher.Refresh();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("OneMoreSpoon/Wire Startup Assets To Active Scene")]
        public static void WireStartupAssetsToActiveScene()
        {
            StartupAssetSceneWirer.WireActiveScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("OneMoreSpoon/Import Definitions from CSV and Wire Catalog")]
        public static void ImportAllAndWireCatalog()
        {
            ImportAll();
            WireStartupAssetsToActiveScene();
        }
    }
}
