using FOIA.Core;
using FOIA.Definitions;
using FOIA.Infrastructure;
using FOIA.Input;
using FOIA.Presentation;
using FOIA.Runtime;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FOIA.Editor
{
    public static class PocSampleSceneSetup
    {
        private const string ScenePath = "Assets/_Project/Scenes/SampleScene.unity";
        private const string RootName = "FOIA PoC Test Board";
        private const string DataRoot = "Assets/_Project/Data/PocDefinitions";

        [InitializeOnLoadMethod]
        private static void AutoSetupAfterReload()
        {
            EditorApplication.delayCall += () =>
            {
                if (Application.isPlaying)
                    return;

                var scene = EditorSceneManager.GetActiveScene();
                if (scene.path != ScenePath)
                    return;

                if (GameObject.Find(RootName) != null)
                    return;

                SetupSampleScene();
            };
        }

        [MenuItem("FOIA/샘플 씬 테스트 보드 생성")]
        public static void SetupSampleScene()
        {
            PocDefinitionAssetFactory.CreatePocAssets();

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var oldRoot = GameObject.Find(RootName);
            if (oldRoot != null)
                Object.DestroyImmediate(oldRoot);

            var catalog = Load<DefinitionCatalog>($"{DataRoot}/카탈로그/정보공개청구 PoC 카탈로그.asset");
            var complaint = Load<ComplaintDefinition>($"{DataRoot}/민원/개인정보 포함 기록 청구.asset");
            var staff = Load<StaffDefinition>($"{DataRoot}/직원/빠른 처리 담당자.asset");
            var agency = Load<AgencyDefinition>($"{DataRoot}/기관/신속 민감 감사실.asset");

            var root = new GameObject(RootName);
            var camera = SetupCamera();

            var pointer = NewChild<BoardPointerInput>(root.transform, "Pointer Input");
            SetObject(pointer, "boardCamera", camera);

            var runController = NewChild<ProcessRunController>(root.transform, "Process Runner");
            SetObject(runController, "testComplaint", complaint);
            SetObject(runController, "selectedStaff", staff);
            SetObject(runController, "selectedAgency", agency);
            SetObjectArray(runController, "path",
                Load<EdgeDefinition>($"{DataRoot}/엣지/접수에서 요건검토.asset"),
                Load<EdgeDefinition>($"{DataRoot}/엣지/요건검토에서 부서조회.asset"),
                Load<EdgeDefinition>($"{DataRoot}/엣지/부서조회에서 비공개검토.asset"),
                Load<EdgeDefinition>($"{DataRoot}/엣지/비공개검토에서 부분공개.asset"),
                Load<EdgeDefinition>($"{DataRoot}/엣지/부분공개에서 최종통지.asset"));

            var dragController = NewChild<EdgeBlockDragController>(root.transform, "Edge Block Drag Controller");
            SetObject(dragController, "pointerInput", pointer);

            var bootstrapper = NewChild<PocStateBootstrapper>(root.transform, "Starter Inventory");
            SetStarterInventory(bootstrapper,
                Load<EdgeBlockDefinition>($"{DataRoot}/엣지블록/개인정보 마스킹.asset"),
                Load<EdgeBlockDefinition>($"{DataRoot}/엣지블록/긴급처리 트랙.asset"),
                Load<EdgeBlockDefinition>($"{DataRoot}/엣지블록/자동 문서분류.asset"));

            var keyboard = NewChild<PocKeyboardTestRunner>(root.transform, "Keyboard Test Runner");
            SetObject(keyboard, "processRunController", runController);

            var scope = root.AddComponent<PocLifetimeScope>();
            SetObject(scope, "catalog", catalog);
            SetObject(scope, "edgeBlockDragController", dragController);
            SetObject(scope, "processRunController", runController);
            SetObject(scope, "stateBootstrapper", bootstrapper);

            CreateBoard(root.transform, runController);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Selection.activeObject = root;
            Debug.Log("SampleScene에 FOIA PoC 테스트 보드를 생성했습니다. Play 후 Space를 눌러 실행하세요.");
        }

        private static void CreateBoard(Transform root, ProcessRunController runController)
        {
            CreateLabel(root, "Title", new Vector3(0f, 4.2f, 0f), "FOIA PoC Test\nSpace: run / Drag blocks to edge slots", 0.45f);

            CreateNode(root, "접수 창구", new Vector3(-6f, 1.5f, 0f), Load<NodeDefinition>($"{DataRoot}/노드/접수 창구.asset"));
            CreateNode(root, "청구요건 검토", new Vector3(-3.5f, 1.5f, 0f), Load<NodeDefinition>($"{DataRoot}/노드/청구요건 검토.asset"));
            CreateNode(root, "보유부서 조회", new Vector3(-1f, 1.5f, 0f), Load<NodeDefinition>($"{DataRoot}/노드/보유부서 조회.asset"));
            CreateNode(root, "비공개 사유 검토", new Vector3(1.5f, 1.5f, 0f), Load<NodeDefinition>($"{DataRoot}/노드/비공개 사유 검토.asset"));
            CreateNode(root, "부분공개 조정", new Vector3(4f, 1.5f, 0f), Load<NodeDefinition>($"{DataRoot}/노드/부분공개 조정 보드.asset"));
            CreateNode(root, "최종 결정 통지", new Vector3(6.5f, 1.5f, 0f), Load<NodeDefinition>($"{DataRoot}/노드/최종 결정 통지.asset"));

            CreateEdgeSlot(root, "접수에서 요건검토", new Vector3(-4.75f, 0.25f, 0f), Load<EdgeDefinition>($"{DataRoot}/엣지/접수에서 요건검토.asset"));
            CreateEdgeSlot(root, "요건검토에서 부서조회", new Vector3(-2.25f, 0.25f, 0f), Load<EdgeDefinition>($"{DataRoot}/엣지/요건검토에서 부서조회.asset"));
            CreateEdgeSlot(root, "부서조회에서 비공개검토", new Vector3(0.25f, 0.25f, 0f), Load<EdgeDefinition>($"{DataRoot}/엣지/부서조회에서 비공개검토.asset"));
            CreateEdgeSlot(root, "비공개검토에서 부분공개", new Vector3(2.75f, 0.25f, 0f), Load<EdgeDefinition>($"{DataRoot}/엣지/비공개검토에서 부분공개.asset"));
            CreateEdgeSlot(root, "부분공개에서 최종통지", new Vector3(5.25f, 0.25f, 0f), Load<EdgeDefinition>($"{DataRoot}/엣지/부분공개에서 최종통지.asset"));

            CreateBlock(root, "개인정보 마스킹", new Vector3(-3.5f, -2.5f, 0f), Load<EdgeBlockDefinition>($"{DataRoot}/엣지블록/개인정보 마스킹.asset"));
            CreateBlock(root, "긴급처리 트랙", new Vector3(0f, -2.5f, 0f), Load<EdgeBlockDefinition>($"{DataRoot}/엣지블록/긴급처리 트랙.asset"));
            CreateBlock(root, "자동 문서분류", new Vector3(3.5f, -2.5f, 0f), Load<EdgeBlockDefinition>($"{DataRoot}/엣지블록/자동 문서분류.asset"));

            CreateEffectText(root, runController, "직원 효과", new Vector3(-6.2f, -1.2f, 0f), EffectTargetType.Staff, "빠른 처리 담당자");
            CreateEffectText(root, runController, "기관 효과", new Vector3(-2.2f, -1.2f, 0f), EffectTargetType.Agency, "신속 민감 감사실");
            CreateEffectText(root, runController, "노드 효과", new Vector3(1.8f, -1.2f, 0f), EffectTargetType.Node, "청구요건 검토");
            CreateEffectText(root, runController, "민원 효과", new Vector3(5.8f, -1.2f, 0f), EffectTargetType.Complaint, "개인정보 포함 기록 청구");
        }

        private static Camera SetupCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var go = new GameObject("Main Camera");
                camera = go.AddComponent<Camera>();
                go.tag = "MainCamera";
            }

            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.transform.rotation = Quaternion.identity;
            camera.orthographic = true;
            camera.orthographicSize = 5.2f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.08f, 0.09f, 0.1f);
            return camera;
        }

        private static void CreateNode(Transform root, string name, Vector3 position, NodeDefinition definition)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = $"Node - {name}";
            go.transform.SetParent(root);
            go.transform.position = position;
            go.transform.localScale = new Vector3(1.8f, 0.75f, 0.2f);
            go.GetComponent<Renderer>().sharedMaterial = MakeMaterial(new Color(0.18f, 0.32f, 0.42f));
            var view = go.AddComponent<NodeView>();
            SetObject(view, "definition", definition);
            CreateLabel(go.transform, "Label", new Vector3(0f, 0f, -0.2f), name, 0.22f);
        }

        private static void CreateEdgeSlot(Transform root, string name, Vector3 position, EdgeDefinition definition)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = $"Edge Slot - {name}";
            go.transform.SetParent(root);
            go.transform.position = position;
            go.transform.localScale = new Vector3(1.45f, 0.45f, 0.2f);
            go.GetComponent<Renderer>().sharedMaterial = MakeMaterial(new Color(0.32f, 0.29f, 0.16f));
            var view = go.AddComponent<EdgeSlotView>();
            SetObject(view, "definition", definition);
            CreateLabel(go.transform, "Label", new Vector3(0f, 0f, -0.2f), "slot", 0.2f);
        }

        private static void CreateBlock(Transform root, string name, Vector3 position, EdgeBlockDefinition definition)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = $"Block - {name}";
            go.transform.SetParent(root);
            go.transform.position = position;
            go.transform.localScale = new Vector3(2.2f, 0.75f, 0.2f);
            go.GetComponent<Renderer>().sharedMaterial = MakeMaterial(new Color(0.42f, 0.23f, 0.28f));
            var view = go.AddComponent<EdgeBlockView>();
            SetObject(view, "definition", definition);
            CreateLabel(go.transform, "Label", new Vector3(0f, 0f, -0.2f), name, 0.22f);
        }

        private static void CreateEffectText(Transform root, ProcessRunController runController, string name, Vector3 position, EffectTargetType targetType, string targetId)
        {
            var text = CreateLabel(root, name, position, $"{name}\n-", 0.24f);
            var view = text.gameObject.AddComponent<ProcessEffectTextView>();
            SetObject(view, "processRunController", runController);
            SetEnum(view, "targetType", (int)targetType);
            SetString(view, "targetId", targetId);
            SetObject(view, "text", text);
            SetString(view, "emptyText", $"{name}\n-");
        }

        private static TMP_Text CreateLabel(Transform parent, string name, Vector3 localPosition, string value, float size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.identity;
            var text = go.AddComponent<TextMeshPro>();
            text.text = value;
            text.fontSize = size * 10f;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.rectTransform.sizeDelta = new Vector2(4f, 1.2f);
            return text;
        }

        private static T NewChild<T>(Transform parent, string name) where T : Component
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            return go.AddComponent<T>();
        }

        private static T Load<T>(string path) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private static Material MakeMaterial(Color color)
        {
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            material.color = color;
            return material;
        }

        private static void SetObject(Object target, string propertyName, Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(propertyName).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetString(Object target, string propertyName, string value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(propertyName).stringValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetEnum(Object target, string propertyName, int value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(propertyName).enumValueIndex = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObjectArray(Object target, string propertyName, params Object[] values)
        {
            var so = new SerializedObject(target);
            var property = so.FindProperty(propertyName);
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetStarterInventory(PocStateBootstrapper target, params EdgeBlockDefinition[] blocks)
        {
            var so = new SerializedObject(target);
            var property = so.FindProperty("startingBlocks");
            property.arraySize = blocks.Length;
            for (var i = 0; i < blocks.Length; i++)
            {
                var element = property.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("block").objectReferenceValue = blocks[i];
                element.FindPropertyRelative("amount").intValue = 1;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
