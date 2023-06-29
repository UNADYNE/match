using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    
    //getter
    
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError($"No instance of {typeof(T)} is in the scene.");
            }
            return _instance;
        }
    }

    protected void Awake()
    {
        if(_instance == null)
        {
            _instance = (T) this;

            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected void OnDestroy()
    {
        if (this == _instance)
        {
            _instance = null;
        }
    }

    // Init will replace the functionality of Awake

    protected virtual void Init(){}



}
