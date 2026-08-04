using UnityEngine;

namespace Zlipacket.CoreZlipacket.Scene.Transition
{
    [CreateAssetMenu(menuName = "Zlipacket/Scene/Transition", fileName = "SceneTransition")]
    public class SO_SceneTransition : ScriptableObject
    {
        public GameObject transitionPrefab;

        public SceneTransition InitializeTransition(Transform parent)
        {
            SceneTransition sceneTransition = Instantiate(transitionPrefab, parent).GetComponent<SceneTransition>();
            return sceneTransition;
        }
    }
}