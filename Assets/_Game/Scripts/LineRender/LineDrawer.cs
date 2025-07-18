using System;
using UnityEngine;

namespace _Game.Scripts.LineRender
{
    public class LineDrawer : GameUnit
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Material material;
        private Vector2Int startPoint;
        private Vector2Int endPoint;

        private void Start()
        {
            OnInit();
        }

        private void OnInit()
        {
            // cau hinh LineRenderer
            lineRenderer.startWidth = 0.05f; // do rong cua line
            lineRenderer.endWidth = 0.05f; // do rong cua line
            lineRenderer.material = material; // gan material cho line renderer
            lineRenderer.startColor = Color.green; // mau bat dau cua line
            lineRenderer.endColor = Color.green; // mau ket thuc cua line
        }
    
        public void DrawLine(Vector3 startPos, Vector3 endPos)
        {
            // cap nhat toa do cua line renderer
            lineRenderer.positionCount = 2; // set so diem cua line renderer
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
            Invoke(nameof(OnDespawn), 1f);
        }

        private void OnDespawn()
        {
            SimplePool.Despawn(this);
        }
    }
}
