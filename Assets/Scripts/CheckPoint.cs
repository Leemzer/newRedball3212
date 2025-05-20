using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Respawn respawnObject;
    private AudioSource audioSource;
      private bool isActivated = false;
    
    void Start()
    {
        respawnObject = GameObject.Find("Respawn").GetComponent<Respawn>(); // Find the Respawn object in the scene
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component attached to the checkpoint

          if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
    }
    void OnTriggerEnter(Collider touch)
    {
        if(touch.CompareTag("Player"))
        {
           respawnObject.transform.position = transform.position + new Vector3(0.8f, 0.5f, 0f); // Set the respawn point to the checkpoint position

            if (audioSource != null && audioSource.clip != null && !isActivated)
            {
                audioSource.Play();
                isActivated = true; // Prevents the audio from playing multiple times
            }

        }
    }

}