using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EncounterDialog : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;
    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] List<Text> actionTexts;
    [SerializeField] GameObject spellSelector;
    [SerializeField] List<Text> spellTexts;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableSpellSelector(bool enabled)
    {
        spellSelector.SetActive(enabled);
    }

    public void UpdateActionSelection(int currentAction)
    {
        for (int i=0; i<actionTexts.Count; ++i)
        {
            if (i == currentAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.white;
        }
    }

    public void UpdateSpellSelection(int currentSpell)
    {
        for (int i=0; i<spellTexts.Count; ++i)
        {
            if (i == currentSpell)
                spellTexts[i].color = highlightedColor;
            else
                spellTexts[i].color = Color.white;
        }
    }
}
