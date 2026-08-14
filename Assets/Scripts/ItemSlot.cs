using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
 
 
 
public class ItemSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount != 0 || eventData.pointerDrag == null)
        {
            return;
        }

        DragDrop item = eventData.pointerDrag.GetComponent<DragDrop>();
        if (item != null)
        {
            item.MoveTo(transform);
        }
    }
}