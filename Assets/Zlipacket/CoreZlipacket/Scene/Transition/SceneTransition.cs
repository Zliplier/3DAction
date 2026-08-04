using System.Collections;
using UnityEngine;

namespace Zlipacket.CoreZlipacket.Scene.Transition
{
    public class SceneTransition : MonoBehaviour
    {
        public Animator transitionAnimator;
        public float startTransitionTime = 0.5f;
        public float endTransitionTime = 0.5f;
        
        public IEnumerator StartTransition()
        {
            transitionAnimator.SetTrigger("Start");
            yield return new WaitForSeconds(startTransitionTime);
        }

        public IEnumerator EndTransition()
        {
            transitionAnimator.SetTrigger("End");
            yield return new WaitForSeconds(endTransitionTime);
            
            Destroy(gameObject);
        }
    }
}