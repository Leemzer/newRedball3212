using UnityEngine;

public class SuperCheckpoint : MonoBehaviour
{
    public Respawn respawnObject;
    public Vector3 offset = new Vector3(3f, 5f, 0f);
    private AudioSource audioSource;
    private bool isPlayingLoop = false;

    void Start()
    {
        respawnObject = GameObject.Find("Respawn").GetComponent<Respawn>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    void OnTriggerEnter(Collider touch)
    {
        if (touch.CompareTag("Player") && !isPlayingLoop)
        {
            respawnObject.transform.position = transform.position + offset;
            isPlayingLoop = true;
            StartCoroutine(PlaySoundLoop());
        }
    }

    System.Collections.IEnumerator PlaySoundLoop()
    {
        while (true)
        {
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.pitch = Random.Range(0.7f, 1.5f);
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length / audioSource.pitch);
            }
            else
            {
                yield break; // зупинити корутину, якщо звук зник
            }
        }
    }
}
