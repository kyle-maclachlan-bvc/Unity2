using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float sinkSpeed = 2f;

    private bool _isSinking = false;

    public void StartSinking()
    {
        _isSinking = true;
    }

    void update()
    {
        if (_isSinking)
        {
            transform.position += Vector3.down * sinkSpeed * Time.deltaTime;
        }
    }

}
