using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI Description;
    public Image SpriteImage;

    // Leave blank if Combat
    [Header("Inventory")]
    [Space(10)]
    public TextMeshProUGUI Mana;
    public TextMeshProUGUI Health;
    public TextMeshProUGUI Atk;

    public void UpdateInfoPanelUnitCard(UnitCard uc) {
        CardName.text = uc.Name;
        Description.text = uc.description;
        if (SpriteImage != null){
            SpriteImage.sprite = uc.Artwork;
        }
    }

    public void UpdateInventoryInfoPanelUnitCard(UnitCard uc)
    {
        CardName.text = uc.Name;
        Description.text = uc.description;
        SpriteImage.sprite = uc.Artwork;

        Mana.text = uc.ManaCost + "";
        Health.text = uc.Health + "";
        Atk.text = uc.BaseDamage + "";
    }

    public void UpdateInfoPanelSpellCard(SpellCard sc) {
        CardName.text = sc.Name;
        Description.text = sc.description;
        if (SpriteImage != null) SpriteImage.sprite = sc.Artwork;
    }
}
