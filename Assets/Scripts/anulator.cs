using System.IO;
using UnityEngine;

public class Anulator : MonoBehaviour
{
    private string savePath;

    private void Start()
    {
        savePath = Path.Combine(Application.dataPath, "AbilityBoughtStatus.json");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!File.Exists(savePath))
        {
            Debug.LogWarning("⚠️ AbilityBoughtStatus.json not found.");
            return;
        }

        string json = File.ReadAllText(savePath);
        AbilityData data = JsonUtility.FromJson<AbilityData>(json);

        // Обнуляємо здібності
        data.isBoughtTurbo = false;
        data.isBoughtStopMovement = false;
        data.isBoughtFall = false;
        data.isBoughtStick = false;

        // Зберігаємо без зміни coins
        string updatedJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, updatedJson);

        Debug.Log("🔄 Abilities reset to false (coins preserved).");
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
}
