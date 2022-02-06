using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text hpText;
    [SerializeField] Text atText;
    [SerializeField] Text costText;
    [SerializeField] Image iconImage;
    [SerializeField] GameObject selectablePanel;
    [SerializeField] GameObject shieldPanel;

    public void Show(CardModel cardModel)
    {
        nameText.text = cardModel.name;
        hpText.text = cardModel.hp.ToString();
        atText.text = cardModel.at.ToString();
        costText.text = cardModel.cost.ToString(); 
        iconImage.sprite = cardModel.icon;
        // シールドもちの場合の見かけを帰る
        if (cardModel.ability == ABILITY.SHIELD)
        {
            shieldPanel.SetActive(true);
        }
        else
        {
            shieldPanel.SetActive(false);
        }
    }

    public void Refresh(CardModel cardModel)
    {
        hpText.text = cardModel.hp.ToString();
        atText.text = cardModel.at.ToString();
    }

    public void DisplaySelectablePanel(bool show)
    {
        selectablePanel.SetActive(show);
    }
}
