using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Profile
{
    public string Name;
    public FriendSlot.NetworkMode NetworkMode;
}

public class PlayerJSON : ScriptableObject 
{
    public Profile[] Players;
}
