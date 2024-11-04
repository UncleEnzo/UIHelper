using UnityEngine;
using UnityEngine.EventSystems;

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
            if (IsEnabled && pressedSound != null)
            {
                audioSource.PlayOneShot(pressedSound);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsEnabled && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (IsEnabled && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (IsEnabled && pressedSound != null)
            {
                audioSource.PlayOneShot(pressedSound);
            }
        }
    }
}