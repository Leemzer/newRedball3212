using UnityEngine;

public class DeadZone : MonoBehaviour
{
    
    public Respawn respawnObject;

    private void OnTriggerEnter(Collider touch)
    {
        if (touch.CompareTag("Player"))
        {
                respawnObject.RespawnPlayer();
        }
    }
    void Start()
    {
        respawnObject = GameObject.Find("Respawn").GetComponent<Respawn>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
