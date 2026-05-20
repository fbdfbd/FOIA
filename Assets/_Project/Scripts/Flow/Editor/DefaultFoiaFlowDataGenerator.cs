#if UNITY_EDITOR
using System.Collections.Generic;
using FOIA.Flow.Definitions;
using UnityEditor;
using UnityEngine;

namespace FOIA.Flow.Editor
{
    public static class DefaultFoiaFlowDataGenerator
    {
        private const string RootPath = "Assets/_Project/Data/Flow";

        [MenuItem("FOIA/Generate Default Flow Data")]
        public static void Generate()
        {
            EnsureFolder("Assets/_Project", "Data");
            EnsureFolder("Assets/_Project/Data", "Flow");

            FlowItemDefinition complaint = CreateItem("Item_Complaint_Basic", "complaint_basic", "민원 서류", FlowItemKind.Complaint, Color.white, "민원");
            FlowItemDefinition documentTangle = CreateItem("Item_Document_Tangle", "document_tangle", "서류뭉치", FlowItemKind.Product, new Color(0.75f, 0.75f, 0.75f), "부산물");
            FlowItemDefinition complaintPressure = CreateItem("Item_Complaint_Pressure", "complaint_pressure", "항의전화", FlowItemKind.Product, new Color(0.95f, 0.65f, 0.2f), "부산물");
            FlowItemDefinition urgentDeadline = CreateItem("Item_Urgent_Deadline", "urgent_deadline", "비자기한", FlowItemKind.Product, new Color(0.95f, 0.25f, 0.25f), "부산물");
            FlowItemDefinition kindDelay = CreateItem("Item_Kind_Delay", "kind_delay", "친절지연", FlowItemKind.Product, new Color(0.45f, 0.8f, 0.95f), "부산물");

            EdgeBlockDefinition pressureBlock = CreateEdgeBlock("Block_Pressure_Letter", "pressure_letter", "압박공문", false, true, 15, complaintPressure, "압박공문");
            EdgeBlockDefinition forceBlock = CreateEdgeBlock("Block_Forced_Order", "forced_order", "적발명령", false, true, 30, urgentDeadline, "적발명령");
            EdgeBlockDefinition manualBlock = CreateEdgeBlock("Block_Manual", "manual", "매뉴얼", true, false, 0, null, "매뉴얼");

            FlowItemDefinition pressureBlockItem = CreateBlockItem("Item_Block_Pressure_Letter", "block_pressure_letter", "압박공문", pressureBlock, new Color(1f, 0.7f, 0.1f));
            FlowItemDefinition forceBlockItem = CreateBlockItem("Item_Block_Forced_Order", "block_forced_order", "적발명령", forceBlock, new Color(1f, 0.45f, 0.1f));
            FlowItemDefinition manualBlockItem = CreateBlockItem("Item_Block_Manual", "block_manual", "매뉴얼", manualBlock, new Color(0.4f, 0.95f, 0.65f));

            StaffDefinition newcomer = CreateStaff("Staff_Newcomer", "staff_newcomer", "신입", "안정", 0);
            StaffDefinition manager = CreateStaff("Staff_Manager", "staff_manager", "과장", "원칙", 30);
            StaffDefinition director = CreateStaff("Staff_Director", "staff_director", "국장", "꼼수", 0);

            AgencyDefinition police = CreateAgency("Agency_Police", "agency_police", "경찰청", "강압", 80);
            AgencyDefinition tax = CreateAgency("Agency_Tax", "agency_tax", "국세청", "지연", 50);
            AgencyDefinition education = CreateAgency("Agency_Education", "agency_education", "교육부", "온건", 100);

            AgencyOutcomeDefinition outcomeDelay = CreateOutcome("Outcome_Stable_Delay", "안정", "지연", documentTangle, "직원의 안정성과 기관의 지연 성향이 충돌해 서류뭉치가 발생했습니다.");
            AgencyOutcomeDefinition outcomePressure = CreateOutcome("Outcome_Trick_Pressure", "꼼수", "강압", urgentDeadline, "꼼수와 강압이 맞물려 비자기한 문제가 드러났습니다.");
            AgencyOutcomeDefinition outcomeComplaint = CreateOutcome("Outcome_Principle_Gentle", "원칙", "온건", complaintPressure, "원칙주의와 온건 대응이 엇갈려 항의전화가 발생했습니다.");

            RecipeDefinition recipePressure = CreateRecipe("Recipe_Pressure_Letter", documentTangle, complaintPressure, pressureBlockItem, "새 엣지블럭 발견: 압박공문");
            RecipeDefinition recipeForce = CreateRecipe("Recipe_Forced_Order", complaintPressure, urgentDeadline, forceBlockItem, "새 엣지블럭 발견: 적발명령");
            RecipeDefinition recipeManual = CreateRecipe("Recipe_Manual", kindDelay, documentTangle, manualBlockItem, "새 엣지블럭 발견: 매뉴얼");

            FoiaFlowDatabase database = CreateAsset<FoiaFlowDatabase>("Database_Default_FOIA_Flow");
            SetObject(database, "defaultComplaint", complaint);
            SetList(database, "staff", newcomer, manager, director);
            SetList(database, "agencies", police, tax, education);
            SetList(database, "agencyOutcomes", outcomeDelay, outcomePressure, outcomeComplaint);
            SetList(database, "recipes", recipePressure, recipeForce, recipeManual);
            SetObject(database, "normalByproduct", kindDelay);
            SetObject(database, "refusalByproduct", complaintPressure);
            EditorUtility.SetDirty(database);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = database;
        }

        private static FlowItemDefinition CreateItem(string assetName, string itemId, string displayName, FlowItemKind kind, Color color, string startTag)
        {
            FlowItemDefinition item = CreateAsset<FlowItemDefinition>(assetName);
            SetString(item, "itemId", itemId);
            SetString(item, "displayName", displayName);
            SetEnum(item, "kind", (int)kind);
            SetColor(item, "color", color);
            SetStringList(item, "startTags", startTag);
            return item;
        }

        private static FlowItemDefinition CreateBlockItem(string assetName, string itemId, string displayName, EdgeBlockDefinition block, Color color)
        {
            FlowItemDefinition item = CreateItem(assetName, itemId, displayName, FlowItemKind.EdgeBlock, color, "엣지블럭");
            SetObject(item, "edgeBlock", block);
            return item;
        }

        private static EdgeBlockDefinition CreateEdgeBlock(string assetName, string blockId, string displayName, bool preventsStress, bool forcesApproval, int relationLoss, FlowItemDefinition byproduct, string tag)
        {
            EdgeBlockDefinition block = CreateAsset<EdgeBlockDefinition>(assetName);
            SetString(block, "blockId", blockId);
            SetString(block, "displayName", displayName);
            SetBool(block, "preventsIntakeStress", preventsStress);
            SetBool(block, "forcesAgencyApproval", forcesApproval);
            SetInt(block, "forcedApprovalRelationshipLoss", relationLoss);
            SetObject(block, "forcedByproduct", byproduct);
            SetEffectList(block, "effects", tag);
            return block;
        }

        private static StaffDefinition CreateStaff(string assetName, string staffId, string displayName, string traitTag, int stress)
        {
            StaffDefinition staff = CreateAsset<StaffDefinition>(assetName);
            SetString(staff, "staffId", staffId);
            SetString(staff, "displayName", displayName);
            SetString(staff, "traitTag", traitTag);
            SetInt(staff, "startStress", stress);
            return staff;
        }

        private static AgencyDefinition CreateAgency(string assetName, string agencyId, string displayName, string traitTag, int relationship)
        {
            AgencyDefinition agency = CreateAsset<AgencyDefinition>(assetName);
            SetString(agency, "agencyId", agencyId);
            SetString(agency, "displayName", displayName);
            SetString(agency, "traitTag", traitTag);
            SetInt(agency, "startRelationship", relationship);
            return agency;
        }

        private static AgencyOutcomeDefinition CreateOutcome(string assetName, string staffTag, string agencyTrait, FlowItemDefinition byproduct, string message)
        {
            AgencyOutcomeDefinition outcome = CreateAsset<AgencyOutcomeDefinition>(assetName);
            SetString(outcome, "outcomeId", assetName);
            SetString(outcome, "requiredStaffTag", staffTag);
            SetString(outcome, "requiredAgencyTraitTag", agencyTrait);
            SetObject(outcome, "byproduct", byproduct);
            SetString(outcome, "logMessage", message);
            return outcome;
        }

        private static RecipeDefinition CreateRecipe(string assetName, FlowItemDefinition first, FlowItemDefinition second, FlowItemDefinition result, string discovery)
        {
            RecipeDefinition recipe = CreateAsset<RecipeDefinition>(assetName);
            SetString(recipe, "recipeId", assetName);
            SetObject(recipe, "firstIngredient", first);
            SetObject(recipe, "secondIngredient", second);
            SetObject(recipe, "result", result);
            SetString(recipe, "discoveryText", discovery);
            return recipe;
        }

        private static T CreateAsset<T>(string assetName) where T : ScriptableObject
        {
            string path = $"{RootPath}/{assetName}.asset";
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolder(string parent, string child)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{child}"))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static void SetString(Object target, string propertyName, string value) => Apply(target, propertyName, property => property.stringValue = value);
        private static void SetInt(Object target, string propertyName, int value) => Apply(target, propertyName, property => property.intValue = value);
        private static void SetBool(Object target, string propertyName, bool value) => Apply(target, propertyName, property => property.boolValue = value);
        private static void SetColor(Object target, string propertyName, Color value) => Apply(target, propertyName, property => property.colorValue = value);
        private static void SetEnum(Object target, string propertyName, int value) => Apply(target, propertyName, property => property.enumValueIndex = value);
        private static void SetObject(Object target, string propertyName, Object value) => Apply(target, propertyName, property => property.objectReferenceValue = value);

        private static void SetStringList(Object target, string propertyName, params string[] values)
        {
            Apply(target, propertyName, property =>
            {
                property.arraySize = values.Length;

                for (int i = 0; i < values.Length; i++)
                {
                    property.GetArrayElementAtIndex(i).stringValue = values[i];
                }
            });
        }

        private static void SetList(Object target, string propertyName, params Object[] values)
        {
            Apply(target, propertyName, property =>
            {
                property.arraySize = values.Length;

                for (int i = 0; i < values.Length; i++)
                {
                    property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
                }
            });
        }

        private static void SetEffectList(Object target, string propertyName, string tag)
        {
            Apply(target, propertyName, property =>
            {
                property.arraySize = string.IsNullOrEmpty(tag) ? 0 : 1;

                if (property.arraySize == 0)
                {
                    return;
                }

                SerializedProperty effect = property.GetArrayElementAtIndex(0);
                effect.FindPropertyRelative("Operation").enumValueIndex = (int)FlowTagOperation.Add;
                effect.FindPropertyRelative("Tag").stringValue = tag;
            });
        }

        private static void Apply(Object target, string propertyName, System.Action<SerializedProperty> action)
        {
            SerializedObject serializedObject = new(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            action(property);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }
}
#endif
