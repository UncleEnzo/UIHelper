using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    public class UIAudio : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler, ISubmitHandler
    {
        AudioSource audioSource;
        AudioClip hoverSound;
        AudioClip pressedSound;
        public bool IsEnabled { get; set; } = true;

        public void Init(AudioSource audioSource, AudioClip hoverSound, AudioClip pressedSound)
        {
            this.audioSource = audioSource;
            this.hoverSound = hoverSound;
            this.pressedSound = pressedSound;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Selectable selectable = GetComponent<Selectable>();
            if (selectable != null && !selectable.interactable)
            {
                return;
            }

            if (IsEnabled && pressedSound != null)
            {
                audioSource.PlayOneShot(pressedSound);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Selectable selectable = GetComponent<Selectable>();
            if (selectable != null && !selectable.interactable)
            {
                return;
            }

            if (IsEnabled && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            Selectable selectable = GetComponent<Selectable>();
            if (selectable != null && !selectable.interactable)
            {
                return;
            }

            if (IsEnabled && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Selectable selectable = GetComponent<Selectable>();
            if (selectable != null && !selectable.interactable)
            {
                return;
            }

            if (IsEnabled && pressedSound != null)
            {
                audioSource.PlayOneShot(pressedSound);
            }
        }
    }
}