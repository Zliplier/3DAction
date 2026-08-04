using System;
using UnityEngine;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.CoreZlipacket.UI.Canvas_Management
{
    public class SceneCanvasManager : Singleton<SceneCanvasManager>
    {
        [SerializeField] private CanvasLayerManager layerManager;
        
        public PanelLayer GetLayer(string layerName) => layerManager.GetLayer(layerName);

        private void Start()
        {
            Canvas.ForceUpdateCanvases();
        }
    }
}