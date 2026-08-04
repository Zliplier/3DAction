using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zlipacket.CoreZlipacket.Audio;
using Zlipacket.CoreZlipacket.Scene;

namespace Zlipacket.CoreZlipacket.UI
{
    public class ZlipButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject root;
        
        [Header("Curser Hover Sprite")]
        [SerializeField] public Texture2D curserSprite;
        [SerializeField] public Texture2D hoverSprite;
        
        [Header("Hover Scale")]
        public bool useHoverScale = false;
        public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f);
        public float scaleSpeed = 5f;
        
        [Header("Audio")]
        [SerializeField] public AudioClip clickSound;
        
        [Header("Others")]
        public bool onlyClickOnce = false;
        
        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
            if (root == null)
                root = gameObject;
        }

        private void Start()
        {
            if (button != null)
            {
                button.onClick.AddListener(PlayClickSound);
                
                if (onlyClickOnce)
                    button.onClick.AddListener(DisableButton);
            }
        }

        public void DisableButton()
        {
            if (button != null)
                button.enabled = false;
        }

        public void PlayClickSound()
        {
            if (clickSound != null)
            {
                SfxManager.Instance.PlaySoundFX(clickSound);
            }
        }
        
        public void ChangeToScene(string sceneName)
        {
            SceneController.Instance.LoadScene(sceneName);
        }

        public void OverlayToScene(string sceneName)
        {
            SceneController.Instance.LoadSceneAdditive(sceneName);
        }

        public void UnOverlayToScene(string sceneName)
        {
            SceneController.Instance.UnloadScene(sceneName);
        }
        
        public void Quit()
        {
            Application.Quit();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverSprite != null)
            {
                Vector2 cursorHotSpot = new Vector2(hoverSprite.width / 2, hoverSprite.height / 2);
                Cursor.SetCursor(hoverSprite, cursorHotSpot, CursorMode.Auto);
            }

            if (useHoverScale)
            {
                Scale(hoverScale);
            }
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (curserSprite != null)
            {
                Vector2 cursorHotSpot = new Vector2(curserSprite.width / 2, curserSprite.height / 2);
                Cursor.SetCursor(curserSprite, cursorHotSpot, CursorMode.Auto);
            }
            
            if (useHoverScale)
            {
                Scale(Vector3.one);
            }
        }
        
        private Coroutine co_Scale;
        public bool isScaling => co_Scale != null;

        public void Scale(Vector3 targetScale)
        {
            if (isScaling)
                StopCoroutine(co_Scale);
            
            co_Scale = StartCoroutine(Scaling(targetScale));
        }

        public IEnumerator Scaling(Vector3 targetScale)
        {
            while (root.transform.localScale != targetScale)
            {
                root.transform.localScale = Vector3.MoveTowards(root.transform.localScale, targetScale, scaleSpeed * Time.deltaTime);
                
                yield return null;
            }
            
            co_Scale = null;
        }
    }
}