using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Applies a vertical color treatment to legacy UGUI text without a baked title image.</summary>
[AddComponentMenu("")]
public sealed class WorldMapTextGradient : BaseMeshEffect
{
    [SerializeField] private Color topColor = new Color(1f, 0.94f, 0.6f, 1f);
    [SerializeField] private Color bottomColor = new Color(0.72f, 0.31f, 0.06f, 1f);

    public void SetColors(Color top, Color bottom)
    {
        topColor = top;
        bottomColor = bottom;
        graphic.SetVerticesDirty();
    }

    public override void ModifyMesh(VertexHelper vertexHelper)
    {
        if (!IsActive() || vertexHelper.currentVertCount == 0)
        {
            return;
        }

        var vertices = new List<UIVertex>();
        vertexHelper.GetUIVertexStream(vertices);

        float minY = float.MaxValue;
        float maxY = float.MinValue;
        foreach (UIVertex vertex in vertices)
        {
            minY = Mathf.Min(minY, vertex.position.y);
            maxY = Mathf.Max(maxY, vertex.position.y);
        }

        float height = Mathf.Max(maxY - minY, 0.0001f);
        for (int index = 0; index < vertices.Count; index++)
        {
            UIVertex vertex = vertices[index];
            float t = (vertex.position.y - minY) / height;
            Color gradientColor = Color.Lerp(bottomColor, topColor, t);
            vertex.color = new Color(gradientColor.r, gradientColor.g, gradientColor.b, vertex.color.a * gradientColor.a);
            vertices[index] = vertex;
        }

        vertexHelper.Clear();
        vertexHelper.AddUIVertexTriangleStream(vertices);
    }
}
