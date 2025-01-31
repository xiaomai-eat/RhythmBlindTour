using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISliderAttribute : UIAttributeBase, IPointerClickHandler
    {
        [SerializeField]
        Slider Slider;
        private void Start()
        {
            SetParameterType(ParameterType.Slider);
            Slider.onValueChanged.AddListener(v =>
            {
                RunAction(v);
            });
        }
        public void SetValueShow(float Value)
        {
            if (Slider != null)
                Slider.value = Value;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            SelectManager.SetAttribute(this);
        }
    }