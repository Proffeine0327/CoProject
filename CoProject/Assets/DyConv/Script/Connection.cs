using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Dialogue
{
    public class Connection : ScriptableObject
    {
        public Situation target;
        public Sentence begin;
        public NextSentenceInfo end;
        public Action<Connection> selectAction;
        public Action<Connection> removeAction;
        public bool isSelected;

        public void Draw()
        {
            if (isSelected) Handles.color = Color.cyan;

            Handles.DrawLine(begin.rect.center, end.next.rect.center);

            var middle = (begin.rect.center + end.next.rect.center) / 2;
            var dir = (end.next.rect.center - begin.rect.center);
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Handles.DrawLine
            (
                middle + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (angle + 120)), Mathf.Sin(Mathf.Deg2Rad * (angle + 120))) * 7f,
                middle + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (angle + 240)), Mathf.Sin(Mathf.Deg2Rad * (angle + 240))) * 7f
            );

            Handles.DrawLine
            (
                middle + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (angle + 240)), Mathf.Sin(Mathf.Deg2Rad * (angle + 240))) * 7f,
                middle + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (angle + 360)), Mathf.Sin(Mathf.Deg2Rad * (angle + 360))) * 7f
            );

            Handles.DrawLine
            (
                middle + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (angle + 360)), Mathf.Sin(Mathf.Deg2Rad * (angle + 360))) * 7f,
                middle + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (angle + 120)), Mathf.Sin(Mathf.Deg2Rad * (angle + 120))) * 7f
            );
            Handles.color = Color.white;
        }

        public void EventHandler(Event e)
        {
            var middle = (begin.rect.center + end.next.rect.center) / 2;

            switch (e.type)
            {
                case EventType.Used:
                    isSelected = false;
                    GUI.changed = true;
                    break;

                case EventType.MouseDown:

                    if (Vector2.Distance(middle, e.mousePosition) <= 8)
                    {
                        isSelected = true;
                        selectAction?.Invoke(this);
                        GUI.changed = true;

                        if (e.button == 0)
                        {

                            GUI.FocusControl("");
                            e.Use();
                        }

                        if (e.button == 1)
                        {
                            ShowContextMenu();
                            e.Use();
                        }
                    }
                    else
                    {
                        isSelected = false;
                        GUI.changed = true;
                    }
                    break;
            }
        }

        public void ShowContextMenu()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Remove connection"), false, () => removeAction?.Invoke(this));
            menu.ShowAsContext();
        }
    }
}
#endif