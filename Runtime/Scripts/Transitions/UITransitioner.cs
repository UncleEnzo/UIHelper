using System.Collections;
using UnityEngine;
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
            SetAnimator();
            animator.CrossFade(START_ANIM_STATE, 0);
        }

        public void On_EndTransition()
        {
            SetAnimator();
            animator.CrossFade(END_ANIM_STATE, 0);
        }

        public void On_FullTransition(float waitBetweenTransitions)
        {
            StartCoroutine(WaitForAnimationCo(waitBetweenTransitions));
        }

        void Start()
        {
            var animators = GetComponentsInChildren<Animator>();
            for (int i = 0; i < animators.Length; i++)
            {
                if (exitTransitionOnAwake && i == (int)transitions)
                {
                    Debug.Log("This happened");
                    On_EndTransition();
                }
            }
        }

        IEnumerator WaitForAnimationCo(float waitBetweenTransitions)
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