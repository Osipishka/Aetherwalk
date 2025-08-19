using UnityEngine;
using System.Collections;

public class AnimController : MonoBehaviour
{
    [Header("Animation Parameters")]
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string closeTrigger = "Close";
    [SerializeField] private float closeAnimationDuration = 0.5f;

    private Animator animator;
    private Canvas canvas;
    private Coroutine closeCoroutine;
    private bool isOpen;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        canvas = GetComponent<Canvas>();

        canvas.enabled = false;
        isOpen = false;
    }

    public virtual void Open()
    {
        if (isOpen) return;

        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
            closeCoroutine = null;
        }

        isOpen = true;
        canvas.enabled = true;
        animator.ResetTrigger(closeTrigger);
        animator.SetTrigger(openTrigger);
    }

    public virtual void Close()
    {
        if (!isOpen) return;

        isOpen = false;
        animator.ResetTrigger(openTrigger);
        animator.SetTrigger(closeTrigger);

        closeCoroutine = StartCoroutine(DisableCanvasAfterAnimation());
    }

    private IEnumerator DisableCanvasAfterAnimation()
    {
        yield return new WaitForSeconds(closeAnimationDuration);
        canvas.enabled = false;
        closeCoroutine = null;
    }
    public void OnCloseAnimationComplete()
    {
        canvas.enabled = false;
    }
    protected virtual void OnEnable()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }
}
