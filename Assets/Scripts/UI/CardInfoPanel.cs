using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI Description;
    public Image Image;

    public void UpdateInfoPanelUnitCard(UnitCard uc) {
        CardName.text = uc.Name;
        Description.text = uc.description;
        Image.sprite = uc.Artwork;
    }

    public void UpdateInfoPanelSpellCard(SpellCard sc) {
        CardName.text = sc.Name;
        Description.text = sc.description;
        Image.sprite = sc.Artwork;
    }
}
