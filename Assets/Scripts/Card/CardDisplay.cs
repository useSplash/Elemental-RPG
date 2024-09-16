using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Card card;
    public Image artwork;
    public TMP_Text nameText;
    public TMP_Text targettingText;
    public TMP_Text descriptionText;
    public TMP_Text damageText;


    private void Start(){
        artwork.sprite = card.artwork;
        nameText.text = card.name;
        targettingText.text = card.target.ToString();
        if (card.description == ""){
            descriptionText.text = "No Description";
        }
        else { 
            descriptionText.text = card.description;
        }
        damageText.text = card.damageAmount.ToString();
    }
}
