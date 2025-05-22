using UnityEngine;

public class DirectionPointer : MonoBehaviour
{
    public GameObject cameraObject;

    void Start()
    {
        
    }

    void Update()
    {
        var angles = cameraObject.transform.eulerAngles;
        angles.x = 0;
        angles.z = 0;
        transform.eulerAngles = angles;
        
    }
}
 