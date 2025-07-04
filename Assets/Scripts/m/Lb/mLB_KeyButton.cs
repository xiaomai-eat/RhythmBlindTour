using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class mLB_KeyButton : MonoBehaviour
{
    public enum mLB_KeyTriggerTiming
    {
        Down,
        Up,
        Click
    }
    [Header("Button按键触发方式")]
    public mLB_KeyTriggerTiming mLB_triggerTiming = mLB_KeyTriggerTiming.Click;

    public KeyCode mLB_key = KeyCode.None;
    public bool mLB_isSelected = false;
    public Color mLB_normalColor = Color.white;
    public Color mLB_selectedColor = Color.green;

    public bool mLB_showKeyHint = true;

    private Button mLB_button;
    private EventTrigger mLB_eventTrigger;
    private Image mLB_image;
    private mLB_KeyButtonParent mLB_parent;
    private bool mLB_isPressed = false;

    private Text mLB_hintText;
    private const string mLB_hintGOName = "KeyHint";

    void Awake()
    {
        mLB_button = GetComponent<Button>();
        mLB_eventTrigger = GetComponent<EventTrigger>();
        mLB_image = GetComponent<Image>();

        if (Application.isPlaying)
        {
            if (mLB_button != null)
            {
                var nav = mLB_button.navigation;
                nav.mode = Navigation.Mode.None;
                mLB_button.navigation = nav;
            }

            UpdateColor();
        }
    }
#if UNITY_EDITOR
    public void mLB_ApplyKeyHint(bool enable)
    {
        const string hintName = mLB_hintGOName;

        Transform existing = transform.Find(hintName);
        GameObject textGO;

        // 不再销毁，改为启用/禁用
        if (!enable)
        {
            if (existing != null)
                existing.gameObject.SetActive(false);
            return;
        }

        if (mLB_key == KeyCode.None) return;

        if (existing == null)
        {
            textGO = new GameObject(hintName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textGO.transform.SetParent(transform, false);
        }
        else
        {
            textGO = existing.gameObject;
            textGO.SetActive(true);
        }

        textGO.transform.SetAsLastSibling();

        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Text hintText = textGO.GetComponent<Text>();
        hintText.text = mLB_key.ToString();
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.raycastTarget = false;

        // 从 parent 获取颜色和字体
        Font fontToUse = mLB_parent != null && mLB_parent.mLB_sharedFont != null
            ? mLB_parent.mLB_sharedFont
            : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Color colorToUse = mLB_parent != null
            ? mLB_parent.mLB_hintColor
            : new Color32(0xDD, 0x56, 0x00, 0xFF);

        hintText.font = fontToUse;
        hintText.color = colorToUse;
        hintText.resizeTextForBestFit = true;
        hintText.resizeTextMinSize = 1;
        hintText.resizeTextMaxSize = 300;
    }
#endif



    void Update()
    {
        if (!Application.isPlaying || mLB_key == KeyCode.None) return;

        if (Input.GetKeyDown(mLB_key)) OnKeyDown();
        else if (Input.GetKey(mLB_key)) OnKeyPressed();
        else if (Input.GetKeyUp(mLB_key)) OnKeyUp();
    }

    private void OnKeyDown()
    {
        mLB_isPressed = true;
        mLB_parent?.mLB_SetSelected(this);

        if (mLB_button != null)
        {
            if (mLB_triggerTiming == mLB_KeyTriggerTiming.Down)
            {
                mLB_button.onClick.Invoke();
            }
            else if (mLB_triggerTiming == mLB_KeyTriggerTiming.Click)
            {
                var ped = new PointerEventData(EventSystem.current);
                mLB_button.OnPointerDown(ped);
            }
        }
        else if (mLB_eventTrigger != null)
        {
            ExecuteEventTrigger(EventTriggerType.PointerDown, new PointerEventData(EventSystem.current));
        }
    }

    private void OnKeyUp()
    {
        mLB_isPressed = false;

        if (mLB_button != null)
        {
            if (mLB_triggerTiming == mLB_KeyTriggerTiming.Up)
            {
                mLB_button.onClick.Invoke();
            }
            else if (mLB_triggerTiming == mLB_KeyTriggerTiming.Click)
            {
                mLB_button.onClick.Invoke();
                var ped = new PointerEventData(EventSystem.current);
                mLB_button.OnPointerUp(ped);
            }
        }
        else if (mLB_eventTrigger != null)
        {
            ExecuteEventTrigger(EventTriggerType.PointerUp, new PointerEventData(EventSystem.current));
        }
    }

    private void OnKeyPressed() { }


    private void ExecuteEventTrigger(EventTriggerType type, PointerEventData data)
    {
        foreach (var entry in mLB_eventTrigger.triggers)
        {
            if (entry.eventID == type)
            {
                entry.callback.Invoke(data);
            }
        }
    }

    public void mLB_SetParent(mLB_KeyButtonParent parentRef)
    {
        mLB_parent = parentRef;
    }

    public void mLB_SetSelected(bool selected)
    {
        mLB_isSelected = selected;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (mLB_image == null) return;

        // 如果不启用选择，则始终为 normalColor
        if (mLB_parent != null && !mLB_parent.mLB_enableSelection)
        {
            mLB_image.color = mLB_normalColor;
            return;
        }

        mLB_image.color = mLB_isSelected ? mLB_selectedColor : mLB_normalColor;
    }

}
