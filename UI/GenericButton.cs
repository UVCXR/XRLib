using System;
using TMPro;
using UnityEngine.UI;

namespace WIFramework
{
    public partial class GenericButton : MonoBehaviour
    {
        public IGenerable value;
        public Action<IGenerable> OnClickEvent;
        TextMeshProUGUI _title;
        TextMeshProUGUI Text_Title
        {
            get
            {
                if (_title == null)
                    _title = GetComponentInChildren<TextMeshProUGUI>();
                return _title;
            }
        }

        public override void Initialize()
        {
            GetComponent<Button>().onClick.AddListener(delegate { OnClickEvent?.Invoke(value); });
        }

        public void SetValue<T>(T obj, Action<IGenerable> action) where T : IGenerable
        {
            value = obj;
            OnClickEvent += action;
            SetTitle(value.id);
        }
        public void SetValue(string title, Action action)
        {
            OnClickEvent += delegate { action?.Invoke(); };
            SetTitle(title);
        }

        public void SetTitle(string text)
        {
            Text_Title.SetText(text);
        }
    }
}