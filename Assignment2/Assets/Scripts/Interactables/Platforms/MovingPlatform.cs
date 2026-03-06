using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private float cycleTime = 5f;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float currentTime;

    private float _speed = 1f;

    void Update()
    {
        currentTime += _speed * Time.deltaTime;

        if (currentTime > cycleTime) _speed = -1f;
        if (currentTime < 0f) _speed = 1f;

        float t = currentTime / cycleTime;

        transform.position = Vector3.Lerp(pointA.position, pointB.position, t);
    }
}
