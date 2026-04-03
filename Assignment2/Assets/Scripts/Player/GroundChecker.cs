using UnityEngine;

public class GroundChecker
{
    private float _lastGroundedTime;

    public bool CheckGrounded(
        Transform transform,
        Vector3 offset,
        float radius,
        float distance,
        LayerMask layer,
        float coyoteTime)
    {
        bool isGrounded = Physics.SphereCast(
            transform.position + offset,
            radius,
            Vector3.down,
            out RaycastHit hit,
            distance,
            layer
        );
        
        if (isGrounded)
            _lastGroundedTime = Time.time;
        
        return Time.time - _lastGroundedTime <= coyoteTime;
    }
    
}
