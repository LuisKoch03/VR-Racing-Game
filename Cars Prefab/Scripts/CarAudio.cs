using UnityEngine;

public class CarAudio : MonoBehaviour
{
    public AudioSource idleSource;
    public AudioSource driveSource;

    public Rigidbody carRb;
    public float speedThreshold = 1f;

    void Start()
    {
        idleSource.Play();
        driveSource.Play();
    }

    void Update()
    {
        float speed = carRb.linearVelocity.magnitude;

        if (speed > speedThreshold)
        {
            idleSource.volume = Mathf.Lerp(idleSource.volume, 0f, Time.deltaTime * 5f);
            driveSource.volume = Mathf.Lerp(driveSource.volume, 1f, Time.deltaTime * 5f);

            driveSource.pitch = 1f + (speed / 50f);
        }
        else
        {
            idleSource.volume = Mathf.Lerp(idleSource.volume, 1f, Time.deltaTime * 5f);
            driveSource.volume = Mathf.Lerp(driveSource.volume, 0f, Time.deltaTime * 5f);
            driveSource.pitch = 1f;
        }
    }
}
