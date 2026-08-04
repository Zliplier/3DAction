using System;
using System.Linq;
using UnityEngine;

namespace Zlipacket.CoreZlipacket.UI.Canvas_Management
{
    public class CanvasLayerManager : MonoBehaviour
    {
        [SerializeField] private PanelLayer[] layers;
        
        public PanelLayer GetLayer(string layerName)
        {
            PanelLayer layer = layers.FirstOrDefault(l => string.Equals(l.layerName.ToLower(), layerName.ToLower(),
                StringComparison.InvariantCultureIgnoreCase));
            if (layer == null)
            {
                Debug.LogError($"Canvas layer {layerName} not found.");
            }
            
            return layer;
        }
        
        public void ShowLayer(string layerName) => GetLayer(layerName).Show();
        public void HideLayer(string layerName) => GetLayer(layerName).Hide();
    }
}