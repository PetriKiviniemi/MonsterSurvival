using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

namespace utilities{

    public class WaitForUIButtons
    {
        private List<Button> btns = new List<Button>();
        public Button pressedButton = null;
        public WaitForUIButtons(params Button[] buttons)
        {
            Cursor.lockState = CursorLockMode.None;
            var i = 0;
            foreach (Button b in buttons)
            {
                int valCopy = i;
                btns.Add(b);
                b.onClick.AddListener(delegate { OnButtonPress(valCopy); });
                i++;
            }
        }

        public void OnButtonPress(int btnIdx)
        {
            pressedButton = btns[btnIdx];
        }

        public IEnumerator GetButtonInput()
        {
            //Wait for button input
            yield return new WaitUntil(() => pressedButton != null);
        }
    }

    static class UtilityMethods
    {
        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy as T;
        }
    }
}
