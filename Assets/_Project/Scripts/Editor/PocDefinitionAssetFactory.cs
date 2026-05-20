using FOIA.Core;
using FOIA.Definitions;
using FOIA.Runtime;
using UnityEditor;
using UnityEngine;

namespace FOIA.Editor
{
    public static class PocDefinitionAssetFactory
    {
        private const string Root = "Assets/_Project/Data/PocDefinitions";
        private const string CatalogPath = Root + "/카탈로그/정보공개청구 PoC 카탈로그.asset";

        [InitializeOnLoadMethod]
        private static void CreatePocAssetsAfterReload()
        {
            EditorApplication.delayCall += () =>
            {
                if (!AssetDatabase.LoadAssetAtPath<DefinitionCatalog>(CatalogPath))
                    CreatePocAssets();
            };
        }

        [MenuItem("FOIA/PoC 검증용 SO 생성")]
        public static void CreatePocAssets()
        {
            EnsureFolder("Assets/_Project", "Data");
            EnsureFolder("Assets/_Project/Data", "PocDefinitions");
            EnsureFolder(Root, "민원");
            EnsureFolder(Root, "노드");
            EnsureFolder(Root, "엣지");
            EnsureFolder(Root, "엣지블록");
            EnsureFolder(Root, "제작식");
            EnsureFolder(Root, "직원");
            EnsureFolder(Root, "기관");
            EnsureFolder(Root, "카탈로그");

            var simple = CreateComplaint("단순 정보공개 청구", "단순 정보공개 청구",
                Mod(ProcessTag.PublicSatisfaction, 1));
            var privacy = CreateComplaint("개인정보 포함 기록 청구", "개인정보 포함 기록 청구",
                Mod(ProcessTag.PrivacyRisk, 3),
                Mod(ProcessTag.LegalRisk, 1));
            var audit = CreateComplaint("감사 수사 관련 청구", "감사 수사 관련 청구",
                Mod(ProcessTag.LegalRisk, 3),
                Mod(ProcessTag.MissingEvidence, 1));
            var urgent = CreateComplaint("긴급 공익 청구", "긴급 공익 청구",
                Mod(ProcessTag.PublicSatisfaction, 2),
                Mod(ProcessTag.LegalRisk, 1));

            var fastStaff = CreateStaff("빠른 처리 담당자", "빠른 처리 담당자", StaffProfile.FastStressful,
                Mod(ProcessTag.Delay, -2),
                Mod(ProcessTag.AutomationNoise, 1));
            var stableStaff = CreateStaff("신중한 안정 담당자", "신중한 안정 담당자", StaffProfile.SlowStable,
                Mod(ProcessTag.Delay, 1),
                Mod(ProcessTag.Verified, 1));
            var finderStaff = CreateStaff("문제 발견 담당자", "문제 발견 담당자", StaffProfile.ProblemFinder,
                Mod(ProcessTag.MissingEvidence, -2),
                Mod(ProcessTag.LegalRisk, -1),
                Mod(ProcessTag.Delay, 1));

            var slowAgency = CreateAgency("협조적 기록보관부", "협조적 기록보관부", AgencyProfile.CooperativeSlow, 0,
                Mod(ProcessTag.Delay, 2),
                Mod(ProcessTag.AgencyFriction, -1));
            var sensitiveAgency = CreateAgency("신속 민감 감사실", "신속 민감 감사실", AgencyProfile.FastSensitive, 1,
                Mod(ProcessTag.Delay, -2),
                Mod(ProcessTag.AgencyFriction, 1));

            var intake = CreateNode("접수 창구", "접수 창구", NodeKind.Intake);
            var requirement = CreateNode("청구요건 검토", "청구요건 검토", NodeKind.ResultBoard);
            var search = CreateNode("보유부서 조회", "보유부서 조회", NodeKind.Agency);
            var exemption = CreateNode("비공개 사유 검토", "비공개 사유 검토", NodeKind.ResultBoard);
            var partial = CreateNode("부분공개 조정 보드", "부분공개 조정 보드", NodeKind.ResultBoard);
            var notice = CreateNode("최종 결정 통지", "최종 결정 통지", NodeKind.FinalResult);

            var e1 = CreateEdge("접수에서 요건검토", "접수 -> 청구요건 검토", intake, requirement,
                Mod(ProcessTag.MissingEvidence, 1));
            var e2 = CreateEdge("요건검토에서 부서조회", "청구요건 검토 -> 보유부서 조회", requirement, search,
                Mod(ProcessTag.Classified, 1));
            var e3 = CreateEdge("부서조회에서 비공개검토", "보유부서 조회 -> 비공개 사유 검토", search, exemption,
                Mod(ProcessTag.Delay, 1),
                Mod(ProcessTag.AgencyFriction, 1));
            var e4 = CreateEdge("비공개검토에서 부분공개", "비공개 사유 검토 -> 부분공개 조정 보드", exemption, partial,
                Mod(ProcessTag.LegalRisk, -1),
                Mod(ProcessTag.PartialDisclosure, 1));
            var e5 = CreateEdge("부분공개에서 최종통지", "부분공개 조정 보드 -> 최종 결정 통지", partial, notice,
                Mod(ProcessTag.PublicSatisfaction, 1));

            var identity = CreateBlock("신원확인 강화", "신원확인 강화",
                new[] { Mod(ProcessTag.Verified, 2), Mod(ProcessTag.PrivacyRisk, -1), Mod(ProcessTag.Delay, 1) },
                new[] { Bp(ByproductType.PrivacyFragment, 1) });
            var scope = CreateBlock("청구범위 보정요청", "청구범위 보정요청",
                new[] { Mod(ProcessTag.MissingEvidence, -2), Mod(ProcessTag.Delay, 1), Mod(ProcessTag.Verified, 1) },
                new[] { Bp(ByproductType.MissingClue, 1), Bp(ByproductType.ProcessingKnowHow, 1) });
            var autoClassify = CreateBlock("자동 문서분류", "자동 문서분류",
                new[] { Mod(ProcessTag.Classified, 2), Mod(ProcessTag.Delay, -1), Mod(ProcessTag.AutomationNoise, 1) },
                new[] { Bp(ByproductType.ProcessingKnowHow, 1) });
            var foreseeable = CreateBlock("예견가능 피해 테스트", "예견가능 피해 테스트",
                new[] { Mod(ProcessTag.LegalRisk, -2), Mod(ProcessTag.Delay, 1), Mod(ProcessTag.PartialDisclosure, 1) },
                new[] { Bp(ByproductType.LegalConcern, 1) });
            var masking = CreateBlock("개인정보 마스킹", "개인정보 마스킹",
                new[] { Mod(ProcessTag.PrivacyRisk, -3), Mod(ProcessTag.Delay, 1), Mod(ProcessTag.PublicSatisfaction, 1) },
                new[] { Bp(ByproductType.PrivacyFragment, 1), Bp(ByproductType.ProcessingKnowHow, 1) });
            var vaughn = CreateBlock("비공개 사유 색인", "비공개 사유 색인",
                new[] { Mod(ProcessTag.LegalRisk, -2), Mod(ProcessTag.Classified, 1), Mod(ProcessTag.Delay, 2) },
                new[] { Bp(ByproductType.LegalConcern, 1), Bp(ByproductType.MissingClue, 1) });
            var expedite = CreateBlock("긴급처리 트랙", "긴급처리 트랙",
                new[] { Mod(ProcessTag.Delay, -3), Mod(ProcessTag.PublicSatisfaction, 2), Mod(ProcessTag.AgencyFriction, 1) },
                new[] { Bp(ByproductType.ComplaintPressure, 1), Bp(ByproductType.AgencyFriction, 1) });
            var consult = CreateBlock("제3자 의견청취", "제3자 의견청취",
                new[] { Mod(ProcessTag.LegalRisk, -1), Mod(ProcessTag.PrivacyRisk, -1), Mod(ProcessTag.Delay, 2) },
                new[] { Bp(ByproductType.AgencyFriction, 1), Bp(ByproductType.LegalConcern, 1) });
            var partialDisclosure = CreateBlock("부분공개 패키징", "부분공개 패키징",
                new[] { Mod(ProcessTag.PartialDisclosure, 3), Mod(ProcessTag.PublicSatisfaction, 1), Mod(ProcessTag.LegalRisk, -1) },
                new[] { Bp(ByproductType.ProcessingKnowHow, 2) });

            var r1 = CreateRecipe("보정요청 제작", "보정요청 제작",
                new[] { Bp(ByproductType.MissingClue, 1), Bp(ByproductType.ProcessingKnowHow, 1) },
                scope);
            var r2 = CreateRecipe("마스킹 제작", "마스킹 제작",
                new[] { Bp(ByproductType.PrivacyFragment, 2), Bp(ByproductType.LegalConcern, 1) },
                masking);
            var r3 = CreateRecipe("비공개 색인 제작", "비공개 사유 색인 제작",
                new[] { Bp(ByproductType.LegalConcern, 2), Bp(ByproductType.MissingClue, 1) },
                vaughn);
            var r4 = CreateRecipe("긴급처리 제작", "긴급처리 트랙 제작",
                new[] { Bp(ByproductType.ComplaintPressure, 1), Bp(ByproductType.ProcessingKnowHow, 2) },
                expedite);
            var r5 = CreateRecipe("부분공개 제작", "부분공개 패키징 제작",
                new[] { Bp(ByproductType.PrivacyFragment, 1), Bp(ByproductType.LegalConcern, 1), Bp(ByproductType.ProcessingKnowHow, 1) },
                partialDisclosure);

            var catalog = CreateOrLoad<DefinitionCatalog>(CatalogPath);
            var catalogObject = new SerializedObject(catalog);
            SetObjectArray(catalogObject.FindProperty("complaints"), simple, privacy, audit, urgent);
            SetObjectArray(catalogObject.FindProperty("nodes"), intake, requirement, search, exemption, partial, notice);
            SetObjectArray(catalogObject.FindProperty("edges"), e1, e2, e3, e4, e5);
            SetObjectArray(catalogObject.FindProperty("edgeBlocks"), identity, scope, autoClassify, foreseeable, masking, vaughn, expedite, consult, partialDisclosure);
            SetObjectArray(catalogObject.FindProperty("recipes"), r1, r2, r3, r4, r5);
            SetObjectArray(catalogObject.FindProperty("staff"), fastStaff, stableStaff, finderStaff);
            SetObjectArray(catalogObject.FindProperty("agencies"), slowAgency, sensitiveAgency);
            catalogObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = catalog;
            Debug.Log("정보공개청구 PoC 검증용 SO 세트를 생성했습니다.");
        }

        private static ComplaintDefinition CreateComplaint(string id, string displayName, params TagModifier[] initialTags)
        {
            var asset = CreateOrLoad<ComplaintDefinition>($"{Root}/민원/{id}.asset");
            var so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName;
            SetTagModifiers(so.FindProperty("initialTags"), initialTags);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static NodeDefinition CreateNode(string id, string displayName, NodeKind kind)
        {
            var asset = CreateOrLoad<NodeDefinition>($"{Root}/노드/{id}.asset");
            var so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName;
            so.FindProperty("kind").enumValueIndex = (int)kind;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static EdgeDefinition CreateEdge(string id, string displayName, NodeDefinition from, NodeDefinition to, params TagModifier[] baseModifiers)
        {
            var asset = CreateOrLoad<EdgeDefinition>($"{Root}/엣지/{id}.asset");
            var so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName;
            so.FindProperty("from").objectReferenceValue = from;
            so.FindProperty("to").objectReferenceValue = to;
            SetTagModifiers(so.FindProperty("baseModifiers"), baseModifiers);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static StaffDefinition CreateStaff(string id, string displayName, StaffProfile profile, params TagModifier[] tagModifiers)
        {
            var asset = CreateOrLoad<StaffDefinition>($"{Root}/직원/{id}.asset");
            var so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName;
            so.FindProperty("profile").enumValueIndex = (int)profile;
            SetTagModifiers(so.FindProperty("tagModifiers"), tagModifiers);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static AgencyDefinition CreateAgency(string id, string displayName, AgencyProfile profile, int relationshipSensitivity, params TagModifier[] tagModifiers)
        {
            var asset = CreateOrLoad<AgencyDefinition>($"{Root}/기관/{id}.asset");
            var so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName;
            so.FindProperty("profile").enumValueIndex = (int)profile;
            so.FindProperty("relationshipSensitivity").intValue = relationshipSensitivity;
            SetTagModifiers(so.FindProperty("tagModifiers"), tagModifiers);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static EdgeBlockDefinition CreateBlock(string id, string displayName, TagModifier[] tagModifiers, ByproductAmount[] byproducts)
        {
            var asset = CreateOrLoad<EdgeBlockDefinition>($"{Root}/엣지블록/{id}.asset");
            var so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName;
            SetTagModifiers(so.FindProperty("tagModifiers"), tagModifiers);
            SetByproducts(so.FindProperty("byproducts"), byproducts);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static CraftingRecipeDefinition CreateRecipe(string id, string displayName, ByproductAmount[] costs, EdgeBlockDefinition outputBlock)
        {
            var asset = CreateOrLoad<CraftingRecipeDefinition>($"{Root}/제작식/{id}.asset");
            var so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName;
            SetByproducts(so.FindProperty("costs"), costs);
            so.FindProperty("outputBlock").objectReferenceValue = outputBlock;
            so.FindProperty("outputAmount").intValue = 1;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static T CreateOrLoad<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
                return asset;

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void SetTagModifiers(SerializedProperty property, TagModifier[] values)
        {
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
            {
                var element = property.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("tag").enumValueIndex = (int)values[i].tag;
                element.FindPropertyRelative("amount").intValue = values[i].amount;
            }
        }

        private static void SetByproducts(SerializedProperty property, ByproductAmount[] values)
        {
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
            {
                var element = property.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("type").enumValueIndex = (int)values[i].type;
                element.FindPropertyRelative("amount").intValue = values[i].amount;
            }
        }

        private static void SetObjectArray(SerializedProperty property, params Object[] values)
        {
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }

        private static TagModifier Mod(ProcessTag tag, int amount)
        {
            return new TagModifier { tag = tag, amount = amount };
        }

        private static ByproductAmount Bp(ByproductType type, int amount)
        {
            return new ByproductAmount { type = type, amount = amount };
        }

        private static void EnsureFolder(string parent, string child)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{child}"))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}
