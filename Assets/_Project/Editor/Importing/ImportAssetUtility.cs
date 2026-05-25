using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OneMoreSpoon.Editor
{
    internal static class ImportAssetUtility
    {
        public static void EnsureGeneratedFolders()
        {
            foreach (var path in new[]
            {
                ImportPaths.GeneratedBase,
                ImportPaths.NodeDir,
                ImportPaths.SubstanceDir,
                ImportPaths.OperationDir,
                ImportPaths.OutputRuleDir,
                ImportPaths.RecipeDir,
                ImportPaths.NodeInspectDir,
                ImportPaths.SubstanceInspectDir
            })
            {
                EnsureFolder(path);
            }
        }

        public static T FindOrCreate<T>(string folder, string id, string idPropName)
            where T : ScriptableObject
        {
            var expectedPath = $"{folder}/{typeof(T).Name}_{id}.asset";

            foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var existing = AssetDatabase.LoadAssetAtPath<T>(path);
                if (existing == null)
                    continue;

                if (ReadStringProperty(existing, idPropName) == id)
                    return existing;
            }

            var existingAtExpectedPath = AssetDatabase.LoadAssetAtPath<T>(expectedPath);
            if (existingAtExpectedPath != null)
                return existingAtExpectedPath;

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, expectedPath);
            return asset;
        }

        public static T[] LoadGeneratedAssets<T>(string folder)
            where T : UnityEngine.Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder });
            Array.Sort(guids, StringComparer.Ordinal);

            var assets = new List<T>();
            foreach (var guid in guids)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset != null)
                    assets.Add(asset);
            }

            return assets.ToArray();
        }

        public static Dictionary<string, T> LoadGeneratedAssetLookup<T>(string folder, string idPropName)
            where T : UnityEngine.Object
        {
            var lookup = new Dictionary<string, T>();
            foreach (var asset in LoadGeneratedAssets<T>(folder))
            {
                var id = ReadStringProperty(asset, idPropName);
                if (!string.IsNullOrEmpty(id))
                    lookup[id] = asset;
            }

            return lookup;
        }

        public static string ReadStringProperty(UnityEngine.Object asset, string propName)
        {
            if (asset == null)
                return null;

            return new SerializedObject(asset).FindProperty(propName)?.stringValue;
        }

        public static void SetObjectArray<T>(SerializedObject so, string propName, IReadOnlyList<T> assets)
            where T : UnityEngine.Object
        {
            var prop = so.FindProperty(propName);
            if (prop == null)
            {
                Debug.LogWarning($"[CsvImporter] Serialized property not found: {propName}");
                return;
            }

            prop.ClearArray();
            for (var i = 0; i < assets.Count; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = assets[i];
            }
        }

        public static void SetStringList(SerializedObject so, string propName, string raw)
        {
            var prop = so.FindProperty(propName);
            if (prop == null)
                return;

            var items = CsvReader.SplitList(raw);
            prop.ClearArray();
            for (var i = 0; i < items.Count; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).stringValue = items[i];
            }
        }

        public static void SetEnum<TEnum>(SerializedObject so, string propName, string value, TEnum fallback)
            where TEnum : struct, Enum
        {
            var prop = so.FindProperty(propName);
            if (prop == null)
                return;

            var parsed = Enum.TryParse<TEnum>(value, true, out var result) ? result : fallback;
            prop.intValue = (int)(object)parsed;
        }

        public static int ParseInt(string value, int fallback = 0)
            => int.TryParse(value, out var result) ? result : fallback;

        public static float ParseFloat(string value, float fallback = 0f)
            => float.TryParse(
                value,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out var result)
                ? result
                : fallback;

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            var parts = path.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
