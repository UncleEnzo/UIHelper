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
        public TabGroup TabGroup { get; set; }
        public UnityEvent onTabSelected;
        public UnityEvent onTabDeselected;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"Mouse Click On Tab Button {gameObject.name}");
            TabGroup.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"Mouse Hover Over Tab Button {gameObject.name}");
            TabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log($"Mouse Exit Tab Button {gameObject.name}");
            TabGroup.OnTabExit(this);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Debug.Log($"Select On Tab Button {gameObject.name}");
            TabGroup.OnTabSelected(this);
        }

        public void OnSelect(BaseEventData eventData)
        {
            Debug.Log($"Select Hover Over Tab Button {gameObject.name}");
            TabGroup.OnTabEnter(this);
        }

        public void OnDeselect(BaseEventData eventData)
        {
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