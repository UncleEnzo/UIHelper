using UnityEngine;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    [RequireComponent(typeof(AudioSource))]
    public class UIScreenManager : MonoBehaviour, IManageScreens
    {
        [SerializeField] bool isUsingController = false;
        [SerializeField] AudioClip hoverSound;
        [SerializeField] AudioClip pressedSound;

        AudioSource audioSource;
        UIScreenBase[] uiScreens;
        UIScreenBase currentScreen;
        bool currentIsUsingController;
        public bool IsUsingController
        {
            get => isUsingController;
            set
            {
                if (value != isUsingController)
                {
                    RefreshScreenControls();
                }
                isUsingController = value;
                currentIsUsingController = isUsingController;
            }
        }

        public void ChangeToNextScreen(UIScreenBase nextScreen)
        {
            if (nextScreen == null)
            {
                Debug.LogError("Next Screen is Null");
                return;
            }

            IScreen currentIScreen = currentScreen.GetComponent<IScreen>();
            IScreen nextIScreen = nextScreen.GetComponent<IScreen>();

            currentIScreen.Hide();
            nextIScreen.Display();
            currentScreen = nextScreen;
        }

        void Start()
        {
            Init();
            StartFirstScreen();
        }

        void Update()
        {
            //handles UI controller switching through inspector
            if (currentIsUsingController != isUsingController)
            {
                RefreshScreenControls();
                currentIsUsingController = isUsingController;
            }
        }

        void Init()
        {
            audioSource = GetComponent<AudioSource>();
            foreach (var selectable in GetComponentsInChildren<Selectable>())
            {
                UIAudio newComponent = selectable.gameObject.AddComponent<UIAudio>();
                newComponent.Init(audioSource, hoverSound, pressedSound);
            }
            uiScreens = GetComponentsInChildren<UIScreenBase>();
            if (uiScreens == null || uiScreens.Length == 0)
            {
                Debug.LogError("Could not find any UIScreens on canvas");
                return;
            }
        }

        void RefreshScreenControls()
        {
            foreach (var screen in uiScreens)
            {
                screen.SetUsingController(isUsingController);
            }
            currentScreen.SetUIFocus();
        }

        void StartFirstScreen()
        {
            if (uiScreens.Length == 0)
            {
                Debug.LogError($"Could not find any UIScreen under UIScreenManager on go: {gameObject.name}");
                return;
            }

            for (int i = 0; i < uiScreens.Length; i++)
            {
                IScreen iScreen = uiScreens[i].GetComponent<IScreen>();
                iScreen.Hide();
            }

            currentScreen = uiScreens[0];
            IScreen currentIScreen = currentScreen.GetComponent<IScreen>();
            currentIScreen.Display();
        }
    }
}