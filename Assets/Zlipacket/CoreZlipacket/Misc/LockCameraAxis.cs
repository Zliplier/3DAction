using Unity.Cinemachine;
using UnityEngine;

namespace Zlipacket.CoreZlipacket.Misc
{
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that locks the camera's Z co-ordinate
    /// </summary>
    [ExecuteInEditMode] [SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
    public class LockCameraAxis : CinemachineExtension
    {
        public bool lockX = false;
        public bool lockY = false;
        public bool lockZ = false;
        
        [Tooltip("Lock the camera's X position to this value")]
        public float m_XPosition = 0;
        [Tooltip("Lock the camera's Y position to this value")]
        public float m_YPosition = 0;
        [Tooltip("Lock the camera's Z position to this value")]
        public float m_ZPosition = 0;

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                if (lockX)
                    pos.x = m_XPosition;
                if (lockY)
                    pos.y = m_YPosition;
                if (lockZ)
                    pos.z = m_ZPosition;
                state.RawPosition = pos;
            }
        }
    }
}