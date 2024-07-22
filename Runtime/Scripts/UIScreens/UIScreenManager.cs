using Nevelson.Utils;
using Rewired;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        private Player player;
        bool isUsingController = false;
        ControllerType currentCType;
        public bool IsUsingController
        {
            get => isUsingController;
            set
            {
                if (isUsingController == value) return;
                isUsingController = value;
                foreach (var screen in uiScreens)
                {
                    screen.SetUsingController(isUsingController);
                }
                currentScreen.SetUIFocus();
            }
        }

        void Start()
        {
            player = ReInput.players.GetPlayer(0); // Assuming Player 0
            ReInput.ControllerConnectedEvent += OnControllerConnected;
            ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;

            Init();
            StartFirstScreen();
        }

        void OnControllerConnected(ControllerStatusChangedEventArgs args)
        {
            Debug.Log($"{UISCREEN_MANAGER_LOG} Controller Connected: " + args.name);
            IsUsingController = true;
        }

        void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
        {
            Debug.Log($"{UISCREEN_MANAGER_LOG} Controller Disconnected: " + args.name);
            IsUsingController = false;
        }

        void Update()
        {
            UpdateCurrentController();

            // Removing functionality to switch using a serialized BOOl
            //if (currentCType != isUsingController)
            //{
            //    RefreshScreenControls();
            //}
        }

        void UpdateCurrentController()
        {
            //handles dynamic controller switching.
            //mouse and keyboard are treated as the SAME controller
            ControllerType cType = player.controllers.GetLastActiveController().type;
            if ((cType == ControllerType.Joystick && cType != currentCType) ||
                (cType == ControllerType.Mouse && currentCType == ControllerType.Joystick) ||
                (cType == ControllerType.Keyboard && currentCType == ControllerType.Joystick))
            {
                Debug.Log($"{UISCREEN_MANAGER_LOG} Switching from {currentCType} to {cType}");
                //is using controller automatically refreshes controls
                IsUsingController = cType == ControllerType.Joystick;
                currentCType = cType;
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

            ReInput.ControllerConnectedEvent -= OnControllerConnected;
            ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
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

                        foreach (Selectable selectable in tab.tabPage.GetComponentsInChildren<Selectable>())
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