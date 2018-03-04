using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableManager : MonoBehaviour {

    public int PoolCount = 40;

    private PoolableObject[] _pooledInstances;

    public PoolableObject UITemplate;

    void Awake()
    {
        _pooledInstances = new PoolableObject[PoolCount];
        for (int i = 0; i < _pooledInstances.Length; i++)
        {
            _pooledInstances[i] = Instantiate(UITemplate);
            _pooledInstances[i].Init(this);
        }
    }

    public PoolableObject GetPoolableObject()
    {
        for (int i = 0; i < _pooledInstances.Length; i++)
        {
            if (!_pooledInstances[i].IsActivated)
            {
                return _pooledInstances[i];
            }
        }

        Debug.LogError("There are no more availability on this Pool. Consider increasing maximum pool count.");
        return null;
    }
}
