using System.IO;
using UnityEngine;
using System.Collections;
using TMPro;

public class AbilityBuyer : MonoBehaviour
{
    public enum AbilityType { Turbo, StopMovement, Fall, Stick }
    public AbilityType abilityType = AbilityType.Turbo;

    public int price = 5;
    public Material purchasedMaterial;

    private string savePath;
    private TextMeshProUGUI coinText;
    private Collider zoneCollider;
    private Renderer zoneRenderer;

    private void Start()
    {
        savePath = Path.Combine(Application.dataPath, "AbilityBoughtStatus.json");

        GameObject coinTextObject = GameObject.Find("CountCoin");
        if (coinTextObject != null)
            coinText = coinTextObject.GetComponent<TextMeshProUGUI>();

        zoneCollider = GetComponent<Collider>();
        zoneRenderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        StartCoroutine(HandleTouch(other));
    }

    private IEnumerator HandleTouch(Collider other)
    {
        zoneCollider.enabled = false;

        Transform statusTransform = other.transform.Find("Status");
        if (statusTransform == null)
        {
            Debug.LogWarning("Status object not found!");
            yield return new WaitForSeconds(2f);
            zoneCollider.enabled = true;
            yield break;
        }

        Status status = statusTransform.GetComponent<Status>();
        if (status == null)
        {
            Debug.LogWarning("Status script not found!");
            yield return new WaitForSeconds(2f);
            zoneCollider.enabled = true;
            yield break;
        }

        AbilityData data = LoadData();

        // Перевірка чи здібність уже куплена
        if (IsAbilityAlreadyBought(data))
        {
            Debug.Log("Ability already purchased");
            yield break;
        }

        if (status.coins >= price)
        {
            status.coins -= price;
            Debug.Log("The purchase was made successfully");

            // Встановлюємо куплену здібність
            MarkAbilityAsBought(ref data);

            // Оновлюємо монети в JSON
            data.coins = status.coins;
            SaveData(data);

            if (coinText != null)
                coinText.text = "Coins: " + status.coins;

            if (purchasedMaterial != null && zoneRenderer != null)
                zoneRenderer.material = purchasedMaterial;

            zoneCollider.isTrigger = false;
        }
        else
        {
            Debug.Log("not enought coins to buy");
            yield return new WaitForSeconds(2f);
            zoneCollider.enabled = true;
        }
    }

    private bool IsAbilityAlreadyBought(AbilityData data)
    {
        return abilityType switch
        {
            AbilityType.Turbo => data.isBoughtTurbo,
            AbilityType.StopMovement => data.isBoughtStopMovement,
            AbilityType.Fall => data.isBoughtFall,
            AbilityType.Stick => data.isBoughtStick,
            _ => false
        };
    }

    private void MarkAbilityAsBought(ref AbilityData data)
    {
        switch (abilityType)
        {
            case AbilityType.Turbo:
                data.isBoughtTurbo = true;
                break;
            case AbilityType.StopMovement:
                data.isBoughtStopMovement = true;
                break;
            case AbilityType.Fall:
                data.isBoughtFall = true;
                break;
            case AbilityType.Stick:
                data.isBoughtStick = true;
                break;
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
