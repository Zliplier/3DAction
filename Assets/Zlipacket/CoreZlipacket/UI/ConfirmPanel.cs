using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zlipacket.CoreZlipacket.UI
{
    public class ConfirmPanel : MonoBehaviour
    {
        [Header("References")]
        public TextMeshProUGUI title;
        public TextMeshProUGUI subtitle;
        public Button confirmButton;
        public TextMeshProUGUI confirmText;
        public Button cancelButton;
        public TextMeshProUGUI cancelText;
        
        private ConfirmPanelController controller;

        public void Initialize(ConfirmPanelController controller)
        {
            this.controller = controller;
            
            if (title != null)
                title.SetText(controller.title.Trim());
            if (subtitle != null)
                subtitle.SetText(controller.subtitle.Trim());
            
            if (cancelText != null)
                cancelText.SetText(controller.cancelText.Trim());
            if (confirmText != null)
                confirmText.SetText(controller.confirmText.Trim());
            
            confirmButton.onClick.AddListener(controller.onConfirmed.Invoke);
            confirmButton.onClick.AddListener(controller.Close);
            
            cancelButton.onClick.AddListener(controller.onCanceled.Invoke);
            cancelButton.onClick.AddListener(controller.Close);
        }

        private void OnDestroy()
        {
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
        }
    }
}