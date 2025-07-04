using System.Collections.Generic;
using UnityEngine;

public class mLB_KeyButtonParent : MonoBehaviour
{
    [Header("快捷键提示字体设置")]
    public Font mLB_sharedFont;
    [Header("快捷键提示样式")]
    
    public Color mLB_hintColor = new Color32(0xDD, 0x56, 0x00, 0xFF);
    [Header("是否启用按钮选中状态（高亮）")]
    public bool mLB_enableSelection = true;


    [System.Serializable]
    public class mLB_KeyButtonGroup
    {
        public List<mLB_KeyButton> mLB_buttons = new List<mLB_KeyButton>();
    }

    public mLB_KeyButtonGroup mLB_group = new mLB_KeyButtonGroup();

    [Header("Hint Text 全局控制")]
    public bool mLB_showAllKeyHints = true;

    private mLB_KeyButton mLB_currentSelected;

    void Start()
    {
        foreach (var btn in mLB_group.mLB_buttons)
        {
            if (btn != null)
            {
                btn.mLB_SetParent(this);
                btn.mLB_SetSelected(false);
                btn.mLB_ApplyKeyHint(mLB_showAllKeyHints);
            }
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        foreach (var btn in mLB_group.mLB_buttons)
        {
            if (btn != null)
            {
                btn.mLB_ApplyKeyHint(mLB_showAllKeyHints);
            }
        }
    }
#endif

    public void mLB_SetSelected(mLB_KeyButton newSelected)
    {
        if (!mLB_enableSelection)
            return;

        if (mLB_currentSelected != null && mLB_currentSelected != newSelected)
        {
            mLB_currentSelected.mLB_SetSelected(false);
        }

        mLB_currentSelected = newSelected;
        mLB_currentSelected?.mLB_SetSelected(true);
    }

}
