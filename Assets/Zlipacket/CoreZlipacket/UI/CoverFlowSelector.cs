using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zlipacket.CoreZlipacket.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class CoverFlowSelector : MonoBehaviour, IEndDragHandler, IBeginDragHandler
    {
        [Header("References")]
        public ScrollRect scrollRect;
        public RectTransform content;
        public RectTransform viewport;

        [Header("Behavior")]
        public bool horizontal = true;
        public float snapSpeed = 10f;
        public float velocityThreshold = 50f; // below this, we consider it "settled"

        [Header("Scaling")]
        public float maxScale = 1.2f;   // scale of the centered item
        public float minScale = 0.7f;   // scale of items at the edge
        public float falloffRange = 300f; // distance (px) at which item reaches minScale
        public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); // 1 = center, 0 = far

        [Header("Configs")]
        public int intIndex = 0;
        
        private List<RectTransform> items = new List<RectTransform>();
        private bool isSnapping = false;
        private bool isDragging = false;
        private Vector2 targetPos;
        private int selectedIndex = -1;

        void Start()
        {
            //LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            
            foreach (RectTransform child in content)
            {
                // skip spacers if you tag/name them, e.g. "Spacer"
                if (!child.name.Contains("Spacer"))
                    items.Add(child);
            }
            
            ScrollTo(intIndex);
        }

        void Update()
        {
            UpdateItemScales();

            if (isSnapping)
            {
                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, targetPos, Time.deltaTime * snapSpeed);
                if (Vector2.Distance(content.anchoredPosition, targetPos) < 0.5f)
                {
                    content.anchoredPosition = targetPos;
                    isSnapping = false;
                }
            }
            /*else if (!isDragging && scrollRect.velocity.magnitude < velocityThreshold && scrollRect.velocity.magnitude > 0.01f)
            {
                // Momentum has settled naturally, snap to nearest
                SnapToNearest();
            }*/
        }

        void UpdateItemScales()
        {
            foreach (var item in items)
            {
                float dist = GetDistanceFromCenter(item);
                float t = Mathf.Clamp01(1f - (dist / falloffRange)); // 1 = at center, 0 = at/after falloff
                float curveVal = scaleCurve.Evaluate(t);
                float scale = Mathf.Lerp(minScale, maxScale, curveVal);
                item.localScale = new Vector3(scale, scale, 1f);
            }
        }

        float GetDistanceFromCenter(RectTransform item)
        {
            Vector3 worldPos = item.position;
            Vector3 localPos = viewport.InverseTransformPoint(worldPos);
            float distance = horizontal ? Mathf.Abs(localPos.x) : Mathf.Abs(localPos.y);
            return distance;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            // Let momentum play out; Update() will call SnapToNearest() once velocity settles.
            // If you want an instant snap instead, uncomment:
            SnapToNearest();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            isSnapping = false;
        }

        void SnapToNearest()
        {
            float closestDist = float.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < items.Count; i++)
            {
                float dist = GetDistanceFromCenter(items[i]);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestIndex = i;
                }
            }

            selectedIndex = closestIndex;
            StartSnapTo(items[closestIndex]);
            OnItemSelected(selectedIndex);
        }

        void StartSnapTo(RectTransform target)
        {
            Vector2 targetLocalPos = (Vector2)viewport.InverseTransformPoint(target.position);
            Vector2 newContentPos = content.anchoredPosition;

            if (horizontal)
                newContentPos.x = content.anchoredPosition.x - targetLocalPos.x;
            else
                newContentPos.y = content.anchoredPosition.y - targetLocalPos.y;

            targetPos = newContentPos;
            isSnapping = true;
            scrollRect.velocity = Vector2.zero; // stop any residual momentum
        }

        public void ScrollTo(int index)
        {
            selectedIndex = index;
            StartSnapTo(items[selectedIndex]);
            OnItemSelected(selectedIndex);
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            Canvas.ForceUpdateCanvases();
        }

        public void ScrollPrev()
        {
            if (selectedIndex - 1 >= 0)
            {
                ScrollTo(selectedIndex - 1);
            }
        }
        
        public void ScrollNext()
        {
            if (selectedIndex + 1 < items.Count)
            {
                ScrollTo(selectedIndex + 1);
            }
        }

        void OnItemSelected(int index)
        {
            Debug.Log("Selected item: " + index);

            for (int i = 0; i < items.Count; i++)
            {
                Button button = items[i].GetComponent<Button>();
                
                if (button == null)
                    continue;
                
                if (i == index)
                    button.interactable = true;
                else
                {
                    button.interactable = false;
                }
                
            }
            
            // Fire event, update label, trigger haptic, etc.
        }
    }
}