using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;   
    public Vector3 offset = new Vector3(-65, 30, 15);  
    public float followSpeed = 0.3f;      
    private bool followPlayer = true;   
    private Vector3 velocity = Vector3.zero; 

    void FixedUpdate() 
    {
        if (followPlayer && playerTransform != null)
        {
            Vector3 targetPosition = playerTransform.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followSpeed);
        }
    }

    public void StopFollowingPlayer()
    {
        followPlayer = false;
    }
}
