using System.Collections;
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

            StartCoroutine(HideThenDisplayCo(currentScreen, nextScreen));
        }

        IEnumerator HideThenDisplayCo(UIScreenBase _currentScreen, UIScreenBase _nextScreen)
        {
            _currentScreen.Hide();
            while (_currentScreen.IsScreenDisplayed)
            {
                yield return null;
            }

            _nextScreen.Display();
            currentScreen = _nextScreen;
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
            uiScreens = GetComponentsInChildren<UIScreenBase>(true);
            if (uiScreens == null || uiScreens.Length == 0)
            {
                Debug.LogError("Could not find any UIScreens on canvas");
                return;
            }
            foreach (UIScreenBase uiScreen in uiScreens)
            {
                //first find all popups and give their selectables the popup reference
                foreach (Popup popup in uiScreen.GetComponentsInChildren<Popup>(true))
                {
                    foreach (Selectable popupSelectable in popup.GetComponentsInChildren<Selectable>(true))
                    {
                        UICancelHandler uiCancel = popupSelectable.gameObject.AddComponent<UICancelHandler>();
                        uiCancel.Init(uiScreen, popup);
                    }
                }

                //foreach tab... something


                //Finally iterate through all selectables and if they're still missing a UICanceller slap it on here
                foreach (Selectable selectable in uiScreen.GetComponentsInChildren<Selectable>(true))
                {
                    UIAudio uiAudio = selectable.gameObject.AddComponent<UIAudio>();
                    uiAudio.Init(audioSource, hoverSound, pressedSound);

                    if (!selectable.gameObject.TryGetComponent(out UICancelHandler contains))
                    {
                        UICancelHandler uiCancel = selectable.gameObject.AddComponent<UICancelHandler>();
                        uiCancel.Init(uiScreen);
                    }
                }
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
                iScreen.Init();
            }

            currentScreen = uiScreens[0];
            IScreen currentIScreen = currentScreen.GetComponent<IScreen>();
            currentIScreen.Display();
        }
    }
}