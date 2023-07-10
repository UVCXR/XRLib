using System.Collections.Generic;
using UnityEngine;

namespace WIFramework
{
    public partial class Panel_TabList : PanelBase, ISingle
    {
        List<PanelBase> tabList = new();
        PanelBase currentTab;
        Dictionary<PanelBase, GenericButton> tabButtons = new();
        RectTransform parent_Button;

        public override void Initialize()
        {
            var panels = GetComponentsInChildren<PanelBase>();
            foreach(var p in panels)
            {
                if (p == this)
                    continue;
                p.gameObject.SetActive(false);
                AddTab(p);
            }
            if(tabList.Count>0)
            {
                currentTab = tabList[0];
                currentTab.gameObject.SetActive(true);
            }
            gameObject.SetActive(false);
        }

        public void AddTab(PanelBase tab)
        {
            tabList.Add(tab);
            void ActivePanel()
            {
                if (currentTab != null)
                    currentTab.gameObject.SetActive(false);
                currentTab = tab;
                currentTab.gameObject.SetActive(true);
            }
            var bt = WIManager.FindSingle<ButtonFactory>().GenerateGVButton(parent_Button, tab.name, ActivePanel);
            tabButtons.Add(tab, bt);
        }
        
        public void RemoveTab(PanelBase tab)
        {
            tabList.Remove(tab);
            tabButtons.Remove(tab);
        }
    }
}