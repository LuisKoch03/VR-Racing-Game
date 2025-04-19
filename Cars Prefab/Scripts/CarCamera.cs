using UnityEngine;

public class CarCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -5f);
    public float smoothSpeed = 5f;
    public bool thirdPerson;
  
    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        if (thirdPerson)
        {
            transform.LookAt(target);
        }
    }
}


