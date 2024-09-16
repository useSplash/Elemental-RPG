using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHover : MonoBehaviour
{
    CardMovement cardMovement;
    float hoverOffsetY = 70f;
    private void Awake(){
        cardMovement = GetComponent<CardMovement>();
    }
    public void OnMouseEnter()
    {
        if (cardMovement == null) return;
        cardMovement.SetOffsetPosition(new Vector2(0, hoverOffsetY));
        cardMovement.SetTargetScale(1.5f);
        cardMovement.SetSortingOrder(10);
    }

    public void OnMouseExit()
    {
        if (cardMovement == null) return;
        cardMovement.SetOffsetPosition(new Vector2(0, 0));
        cardMovement.SetTargetScale(1f);
        cardMovement.SetSortingOrder(cardMovement.sortingOrder);
    }
}
