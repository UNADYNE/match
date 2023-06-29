using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will allow any game object to be moved smoothly
/// from one position to another at a speed set in the inspector
/// using a coroutine.
///
/// Speed must be a positive number.
///
/// You can see if the object is currently moving by using idle.
/// 
/// There is an easing function to alter the speed of the animation over time
/// </summary>
public class Movable : MonoBehaviour
{
    private Vector3 _from;
    private Vector3 _to;
    private float _howFar;
    [SerializeField] private float speed = 1f;
    private bool _idle = true;
    public bool Idle => _idle;
    
    public IEnumerator MoveToPosition(Vector3 targetPosition, bool isMatched = false)
    {
        if (speed <= 0f)
            throw new System.ArgumentOutOfRangeException("Speed cannot be less than or equal to 0");
        _idle = false;
        _from = transform.position;
        _to = targetPosition;
        _howFar = 0f;
        do
        {
            _howFar += speed * Time.deltaTime;
            if(_howFar > 1f)
                _howFar = 1f;
            transform.position = Vector3.LerpUnclamped(_from, _to, EasingFunction(_howFar));
            yield return null;
        }
        while (_howFar != 1f);
        _idle = true;
        speed = 1f;
    }
    
    private float EasingFunction(float x)
    {
        // return x * x;
        return x * x * (3f - 2f * x);
        // return x * x * x * (x * (x * 6f - 15f) + 10f);
    }
    
}
