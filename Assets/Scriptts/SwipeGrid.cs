using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeGrid : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] float speed = 1;
    [SerializeField] float inertia = 1000f;
    [SerializeField] float minY = 0;
    float velocity;
    float startY;
    bool Drag;
    public void OnPointerDown(PointerEventData eventData)
    {
        Drag = true;
        startY = eventData.position.y;
        velocity = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Drag = false;
    }

    private void Update()
    {
        if (Drag)
        {

            if (Input.touchCount > 0)
            {

                float currentY = Input.GetTouch(0).position.y;
                float delayY = currentY - startY;

                transform.position += Vector3.up * delayY * speed;


                velocity = delayY * speed;
                startY = currentY;

            }


        }
        else
        {
            transform.position += Vector3.up * velocity * Time.deltaTime;
            velocity = Mathf.MoveTowards(velocity, 0, inertia * Time.deltaTime);
        }
        var pos = transform.localPosition;
        pos.y = Mathf.Max(pos.y, minY);
        transform.localPosition = pos;
    }
}
