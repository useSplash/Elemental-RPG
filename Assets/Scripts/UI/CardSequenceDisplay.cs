using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CardSequenceDisplay : MonoBehaviour
{
    public static CardSequenceDisplay Instance { get; private set; }
    public TMP_Text card1Text;
    public TMP_Text card2Text;
    public TMP_Text card3Text;
    public TMP_Text card4Text;

    private void Awake(){
        Instance = this;
    }

    public void SetCardText(List<Card> cardSequenceList){

        card1Text.text = "-";
        card2Text.text = "-";
        card3Text.text = "-";
        card4Text.text = "-";

        if (cardSequenceList.Count >= 1){
            card1Text.text = cardSequenceList[0].name;
        }
        if (cardSequenceList.Count >= 2){
            card2Text.text = cardSequenceList[1].name;
        }
        if (cardSequenceList.Count >= 3){
            card3Text.text = cardSequenceList[2].name;
        }
        if (cardSequenceList.Count >= 4){
            card4Text.text = cardSequenceList[3].name;
        }
    }
}
