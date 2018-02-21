using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendListManager : GameBehaviour 
{
    public float IntervalDist;
    public float SelectedTValue;
    public float SelectedTLockInterval;

    public GameObject FriendSlotTemplate;
    public PlayerJSON PlayersJSON;

    private List<FriendSlot> _friendSlots = new List<FriendSlot>();
    private int _playerID;

	void Start () 
    {
        _playerID = 0;
	}

    public void RemoveFriendFromSlot(FriendSlot slot, bool isRemovedBackward = false)
    {
        _friendSlots.Remove(slot);
        if (isRemovedBackward)
            _playerID--;
    }

    bool IsSpaceEnough()
    {
        // when there is no motion, dont spawn anything
        if (GlobalGestureCircle.DragMotion <= 0f)
            return false;

        foreach (FriendSlot slot in _friendSlots)
        {
            if (slot.t < IntervalDist)
            {
                return false;
            }
        }
        return true;
    }

    public void OnFriendlistExit()
    {
        GlobalSpiralMaster.OnFriendlistExit();
        for (int i = 0; i < _friendSlots.Count; i++)
        {
            _friendSlots[i].OnFriendlistExit(); 
        }
        _playerID = 0;
    }

    public void ResetPlayerID()
    {
        _playerID = 0;
    }

    void SpawnFriendSlot()
    {
        Vector3 startPos = Vector3.zero;
        float radiusX = GlobalSpiralMaster.GetCurrentRadius(0f, ref GlobalSpiralMaster.RadiusMapX);
        float radiusY = GlobalSpiralMaster.GetCurrentRadius(0f, ref GlobalSpiralMaster.RadiusMapY);
        SpiralMath.GetPositionAt(0f, ref startPos, radiusX, radiusY, GlobalSpiralMaster.ScaleZ, GlobalSpiralMaster.NoiseOn);
        startPos += GlobalSpiralMaster.transform.position;

        // TODO: could use pool for static memory usage but for TGC test time, just go with dynamic instantiation.
        GameObject gameObj = Instantiate(FriendSlotTemplate, startPos, Quaternion.identity);

        // Init the slot
        FriendSlot friendSlot = gameObj.GetComponent<FriendSlot>();
        if (_playerID < 0) // temp fix for TGC iPhone demo
            _playerID = 0;
        friendSlot.Init(0f, PlayersJSON.Players[_playerID].NetworkMode, PlayersJSON.Players[_playerID].Name);

        // Add the slot to list
        _friendSlots.Add(friendSlot);
        _playerID++;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (IsSpaceEnough())
        {
            SpawnFriendSlot();
        }
        UpdateListAnimation();
	}

    void UpdateListAnimation()
    {
        foreach (FriendSlot slot in _friendSlots)
        {
            slot.t += GlobalGestureCircle.DragMotion;
        }            
    }
}
