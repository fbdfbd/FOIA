#if UNITY_EDITOR
using FOIA.Flow.Definitions;
using FOIA.Flow.Presentation;
using FOIA.Flow.Runtime;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FOIA.Flow.Editor
{
    public static class FoiaProcessSceneSetup
    {
        private const string UiRootName = "FOIA_Process_UI";
        private const string RuntimeRootName = "FOIA_Process_Runtime";
        private const string DatabasePath = "Assets/_Project/Data/Flow/Database_Default_FOIA_Flow.asset";

        [MenuItem("FOIA/Setup Process Scene UI")]
        public static void Setup()
        {
            FoiaFlowDatabase database = LoadOrCreateDatabase();
            GameObject runtimeRoot = GetOrCreateRoot(RuntimeRootName);

            FlowRuntimeStore flowStore = GetOrAdd<FlowRuntimeStore>(runtimeRoot);
            StaffRuntimeStore staffStore = GetOrAdd<StaffRuntimeStore>(runtimeRoot);
            AgencyRuntimeStore agencyStore = GetOrAdd<AgencyRuntimeStore>(runtimeRoot);
            ProcessStateStore processState = GetOrAdd<ProcessStateStore>(runtimeRoot);
            FlowLogStore logStore = GetOrAdd<FlowLogStore>(runtimeRoot);
            FoiaProcessSystem processSystem = GetOrAdd<FoiaProcessSystem>(runtimeRoot);

            SetObject(staffStore, "database", database);
            SetObject(agencyStore, "database", database);
            SetObject(processSystem, "database", database);
            SetObject(processSystem, "flowStore", flowStore);
            SetObject(processSystem, "staffStore", staffStore);
            SetObject(processSystem, "agencyStore", agencyStore);
            SetObject(processSystem, "processState", processState);
            SetObject(processSystem, "logStore", logStore);

            Canvas canvas = GetOrCreateCanvas();
            EnsureEventSystem();

            GameObject uiRoot = GetOrCreateUiRoot(canvas.transform);
            RectTransform uiRect = (RectTransform)uiRoot.transform;
            Stretch(uiRect);

            RectTransform topBoard = CreatePanel(uiRoot.transform, "TopBoard", new Vector2(0f, 0.35f), new Vector2(1f, 1f), new Vector2(16f, 16f), new Vector2(-16f, -16f));
            RectTransform bottomInventory = CreatePanel(uiRoot.transform, "BottomInventory", new Vector2(0f, 0f), new Vector2(1f, 0.35f), new Vector2(16f, 16f), new Vector2(-16f, -10f));

            RectTransform intakeArea = CreatePanel(topBoard, "IntakeArea", new Vector2(0f, 0.05f), new Vector2(0.16f, 0.95f), Vector2.zero, Vector2.zero);
            RectTransform edgeArea = CreatePanel(topBoard, "EdgeBlockArea", new Vector2(0.18f, 0.25f), new Vector2(0.31f, 0.75f), Vector2.zero, Vector2.zero);
            RectTransform boardArea = CreatePanel(topBoard, "BoardArea", new Vector2(0.33f, 0.05f), new Vector2(0.49f, 0.95f), Vector2.zero, Vector2.zero);
            RectTransform agencyArea = CreatePanel(topBoard, "AgencyArea", new Vector2(0.51f, 0.05f), new Vector2(0.68f, 0.95f), Vector2.zero, Vector2.zero);
            RectTransform logArea = CreatePanel(topBoard, "LogArea", new Vector2(0.70f, 0.05f), new Vector2(1f, 0.95f), Vector2.zero, Vector2.zero);

            CreateLabel(intakeArea, "접수", new Vector2(0f, 0.82f), new Vector2(1f, 1f));
            RectTransform intakeSlot = CreateSlot(intakeArea, "IntakeSlot", "직원 배치", new Vector2(0.08f, 0.42f), new Vector2(0.92f, 0.78f));
            IntakeStaffSlotView intakeView = GetOrAdd<IntakeStaffSlotView>(intakeSlot.gameObject);
            SetObject(intakeView, "processSystem", processSystem);
            SetObject(intakeView, "staffStore", staffStore);
            SetObject(intakeView, "processState", processState);
            SetObject(intakeView, "label", intakeSlot.GetComponentInChildren<TMP_Text>());

            Button startButton = CreateButton(intakeArea, "StartIntakeButton", "민원 접수 시작", new Vector2(0.08f, 0.20f), new Vector2(0.92f, 0.34f));
            StartIntakeButtonView startView = GetOrAdd<StartIntakeButtonView>(startButton.gameObject);
            SetObject(startView, "processSystem", processSystem);
            SetObject(startView, "processState", processState);

            CreateLabel(edgeArea, "시스템 엣지", new Vector2(0f, 0.78f), new Vector2(1f, 1f));
            RectTransform edgeSlot = CreateSlot(edgeArea, "EdgeBlockSlot", "엣지 블럭", new Vector2(0.10f, 0.36f), new Vector2(0.90f, 0.72f));
            EdgeBlockSlotView edgeView = GetOrAdd<EdgeBlockSlotView>(edgeSlot.gameObject);
            SetObject(edgeView, "processSystem", processSystem);
            SetObject(edgeView, "processState", processState);
            SetObject(edgeView, "label", edgeSlot.GetComponentInChildren<TMP_Text>());

            CreateLabel(boardArea, "분기 대기열", new Vector2(0f, 0.82f), new Vector2(1f, 1f));
            RectTransform boardSlot = CreateSlot(boardArea, "BoardDocument", "대기중인 민원 없음", new Vector2(0.08f, 0.22f), new Vector2(0.92f, 0.76f));
            BoardDocumentView boardView = GetOrAdd<BoardDocumentView>(boardSlot.gameObject);
            SetObject(boardView, "processState", processState);
            SetObject(boardView, "label", boardSlot.GetComponentInChildren<TMP_Text>());

            CreateLabel(agencyArea, "기관", new Vector2(0f, 0.88f), new Vector2(1f, 1f));
            RectTransform agencyList = CreateContentRoot(agencyArea, "AgencyList", new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.86f));
            AgencyListView agencyListView = GetOrAdd<AgencyListView>(agencyList.gameObject);
            SetObject(agencyListView, "agencyStore", agencyStore);
            SetObject(agencyListView, "processSystem", processSystem);
            SetObject(agencyListView, "contentRoot", agencyList);

            RectTransform logTextRoot = CreatePanel(logArea, "LogPanel", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            TMP_Text logText = CreateText(logTextRoot, "LogText", "", 18f, TextAlignmentOptions.TopLeft, Vector2.zero, Vector2.one);
            FlowLogView logView = GetOrAdd<FlowLogView>(logTextRoot.gameObject);
            SetObject(logView, "logStore", logStore);
            SetObject(logView, "label", logText);

            RectTransform staffInventory = CreatePanel(bottomInventory, "StaffInventory", new Vector2(0f, 0f), new Vector2(0.26f, 1f), Vector2.zero, Vector2.zero);
            CreateLabel(staffInventory, "직원", new Vector2(0f, 0.78f), new Vector2(1f, 1f));
            RectTransform staffContent = CreateContentRoot(staffInventory, "Content", new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.74f));
            StaffInventoryView staffView = GetOrAdd<StaffInventoryView>(staffContent.gameObject);
            SetObject(staffView, "staffStore", staffStore);
            SetObject(staffView, "processState", processState);
            SetObject(staffView, "contentRoot", staffContent);

            RectTransform itemInventory = CreatePanel(bottomInventory, "ByproductInventory", new Vector2(0.28f, 0f), new Vector2(0.56f, 1f), Vector2.zero, Vector2.zero);
            CreateLabel(itemInventory, "부산물", new Vector2(0f, 0.78f), new Vector2(1f, 1f));
            RectTransform itemContent = CreateContentRoot(itemInventory, "Content", new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.74f));
            FlowContainerView itemView = GetOrAdd<FlowContainerView>(itemContent.gameObject);
            SetObject(itemView, "flowStore", flowStore);
            SetString(itemView, "containerId", "inventory");
            SetObject(itemView, "contentRoot", itemContent);

            RectTransform blockInventory = CreatePanel(bottomInventory, "BlockInventory", new Vector2(0.58f, 0f), new Vector2(0.78f, 1f), Vector2.zero, Vector2.zero);
            CreateLabel(blockInventory, "엣지블럭", new Vector2(0f, 0.78f), new Vector2(1f, 1f));
            RectTransform blockContent = CreateContentRoot(blockInventory, "Content", new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.74f));
            FlowContainerView blockView = GetOrAdd<FlowContainerView>(blockContent.gameObject);
            SetObject(blockView, "flowStore", flowStore);
            SetString(blockView, "containerId", "blocks");
            SetObject(blockView, "contentRoot", blockContent);

            RectTransform craftArea = CreatePanel(bottomInventory, "CraftArea", new Vector2(0.80f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            CreateLabel(craftArea, "조합", new Vector2(0f, 0.78f), new Vector2(1f, 1f));
            RectTransform craftSlot1 = CreateSlot(craftArea, "CraftSlot1", "재료 1", new Vector2(0.06f, 0.42f), new Vector2(0.44f, 0.68f));
            RectTransform craftSlot2 = CreateSlot(craftArea, "CraftSlot2", "재료 2", new Vector2(0.56f, 0.42f), new Vector2(0.94f, 0.68f));
            CraftSlotView craftView1 = GetOrAdd<CraftSlotView>(craftSlot1.gameObject);
            CraftSlotView craftView2 = GetOrAdd<CraftSlotView>(craftSlot2.gameObject);
            SetInt(craftView1, "slotIndex", 0);
            SetObject(craftView1, "processSystem", processSystem);
            SetObject(craftView1, "processState", processState);
            SetObject(craftView1, "label", craftSlot1.GetComponentInChildren<TMP_Text>());
            SetInt(craftView2, "slotIndex", 1);
            SetObject(craftView2, "processSystem", processSystem);
            SetObject(craftView2, "processState", processState);
            SetObject(craftView2, "label", craftSlot2.GetComponentInChildren<TMP_Text>());

            Button craftButton = CreateButton(craftArea, "CraftButton", "조합", new Vector2(0.06f, 0.16f), new Vector2(0.94f, 0.32f));
            CraftButtonView craftButtonView = GetOrAdd<CraftButtonView>(craftButton.gameObject);
            SetObject(craftButtonView, "processSystem", processSystem);

            MarkDirty(runtimeRoot, uiRoot);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Selection.activeGameObject = uiRoot;
        }

        private static FoiaFlowDatabase LoadOrCreateDatabase()
        {
            FoiaFlowDatabase database = AssetDatabase.LoadAssetAtPath<FoiaFlowDatabase>(DatabasePath);

            if (database == null)
            {
                DefaultFoiaFlowDataGenerator.Generate();
                database = AssetDatabase.LoadAssetAtPath<FoiaFlowDatabase>(DatabasePath);
            }

            return database;
        }

        private static Canvas GetOrCreateCanvas()
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();

            if (canvas != null)
            {
                return canvas;
            }

            GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            return canvas;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private static GameObject GetOrCreateRoot(string name)
        {
            GameObject existing = GameObject.Find(name);
            return existing != null ? existing : new GameObject(name);
        }

        private static GameObject GetOrCreateUiRoot(Transform parent)
        {
            Transform existing = parent.Find(UiRootName);

            if (existing != null)
            {
                return existing.gameObject;
            }

            GameObject root = new(UiRootName, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            return root;
        }

        private static RectTransform CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            Transform existing = parent.Find(name);
            GameObject panel = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

            panel.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)panel.transform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            Image image = panel.GetComponent<Image>();
            image.color = new Color(0.12f, 0.12f, 0.14f, 0.65f);
            image.raycastTarget = true;
            return rect;
        }

        private static RectTransform CreateContentRoot(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform rect = CreatePanel(parent, name, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
            rect.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
            return rect;
        }

        private static RectTransform CreateSlot(Transform parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform slot = CreatePanel(parent, name, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
            slot.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.07f, 0.85f);
            CreateText(slot, "Label", text, 20f, TextAlignmentOptions.Center, Vector2.zero, Vector2.one);
            return slot;
        }

        private static Button CreateButton(Transform parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform rect = CreatePanel(parent, name, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
            Image image = rect.GetComponent<Image>();
            image.color = new Color(0.15f, 0.65f, 0.9f, 1f);

            Button button = GetOrAdd<Button>(rect.gameObject);
            CreateText(rect, "Label", text, 20f, TextAlignmentOptions.Center, Vector2.zero, Vector2.one);
            return button;
        }

        private static TMP_Text CreateLabel(Transform parent, string text, Vector2 anchorMin, Vector2 anchorMax)
        {
            return CreateText((RectTransform)parent, "Title", text, 24f, TextAlignmentOptions.Center, anchorMin, anchorMax);
        }

        private static TMP_Text CreateText(RectTransform parent, string name, string text, float fontSize, TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax)
        {
            Transform existing = parent.Find(name);
            GameObject textObject = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));

            textObject.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)textObject.transform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = new Vector2(8f, 6f);
            rect.offsetMax = new Vector2(-8f, -6f);

            TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
            FlowTextStyle.Apply(label);
            label.text = text;
            label.fontSize = fontSize;
            label.alignment = alignment;
            label.color = Color.white;
            label.raycastTarget = false;
            label.textWrappingMode = TextWrappingModes.Normal;
            label.overflowMode = TextOverflowModes.Ellipsis;
            return label;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static T GetOrAdd<T>(GameObject target) where T : Component
        {
            if (!target.TryGetComponent(out T component))
            {
                component = target.AddComponent<T>();
            }

            return component;
        }

        private static void SetObject(Object target, string propertyName, Object value)
        {
            Apply(target, propertyName, property => property.objectReferenceValue = value);
        }

        private static void SetString(Object target, string propertyName, string value)
        {
            Apply(target, propertyName, property => property.stringValue = value);
        }

        private static void SetInt(Object target, string propertyName, int value)
        {
            Apply(target, propertyName, property => property.intValue = value);
        }

        private static void Apply(Object target, string propertyName, System.Action<SerializedProperty> action)
        {
            SerializedObject serializedObject = new(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            if (property == null)
            {
                return;
            }

            action(property);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void MarkDirty(params Object[] objects)
        {
            foreach (Object target in objects)
            {
                if (target != null)
                {
                    EditorUtility.SetDirty(target);
                }
            }
        }
    }
}
#endif
