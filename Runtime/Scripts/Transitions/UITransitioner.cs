using System;
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
        public static EventHandler on_StartTransitionBegin;
        [SerializeField] UnityEvent onStartTransitionComplete;
        public static EventHandler on_StartTransitionComplete;
        [SerializeField] UnityEvent onExitTransitionBegin;
        public static EventHandler on_ExitTransitionBegin;
        [SerializeField] UnityEvent onExitTransitionComplete;
        public static EventHandler on_ExitTransitionComplete;


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
            on_StartTransitionBegin?.Invoke(this, EventArgs.Empty);
            onStartTransitionBegin?.Invoke();
            SetAnimator();
            animator.CrossFade(START_ANIM_STATE, 0);
            StartCoroutine(AfterTransitionEventCo(onStartTransitionComplete, on_StartTransitionComplete));
        }

        public void On_EndTransition()
        {
            on_ExitTransitionBegin?.Invoke(this, EventArgs.Empty);
            onExitTransitionBegin?.Invoke();
            SetAnimator();
            animator.CrossFade(END_ANIM_STATE, 0);
            StartCoroutine(AfterTransitionEventCo(onExitTransitionComplete, on_ExitTransitionComplete));
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

        IEnumerator AfterTransitionEventCo(UnityEvent uEvent, EventHandler sEvent)
        {
            yield return null;
            while (!IsTransitionPlaying)
            {
                yield return null;
            }
            sEvent?.Invoke(this, EventArgs.Empty);
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