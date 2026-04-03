using UnityEngine;

public class MovementHandler
{
    public Vector3 CalculateExploreMovement(
        Vector2 moveInput,
        Camera camera,
        float moveSpeed,
        float rotationSpeed,
        Transform playerTransform,
        ref Vector3 velocity,
        float gravity)
    {
        Vector3 camForward = camera.transform.forward;
        Vector3 camRight = camera.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        
        Vector3 moveDirection = camRight * moveInput.x + camForward * moveInput.y;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            playerTransform.rotation = Quaternion.Slerp(
                playerTransform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        velocity = velocity.y * Vector3.up + moveSpeed * moveDirection;
        velocity.y += gravity * Time.deltaTime;

        return velocity;
    }

    public Vector3 CalculateAimMovement(
        Vector2 moveInput,
        Vector2 lookInput,
        float moveSpeed,
        float rotationSpeed,
        Transform playerTransform,
        ref Vector3 velocity,
        float gravity)
    {
        playerTransform.Rotate(Vector3.up, rotationSpeed * lookInput.x * Time.deltaTime);

        Vector3 moveDirection = moveInput.x * playerTransform.right + moveInput.y * playerTransform.forward;

        velocity = velocity.y * Vector3.up + moveSpeed * moveDirection;
        velocity.y += gravity * Time.deltaTime;
        
        return velocity;
    }
}
