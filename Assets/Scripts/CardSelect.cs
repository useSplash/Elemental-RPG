using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelect : MonoBehaviour
{
    CardDisplay cardDisplay;
    RectTransform uiElement;

    private void Awake(){
        cardDisplay = GetComponent<CardDisplay>();
        uiElement = this.GetComponent<RectTransform>();
    }

    public void SetCard(){
        BattleHandler.GetInstance().card = cardDisplay.card;
    }

    void OnMouseDown(){
        SetCard();
    }

    void OnMouseEnter(){
        uiElement.anchoredPosition = new Vector2(uiElement.anchoredPosition.x, 30);
    }
    void OnMouseExit(){
        uiElement.anchoredPosition = new Vector2(uiElement.anchoredPosition.x, -30);
    }
}