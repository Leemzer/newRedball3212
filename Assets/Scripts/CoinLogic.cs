using UnityEngine;
using TMPro;
using System.IO;

public class CoinLogic : MonoBehaviour
{
    private Status playerStatus;
    public AudioClip coinSound;
    private TextMeshProUGUI coinText;
    private string savePath;

    void Start()
    {
        playerStatus = GameObject.Find("Status").GetComponent<Status>();
        GameObject textObject = GameObject.Find("CountCoin");
        if (textObject != null)
        {
            coinText = textObject.GetComponent<TextMeshProUGUI>();
        }

        // JSON-файл у папці Assets
        savePath = Path.Combine(Application.dataPath, "AbilityBoughtStatus.json");
    }

    void OnTriggerEnter(Collider touch)
    {
        if (touch.gameObject.CompareTag("Player"))
        {
            playerStatus.AddCoin();

            // Програємо звук
            touch.GetComponent<AudioSource>().PlayOneShot(coinSound);

            // Оновлення тексту монет
            if (coinText != null)
            {
                coinText.text = "Coins: " + playerStatus.coins;
            }

            // Оновлення JSON
            AbilityData data = LoadData();
            data.coins = playerStatus.coins;
            SaveData(data);

            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class AbilityData
    {
        public int coins = 0;
        public bool isBoughtTurbo = false;
        public bool isBoughtStopMovement = false;
        public bool isBoughtFall = false;
        public bool isBoughtStick = false;
    }

    private AbilityData LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<AbilityData>(json);
        }
        else
        {
            return new AbilityData();
        }
    }

    private void SaveData(AbilityData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }
}
