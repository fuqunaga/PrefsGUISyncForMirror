﻿using UnityEngine;

#if PrefsGUI_RapidGUI
using RapidGUI;
using PrefsGUI.RapidGUI;
#endif

namespace PrefsGUI.Example
{
#if PrefsGUI_RapidGUI
    public class PrefsGUIRapidGUIExample : PrefsGUIRapidGUIExampleBase
    {
        public Vector2 position;
        private WindowLaunchers windowLaunchers;
        
        private void Start()
        {
            windowLaunchers = new WindowLaunchers
            {
                isWindow = false
            };
            windowLaunchers.Add("Part1", typeof(PrefsGUIExample_Part1));
            windowLaunchers.Add("Part2", typeof(PrefsGUIExample_Part2));
            windowLaunchers.Add("Part3", typeof(PrefsGUIExample_Part3));
            windowLaunchers.Add("PrefsSearch", PrefsSearch.DoGUI).SetWidth(600f).SetHeight(800f);

            windowRect.position = position;
        }

        protected override void DoGUI()
        {
            windowLaunchers.DoGUI();
            base.DoGUI();
        }
    }
#else
    public class PrefsGUIRapidGUIExample : MonoBehaviour
    {}
#endif
}