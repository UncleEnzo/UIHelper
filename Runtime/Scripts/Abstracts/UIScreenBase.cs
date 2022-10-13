using UnityEngine;

namespace Nevelson.UIHelper
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreenBase : ButtonsBase, IScreen
    {
        CanvasGroup _canvasGroup;
        CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }

        public void BeforeDisplay()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Display()
        {
            CanvasGroup.alpha = 1f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
            //todo need to enable ui elements that player can focus on for controller
            //todo make it so that it focuses if using controller
        }

        public void AfterDisplay()
        {
            SetUIFocus();
            throw new System.NotImplementedException();
        }

        public void BeforeHide()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Hide()
        {
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            //TODO: NEED TO PREVENT FOCUSING ON AND INTERACTING WITH BUTTONS ON CLOSE
            //TODO: NEED TO SET PLAYER FOCUS ON APPEARING
        }

        public void AfterHide()
        {
            throw new System.NotImplementedException();
        }

        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void Update() { }

        void SetUIFocus()
        {
            throw new System.NotImplementedException();
        }
    }
}