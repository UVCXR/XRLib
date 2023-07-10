using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WIFramework
{
    public partial class GenericDropdown : MonoBehaviour
    {
        public IGenerable value {
            get
            {
                if (dropDown.options.Count==0)
                {
                    return null;
                }
                return optionTable[dropDown.options[dropDown.value]];
            }
        }
        public Action<IGenerable> onValueChanged;
        HashSet<IGenerable> _options = new HashSet<IGenerable>();
        public HashSet<IGenerable> options
        {
            get
            {
                return _options;
            }
        }
        Dictionary<TMP_Dropdown.OptionData, IGenerable> optionTable = new();
        TMP_Dropdown dd;
        TMP_Dropdown dropDown
        {
            get
            {
                if (dd == null)
                {
                    dd = GetComponent<TMP_Dropdown>();
                    dd.onValueChanged.AddListener(ValueChanged);
                }
                dd.RefreshShownValue();
                return dd;
            }
        }

        void ValueChanged(int value)
        {
            var option = dropDown.options[value];
            var targetValue = optionTable[option];
            //Debug.Log($"Value Changed : {dropDown.captionText}");
            onValueChanged?.Invoke(targetValue);
        }

        public void Clear()
        {
            _options.Clear();
            optionTable.Clear();
            dropDown.ClearOptions();
        }

        public void SetOption(IGenerable[] values)
        {
            dropDown.ClearOptions();
            foreach (var value in values)
            {
                AddOption(value);
            }
        }

        public void AddOption(IGenerable value)
        {
            _options.RemoveWhere(o => o == null);
            var optionData = new TMP_Dropdown.OptionData(value.id);
            dropDown.options.Add(optionData);
            _options.Add(value);
            optionTable.Add(optionData, value);
        }

        public void RemoveOption(IGenerable option)
        {
            _options.Remove(option);
            _options.RemoveWhere(o => o == null);
            dropDown.options.RemoveAll(o => o.text == option.id);
            optionTable.NullCleaning();
            dropDown.value = 0;
        }
    }
}