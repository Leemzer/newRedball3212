using System.Collections;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    
    public GameObject playerObject;




    public void RespawnPlayer()
    {
        playerObject.GetComponent<Rigidbody>().isKinematic = true; // Disable physics
        playerObject.transform.position = transform.position; // Replace with your respawn coordinates
        
        StartCoroutine(UnfreezePlayer()); 

    }

    IEnumerator UnfreezePlayer()
    {
        yield return new WaitForSeconds(0.1f);
        playerObject.GetComponent<Rigidbody>().isKinematic = false;
    }



    void Update()
    {
        if (playerObject.transform.position.y < -10)
        {
            RespawnPlayer();
        }
    }

    void Start()
    {
        RespawnPlayer();
    }

}
