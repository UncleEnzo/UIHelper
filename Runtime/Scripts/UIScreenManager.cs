using UnityEngine;

namespace Nevelson.UIHelper
{
    public class UIScreenManager : MonoBehaviour, IManageScreens
    {
        IScreen[] uiScreens;
        IScreen currentScreen;

        public void ChangeToNextScreen(IScreen nextScreen)
        {
            if (nextScreen == null)
            {
                Debug.LogError("Next Screen is Null");
                return;
            }

            currentScreen.BeforeHide();
            currentScreen.Hide();
            currentScreen.AfterHide();
            nextScreen.BeforeDisplay();
            nextScreen.Display();
            nextScreen.AfterDisplay();
            currentScreen = nextScreen;
        }

        void Start()
        {
            Init();
            StartFirstScreen();
        }

        void Init()
        {
            uiScreens = GetComponentsInChildren<IScreen>();
            if (uiScreens == null || uiScreens.Length == 0)
            {
                Debug.LogError("Could not find any UIScreens on canvas");
                return;
            }
        }

        void StartFirstScreen()
        {
            currentScreen = uiScreens[0];
            currentScreen.BeforeDisplay();
            currentScreen.Display();
            currentScreen.AfterDisplay();
            for (int i = 1; i < uiScreens.Length; i++)
            {
                uiScreens[i].BeforeHide();
                uiScreens[i].Hide();
                uiScreens[i].AfterHide();
            }
        }
    }
}