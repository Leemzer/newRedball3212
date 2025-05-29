using UnityEngine;
using TMPro;
public class CoinLogic : MonoBehaviour
{
    private Status playerStatus;
    public AudioClip coinSound;
    private TextMeshProUGUI coinText;

    void Start()
    {
        playerStatus = GameObject.Find("Status").GetComponent<Status>();
        GameObject textObject = GameObject.Find("CountCoin");
        if (textObject != null)
        {
            coinText = textObject.GetComponent<TextMeshProUGUI>();
        }
    }

    void OnTriggerEnter(Collider touch)
    {
        if (touch.gameObject.CompareTag("Player"))
        {
            playerStatus.AddCoin();
            touch.GetComponent<AudioSource>().PlayOneShot(coinSound);
            if (coinText != null)
            {
                coinText.text = "Coins: " + playerStatus.coins;
            }
            Destroy(gameObject);
        }
    }
}
