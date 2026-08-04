using System;
using UnityEngine;

namespace Zlipacket.CoreZlipacket.UI.Canvas_Management
{
    public class PanelLayer : MonoBehaviour
    {
        public string layerName;
        public GameObject layerRoot => gameObject;
        [SerializeField] private CanvasGroup canvasGroup;
        public CanvasGroupController cgController;

        private void Awake()
        {
            if (string.IsNullOrEmpty(layerName))
            {
                layerName = gameObject.name;
            }
            
            cgController = new CanvasGroupController(this, canvasGroup);
        }
        
        public void Show(float speed = 1f, bool immediate = false, Action callback = null) => cgController.Show(speed, immediate, callback);
        public void Hide(float speed = 1f, bool immediate = false, Action callback = null) => cgController.Hide(speed, immediate, callback);

        public void SetInteractableState(bool state) => cgController.SetInteractableState(state);
    }
}