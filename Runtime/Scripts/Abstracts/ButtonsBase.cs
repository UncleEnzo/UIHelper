using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

namespace Nevelson.UIHelper
{
    public abstract class ButtonsBase : MonoBehaviour
    {
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

        public void OnClick_ChangeUIScreen(UIScreenBase toUIScreen)
        {
            Debug.Log($"Changing UI to screen: {toUIScreen.gameObject.name}");
            IManageScreens.ChangeToNextScreen(toUIScreen);
        }

        public void OnClick_ChangeScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            Debug.Log($"Changing Scenes to: {scene.name} | build Index: {scene.buildIndex}");
            SceneManager.LoadScene(sceneName);
        }

        public void OnClick_ChangeScene(int sceneBuildIndex)
        {
            Scene scene = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);
            Debug.Log($"Changing Scenes to: {scene.name} | build Index: {scene.buildIndex}");
            SceneManager.LoadScene(sceneBuildIndex);
        }

        public void OnClick_ReloadCurrentScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            Debug.Log($"Reloading Scene {scene.name} | build Index: {scene.buildIndex}");
            SceneManager.LoadScene(scene.buildIndex);
        }

        public void OnClick_QuitApplication()
        {
            Debug.Log("Quitting Application");
            Application.Quit();
        }

        //below will be the button listeners for controllers


        /// <summary>
        /// Function for listening to controller input to return to a previous screen
        /// </summary>
        protected virtual void BackButton()
        {
            //HOW TO I MAKE THIS INDEPENDENT OF WHAT CONTROLLER SYSTEM THEY USE
            //if (playerOne.GetButtonDown("Back") || playerTwo.GetButtonDown("Back"))
            //{
            //    if (previousScreen == null)
            //    {
            //        Debug.Log("No back button target");
            //        return;
            //    }
            //    uiManager.ScreenDone(previousScreen);
            //}
        }
    }
}