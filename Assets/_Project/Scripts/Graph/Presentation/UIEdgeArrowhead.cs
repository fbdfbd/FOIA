using UnityEngine;
using UnityEngine.UI;

namespace FOIA.Graph.Presentation
{
    public sealed class UIEdgeArrowhead : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            Rect rect = GetPixelAdjustedRect();
            Color32 vertexColor = color;

            vertexHelper.AddVert(new Vector2(rect.xMax, rect.center.y), vertexColor, Vector2.zero);
            vertexHelper.AddVert(new Vector2(rect.xMin, rect.yMax), vertexColor, Vector2.zero);
            vertexHelper.AddVert(new Vector2(rect.xMin, rect.yMin), vertexColor, Vector2.zero);
            vertexHelper.AddTriangle(0, 1, 2);
        }
    }
}
