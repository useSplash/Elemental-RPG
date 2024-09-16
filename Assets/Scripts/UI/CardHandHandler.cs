using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandHandler : MonoBehaviour
{
    List<Transform> cardHand = new List<Transform>();
    float distanceBetweenCards = 120f;
    float adjustmentBetweenCards = 8f;
    float minimumDistanceBetweenCards = 100f;
    [SerializeField] Transform pfCardBase;
    [SerializeField] Card testCard;
    Vector3 spawnOffset = new Vector3(-5f, -5f);

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            DrawCard(testCard);
        }
    }

    public void DrawCard(Card card){
        Vector3 handPosition = transform.position;
        Transform cardTransform = Instantiate(pfCardBase, handPosition + spawnOffset, Quaternion.identity, transform);
        CardDisplay cardDisplay = cardTransform.GetComponent<CardDisplay>();
        cardDisplay.card = card;
        CardMovement cardMovement = cardTransform.GetComponent<CardMovement>();
        cardMovement.SetTargetPosition(new Vector2(cardMovement.GetTargetPosition().x, -30f));
        cardHand.Add(cardTransform);
        AdjustHand();
    }

    public void AdjustHand(){
        float calculatedDistanceBetweenCards = distanceBetweenCards - Mathf.Min(adjustmentBetweenCards * cardHand.Count, minimumDistanceBetweenCards);
        float rightMostCardDistance = (cardHand.Count - 1) * calculatedDistanceBetweenCards/2;

        for (int i = 0; i < cardHand.Count; i++) {
            cardHand[i].GetComponent<CardMovement>().SetTargetPosition(
                new Vector2 (rightMostCardDistance - (i * calculatedDistanceBetweenCards), 
                             cardHand[i].GetComponent<CardMovement>().GetTargetPosition().y));
            cardHand[i].GetComponent<CardMovement>().sortingOrder = i;
            cardHand[i].GetComponent<Canvas>().sortingOrder = i;
        }
    }
}
