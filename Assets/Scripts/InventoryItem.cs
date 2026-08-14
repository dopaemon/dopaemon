using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public string itemName;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && InventorySystem.Instance != null)
        {
            InventorySystem.Instance.DropItem(itemName, gameObject);
        }
    }
}
