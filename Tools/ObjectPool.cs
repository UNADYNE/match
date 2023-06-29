using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// This class will allow us to instantiate as many objects as we will need
    /// while the scene is being set up so that we don't slow down the game by
    /// calling Instantiate() during gameplay.
    ///
    /// It is preferred, but optional, to call PoolObjects() first with a specified amount
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectPool<T> : Singleton<ObjectPool<T>> where T : MonoBehaviour
    {
        [SerializeField] protected T prefab;

        protected List<T> pooledObjects;
        private int _amount;
        private bool _isReady;


        public void PoolObjects(int amount = 0)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("Pool amount cannot be less than 0");
            }

            _amount = amount;
            pooledObjects = new List<T>(amount);

            GameObject newObject;

            for (int i = 0; i < amount; i++)
            {
                newObject = Instantiate(prefab.gameObject, transform);
                newObject.SetActive(false);
                pooledObjects.Add(newObject.GetComponent<T>());
            }

            _isReady = true;
        }

        public T GetPooledObject()
        {
            if (!_isReady)
            {
                PoolObjects(1);
            }

            for (int i = 0; i < _amount; i++)
            {
                if (!pooledObjects[i].isActiveAndEnabled)
                {
                    return pooledObjects[i];
                }
            }

            GameObject newObject = Instantiate(prefab.gameObject, transform);
            newObject.SetActive(false);
            pooledObjects.Add(newObject.GetComponent<T>());
            _amount++;
            return newObject.GetComponent<T>();
        }

        public void ReturnObjectToPool(T toBeReturned)
        {
            // verify args
            if (toBeReturned == null) return;

            if (!_isReady)
            {
                PoolObjects();
                pooledObjects.Add(toBeReturned);
            }

            toBeReturned.gameObject.SetActive(false);
        }
    }
}