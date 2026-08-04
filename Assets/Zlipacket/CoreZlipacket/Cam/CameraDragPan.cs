
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Zlipacket.CoreZlipacket.Player.Input;

namespace Zlipacket.CoreZlipacket.Cam
{
    /// <summary>
    /// Drag-to-pan controller for a 2D Cinemachine setup, driven by the new Input System.
    /// Attach this to any manager object (not the vcam itself). "panTarget" should
    /// be the empty Transform your CinemachineCamera / CinemachineVirtualCamera
    /// is Following. Keep Body damping at 0 on the vcam so it tracks this rig 1:1.
    ///
    /// This script owns the "soft" elastic bounds as a plain rect (no Polygon2D
    /// needed). If you also use Cinemachine's Confiner2D extension on the vcam,
    /// give its bounding shape extra padding (at least overscrollRange) beyond
    /// these soft bounds so it acts purely as a hard safety net and never
    /// visually fights the elastic effect. See OnDrawGizmos for a visual of both.
    /// </summary>
    public class CameraDragPan : MonoBehaviour
    {
        [Header("References")]
        public Camera cam;            // Main/Brain camera, used for screen->world conversion
        public Transform panTarget;   // The Cinemachine Follow/Tracking target
        [SerializeField] private SO_InputReader inputReader;

        [Header("Level Bounds (world space, true level edges)")]
        [Tooltip("The actual edges of your level/world - NOT pre-shrunk. The rig's " +
                 "allowed range is derived from this minus the camera's current " +
                 "half-extents, so it stays correct automatically as the camera zooms.")]
        public Vector2 levelBoundsMin;
        public Vector2 levelBoundsMax;
        
        [Header("Drag Feel")]
        [Tooltip("Scales pointer movement -> camera movement. 1 = camera tracks the " +
                 "pointer 1:1. >1 = pans faster than the drag, <1 = slower/more sluggish.")]
        [Range(0.1f, 5f)]
        public float dragSpeedMultiplier = 1f;

        [Header("Elastic Feel")]
        [Tooltip("Higher = more resistance when dragging past bounds.")]
        public float elasticity = 0.55f;
        [Tooltip("World-space distance used to normalize the rubber-band curve. " +
                 "Roughly: how far it should be able to stretch before feeling 'maxed out'.")]
        public float overscrollRange = 3f;

        [Header("Spring Back (DOTween)")]
        public float springBackDuration = 0.45f;
        public Ease springBackEase = Ease.OutBack;

        Vector3 _dragOriginWorld;
        Vector3 _targetOriginPos;
        Tween _springTween;

        private Coroutine co_Dragging = null;
        public bool IsDragging => co_Dragging != null;
        
        private void OnEnable()
        {
            inputReader.playerInputMap.leftMouseStartEvent += OnDragStarted;
            inputReader.playerInputMap.leftMouseCancelEvent += OnDragCanceled;
        }

        private void OnDisable()
        {
            inputReader.playerInputMap.leftMouseStartEvent -= OnDragStarted;
            inputReader.playerInputMap.leftMouseCancelEvent -= OnDragCanceled;
        }
        
        public void OnDragStarted()
        {
            //Debug.Log("Started dragging");
            if (IsDragging)
            {
                StopCoroutine(co_Dragging);
                co_Dragging = null;
            }
            
            _springTween?.Kill();

            Vector2 screenPos = GetScreenPosition();
            _dragOriginWorld = ScreenToWorld(screenPos);
            _targetOriginPos = panTarget.position;

            co_Dragging = StartCoroutine(Dragging());
        }

        private IEnumerator Dragging()
        {
            yield return new WaitForEndOfFrame();
            
            while (IsDragging)
            {
                //Debug.Log("Dragging");
                Vector2 screenPos = GetScreenPosition();
                Vector3 currentWorld = ScreenToWorld(screenPos);
                Vector3 delta = (_dragOriginWorld - currentWorld) * dragSpeedMultiplier; // drag right -> world content follows the pointer
                Vector3 desired = _targetOriginPos + delta;
                panTarget.position = ApplyElastic(desired);
                
                yield return null;
            }
            
            co_Dragging = null;
        }

        public void OnDragCanceled()
        {
            //Debug.Log("Canceled dragging");
            if (IsDragging)
            {
                StopCoroutine(co_Dragging);
                co_Dragging = null;
            }

            if (IsOutsideBounds(panTarget.position))
                SpringBack();
        }

        // ---------------------------------------------------

        Vector2 GetScreenPosition()
        {
            return inputReader.playerInputMap.mousePosition;

            //return Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero;
        }

        Vector3 ScreenToWorld(Vector3 screenPos)
        {
            screenPos.z = Mathf.Abs(cam.transform.position.z - panTarget.position.z);
            return cam.ScreenToWorldPoint(screenPos);
        }
 
        Vector3 ApplyElastic(Vector3 desired)
        {
            GetSoftBounds(out Vector2 min, out Vector2 max);
            return new Vector3(
                ElasticAxis(desired.x, min.x, max.x),
                ElasticAxis(desired.y, min.y, max.y),
                panTarget.position.z
            );
        }
 
        float ElasticAxis(float value, float min, float max)
        {
            if (value < min) return min - RubberBand(min - value);
            if (value > max) return max + RubberBand(value - max);
            return value;
        }
 
        // Same shaped curve UGUI's ScrollRect uses for overscroll: fast at first,
        // asymptotically approaches overscrollRange as overflow -> infinity.
        float RubberBand(float overflow)
        {
            return (1f - 1f / (overflow * elasticity / overscrollRange + 1f)) * overscrollRange;
        }
 
        bool IsOutsideBounds(Vector3 pos)
        {
            GetSoftBounds(out Vector2 min, out Vector2 max);
            return pos.x < min.x || pos.x > max.x ||
                   pos.y < min.y || pos.y > max.y;
        }

        void SpringBack()
        {
            GetSoftBounds(out Vector2 min, out Vector2 max);
            Vector3 clamped = new Vector3(
                Mathf.Clamp(panTarget.position.x, min.x, max.x),
                Mathf.Clamp(panTarget.position.y, min.y, max.y),
                panTarget.position.z
            );
 
            _springTween?.Kill();
            _springTween = panTarget
                .DOMove(clamped, springBackDuration)
                .SetEase(springBackEase);
        }
        
        // Shrinks the true level edges by the camera's CURRENT half-extents, so this
        // stays correct at any zoom level rather than being tuned for one fixed size.
        void GetSoftBounds(out Vector2 min, out Vector2 max)
        {
            Vector2 camExtents = GetCameraHalfExtents();
            min = levelBoundsMin + camExtents;
            max = levelBoundsMax - camExtents;
        }

        // ---- Debug ----

        void OnDrawGizmos()
        {
            if (cam == null) return;
 
            GetSoftBounds(out Vector2 softMin, out Vector2 softMax);
 
            // Rig soft bounds AT THE CURRENT ZOOM: where the drag target is allowed to sit.
            Gizmos.color = Color.yellow;
            DrawRectGizmo(softMin, softMax);
 
            // Max elastic stretch at the current zoom.
            Gizmos.color = Color.magenta;
            DrawRectGizmo(softMin - Vector2.one * overscrollRange, softMax + Vector2.one * overscrollRange);
 
            // True level edges, as authored - constant regardless of zoom.
            Gizmos.color = Color.cyan;
            DrawRectGizmo(levelBoundsMin, levelBoundsMax);
        }

        Vector2 GetCameraHalfExtents()
        {
            if (cam.orthographic)
                return new Vector2(cam.orthographicSize * cam.aspect, cam.orthographicSize);

            // Perspective cameras don't have a fixed frustum size - this is only a
            // rough estimate at z = panTarget's depth, mainly useful for a top-down 2.5D setup.
            float distance = Mathf.Abs(cam.transform.position.z - (panTarget != null ? panTarget.position.z : 0f));
            float height = 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            return new Vector2(height * cam.aspect * 0.5f, height * 0.5f);
        }

        void DrawRectGizmo(Vector2 min, Vector2 max)
        {
            Vector3 bl = new Vector3(min.x, min.y, panTarget != null ? panTarget.position.z : 0f);
            Vector3 br = new Vector3(max.x, min.y, bl.z);
            Vector3 tr = new Vector3(max.x, max.y, bl.z);
            Vector3 tl = new Vector3(min.x, max.y, bl.z);

            Gizmos.DrawLine(bl, br);
            Gizmos.DrawLine(br, tr);
            Gizmos.DrawLine(tr, tl);
            Gizmos.DrawLine(tl, bl);
        }
    }
}
