using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    public class UITransitioner : MonoBehaviour
    {
        public enum Transitions { FADE, CIRCLE_WIPE }
        [SerializeField] Transitions transitions = Transitions.FADE;
        [SerializeField, Range(0, 1)] public float transitionSpeed = 1;
        [SerializeField] Color color = Color.black;
        [SerializeField] bool exitTransitionOnAwake;
        [SerializeField] UnityEvent onStartTransitionBegin;
        [SerializeField] UnityEvent onStartTransitionComplete;
        [SerializeField] UnityEvent onExitTransitionBegin;
        [SerializeField] UnityEvent onExitTransitionComplete;

        Animator animator;
        const string START_ANIM_STATE = "StartTransition";
        const string END_ANIM_STATE = "EndTransition";
        const string NONE_STATE = "None"; //Set as entry state in animations by default

        public bool IsTransitionPlaying
        {
            get =>
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 &&
                !animator.IsInTransition(0);
        }

        public void On_TransitionAndChangeScene(string sceneName)
        {
            StartCoroutine(WaitForAnimationChangeSceneCo(sceneName));
        }

        public void On_StartTransition()
        {
            onStartTransitionBegin?.Invoke();
            SetAnimator();
            animator.CrossFade(START_ANIM_STATE, 0);
            StartCoroutine(AfterTransitionEventCo(onStartTransitionComplete));
        }

        public void On_EndTransition()
        {
            onExitTransitionBegin?.Invoke();
            SetAnimator();
            animator.CrossFade(END_ANIM_STATE, 0);
            StartCoroutine(AfterTransitionEventCo(onExitTransitionComplete));
        }

        public void On_FullTransition(float waitBetweenTransitions)
        {
            StartCoroutine(WaitForAnimationFullTransitionCo(waitBetweenTransitions));
        }

        void Start()
        {
            var animators = GetComponentsInChildren<Animator>();
            for (int i = 0; i < animators.Length; i++)
            {
                if (exitTransitionOnAwake && i == (int)transitions)
                {
                    On_EndTransition();
                }
            }
        }

        IEnumerator AfterTransitionEventCo(UnityEvent uEvent)
        {
            yield return null;
            while (!IsTransitionPlaying)
            {
                yield return null;
            }
            uEvent?.Invoke();
        }

        IEnumerator WaitForAnimationFullTransitionCo(float waitBetweenTransitions)
        {
            SetAnimator();
            On_StartTransition();
            yield return null;
            while (!IsTransitionPlaying)
            {
                yield return null;
            }
            yield return new WaitForSeconds(waitBetweenTransitions);
            On_EndTransition();
        }

        IEnumerator WaitForAnimationChangeSceneCo(string sceneName)
        {
            SetAnimator();
            On_StartTransition();
            yield return null;
            while (!IsTransitionPlaying)
            {
                yield return null;
            }
            SceneManager.LoadScene(sceneName);
        }

        void SetAnimator()
        {
            animator = transform.GetChild((int)transitions).GetComponent<Animator>();
            transform.GetChild((int)transitions).GetComponent<Image>().color = color;
            animator.speed = transitionSpeed;
        }
    }
}