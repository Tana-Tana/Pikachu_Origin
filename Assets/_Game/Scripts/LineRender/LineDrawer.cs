
using System;
using UnityEngine;

public class LineDrawer : GameUnit
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Material material;
    [SerializeField] private Transform hitVFX1;
    [SerializeField] private Transform hitVFX2;
    
    public void DrawLine(int numberPoint, Vector2Int[] pointPos)
    {
        // set size cua material cho line dep

        lineRenderer.material = material; // set material cho line renderer

        // cap nhat toa do cua line renderer
        lineRenderer.positionCount = numberPoint; // set so diem cua line renderer

        for (int i = 0; i < pointPos.Length; ++i) // dat cac diem de noi
        {
            // dat vfx tai diem dau va diem cuoi
            if (i == 0) hitVFX1.position = new Vector3(pointPos[i].y, pointPos[i].x, 0);
            if (i == pointPos.Length - 1) hitVFX2.position = new Vector3(pointPos[i].y, pointPos[i].x, 0);

            // dat vi tri line can set
            lineRenderer.SetPosition(i, new Vector3(pointPos[i].y, pointPos[i].x, 0));
        }

        Invoke(nameof(OnDespawn), 0.35f);
    }

    private void OnDespawn()
    {
        SimplePool.Despawn(this);
    }
}

