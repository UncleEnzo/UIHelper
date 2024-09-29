using Nevelson.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    public static class ControllerManager
    {
        public static event Action<bool> On_ChangeController;

        public static void Invoke(bool isUsingController)
        {
            On_ChangeController?.Invoke(isUsingController);
        }
    }
}


namespace Nevelson.UIHelper
{
    [Serializable]
    public struct CanvasEventListener
    {
        [SerializeField] public GameEventSO gameEvent;
        [SerializeField] public UnityEvent canvasResponse;
    }

    [Serializable]
    public struct CanvasStringChannelListener
    {
        [SerializeField] public StringGameEventSO stringGameEvent;
        [SerializeField] public UnityEvent<string> canvasResponse;
    }

    [RequireComponent(typeof(AudioSource))]
    public class UIScreenManager : MonoBehaviour, IManageScreens
    {
        const string UISCREEN_MANAGER_LOG = "UI Screen Manager: ";
        [SerializeField] AudioClip hoverSound;
        [SerializeField] AudioClip pressedSound;
        [SerializeField] CanvasEventListener[] canvasListeners = new CanvasEventListener[0];
        [SerializeField] CanvasStringChannelListener[] stringCanvasListeners = new CanvasStringChannelListener[0];

        AudioSource audioSource;
        UIScreenBase[] uiScreens;
        UIScreenBase currentScreen;

        public bool IsUsingController
        {
            get; private set;
        }

        void OnEnable()
        {
            ControllerManager.On_ChangeController += HandleControllerUpdate;
            Init();
            StartFirstScreen();
        }

        void Start()
        {

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
            _currentScreen.Hide(_nextScreen.HidePreviousScreenElements);
            while (_currentScreen.IsScreenDisplayed)
            {
                yield return null;
            }

            _nextScreen.Display();
            currentScreen = _nextScreen;
        }

        void OnDestroy()
        {
            for (int i = 0; i < canvasListeners.Length; i++)
            {
                canvasListeners[i].gameEvent.OnEventRaised -= canvasListeners[i].canvasResponse.Invoke;
            }

            for (int i = 0; i < stringCanvasListeners.Length; i++)
            {
                stringCanvasListeners[i].stringGameEvent.OnEventRaised -= stringCanvasListeners[i].canvasResponse.Invoke;
            }

            ControllerManager.On_ChangeController -= HandleControllerUpdate;
        }


        void HandleControllerUpdate(bool isUsingController)
        {
            IsUsingController = isUsingController;
            foreach (var screen in uiScreens)
            {
                screen.SetUsingController(isUsingController);
            }
            currentScreen.SetUIFocus();
        }

        void Init()
        {
            for (int i = 0; i < canvasListeners.Length; i++)
            {
                canvasListeners[i].gameEvent.OnEventRaised += canvasListeners[i].canvasResponse.Invoke;
            }

            for (int i = 0; i < stringCanvasListeners.Length; i++)
            {
                stringCanvasListeners[i].stringGameEvent.OnEventRaised += stringCanvasListeners[i].canvasResponse.Invoke;
            }

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
                    popup.InitSelectables();

                    foreach (Selectable popupSelectable in popup.GetComponentsInChildren<Selectable>(true))
                    {
                        UICancelHandler uiCancel = popupSelectable.gameObject.AddComponent<UICancelHandler>();
                        uiCancel.Init(uiScreen, popup, null);
                    }
                }

                foreach (TabManager tabManager in uiScreen.GetComponentsInChildren<TabManager>(true))
                {
                    foreach (Tab tab in tabManager.Tabs)
                    {
                        Selectable tabButton = tab.button.GetComponent<Selectable>();
                        UICancelHandler uiCancel = tabButton.gameObject.AddComponent<UICancelHandler>();
                        uiCancel.Init(uiScreen, null, tabManager);


                        //todo right here need to add it 
                        foreach (Selectable selectable in tab.tabPage.GetComponentsInChildren<Selectable>(true))
                        {
                            //Need to try/get in case the tabButton is in the tab page
                            if (!selectable.gameObject.TryGetComponent(out UICancelHandler contains))
                            {
                                UICancelHandler uiCancelPage = selectable.gameObject.AddComponent<UICancelHandler>();
                                uiCancelPage.Init(uiScreen, null, tabManager);
                            }
                        }
                    }
                }

                //Finally iterate through all selectables and if they're still missing a UICanceller slap it on here
                foreach (Selectable selectable in uiScreen.GetComponentsInChildren<Selectable>(true))
                {
                    if (!selectable.gameObject.TryGetComponent(out IgnoreUIAudio ignoreAudio))
                    {
                        UIAudio uiAudio = selectable.gameObject.AddComponent<UIAudio>();
                        uiAudio.Init(audioSource, hoverSound, pressedSound);
                    }

                    if (!selectable.gameObject.TryGetComponent(out UICancelHandler contains))
                    {
                        UICancelHandler uiCancel = selectable.gameObject.AddComponent<UICancelHandler>();
                        uiCancel.Init(uiScreen, null, null);
                    }
                }
            }
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