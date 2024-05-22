using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI Description;

    public void UpdateInfoPanelUnitCard(UnitCard uc) {
        CardName.text = uc.Name;
        Description.text = uc.description;
    }

    public void UpdateInfoPanelSpellCard(SpellCard sc) {
        CardName.text = sc.Name;
        Description.text = sc.description;
    }
}
