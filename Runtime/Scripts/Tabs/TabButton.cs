using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Selectable))]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, ISubmitHandler, IDeselectHandler
    {
        public TabManager TabGroup { get; set; }
        public UnityEvent onTabSelected;
        public UnityEvent onTabDeselected;
        Selectable _selectable;
        Selectable Selectable
        {
            get
            {
                if (_selectable == null)
                {
                    _selectable = GetComponent<Selectable>();
                }
                return _selectable;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Selectable.interactable)
            {
                return;
            }

            Debug.Log($"Mouse Click On Tab Button {gameObject.name}");
            TabGroup.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Selectable.interactable)
            {
                return;
            }
            Debug.Log($"Mouse Hover Over Tab Button {gameObject.name}");
            TabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Selectable.interactable)
            {
                return;
            }
            Debug.Log($"Mouse Exit Tab Button {gameObject.name}");
            TabGroup.OnTabExit(this);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!Selectable.interactable)
            {
                return;
            }
            Debug.Log($"Select On Tab Button {gameObject.name}");
            TabGroup.OnTabSelected(this);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!Selectable.interactable)
            {
                return;
            }
            Debug.Log($"Select Hover Over Tab Button {gameObject.name}");
            TabGroup.OnTabEnter(this);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!Selectable.interactable)
            {
                return;
            }
            Debug.Log($"Select Exit Tab Button {gameObject.name}");
            TabGroup.OnTabExit(this);
        }

        public void Select()
        {
            onTabSelected?.Invoke();
        }

        public void Deselect()
        {
            onTabDeselected?.Invoke();
        }
    }
}