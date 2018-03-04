using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Profile
{
    public string Name;
    public SocialDataType.NetworkMode NetworkMode;
}

public class PlayerJSON : ScriptableObject 
{
    public Profile[] Players;
}
