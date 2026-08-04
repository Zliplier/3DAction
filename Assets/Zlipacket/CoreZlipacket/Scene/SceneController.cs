using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zlipacket.CoreZlipacket.Scene.Transition;
using Zlipacket.CoreZlipacket.Tools;
using Zlipacket.CoreZlipacket.Tools.Attribute;

namespace Zlipacket.CoreZlipacket.Scene
{
    public class SceneController : Singleton<SceneController>
    {
        [SerializeField] private string loadingSceneName;
        [SerializeField] private Transform sceneCanvas;
        public SO_SceneTransition transition;
        public float fakeLoadingTime = 0.5f;

        private Coroutine co_Loading = null;
        public bool isLoading => co_Loading != null;
        
        public string currentScene => SceneManager.GetActiveScene().name;
        
        [Header("Inspector Menu")]
        public string InspectorSceneName;
        [InspectorButton(nameof(InspectorLoadScene), ButtonWidth = 200f)]
        public bool LoadInspectorScene;
        private void InspectorLoadScene() => LoadScene(InspectorSceneName);
        
        public bool LoadScene(string sceneName = "", string loadingName = "")
        {
            if (isLoading)
            {
                Debug.Log("Another Scene is already loading.");
                return false;
            }
            
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"Scene name cannot be null or empty.");
                return false;
            }
            
            co_Loading = StartCoroutine(TransitionToScene(sceneName, loadingName));
            
            Debug.Log("Scene " + sceneName + " loaded.");
            return true;
        }

        public bool LoadSceneAdditive(string sceneName)
        {
            if (isLoading)
            {
                Debug.Log("Another Scene is loading.");
                return false;
            }
            
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            Debug.Log("Scene" + sceneName + " loaded async.");
            return true;
        }

        public bool UnloadScene(string sceneName)
        {
            if (isLoading)
            {
                Debug.Log("Another Scene is loading.");
                return false;
            }
            
            SceneManager.UnloadSceneAsync(sceneName);
            Debug.Log("Scene" + sceneName + " unloaded async.");
            return true;
        }
        
        private IEnumerator TransitionToScene(string sceneName, string loadingName)
        {
            sceneCanvas.gameObject.SetActive(true);
            SceneTransition sceneTransition = transition.InitializeTransition(sceneCanvas);
            
            yield return sceneTransition.StartTransition();
            
            //Loading Screen
            SceneManager.LoadScene(loadingName == "" ? loadingSceneName : loadingName);
            yield return null;
            sceneTransition.gameObject.SetActive(false);
            yield return new WaitForSeconds(fakeLoadingTime);
            
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncOperation.isDone)
                yield return null;
            
            sceneTransition.gameObject.SetActive(true);
            yield return null;
            
            yield return sceneTransition.EndTransition();
            sceneCanvas.gameObject.SetActive(false);
            co_Loading = null;
        }
    }
}