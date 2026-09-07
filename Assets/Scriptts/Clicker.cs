using UnityEngine;
using UnityEngine.EventSystems;

public class Clicker : MonoBehaviour, IPointerClickHandler
{
    Animator animator;
    private static readonly int ClickHash = Animator.StringToHash("Click");
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        animator.Play(ClickHash,0,0);

        GameManager.Instance.OnClick.Invoke();
    }
}
