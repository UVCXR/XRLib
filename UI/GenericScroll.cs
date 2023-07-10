using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WIFramework
{
    public abstract partial class GenericScroll : PanelBase
    {
        public Action<IGenerable> onClickContent;
        Dictionary<IGenerable, GameObject> list = new();
        ScrollRect _scroll;
        protected ScrollRect scroll
        {
            get
            {
                if (_scroll == null)
                    _scroll = GetComponentInChildren<ScrollRect>();
                return _scroll;
            }
        }

        public void Add<T>(T obj) where T : IGenerable
        {
            var bt = WIManager.FindSingle<ButtonFactory>().GenerateGVButton(scroll.content, obj, onClickContent);
            list.Add(obj, bt.gameObject);
        }

        public virtual void Add<T>(T[] objs, bool clear = false) where T : IGenerable
        {
            if (clear)
                Clear();

            foreach (var item in objs)
            {
                Add(item);
            }
        }
        public void Clear()
        {
            for (int i = 0; i < scroll.content.childCount; ++i)
            {
                Destroy(scroll.content.GetChild(i).gameObject);
            }
            list.Clear();
        }

        public void Remove<T>(T obj) where T : IGenerable
        {
            if (!list.TryGetValue(obj, out var go))
                return;
            list.Remove(obj);
            Destroy(go);
        }

        public bool Contains<T>(T obj) where T : IGenerable
        {
            return list.ContainsKey(obj);
        }
    }
}