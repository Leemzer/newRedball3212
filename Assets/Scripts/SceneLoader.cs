using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public bool isOnlyPlayerCanUse;
    public UnityEvent OnTriggerEnterEvent;

    void OnTriggerEnter(Collider touch)
    {

        if (isOnlyPlayerCanUse)
        {
            if (touch.CompareTag("Player"))
            {
                OnTriggerEnterEvent.Invoke();
            }
        }
        else
        {
            OnTriggerEnterEvent.Invoke();
        }
    }

    public void LoadScene(int lelvel)
    {
        SceneManager.LoadScene(lelvel);
    }


}
