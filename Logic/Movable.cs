using System.Collections;
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
    [SerializeField] private float testReduction = 1f;

    private Vector3 _from;
    private Vector3 _to;
    private float _howFar;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float speedToTransform = 1f;
    protected bool _idle = true;
    public bool Idle => _idle;

    public IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        if (speed <= 0f)
        {
            Debug.LogError("Speed cannot be less than or equal to 0");
        }

        _from = transform.position;
        _to = targetPosition;
        _howFar = 0f;
        _idle = false;

        do
        {
            _howFar += speed * Time.deltaTime;
            if (_howFar > 1f)
                _howFar = 1f;
            transform.position = Vector3.LerpUnclamped(_from, _to, Easing(_howFar));
            yield return null;
        } while (_howFar != 1f);

        _idle = true;
    }

    public IEnumerator MoveToTransform(Transform target)
    {
        if (speed <= 0f)
        {
            Debug.LogWarning("Speed cannot be less than or equal to 0");
        }

        _from = transform.position;
        _to = target.position;
        _howFar = 0f;
        _idle = false;

        do
        {
            _howFar += speed * Time.deltaTime;
            if (_howFar > 1f)
            {
                _howFar = 1f;
            }
            print($"_howFar: {speed * Time.deltaTime}");

            _to = target.position;
            transform.position = Vector3.LerpUnclamped(_from, _to, Easing(1));
            Debug.Break();
            yield return null;
        } while (_howFar < 1f);

        _idle = true;
    }

    private float Easing(float t)
    {
        return Mathf.Clamp01(t * t * (3f - 2f * t));
    }
}