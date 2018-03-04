using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableUIObject : PoolableObject 
{
    public Renderer Rend;
    public TextMesh TextRend;

    public SocialDataType.SlotMode CurrentSlotMode { protected set; get; }
    public SocialDataType.NetworkMode CurrentNetworkMode { protected set; get; }

    protected bool _initiated = false;

    public override void Deactivate()
    {
        _initiated = false;
        base.Deactivate();
    }
}
