using System;

namespace WIFramework
{
    public partial class ButtonFactory : MonoBehaviour, ISingle
    {
        public GenericButton prefab_GenericValueButton;
        public GenericButton GenerateGVButton<T>(UnityEngine.RectTransform content, T obj, Action<IGenerable> onClickContent) where T : IGenerable
        {
            var button = Instantiate(prefab_GenericValueButton, content);
            button.SetValue(obj, onClickContent);
            return button;
        }
        public GenericButton GenerateGVButton(UnityEngine.RectTransform content, string title, Action onClickContent)
        {
            var button = Instantiate(prefab_GenericValueButton, content);
            button.SetValue(title, onClickContent);
            return button;
        }

    }
}