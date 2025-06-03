using UnityEngine;
using System.IO;

public class ShopLoader : MonoBehaviour
{
    [System.Serializable]
    public class AbilityData
    {
        public int coins = 0;
        public bool isBoughtTurbo = false;
        public bool isBoughtStopMovement = false;
        public bool isBoughtFall = false;
        public bool isBoughtStick = false;
    }

    public Material purchasedMaterial;

    private string savePath;

    void Start()
    {
        savePath = Path.Combine(Application.dataPath, "AbilityBoughtStatus.json");

        if (!File.Exists(savePath))
        {
            Debug.LogWarning("⚠ AbilityBoughtStatus.json not found!");
            return;
        }

        string json = File.ReadAllText(savePath);
        AbilityData data = JsonUtility.FromJson<AbilityData>(json);

        
        CheckAndDisableZone(data.isBoughtTurbo, "Buing_zone_turbo");
        CheckAndDisableZone(data.isBoughtStopMovement, "Buing_zone_stopmovement");
        CheckAndDisableZone(data.isBoughtFall, "Buing_zone_fall");
        CheckAndDisableZone(data.isBoughtStick, "Buing_zone_stick");
    }

    void CheckAndDisableZone(bool isBought, string zoneName)
    {
        if (!isBought) return;

        GameObject zone = GameObject.Find(zoneName);
        if (zone != null)
        {
            // Змінюємо матеріал
            Renderer rend = zone.GetComponent<Renderer>();
            if (rend != null && purchasedMaterial != null)
            {
                rend.material = purchasedMaterial;
            }

            // Вимикаємо колайдер
            Collider col = zone.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            Debug.Log("✅ " + zoneName + " вимкнено — здібність вже куплена");
        }
        else
        {
            Debug.LogWarning("❗ Не знайдено об'єкт " + zoneName);
        }
    }
}
