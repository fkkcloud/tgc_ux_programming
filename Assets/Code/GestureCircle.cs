using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GestureCircle : GameBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler 
{
    /*public float DragMotion { private set; get; }*/
    public float DragMotion;
    public float Speed;
    public Vector2 MakerOffset;
    public Vector2 ClampDragMotion;

    private Vector2 _markerPosition;
    private Queue<Vector2> _motionPositions = new Queue<Vector2>();
    private float _velocityDelta = 0f;

    private void Update()
    {
        // DEBUG
        if (Input.GetKey(KeyCode.D))
        {
            DragMotion = -0.25f;
            GlobalFriendListManager.OnFriendlistExit();
        }
    }

    public void OnDrag(PointerEventData data)
    {
        _motionPositions.Enqueue(data.position);
        if (_motionPositions.Count > 3)
        {
            _motionPositions.Dequeue();
        }
        else if (_motionPositions.Count == 2) //  when user swipe motion starts
        {
            // figure out if user started swipe towards -x or x
            // if x  : set swipe.x + offset
            // if -x : set swipe.x - offset
            // figure out if user started swipe towards -y or y
            // if y  : set swipe.y - offset
            // if -y : set swipe.y + offset
         }

        GetDragMotion();
    }

    public void OnPointerDown(PointerEventData data)
    {
        LeanTween.cancel(gameObject);
        _markerPosition = data.position - MakerOffset;

    }

    public void OnPointerUp(PointerEventData data)
    {
        //DragMotion = 0f;
        LeanTween.value(gameObject, DragMotion, 0f, 0.35f)
                 .setOnUpdate(SetDragMotion)
                 .setEase(LeanTweenType.easeOutQuad);

        _motionPositions.Clear();
    }

    public void SetDragMotion(float newDragMotion)
    {
        DragMotion = newDragMotion;
    }

    void GetDragMotion()
    {
        if (_motionPositions.Count < 3)
        {
            return;
        }

        Vector2[] motionPositions = _motionPositions.ToArray();
        Vector2 t1 = motionPositions[0] - _markerPosition;
        Vector2 t2 = motionPositions[1] - _markerPosition;
        Vector2 t3 = motionPositions[2] - _markerPosition;

        float a1 = SpiralMath.GetAngleAtan2(t1, t2);
        float a2 = SpiralMath.GetAngleAtan2(t2, t3);

        // make sure no matter how fast circular gesture happens, limit the speed
        float targetDragMotion = Mathf.Clamp((a1 + a2) * 0.5f, ClampDragMotion.x, ClampDragMotion.y);

        // any of the direction of motion velocity is applied, they all have to blend together
        DragMotion = Mathf.SmoothDamp(DragMotion, targetDragMotion, ref _velocityDelta, 0.5f);
    }
}
