using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

namespace Nevelson.UIHelper
{
    public abstract class ButtonsBase : MonoBehaviour
    {
        [SerializeField] UIScreenBase backButtonScreen;
        protected bool isLockedForAnimation = false;
        IManageScreens _iManageScreens;
        IManageScreens IManageScreens
        {
            get
            {
                if (_iManageScreens == null)
                {
                    _iManageScreens = GetComponentInParent<IManageScreens>();
                    if (_iManageScreens == null) Debug.LogError("Could not find UIScreenManager in parent canvas component");
                }
                return _iManageScreens;
            }
        }

        public virtual void OnClick_ScreenBack()
        {
            if (backButtonScreen == null)
            {
                Debug.Log("Back button pressed but no back button target!");
                return;
            }

            if (isLockedForAnimation)
            {
                Debug.Log("Back button pressed but screen locked for animation!");
                return;
            }

            Debug.Log($"Back button pressed from screen {gameObject.name}. Changing to: {backButtonScreen.gameObject.name}!");
            OnClick_ChangeUIScreen(backButtonScreen);
        }

        public virtual void OnClick_ChangeUIScreen(UIScreenBase toUIScreen)
        {
            if (isLockedForAnimation)
            {
                Debug.Log("Back button pressed but screen locked for animation!");
                return;
            }

            Debug.Log($"Changing UI to screen: {toUIScreen.gameObject.name}");
            IManageScreens.ChangeToNextScreen(toUIScreen);
        }

        public virtual void OnClick_ChangeScene(string sceneName)
        {
            if (isLockedForAnimation)
            {
                Debug.Log("Back button pressed but screen locked for animation!");
                return;
            }

            Scene scene = SceneManager.GetSceneByName(sceneName);
            Debug.Log($"Changing Scenes to: {scene.name} | build Index: {scene.buildIndex}");
            SceneManager.LoadScene(sceneName);
        }

        public virtual void OnClick_ChangeScene(int sceneBuildIndex)
        {
            if (isLockedForAnimation)
            {
                Debug.Log("Back button pressed but screen locked for animation!");
                return;
            }

            Scene scene = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);
            Debug.Log($"Changing Scenes to: {scene.name} | build Index: {scene.buildIndex}");
            SceneManager.LoadScene(sceneBuildIndex);
        }

        public virtual void OnClick_ReloadCurrentScene()
        {
            if (isLockedForAnimation)
            {
                Debug.Log("Back button pressed but screen locked for animation!");
                return;
            }

            Scene scene = SceneManager.GetActiveScene();
            Debug.Log($"Reloading Scene {scene.name} | build Index: {scene.buildIndex}");
            SceneManager.LoadScene(scene.buildIndex);
        }

        public virtual void OnClick_QuitApplication()
        {
            if (isLockedForAnimation)
            {
                Debug.Log("Back button pressed but screen locked for animation!");
                return;
            }

            Debug.Log("Quitting Application");
            Application.Quit();
        }
    }
}