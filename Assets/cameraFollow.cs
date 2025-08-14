using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset; 
    [SerializeField] private float damping;
    [SerializeField] float max_Y;
    [SerializeField] float min_Y;
    [SerializeField] float lookAhead;

    private Vector3 vel = Vector3.zero;

    private void FixedUpdate()
    {
        offset = PlayerMovement.instance.rb.velocity * lookAhead;
        Vector3 targetPosition = PlayerMovement.instance.transform.position + offset.x * Vector3.right;
        targetPosition.z = transform.position.z;
        if (targetPosition.y < min_Y)
        {
            targetPosition.y = min_Y;
        }
        else if (targetPosition.y > max_Y)
        {
            targetPosition.y = max_Y;
        }

        transform.position = Vector3.SmoothDamp(transform.position,targetPosition, ref vel, damping);

        transform.position = new Vector3(transform.position.x,targetPosition.y,transform.position.z);
    }
}
