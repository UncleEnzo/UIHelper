using UnityEngine;
using UnityEngine.EventSystems;

namespace Nevelson.UIHelper
{
    public class UIAudio : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler, ISubmitHandler
    {
        AudioSource audioSource;
        AudioClip hoverSound;
        AudioClip pressedSound;

        public void Init(AudioSource audioSource, AudioClip hoverSound, AudioClip pressedSound)
        {
            this.audioSource = audioSource;
            this.hoverSound = hoverSound;
            this.pressedSound = pressedSound;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (pressedSound != null)
            {
                audioSource.PlayOneShot(pressedSound);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (pressedSound != null)
            {
                audioSource.PlayOneShot(pressedSound);
            }
        }
    }
}