using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMovement : MonoBehaviour
{
    RectTransform uiElement;
    Vector2 targetPosition;
    Vector3 targetScale = new Vector3(1, 1, 1);
    Vector2 offset;
    public int sortingOrder;

    private void Awake(){
        uiElement = GetComponent<RectTransform>();
    }

    private void Update(){

        if (Vector2.Distance(uiElement.anchoredPosition, targetPosition + offset) < 0.1f) {
            uiElement.anchoredPosition = targetPosition + offset;
        }
        else {
            float cardAdjustmentSpeed = 7f;
            uiElement.anchoredPosition += (targetPosition + offset - uiElement.anchoredPosition) * cardAdjustmentSpeed * Time.deltaTime;   
        }
        if (Vector3.Distance(transform.localScale, targetScale) < 0.01f) {
            transform.localScale = targetScale;
        }
        else {
            float cardScaleSpeed = 20f;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, cardScaleSpeed * Time.deltaTime);
        }
    }

    public void SetTargetPosition(Vector2 position){
        targetPosition = position;
    }

    public Vector2 GetTargetPosition(){
        return targetPosition;
    }

    public void SetOffsetPosition(Vector2 position){
        offset = position;
    }

    public void SetSortingOrder(int index){
        GetComponent<Canvas>().sortingOrder = index;
    }

    public void SetTargetScale(float value){
        targetScale = new Vector3(value, value, value);
    }
}