using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zlipacket.CoreZlipacket.Tools.Attribute;

namespace Zlipacket.CoreZlipacket.UI
{
    public class LineController : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;

        public List<Transform> points;

        [InspectorButton(nameof(RefreshVisuals))] public bool Refresh;
        
        public void Awake()
        {
            if (lineRenderer == null)
                lineRenderer = GetComponent<LineRenderer>();
        }
        
        private void RefreshVisuals()
        {
            if (lineRenderer == null)
                return;
            
            List<Transform> validPoints = points.Where(point => point != null).ToList();
            lineRenderer.positionCount = validPoints.Count;
            for (int i = 0; i < validPoints.Count; i++)
            {
                lineRenderer.SetPosition(i, validPoints[i].position);
            }
        }
    }
}