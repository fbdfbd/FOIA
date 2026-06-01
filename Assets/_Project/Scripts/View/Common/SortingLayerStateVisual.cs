using UnityEngine;

namespace OneMoreSpoon.View.Common
{
    public sealed class SortingLayerStateVisual : MonoBehaviour
    {
        [SerializeField] private string fallbackActiveSortingLayerName = "Active";
        [SerializeField] private SortingLayerMapping[] sortingLayerMappings =
        {
            new("EntityBackground", "ActiveBackground"),
            new("EntityInfo", "ActiveInfo")
        };
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Canvas[] canvases;

        private RendererSortingState[] rendererStates;
        private CanvasSortingState[] canvasStates;
        private bool hasCachedStates;
        private bool isActiveLayer;

        private void Awake()
        {
            CacheStates();
        }

        private void OnDisable()
        {
            ApplyActiveLayer(false);
        }

        public void SetActiveLayer(bool active)
        {
            if (isActiveLayer == active)
                return;

            ApplyActiveLayer(active);
        }

        private void ApplyActiveLayer(bool active)
        {
            CacheStates();

            for (int i = 0; i < rendererStates.Length; i++)
            {
                RendererSortingState state = rendererStates[i];

                if (state.Renderer == null)
                    continue;

                state.Renderer.sortingLayerID = active
                    ? ResolveActiveSortingLayerId(state.SortingLayerName, state.SortingLayerId)
                    : state.SortingLayerId;
            }

            for (int i = 0; i < canvasStates.Length; i++)
            {
                CanvasSortingState state = canvasStates[i];

                if (state.Canvas == null)
                    continue;

                state.Canvas.sortingLayerID = active
                    ? ResolveActiveSortingLayerId(state.SortingLayerName, state.SortingLayerId)
                    : state.SortingLayerId;
            }

            isActiveLayer = active;
        }

        private int ResolveActiveSortingLayerId(string originalSortingLayerName, int originalSortingLayerId)
        {
            for (int i = 0; i < sortingLayerMappings.Length; i++)
            {
                SortingLayerMapping mapping = sortingLayerMappings[i];

                if (mapping.OriginalLayerName != originalSortingLayerName)
                    continue;

                if (TryGetSortingLayerId(mapping.ActiveLayerName, out int mappedSortingLayerId))
                    return mappedSortingLayerId;

                Debug.LogWarning($"Sorting layer not found: {mapping.ActiveLayerName}", this);
                return originalSortingLayerId;
            }

            if (TryGetSortingLayerId(fallbackActiveSortingLayerName, out int fallbackSortingLayerId))
                return fallbackSortingLayerId;

            Debug.LogWarning($"Sorting layer not found: {fallbackActiveSortingLayerName}", this);
            return originalSortingLayerId;
        }

        private static bool TryGetSortingLayerId(string layerName, out int sortingLayerId)
        {
            SortingLayer[] layers = SortingLayer.layers;
            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i].name != layerName)
                    continue;

                sortingLayerId = layers[i].id;
                return true;
            }

            sortingLayerId = 0;
            return false;
        }

        private void CacheStates()
        {
            if (hasCachedStates)
                return;

            if (renderers == null || renderers.Length == 0)
                renderers = GetComponentsInChildren<Renderer>(true);

            if (canvases == null || canvases.Length == 0)
                canvases = GetComponentsInChildren<Canvas>(true);

            rendererStates = new RendererSortingState[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                rendererStates[i] = new RendererSortingState(
                    renderer,
                    renderer != null ? renderer.sortingLayerID : 0,
                    renderer != null ? SortingLayer.IDToName(renderer.sortingLayerID) : string.Empty);
            }

            canvasStates = new CanvasSortingState[canvases.Length];
            for (int i = 0; i < canvases.Length; i++)
            {
                Canvas canvas = canvases[i];
                canvasStates[i] = new CanvasSortingState(
                    canvas,
                    canvas != null ? canvas.sortingLayerID : 0,
                    canvas != null ? SortingLayer.IDToName(canvas.sortingLayerID) : string.Empty);
            }

            hasCachedStates = true;
        }

        [System.Serializable]
        private struct SortingLayerMapping
        {
            [SerializeField] private string originalLayerName;
            [SerializeField] private string activeLayerName;

            public string OriginalLayerName => originalLayerName;
            public string ActiveLayerName => activeLayerName;

            public SortingLayerMapping(string originalLayerName, string activeLayerName)
            {
                this.originalLayerName = originalLayerName;
                this.activeLayerName = activeLayerName;
            }
        }

        private readonly struct RendererSortingState
        {
            public readonly Renderer Renderer;
            public readonly int SortingLayerId;
            public readonly string SortingLayerName;

            public RendererSortingState(Renderer renderer, int sortingLayerId, string sortingLayerName)
            {
                Renderer = renderer;
                SortingLayerId = sortingLayerId;
                SortingLayerName = sortingLayerName;
            }
        }

        private readonly struct CanvasSortingState
        {
            public readonly Canvas Canvas;
            public readonly int SortingLayerId;
            public readonly string SortingLayerName;

            public CanvasSortingState(Canvas canvas, int sortingLayerId, string sortingLayerName)
            {
                Canvas = canvas;
                SortingLayerId = sortingLayerId;
                SortingLayerName = sortingLayerName;
            }
        }
    }
}
