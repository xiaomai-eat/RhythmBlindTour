using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIFileAttributte : UIAttributeBase, IPointerClickHandler
{
    [SerializeField]
    TMP_Text ShowFileName;
    public void SetShowFileName(string Name)
    {
        ShowFileName.text = Name;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SelectManager.SetAttribute(this);
    }
}
