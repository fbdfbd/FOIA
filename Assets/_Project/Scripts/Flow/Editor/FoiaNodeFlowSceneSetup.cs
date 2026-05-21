#if UNITY_EDITOR
using System;
using FOIA.Flow.Definitions;
using FOIA.Flow.Presentation;
using FOIA.Flow.Runtime;
using FOIA.Graph.Input;
using FOIA.Graph.Presentation;
using FOIA.Graph.Runtime;
using FOIA.UI.Components;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FOIA.Flow.Editor
{
    public static class FoiaNodeFlowSceneSetup
    {
        private const string RuntimeRootName = "FOIA_NodeFlow_Runtime";
        private const string UiRootName = "FOIA_NodeFlow_Inventory_UI";
        private const string NodeRootName = "FOIA_NodeFlow_Nodes";
        private const string EdgeRootName = "FOIA_NodeFlow_Edges";
        private const string DatabasePath = "Assets/_Project/Data/Flow/Database_Default_FOIA_Flow.asset";

        [MenuItem("FOIA/Setup Node Flow Scene")]
        public static void Setup()
        {
            FoiaFlowDatabase database = LoadOrCreateDatabase();
            Canvas canvas = GetOrCreateCanvas();
            EnsureEventSystem();

            GraphRuntimeStore graphStore = GetOrAdd<GraphRuntimeStore>(GetOrCreateRoot("GraphRuntime"));
            UIEdgeFactory edgeFactory = GetOrAdd<UIEdgeFactory>(GetOrCreateRoot("UIEdgeFactory"));
            ConnectionSystem connectionSystem = GetOrAdd<ConnectionSystem>(GetOrCreateRoot("ConnectionSystem"));

            RectTransform edgeRoot = GetOrCreateRectRoot(canvas.transform, EdgeRootName);
            SetObject(edgeFactory, "edgeRoot", edgeRoot);
            SetObject(connectionSystem, "graphStore", graphStore);
            SetObject(connectionSystem, "edgeFactory", edgeFactory);

            GameObject runtimeRoot = GetOrCreateRoot(RuntimeRootName);
            FlowRuntimeStore flowStore = GetOrAdd<FlowRuntimeStore>(runtimeRoot);
            StaffRuntimeStore staffStore = GetOrAdd<StaffRuntimeStore>(runtimeRoot);
            AgencyRuntimeStore agencyStore = GetOrAdd<AgencyRuntimeStore>(runtimeRoot);
            ProcessStateStore processState = GetOrAdd<ProcessStateStore>(runtimeRoot);
            FlowLogStore logStore = GetOrAdd<FlowLogStore>(runtimeRoot);
            EdgeBlockRuntimeStore edgeBlockStore = GetOrAdd<EdgeBlockRuntimeStore>(runtimeRoot);
            NodeFlowStateStore nodeStateStore = GetOrAdd<NodeFlowStateStore>(runtimeRoot);
            FoiaProcessSystem processSystem = GetOrAdd<FoiaProcessSystem>(runtimeRoot);
            FlowTickSystem tickSystem = GetOrAdd<FlowTickSystem>(runtimeRoot);

            SetObject(staffStore, "database", database);
            SetObject(agencyStore, "database", database);
            SetObject(processSystem, "database", database);
            SetObject(processSystem, "flowStore", flowStore);
            SetObject(processSystem, "staffStore", staffStore);
            SetObject(processSystem, "agencyStore", agencyStore);
            SetObject(processSystem, "processState", processState);
            SetObject(processSystem, "logStore", logStore);
            SetObject(processSystem, "edgeBlockStore", edgeBlockStore);
            SetObject(processSystem, "graphStore", graphStore);
            SetObject(processSystem, "nodeStateStore", nodeStateStore);
            SetObject(tickSystem, "processSystem", processSystem);

            RectTransform nodeRoot = GetOrCreateRectRoot(canvas.transform, NodeRootName);
            nodeRoot.anchorMin = new Vector2(0f, 0.34f);
            nodeRoot.anchorMax = Vector2.one;
            nodeRoot.offsetMin = new Vector2(24f, 24f);
            nodeRoot.offsetMax = new Vector2(-24f, -24f);

            CreateFlowNode(nodeRoot, "Node_Intake", "접수", NodeFlowRole.Intake, null, new Vector2(150f, -180f), graphStore);
            CreateFlowNode(nodeRoot, "Node_Board", "대기열", NodeFlowRole.Board, null, new Vector2(470f, -180f), graphStore);

            for (int i = 0; i < database.Agencies.Count; i++)
            {
                AgencyDefinition agency = database.Agencies[i];
                CreateFlowNode(nodeRoot, $"Node_Agency_{i + 1}", agency.DisplayName, NodeFlowRole.Agency, agency, new Vector2(790f, -90f - 130f * i), graphStore);
            }

            CreateFlowNode(nodeRoot, "Node_Output", "결과물", NodeFlowRole.Output, null, new Vector2(1130f, -180f), graphStore);
            CreateInventoryUi(canvas.transform, database, flowStore, staffStore, agencyStore, processState, processSystem, logStore);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Selection.activeGameObject = nodeRoot.gameObject;
        }

        private static void CreateInventoryUi(
            Transform canvas,
            FoiaFlowDatabase database,
            FlowRuntimeStore flowStore,
            StaffRuntimeStore staffStore,
            AgencyRuntimeStore agencyStore,
            ProcessStateStore processState,
            FoiaProcessSystem processSystem,
            FlowLogStore logStore)
        {
            RectTransform root = GetOrCreateRectRoot(canvas, UiRootName);
            root.anchorMin = Vector2.zero;
            root.anchorMax = new Vector2(1f, 0.34f);
            root.offsetMin = new Vector2(16f, 12f);
            root.offsetMax = new Vector2(-16f, -8f);

            RectTransform agencyPanel = CreatePanel(root, "AgencyStatus", new Vector2(0f, 0f), new Vector2(0.17f, 1f));
            CreateLabel(agencyPanel, "기관");
            RectTransform agencyContent = CreateContentRoot(agencyPanel, "Content", new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.92f));
            AgencyListView agencyListView = GetOrAdd<AgencyListView>(agencyContent.gameObject);
            SetObject(agencyListView, "agencyStore", agencyStore);
            SetObject(agencyListView, "processSystem", processSystem);
            SetObject(agencyListView, "contentRoot", agencyContent);

            RectTransform staffPanel = CreatePanel(root, "StaffInventory", new Vector2(0f, 0f), new Vector2(0.24f, 1f));
            CreateLabel(staffPanel, "직원");
            RectTransform staffContent = CreateContentRoot(staffPanel, "Content", new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.72f));
            StaffInventoryView staffView = GetOrAdd<StaffInventoryView>(staffContent.gameObject);
            SetObject(staffView, "staffStore", staffStore);
            SetObject(staffView, "processState", processState);
            SetObject(staffView, "contentRoot", staffContent);

            RectTransform itemPanel = CreatePanel(root, "ByproductInventory", new Vector2(0.25f, 0f), new Vector2(0.48f, 1f));
            CreateLabel(itemPanel, "부산물");
            RectTransform itemContent = CreateContentRoot(itemPanel, "Content", new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.72f));
            FlowContainerView itemView = GetOrAdd<FlowContainerView>(itemContent.gameObject);
            SetObject(itemView, "flowStore", flowStore);
            SetString(itemView, "containerId", "inventory");
            SetObject(itemView, "contentRoot", itemContent);

            RectTransform blockPanel = CreatePanel(root, "BlockInventory", new Vector2(0.49f, 0f), new Vector2(0.66f, 1f));
            CreateLabel(blockPanel, "엣지블럭");
            RectTransform blockContent = CreateContentRoot(blockPanel, "Content", new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.72f));
            FlowContainerView blockView = GetOrAdd<FlowContainerView>(blockContent.gameObject);
            SetObject(blockView, "flowStore", flowStore);
            SetString(blockView, "containerId", "blocks");
            SetObject(blockView, "contentRoot", blockContent);

            RectTransform craftPanel = CreatePanel(root, "CraftArea", new Vector2(0.67f, 0f), new Vector2(0.82f, 1f));
            CreateLabel(craftPanel, "조합");
            RectTransform craftSlot1 = CreateSlot(craftPanel, "CraftSlot1", "재료 1", new Vector2(0.06f, 0.44f), new Vector2(0.44f, 0.68f));
            RectTransform craftSlot2 = CreateSlot(craftPanel, "CraftSlot2", "재료 2", new Vector2(0.56f, 0.44f), new Vector2(0.94f, 0.68f));
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

            Button craftButton = CreateButton(craftPanel, "CraftButton", "조합", new Vector2(0.06f, 0.16f), new Vector2(0.94f, 0.34f));
            CraftButtonView craftButtonView = GetOrAdd<CraftButtonView>(craftButton.gameObject);
            SetObject(craftButtonView, "processSystem", processSystem);

            RectTransform logPanel = CreatePanel(root, "LogPanel", new Vector2(0.83f, 0f), Vector2.one);
            TMP_Text logText = CreateText(logPanel, "LogText", "", 16f, TextAlignmentOptions.TopLeft, Vector2.zero, Vector2.one);
            FlowLogView logView = GetOrAdd<FlowLogView>(logPanel.gameObject);
            SetObject(logView, "logStore", logStore);
            SetObject(logView, "label", logText);
        }

        private static RectTransform CreateFlowNode(
            Transform parent,
            string name,
            string label,
            NodeFlowRole role,
            AgencyDefinition agency,
            Vector2 position,
            GraphRuntimeStore graphStore)
        {
            Transform existing = parent.Find(name);
            GameObject nodeObject = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

            nodeObject.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)nodeObject.transform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = role == NodeFlowRole.Board ? new Vector2(190f, 140f) : new Vector2(160f, 110f);

            Image image = nodeObject.GetComponent<Image>();
            image.color = role switch
            {
                NodeFlowRole.Intake => new Color(0.12f, 0.35f, 0.45f, 0.9f),
                NodeFlowRole.Board => new Color(0.28f, 0.18f, 0.38f, 0.9f),
                NodeFlowRole.Agency => new Color(0.18f, 0.22f, 0.24f, 0.9f),
                NodeFlowRole.Output => new Color(0.18f, 0.35f, 0.20f, 0.9f),
                _ => new Color(0.2f, 0.2f, 0.2f, 0.9f),
            };
            image.raycastTarget = true;

            NodeEntity node = GetOrAdd<NodeEntity>(nodeObject);
            EnsureNodeId(node);
            SetObject(node, "graphStore", graphStore);
            GetOrAdd<UIClickable>(nodeObject);
            GetOrAdd<UIRightClickable>(nodeObject);
            GetOrAdd<NodeSelectionInput>(nodeObject);
            GetOrAdd<NodeConnectionInput>(nodeObject);

            NodeFlowData flowData = GetOrAdd<NodeFlowData>(nodeObject);
            SetInt(flowData, "role", (int)role);
            SetObject(flowData, "agency", agency);

            TMP_Text nodeLabel = CreateText(rect, "FlowLabel", label, 18f, TextAlignmentOptions.Center, Vector2.zero, Vector2.one);
            NodeFlowView view = GetOrAdd<NodeFlowView>(nodeObject);
            SetObject(view, "label", nodeLabel);
            view.Refresh();
            return rect;
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
            Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();

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
            if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }

        private static GameObject GetOrCreateRoot(string name)
        {
            GameObject existing = GameObject.Find(name);
            return existing != null ? existing : new GameObject(name);
        }

        private static RectTransform GetOrCreateRectRoot(Transform parent, string name)
        {
            Transform existing = parent.Find(name);

            if (existing != null)
            {
                return (RectTransform)existing;
            }

            GameObject root = new(name, typeof(RectTransform));
            RectTransform rect = (RectTransform)root.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        private static RectTransform CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            Transform existing = parent.Find(name);
            GameObject panel = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

            panel.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)panel.transform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = new Vector2(4f, 4f);
            rect.offsetMax = new Vector2(-4f, -4f);
            Image image = panel.GetComponent<Image>();
            image.color = new Color(0.08f, 0.08f, 0.10f, 0.82f);
            image.raycastTarget = true;
            return rect;
        }

        private static RectTransform CreateContentRoot(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform rect = CreatePanel(parent, name, anchorMin, anchorMax);
            rect.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
            return rect;
        }

        private static RectTransform CreateSlot(Transform parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform slot = CreatePanel(parent, name, anchorMin, anchorMax);
            slot.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.07f, 0.9f);
            CreateText(slot, "Label", text, 18f, TextAlignmentOptions.Center, Vector2.zero, Vector2.one);
            return slot;
        }

        private static Button CreateButton(Transform parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform rect = CreatePanel(parent, name, anchorMin, anchorMax);
            rect.GetComponent<Image>().color = new Color(0.15f, 0.55f, 0.8f, 1f);
            Button button = GetOrAdd<Button>(rect.gameObject);
            CreateText(rect, "Label", text, 18f, TextAlignmentOptions.Center, Vector2.zero, Vector2.one);
            return button;
        }

        private static void CreateLabel(RectTransform parent, string text)
        {
            CreateText(parent, "Title", text, 22f, TextAlignmentOptions.Center, new Vector2(0f, 0.74f), Vector2.one);
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

        private static T GetOrAdd<T>(GameObject target) where T : Component
        {
            if (!target.TryGetComponent(out T component))
            {
                component = target.AddComponent<T>();
            }

            return component;
        }

        private static void EnsureNodeId(NodeEntity node)
        {
            SerializedObject serializedObject = new(node);
            SerializedProperty nodeId = serializedObject.FindProperty("nodeId");

            if (string.IsNullOrEmpty(nodeId.stringValue))
            {
                nodeId.stringValue = Guid.NewGuid().ToString("N");
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void SetObject(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
        {
            Apply(target, propertyName, property => property.objectReferenceValue = value);
        }

        private static void SetString(UnityEngine.Object target, string propertyName, string value)
        {
            Apply(target, propertyName, property => property.stringValue = value);
        }

        private static void SetInt(UnityEngine.Object target, string propertyName, int value)
        {
            Apply(target, propertyName, property => property.intValue = value);
        }

        private static void Apply(UnityEngine.Object target, string propertyName, Action<SerializedProperty> action)
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
    }
}
#endif
