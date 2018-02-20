using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour {

    private GestureCircle _gestureCircle;
    public GestureCircle GlobalGestureCircle
    {
        get
        {
            if (_gestureCircle == null)
                _gestureCircle = FindObjectOfType<GestureCircle>();

            return _gestureCircle;
        }
    }

    private FriendListManager _friendListManager;
    public FriendListManager GlobalFriendListManager
    {
        get
        {
            if (_friendListManager == null)
                _friendListManager = FindObjectOfType<FriendListManager>();

            return _friendListManager;
        }
    }

    private Spiral _spiralMaster;
    public Spiral GlobalSpiralMaster
    {
        get
        {
            if (_spiralMaster == null)
                _spiralMaster = FindObjectOfType<Spiral>();

            return _spiralMaster;
        }
    }
}
