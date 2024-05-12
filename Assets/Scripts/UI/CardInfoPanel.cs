using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI CardName;
    //public TextMeshProUGUI CardName;

    public void UpdateInfoPanelUnitCard(UnitCard uc) {
        CardName.text = uc.Name;
    }

    public void UpdateInfoPanelSpellCard(SpellCard sc) {
        
    }
}
