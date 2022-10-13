using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nevelson.UIHelper
{
    public class UILoader : MonoBehaviour
    {
        [SerializeField] string sceneName;
        [SerializeField] bool loadSceneOnAwake = true;

        void Awake()
        {
            if (!loadSceneOnAwake)
            {
                return;
            }
            LoadUIScene();
        }

        public void LoadUIScene()
        {
            if (sceneName == null || sceneName.Equals(""))
            {
                Debug.LogError("Missing scene name");
                return;
            }

            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene == null)
            {
                Debug.LogError($"Could not find scene by name {sceneName}");
                return;
            }

            if (scene.isLoaded)
            {
                return;
            }

            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}