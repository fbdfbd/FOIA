using System.Collections.Generic;
using System.Linq;
using OneMoreSpoon.App.Config;
using OneMoreSpoon.Game.Definitions;
using UnityEditor;
using UnityEngine;

namespace OneMoreSpoon.Editor
{
    internal static class InitialLevelLayoutRefresher
    {
        public static void Refresh()
        {
            ImportAssetUtility.EnsureGeneratedFolders();

            var layout = AssetDatabase.LoadAssetAtPath<SO_InitialLevelLayout>(ImportPaths.InitialLayoutPath);
            if (layout == null)
            {
                layout = ScriptableObject.CreateInstance<SO_InitialLevelLayout>();
                AssetDatabase.CreateAsset(layout, ImportPaths.InitialLayoutPath);
            }

            var existingNodePositions = ReadInitialNodePositions(layout);
            var existingSubstanceStacks = ReadInitialSubstanceStacks(layout);
            var nodeLookup = ImportAssetUtility.LoadGeneratedAssetLookup<SO_NodeDefinition>(
                ImportPaths.NodeDir,
                "definitionId");
            var substanceLookup = ImportAssetUtility.LoadGeneratedAssetLookup<SO_SubstanceDefinition>(
                ImportPaths.SubstanceDir,
                "substanceId");

            var so = new SerializedObject(layout);
            SetInitialNodes(so, nodeLookup, existingNodePositions);
            SetInitialSubstanceStacks(so, substanceLookup, existingSubstanceStacks);
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(layout);
            Debug.Log($"[CsvImporter] Initial level layout references refreshed: {ImportPaths.InitialLayoutPath}");
        }

        private static Dictionary<string, Vector2> ReadInitialNodePositions(SO_InitialLevelLayout layout)
        {
            var positions = new Dictionary<string, Vector2>();
            var prop = new SerializedObject(layout).FindProperty("initialNodes");
            if (prop == null)
                return positions;

            for (var i = 0; i < prop.arraySize; i++)
            {
                var element = prop.GetArrayElementAtIndex(i);
                var definition = element.FindPropertyRelative("Definition").objectReferenceValue;
                var id = ImportAssetUtility.ReadStringProperty(definition, "definitionId");
                if (!string.IsNullOrEmpty(id))
                    positions[id] = element.FindPropertyRelative("Position").vector2Value;
            }

            return positions;
        }

        private static Dictionary<string, InitialSubstanceStackSnapshot> ReadInitialSubstanceStacks(
            SO_InitialLevelLayout layout)
        {
            var snapshots = new Dictionary<string, InitialSubstanceStackSnapshot>();
            var prop = new SerializedObject(layout).FindProperty("initialSubstanceStacks");
            if (prop == null)
                return snapshots;

            for (var i = 0; i < prop.arraySize; i++)
            {
                var element = prop.GetArrayElementAtIndex(i);
                var definition = element.FindPropertyRelative("SubstanceDefinition").objectReferenceValue;
                var id = ImportAssetUtility.ReadStringProperty(definition, "substanceId");
                if (string.IsNullOrEmpty(id))
                    continue;

                snapshots[id] = new InitialSubstanceStackSnapshot(
                    element.FindPropertyRelative("Amount").intValue,
                    element.FindPropertyRelative("IsInfinite").boolValue,
                    element.FindPropertyRelative("Position").vector2Value);
            }

            return snapshots;
        }

        private static void SetInitialNodes(
            SerializedObject so,
            IReadOnlyDictionary<string, SO_NodeDefinition> lookup,
            IReadOnlyDictionary<string, Vector2> existingPositions)
        {
            var prop = so.FindProperty("initialNodes");
            if (prop == null)
                return;

            prop.ClearArray();
            var ids = existingPositions.Keys
                .Where(lookup.ContainsKey)
                .OrderBy(id => id)
                .ToArray();

            for (var i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                prop.InsertArrayElementAtIndex(i);
                var element = prop.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("Definition").objectReferenceValue = lookup[id];
                element.FindPropertyRelative("Position").vector2Value = existingPositions[id];
            }
        }

        private static void SetInitialSubstanceStacks(
            SerializedObject so,
            IReadOnlyDictionary<string, SO_SubstanceDefinition> lookup,
            IReadOnlyDictionary<string, InitialSubstanceStackSnapshot> existingStacks)
        {
            var prop = so.FindProperty("initialSubstanceStacks");
            if (prop == null)
                return;

            prop.ClearArray();
            var ids = existingStacks.Keys
                .Where(lookup.ContainsKey)
                .OrderBy(id => id)
                .ToArray();

            for (var i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var stack = existingStacks[id];

                prop.InsertArrayElementAtIndex(i);
                var element = prop.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("SubstanceDefinition").objectReferenceValue = lookup[id];
                element.FindPropertyRelative("Amount").intValue = stack.Amount;
                element.FindPropertyRelative("IsInfinite").boolValue = stack.IsInfinite;
                element.FindPropertyRelative("Position").vector2Value = stack.Position;
            }
        }

        private readonly struct InitialSubstanceStackSnapshot
        {
            public InitialSubstanceStackSnapshot(int amount, bool isInfinite, Vector2 position)
            {
                Amount = amount;
                IsInfinite = isInfinite;
                Position = position;
            }

            public int Amount { get; }
            public bool IsInfinite { get; }
            public Vector2 Position { get; }
        }
    }
}
