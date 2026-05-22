using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using OneMoreSpoon.Game.Definitions;

namespace OneMoreSpoon.Editor
{
    public static class CsvDefinitionImporter
    {
        private const string CsvBase  = "Assets/_Project/Data/CSV";
        private const string GenBase  = "Assets/_Project/Data/Generated";

        private const string NodeDir       = GenBase + "/Node";
        private const string SubstanceDir  = GenBase + "/Substance";
        private const string OperationDir  = GenBase + "/Operation";
        private const string OutputRuleDir = GenBase + "/OutputRule";
        private const string RecipeDir     = GenBase + "/MergeRecipe";

        [MenuItem("OneMoreSpoon/Import Definitions from CSV")]
        public static void ImportAll()
        {
            foreach (var dir in new[] { NodeDir, SubstanceDir, OperationDir, OutputRuleDir, RecipeDir })
                EnsureFolder(dir);

            // Substance first — others hold references to it
            var substanceLookup = ImportSubstances();
            ImportNodes();
            ImportOperations();
            ImportOutputRules(substanceLookup);
            ImportMergeRecipes(substanceLookup);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[CsvImporter] Done.");
        }

        // ── Per-type ──────────────────────────────────────────────────

        private static Dictionary<string, SO_SubstanceDefinition> ImportSubstances()
        {
            var lookup = new Dictionary<string, SO_SubstanceDefinition>();
            var rows = ReadCsv($"{CsvBase}/substances.csv");
            if (rows == null) return lookup;

            foreach (var row in rows)
            {
                var id = Get(row, "substanceId");
                if (string.IsNullOrEmpty(id)) continue;

                var asset = FindOrCreate<SO_SubstanceDefinition>(SubstanceDir, id, "substanceId");
                var so    = new SerializedObject(asset);
                so.FindProperty("substanceId").stringValue       = id;
                so.FindProperty("displayName").stringValue       = Get(row, "displayName");
                SetEnum<SubstanceKind>(so, "kind",               Get(row, "kind"), SubstanceKind.Material);
                SetStringList(so, "baseTags",                    Get(row, "baseTags"));
                so.FindProperty("baseValue").intValue            = ParseInt(Get(row, "baseValue"));
                SetStringList(so, "addedTags",                   Get(row, "addedTags"));
                so.FindProperty("durationMultiplier").floatValue = ParseFloat(Get(row, "durationMultiplier"), 1f);
                so.FindProperty("valueMultiplier").floatValue    = ParseFloat(Get(row, "valueMultiplier"), 1f);
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(asset);
                lookup[id] = asset;
            }

            Debug.Log($"[CsvImporter] Substances: {lookup.Count}");
            return lookup;
        }

        private static void ImportNodes()
        {
            var rows = ReadCsv($"{CsvBase}/nodes.csv");
            if (rows == null) return;
            int count = 0;

            foreach (var row in rows)
            {
                var id = Get(row, "definitionId");
                if (string.IsNullOrEmpty(id)) continue;

                var asset = FindOrCreate<SO_NodeDefinition>(NodeDir, id, "definitionId");
                var so    = new SerializedObject(asset);
                so.FindProperty("definitionId").stringValue = id;
                so.FindProperty("displayName").stringValue  = Get(row, "displayName");
                SetEnum<ProcessLayer>(so, "processLayer",   Get(row, "processLayer"), ProcessLayer.Source);
                SetEnum<NodeCategory>(so, "category",       Get(row, "category"), NodeCategory.Input);
                SetStringList(so, "baseTags",               Get(row, "baseTags"));
                SetStringList(so, "addedFlowTags",          Get(row, "addedFlowTags"));
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(asset);
                count++;
            }

            Debug.Log($"[CsvImporter] Nodes: {count}");
        }

        private static void ImportOperations()
        {
            var rows = ReadCsv($"{CsvBase}/operations.csv");
            if (rows == null) return;
            int count = 0;

            foreach (var row in rows)
            {
                var id = Get(row, "operationId");
                if (string.IsNullOrEmpty(id)) continue;

                var asset = FindOrCreate<SO_OperationDefinition>(OperationDir, id, "operationId");
                var so    = new SerializedObject(asset);
                so.FindProperty("operationId").stringValue = id;
                so.FindProperty("displayName").stringValue = Get(row, "displayName");
                SetEnum<OperationCategory>(so, "category", Get(row, "category"), OperationCategory.None);
                SetStringList(so, "baseTags",              Get(row, "baseTags"));
                so.FindProperty("duration").floatValue     = ParseFloat(Get(row, "duration"), 5f);
                SetStringList(so, "baseOutputIds",         Get(row, "baseOutputIds"));
                SetStringList(so, "outputTags",            Get(row, "outputTags"));
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(asset);
                count++;
            }

            Debug.Log($"[CsvImporter] Operations: {count}");
        }

        private static void ImportOutputRules(Dictionary<string, SO_SubstanceDefinition> substanceLookup)
        {
            var rows = ReadCsv($"{CsvBase}/outputRules.csv");
            if (rows == null) return;

            var byproductMap = BuildByproductMap(ReadCsv($"{CsvBase}/outputRuleByproducts.csv"));
            int count = 0;

            foreach (var row in rows)
            {
                var id = Get(row, "ruleId");
                if (string.IsNullOrEmpty(id)) continue;

                var asset = FindOrCreate<SO_OutputRuleDefinition>(OutputRuleDir, id, "ruleId");
                var so    = new SerializedObject(asset);
                so.FindProperty("ruleId").stringValue    = id;
                SetSubRef(so, "requiredSubstance", substanceLookup, Get(row, "requiredSubstanceId"));
                SetStringList(so, "requiredTags",        Get(row, "requiredTags"));
                SetSubRef(so, "resultSubstance",   substanceLookup, Get(row, "resultSubstanceId"));
                so.FindProperty("resultAmount").intValue = ParseInt(Get(row, "resultAmount"), 1);

                var byproducts    = byproductMap.TryGetValue(id, out var list) ? list : new List<(string substanceId, int amount)>();
                var byproductProp = so.FindProperty("byproducts");
                byproductProp.ClearArray();
                for (int i = 0; i < byproducts.Count; i++)
                {
                    byproductProp.InsertArrayElementAtIndex(i);
                    var elem = byproductProp.GetArrayElementAtIndex(i);
                    if (substanceLookup.TryGetValue(byproducts[i].substanceId, out var s))
                        elem.FindPropertyRelative("substance").objectReferenceValue = s;
                    elem.FindPropertyRelative("amount").intValue = byproducts[i].amount;
                }

                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(asset);
                count++;
            }

            Debug.Log($"[CsvImporter] OutputRules: {count}");
        }

        private static void ImportMergeRecipes(Dictionary<string, SO_SubstanceDefinition> substanceLookup)
        {
            var rows = ReadCsv($"{CsvBase}/mergeRecipes.csv");
            if (rows == null) return;
            int count = 0;

            foreach (var row in rows)
            {
                var id = Get(row, "recipeId");
                if (string.IsNullOrEmpty(id)) continue;

                var asset = FindOrCreate<SO_MergeRecipeDefinition>(RecipeDir, id, "recipeId");
                var so    = new SerializedObject(asset);
                so.FindProperty("recipeId").stringValue    = id;

                var inputIds   = SplitList(Get(row, "inputSubstanceIds"));
                var inputsProp = so.FindProperty("inputSubstances");
                inputsProp.ClearArray();
                for (int i = 0; i < inputIds.Count; i++)
                {
                    inputsProp.InsertArrayElementAtIndex(i);
                    if (substanceLookup.TryGetValue(inputIds[i], out var s))
                        inputsProp.GetArrayElementAtIndex(i).objectReferenceValue = s;
                }

                SetSubRef(so, "resultSubstance",   substanceLookup, Get(row, "resultSubstanceId"));
                so.FindProperty("resultAmount").intValue = ParseInt(Get(row, "resultAmount"), 1);
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(asset);
                count++;
            }

            Debug.Log($"[CsvImporter] MergeRecipes: {count}");
        }

        // ── Helpers ───────────────────────────────────────────────────

        private static T FindOrCreate<T>(string folder, string id, string idPropName)
            where T : ScriptableObject
        {
            foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder }))
            {
                var path     = AssetDatabase.GUIDToAssetPath(guid);
                var existing = AssetDatabase.LoadAssetAtPath<T>(path);
                if (existing == null) continue;
                if (new SerializedObject(existing).FindProperty(idPropName)?.stringValue == id)
                    return existing;
            }

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, $"{folder}/{typeof(T).Name}_{id}.asset");
            return asset;
        }

        private static List<Dictionary<string, string>> ReadCsv(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[CsvImporter] Not found: {path}");
                return null;
            }

            var lines  = File.ReadAllLines(path, Encoding.UTF8);
            if (lines.Length < 2) return new List<Dictionary<string, string>>();

            var headers = ParseLine(lines[0]);
            var result  = new List<Dictionary<string, string>>();

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                var values = ParseLine(lines[i]);
                var row    = new Dictionary<string, string>();
                for (int j = 0; j < headers.Count && j < values.Count; j++)
                    row[headers[j].Trim()] = values[j].Trim();
                result.Add(row);
            }

            return result;
        }

        private static List<string> ParseLine(string line)
        {
            var result   = new List<string>();
            var current  = new StringBuilder();
            bool inQuotes = false;

            foreach (char c in line)
            {
                if (c == '"') { inQuotes = !inQuotes; continue; }
                if (c == ',' && !inQuotes) { result.Add(current.ToString()); current.Clear(); continue; }
                current.Append(c);
            }
            result.Add(current.ToString());
            return result;
        }

        private static string Get(Dictionary<string, string> row, string key)
            => row.TryGetValue(key, out var v) ? v : string.Empty;

        private static List<string> SplitList(string value)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(value)) return result;
            foreach (var item in value.Split('|'))
            {
                var t = item.Trim();
                if (!string.IsNullOrEmpty(t)) result.Add(t);
            }
            return result;
        }

        private static void SetStringList(SerializedObject so, string propName, string raw)
        {
            var prop = so.FindProperty(propName);
            if (prop == null) return;
            var items = SplitList(raw);
            prop.ClearArray();
            for (int i = 0; i < items.Count; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).stringValue = items[i];
            }
        }

        private static void SetEnum<TEnum>(
            SerializedObject so, string propName, string value, TEnum defaultVal)
            where TEnum : struct, Enum
        {
            var prop = so.FindProperty(propName);
            if (prop == null) return;
            var parsed = Enum.TryParse<TEnum>(value, ignoreCase: true, out var r) ? r : defaultVal;
            prop.intValue = (int)(object)parsed;
        }

        private static void SetSubRef(
            SerializedObject so, string propName,
            Dictionary<string, SO_SubstanceDefinition> lookup, string substanceId)
        {
            var prop = so.FindProperty(propName);
            if (prop == null) return;
            prop.objectReferenceValue = lookup.TryGetValue(substanceId, out var s) ? s : null;
        }

        private static Dictionary<string, List<(string substanceId, int amount)>> BuildByproductMap(
            List<Dictionary<string, string>> rows)
        {
            var map = new Dictionary<string, List<(string substanceId, int amount)>>();
            if (rows == null) return map;

            foreach (var row in rows)
            {
                var ruleId = Get(row, "ruleId");
                var subId  = Get(row, "substanceId");
                if (string.IsNullOrEmpty(ruleId) || string.IsNullOrEmpty(subId)) continue;
                if (!map.ContainsKey(ruleId)) map[ruleId] = new List<(string substanceId, int amount)>();
                map[ruleId].Add((subId, ParseInt(Get(row, "amount"), 1)));
            }

            return map;
        }

        private static int ParseInt(string value, int fallback = 0)
            => int.TryParse(value, out var r) ? r : fallback;

        private static float ParseFloat(string value, float fallback = 0f)
            => float.TryParse(value,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out var r) ? r : fallback;

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parts   = path.Split('/');
            var current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                var next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
