using UnityEngine;

public class DirectionPointer : MonoBehaviour
{
    public GameObject cameraObject;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var angles = cameraObject.transform.eulerAngles;
        angles.x = 0;
        angles.z = 0;
        transform.eulerAngles = angles;
        
    }
}
 