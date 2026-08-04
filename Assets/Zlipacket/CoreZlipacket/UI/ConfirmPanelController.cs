using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zlipacket.CoreZlipacket.UI.Canvas_Management;

namespace Zlipacket.CoreZlipacket.UI
{
    public class ConfirmPanelController : MonoBehaviour
    {
        public ConfirmPanel panelPrefab;
        [HideInInspector] public ConfirmPanel confirmPanel;
        public PanelLayer layer;
        
        public bool isOpen => confirmPanel != null;
        
        [Header("Settings")]
        public string title;
        public string subtitle;
        public string confirmText;
        public string cancelText;
        public bool interactableOnConfirm = false;
        public bool interactableOnCancel = false;
        
        [Header("Events")]
        public UnityEvent onConfirmed;
        public UnityEvent onCanceled;

        private void Start()
        {
            onConfirmed.AddListener(() => layer.SetInteractableState(interactableOnConfirm));
            onCanceled.AddListener(() => layer.SetInteractableState(interactableOnCancel));
        }

        public void Open()
        {
            if (isOpen)
                return;
            
            layer.SetInteractableState(true);
            confirmPanel = Instantiate(panelPrefab, layer.layerRoot.transform);
            confirmPanel.Initialize(this);
        }

        public void Close()
        {
            if (!isOpen)
                return;
            
            Destroy(confirmPanel.gameObject);
            confirmPanel = null;
        }

        private void OnDestroy()
        {
            onConfirmed.RemoveAllListeners();
            onCanceled.RemoveAllListeners();
        }
    }
}