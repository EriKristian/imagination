using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Imagination.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            var loadingScreen = UIManager.Instance?.ShowLoadingScreen() ?? false;

            yield return new WaitForSeconds(0.1f);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            asyncLoad.allowSceneActivation = true;
        }

        public void LoadSceneImmediate(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}