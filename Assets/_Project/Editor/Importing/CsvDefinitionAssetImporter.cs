using System;
using System.Collections.Generic;
using OneMoreSpoon.Game.Definitions;
using UnityEditor;
using UnityEngine;

namespace OneMoreSpoon.Editor
{
    internal static class CsvDefinitionAssetImporter
    {
        public static void ImportAll()
        {
            var substanceLookup = ImportSubstances();
            ImportNodes();
            ImportOperations();
            ImportOutputRules(substanceLookup);
            ImportMergeRecipes(substanceLookup);
            ImportFirstDiscoveryRewards(substanceLookup);
            ImportNodeInspects();
            ImportSubstanceInspects();
        }

        private static Dictionary<string, SO_SubstanceDefinition> ImportSubstances()
        {
            var lookup = new Dictionary<string, SO_SubstanceDefinition>();
            var rows = CsvReader.Read($"{ImportPaths.CsvBase}/substances.csv");
            if (rows == null)
                return lookup;

            foreach (var row in rows)
            {
                var id = CsvReader.Get(row, "substanceId");
                if (string.IsNullOrEmpty(id))
                    continue;

                var asset = ImportAssetUtility.FindOrCreate<SO_SubstanceDefinition>(
                    ImportPaths.SubstanceDir,
                    id,
                    "substanceId");
                var so = new SerializedObject(asset);
                so.FindProperty("substanceId").stringValue = id;
                so.FindProperty("displayName").stringValue = CsvReader.Get(row, "displayName");
                SetSpriteRef(so, "image", CsvReader.Get(row, "imagePath"));
                ImportAssetUtility.SetEnum(so, "kind", CsvReader.Get(row, "kind"), SubstanceKind.Trash);
                ImportAssetUtility.SetStringList(so, "baseTags", CsvReader.Get(row, "baseTags"));
                so.FindProperty("baseValue").intValue = ImportAssetUtility.ParseInt(CsvReader.Get(row, "baseValue"));
                ImportAssetUtility.SetStringList(so, "addedTags", CsvReader.Get(row, "addedTags"));
                so.FindProperty("durationMultiplier").floatValue =
                    ImportAssetUtility.ParseFloat(CsvReader.Get(row, "durationMultiplier"), 1f);
                so.FindProperty("valueMultiplier").floatValue =
                    ImportAssetUtility.ParseFloat(CsvReader.Get(row, "valueMultiplier"), 1f);
                so.ApplyModifiedProperties();

                EditorUtility.SetDirty(asset);
                lookup[id] = asset;
            }

            Debug.Log($"[CsvImporter] Substances: {lookup.Count}");
            return lookup;
        }

        private static void ImportNodes()
        {
            var rows = CsvReader.Read($"{ImportPaths.CsvBase}/nodes.csv");
            if (rows == null)
                return;

            var count = 0;
            foreach (var row in rows)
            {
                var id = CsvReader.Get(row, "definitionId");
                if (string.IsNullOrEmpty(id))
                    continue;

                var asset = ImportAssetUtility.FindOrCreate<SO_NodeDefinition>(
                    ImportPaths.NodeDir,
                    id,
                    "definitionId");
                var so = new SerializedObject(asset);
                so.FindProperty("definitionId").stringValue = id;
                so.FindProperty("displayName").stringValue = CsvReader.Get(row, "displayName");
                SetSpriteRef(so, "image", CsvReader.Get(row, "imagePath"));
                so.FindProperty("processLayer").intValue =
                    ImportAssetUtility.ParseInt(CsvReader.Get(row, "processLayer"));
                ImportAssetUtility.SetEnum(so, "category", CsvReader.Get(row, "category"), NodeCategory.Input);
                ImportAssetUtility.SetStringList(so, "baseTags", CsvReader.Get(row, "baseTags"));
                ImportAssetUtility.SetStringList(so, "addedFlowTags", CsvReader.Get(row, "addedFlowTags"));
                so.ApplyModifiedProperties();

                EditorUtility.SetDirty(asset);
                count++;
            }

            Debug.Log($"[CsvImporter] Nodes: {count}");
        }

        private static void ImportOperations()
        {
            var rows = CsvReader.Read($"{ImportPaths.CsvBase}/operations.csv");
            if (rows == null)
                return;

            var count = 0;
            foreach (var row in rows)
            {
                var id = CsvReader.Get(row, "operationId");
                if (string.IsNullOrEmpty(id))
                    continue;

                var asset = ImportAssetUtility.FindOrCreate<SO_OperationDefinition>(
                    ImportPaths.OperationDir,
                    id,
                    "operationId");
                var so = new SerializedObject(asset);
                so.FindProperty("operationId").stringValue = id;
                so.FindProperty("displayName").stringValue = CsvReader.Get(row, "displayName");
                ImportAssetUtility.SetEnum(so, "category", CsvReader.Get(row, "category"), OperationCategory.None);
                ImportAssetUtility.SetStringList(so, "baseTags", CsvReader.Get(row, "baseTags"));
                so.FindProperty("duration").floatValue = ImportAssetUtility.ParseFloat(CsvReader.Get(row, "duration"), 5f);
                ImportAssetUtility.SetStringList(so, "baseOutputIds", CsvReader.Get(row, "baseOutputIds"));
                ImportAssetUtility.SetStringList(so, "outputTags", CsvReader.Get(row, "outputTags"));
                so.ApplyModifiedProperties();

                EditorUtility.SetDirty(asset);
                count++;
            }

            Debug.Log($"[CsvImporter] Operations: {count}");
        }

        private static void ImportOutputRules(Dictionary<string, SO_SubstanceDefinition> substanceLookup)
        {
            var rows = CsvReader.Read($"{ImportPaths.CsvBase}/outputRules.csv");
            if (rows == null)
                return;

            var byproductMap = BuildByproductMap(CsvReader.Read($"{ImportPaths.CsvBase}/outputRuleByproducts.csv"));
            var count = 0;

            foreach (var row in rows)
            {
                var id = CsvReader.Get(row, "ruleId");
                if (string.IsNullOrEmpty(id))
                    continue;

                var asset = ImportAssetUtility.FindOrCreate<SO_OutputRuleDefinition>(
                    ImportPaths.OutputRuleDir,
                    id,
                    "ruleId");
                var so = new SerializedObject(asset);
                so.FindProperty("ruleId").stringValue = id;
                so.FindProperty("priority").intValue =
                    ImportAssetUtility.ParseInt(CsvReader.Get(row, "priority"), 0);
                SetSubstanceRef(so, "requiredSubstance", substanceLookup, CsvReader.Get(row, "requiredSubstanceId"));
                ImportAssetUtility.SetStringList(so, "requiredTags", CsvReader.Get(row, "requiredTags"));
                ImportAssetUtility.SetStringList(so, "requiredHistorySequence", CsvReader.Get(row, "requiredHistorySequence"));
                ImportAssetUtility.SetStringList(
                    so,
                    "requiredDiscoveredSubstanceIds",
                    CsvReader.Get(row, "requiredDiscoveredSubstanceIds"));
                ImportAssetUtility.SetStringList(
                    so,
                    "requiredUndiscoveredSubstanceIds",
                    CsvReader.Get(row, "requiredUndiscoveredSubstanceIds"));
                SetSubstanceRef(so, "resultSubstance", substanceLookup, CsvReader.Get(row, "resultSubstanceId"));
                so.FindProperty("resultAmount").intValue =
                    ImportAssetUtility.ParseInt(CsvReader.Get(row, "resultAmount"), 1);
                SetByproducts(
                    so,
                    byproductMap.TryGetValue(id, out var list) ? list : Array.Empty<ByproductImportData>(),
                    substanceLookup);
                so.ApplyModifiedProperties();

                EditorUtility.SetDirty(asset);
                count++;
            }

            Debug.Log($"[CsvImporter] OutputRules: {count}");
        }

        private static void ImportMergeRecipes(Dictionary<string, SO_SubstanceDefinition> substanceLookup)
        {
            var rows = CsvReader.Read($"{ImportPaths.CsvBase}/mergeRecipes.csv");
            if (rows == null)
                return;

            var count = 0;
            foreach (var row in rows)
            {
                var id = CsvReader.Get(row, "recipeId");
                if (string.IsNullOrEmpty(id))
                    continue;

                var asset = ImportAssetUtility.FindOrCreate<SO_MergeRecipeDefinition>(
                    ImportPaths.RecipeDir,
                    id,
                    "recipeId");
                var so = new SerializedObject(asset);
                so.FindProperty("recipeId").stringValue = id;
                SetSubstanceList(
                    so,
                    "inputSubstances",
                    CsvReader.SplitList(CsvReader.Get(row, "inputSubstanceIds")),
                    substanceLookup);
                SetSubstanceRef(so, "resultSubstance", substanceLookup, CsvReader.Get(row, "resultSubstanceId"));
                so.FindProperty("resultAmount").intValue =
                    ImportAssetUtility.ParseInt(CsvReader.Get(row, "resultAmount"), 1);
                so.ApplyModifiedProperties();

                EditorUtility.SetDirty(asset);
                count++;
            }

            Debug.Log($"[CsvImporter] MergeRecipes: {count}");
        }

        private static void ImportFirstDiscoveryRewards(Dictionary<string, SO_SubstanceDefinition> substanceLookup)
        {
            var rows = CsvReader.Read($"{ImportPaths.CsvBase}/firstDiscoveryRewards.csv");
            if (rows == null)
                return;

            var count = 0;
            foreach (var row in rows)
            {
                var triggerTypeText = CsvReader.Get(row, "triggerType");
                var triggerId = CsvReader.Get(row, "triggerId");
                var rewardTypeText = CsvReader.Get(row, "rewardType");
                var rewardId = CsvReader.Get(row, "rewardId");
                if (string.IsNullOrEmpty(triggerTypeText) ||
                    string.IsNullOrEmpty(triggerId) ||
                    string.IsNullOrEmpty(rewardTypeText) ||
                    string.IsNullOrEmpty(rewardId))
                {
                    continue;
                }

                var entryId = BuildFirstDiscoveryRewardEntryId(triggerTypeText, triggerId, rewardTypeText, rewardId);
                var asset = ImportAssetUtility.FindOrCreate<SO_FirstDiscoveryRewardDefinition>(
                    ImportPaths.FirstDiscoveryRewardDir,
                    entryId,
                    "entryId");
                var so = new SerializedObject(asset);
                so.FindProperty("entryId").stringValue = entryId;
                ImportAssetUtility.SetEnum(
                    so,
                    "triggerType",
                    triggerTypeText,
                    FirstDiscoveryRewardTriggerType.Substance);
                so.FindProperty("triggerId").stringValue = triggerId;
                ImportAssetUtility.SetEnum(
                    so,
                    "rewardType",
                    rewardTypeText,
                    FirstDiscoveryRewardType.Substance);
                so.FindProperty("rewardId").stringValue = rewardId;
                so.FindProperty("amount").intValue =
                    ImportAssetUtility.ParseInt(CsvReader.Get(row, "amount"), 1);
                so.FindProperty("rewardGroup").stringValue = CsvReader.Get(row, "rewardGroup");
                ImportAssetUtility.SetStringList(
                    so,
                    "requiredUndiscoveredSubstanceIds",
                    CsvReader.Get(row, "requiredUndiscoveredSubstanceIds"));
                so.FindProperty("note").stringValue = CsvReader.Get(row, "note");
                so.ApplyModifiedProperties();

                EditorUtility.SetDirty(asset);
                count++;
            }

            Debug.Log($"[CsvImporter] FirstDiscoveryRewards: {count}");
        }

        private static void ImportNodeInspects()
        {
            var rows = CsvReader.Read($"{ImportPaths.CsvBase}/nodeInspects.csv");
            if (rows == null)
                return;

            var count = 0;
            foreach (var row in rows)
            {
                var id = CsvReader.Get(row, "definitionId");
                if (string.IsNullOrEmpty(id))
                    continue;

                var asset = ImportAssetUtility.FindOrCreate<SO_NodeInspectDefinition>(
                    ImportPaths.NodeInspectDir,
                    id,
                    "targetDefinitionId");
                var so = new SerializedObject(asset);
                so.FindProperty("targetDefinitionId").stringValue = id;
                so.FindProperty("description").stringValue = CsvReader.Get(row, "description");
                so.ApplyModifiedProperties();

                EditorUtility.SetDirty(asset);
                count++;
            }

            Debug.Log($"[CsvImporter] NodeInspects: {count}");
        }

        private static void ImportSubstanceInspects()
        {
            var rows = CsvReader.Read($"{ImportPaths.CsvBase}/substanceInspects.csv");
            if (rows == null)
                return;

            var count = 0;
            foreach (var row in rows)
            {
                var id = CsvReader.Get(row, "substanceId");
                if (string.IsNullOrEmpty(id))
                    continue;

                var asset = ImportAssetUtility.FindOrCreate<SO_SubstanceInspectDefinition>(
                    ImportPaths.SubstanceInspectDir,
                    id,
                    "targetSubstanceId");
                var so = new SerializedObject(asset);
                so.FindProperty("targetSubstanceId").stringValue = id;
                so.FindProperty("description").stringValue = CsvReader.Get(row, "description");
                so.ApplyModifiedProperties();

                EditorUtility.SetDirty(asset);
                count++;
            }

            Debug.Log($"[CsvImporter] SubstanceInspects: {count}");
        }

        private static void SetSubstanceList(
            SerializedObject so,
            string propName,
            IReadOnlyList<string> ids,
            IReadOnlyDictionary<string, SO_SubstanceDefinition> lookup)
        {
            var prop = so.FindProperty(propName);
            if (prop == null)
                return;

            prop.ClearArray();
            for (var i = 0; i < ids.Count; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue =
                    lookup.TryGetValue(ids[i], out var substance) ? substance : null;
            }
        }

        private static void SetSubstanceRef(
            SerializedObject so,
            string propName,
            IReadOnlyDictionary<string, SO_SubstanceDefinition> lookup,
            string substanceId)
        {
            var prop = so.FindProperty(propName);
            if (prop == null)
                return;

            prop.objectReferenceValue = lookup.TryGetValue(substanceId, out var substance) ? substance : null;
        }

        private static void SetSpriteRef(SerializedObject so, string propName, string assetPath)
        {
            var prop = so.FindProperty(propName);
            if (prop == null)
                return;

            if (string.IsNullOrWhiteSpace(assetPath))
            {
                prop.objectReferenceValue = null;
                return;
            }

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite == null)
                Debug.LogWarning($"[CsvImporter] Sprite not found: {assetPath}");

            prop.objectReferenceValue = sprite;
        }

        private readonly struct ByproductImportData
        {
            public ByproductImportData(string substanceId, int amount, string requiredUndiscoveredSubstanceIds)
            {
                SubstanceId = substanceId;
                Amount = amount;
                RequiredUndiscoveredSubstanceIds = requiredUndiscoveredSubstanceIds;
            }

            public string SubstanceId { get; }
            public int Amount { get; }
            public string RequiredUndiscoveredSubstanceIds { get; }
        }

        private static void SetByproducts(
            SerializedObject so,
            IReadOnlyList<ByproductImportData> byproducts,
            IReadOnlyDictionary<string, SO_SubstanceDefinition> lookup)
        {
            var prop = so.FindProperty("byproducts");
            if (prop == null)
                return;

            prop.ClearArray();
            for (var i = 0; i < byproducts.Count; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                var element = prop.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("substance").objectReferenceValue =
                    lookup.TryGetValue(byproducts[i].SubstanceId, out var substance) ? substance : null;
                element.FindPropertyRelative("amount").intValue = byproducts[i].Amount;
                SetStringList(
                    element.FindPropertyRelative("requiredUndiscoveredSubstanceIds"),
                    CsvReader.SplitList(byproducts[i].RequiredUndiscoveredSubstanceIds));
            }
        }

        private static Dictionary<string, List<ByproductImportData>> BuildByproductMap(
            List<Dictionary<string, string>> rows)
        {
            var map = new Dictionary<string, List<ByproductImportData>>();
            if (rows == null)
                return map;

            foreach (var row in rows)
            {
                var ruleId = CsvReader.Get(row, "ruleId");
                var substanceId = CsvReader.Get(row, "substanceId");
                if (string.IsNullOrEmpty(ruleId) || string.IsNullOrEmpty(substanceId))
                    continue;

                if (!map.TryGetValue(ruleId, out var list))
                {
                    list = new List<ByproductImportData>();
                    map.Add(ruleId, list);
                }

                list.Add(new ByproductImportData(
                    substanceId,
                    ImportAssetUtility.ParseInt(CsvReader.Get(row, "amount"), 1),
                    CsvReader.Get(row, "requiredUndiscoveredSubstanceIds")));
            }

            return map;
        }

        private static void SetStringList(SerializedProperty prop, IReadOnlyList<string> values)
        {
            if (prop == null)
                return;

            prop.ClearArray();
            for (var i = 0; i < values.Count; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).stringValue = values[i];
            }
        }

        private static string BuildFirstDiscoveryRewardEntryId(
            string triggerType,
            string triggerId,
            string rewardType,
            string rewardId)
        {
            return $"{triggerType}_{triggerId}_{rewardType}_{rewardId}";
        }
    }
}
