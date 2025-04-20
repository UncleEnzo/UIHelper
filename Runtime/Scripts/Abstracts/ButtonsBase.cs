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

        public bool IsLockedForAnimation { get => isLockedForAnimation; }
        const string lockedForAnimationError = "Trying to transition screens but another screen is animating!";

        protected virtual void Update()
        {
            //todo either queue orrr maybe make it so you can override somehow... 
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
                Debug.LogError(lockedForAnimationError);
                return;
            }

            Debug.Log($"Back button pressed from screen {gameObject.name}. Changing to: {backButtonScreen.gameObject.name}!");
            OnClick_ChangeUIScreen(backButtonScreen);
        }

        public virtual void OnClick_ChangeUIScreenNoAnims(UIScreenBase toUIScreen)
        {
            if (isLockedForAnimation)
            {
                Debug.LogError(lockedForAnimationError);
                return;
            }

            Debug.Log($"Changing UI to screen: {toUIScreen.gameObject.name} WITHOUT Animation");
            IManageScreens.ChangeToNextScreenNoAnim(toUIScreen);
        }

        public virtual void OnClick_ChangeUIScreen(UIScreenBase toUIScreen)
        {
            if (isLockedForAnimation)
            {
                Debug.LogError(lockedForAnimationError);
                return;
            }

            Debug.Log($"Changing UI to screen: {toUIScreen.gameObject.name}");
            IManageScreens.ChangeToNextScreen(toUIScreen);
        }

        public virtual void OnClick_ChangeScene(string sceneName)
        {
            if (isLockedForAnimation)
            {
                Debug.LogError(lockedForAnimationError);
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
                Debug.LogError(lockedForAnimationError);
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
                Debug.LogError(lockedForAnimationError);
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
                Debug.LogError(lockedForAnimationError);
                return;
            }

            Debug.Log("Quitting Application");
            Application.Quit();
        }
    }
}