using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GestureSwipe : GameBehaviour, IBeginDragHandler, IEndDragHandler {

    private Vector2 DragStartPos;
    private Vector2 DragEndPos;
    private float DragStartTime;

    private bool _exitingFriendlist = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (_exitingFriendlist){

            if (GlobalSpiralMaster.GetActuralLength() > 0f)
            {
                GlobalGestureCircle.DragMotion = -0.6f;
                GlobalFriendListManager.OnFriendlistExit();
            }
            else 
            {
                _exitingFriendlist = false;
                GlobalFriendListManager.ResetPlayerID();
            }
        }


	}

    void ExitFriendlist()
    {
        _exitingFriendlist = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragStartTime = Time.time;
        DragStartPos = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragEndPos = eventData.position;
        float dist = Vector2.Distance(DragStartPos, DragEndPos);
        float diff = Time.time - DragStartTime;

        float speed = dist / diff;

        if (speed > 1000f)
        {
            bool isSwipeRight = DragStartPos.x < DragEndPos.x ? true : false;

            if (isSwipeRight)
            {
                ExitFriendlist();    
            }
        }
    }
}
