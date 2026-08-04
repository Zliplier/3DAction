using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using Zlipacket.CoreZlipacket.Scene;

namespace Zlipacket.CoreZlipacket.UI
{
    public class VideoController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private float endTime = 0f;
        public UnityEvent onVideoEnd;
        
        private void Start()
        {
            if (endTime == 0f)
                endTime = (float)videoPlayer.length;
            Invoke(nameof(VideoEnd), endTime);
        }

        public void VideoEnd()
        {
            onVideoEnd?.Invoke();
        }

        public void EndCutScene(string nextSceneName)
        {
            SceneController.Instance.LoadScene(nextSceneName);
        }

        public void SkipCutScene(string nextSceneName)
        {
            CancelInvoke(nameof(EndCutScene));
            EndCutScene(nextSceneName);
        }
    }
}