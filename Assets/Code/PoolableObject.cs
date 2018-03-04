using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : GameBehaviour {

    public bool IsActivated { private set; get; }
    private PoolableManager _owner;

    public void Init(PoolableManager owner)
    {
        _owner = owner;
        Reset();
    }

    public virtual void Activate()
    {
        transform.parent = null;
        gameObject.SetActive(true);
        IsActivated = true;
    }

    public virtual void Deactivate()
    {
        Reset();
    }

    public virtual void Reset()
    {
        gameObject.transform.position = new Vector3(-999f, -999f, -999f);
        gameObject.SetActive(false);
        IsActivated = false;
        transform.parent = _owner.gameObject.transform;
    }
}
