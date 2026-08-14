using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
public class DragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Canvas rootCanvas;
    CanvasGroup canvasGroup;
    Transform originalParent;

void Awake()
    {
        CacheReferences();
    }

void CacheReferences()
    {
        if (rootCanvas == null)
        {
            Canvas parentCanvas = GetComponentInParent<Canvas>(true);
            if (parentCanvas != null)
            {
                rootCanvas = parentCanvas.rootCanvas;
            }
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }


public void OnBeginDrag(PointerEventData eventData)
    {
        CacheReferences();
        if (rootCanvas == null || canvasGroup == null)
        {
            return;
        }

        originalParent = transform.parent;
        transform.SetParent(rootCanvas.transform, true);
        transform.SetAsLastSibling();
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

public void OnDrag(PointerEventData eventData)
    {
        CacheReferences();
        if (rootCanvas != null)
        {
            ((RectTransform)transform).anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
        }
    }

public void OnEndDrag(PointerEventData eventData)
    {
        CacheReferences();
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (rootCanvas != null && transform.parent == rootCanvas.transform && originalParent != null)
        {
            MoveTo(originalParent);
        }
    }

    public void MoveTo(Transform slot)
    {
        transform.SetParent(slot, false);
        transform.localPosition = Vector3.zero;
    }
}