using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dialogue
{
    public enum SentenceType { start, normal, question, displayPlayable}

    public class Sentence : ScriptableObject
    {
        [SerializeField] private string narrator;
        [SerializeField] private string text = " ";
        [SerializeField] private SentenceType type;
        [SerializeField] private List<NextSentenceInfo> nexts = new List<NextSentenceInfo>();
        [SerializeField] private Sprite sprite;
        [SerializeField] private TimelineAsset playable;
        [SerializeField] private float passTime;

        public string Narrator { get { return narrator; } }
        public string Text { get { return text; } }
        public SentenceType Type { get { return type; } set { type = value; } }
        public List<NextSentenceInfo> Nexts { get { return nexts; } }
        public Sprite Sprite { get { return sprite; } }

#if UNITY_EDITOR
        public Rect rect;
        public GUIStyle normalStyle;
        public GUIStyle selectedStyle;
        public bool isSelected;
        public Action<Sentence> selectAction;
        public Action<Sentence> removeAction;
        public Action<Sentence> transitionAction;
        public Situation target;

        public void Draw()
        {
            rect.width = 200;
            rect.height = 50;

            normalStyle = new GUIStyle();
            normalStyle.alignment = TextAnchor.MiddleCenter;
            normalStyle.normal.textColor = Color.white;
            normalStyle.fontStyle = FontStyle.Bold;
            normalStyle.wordWrap = true;

            selectedStyle = new GUIStyle();
            selectedStyle.alignment = TextAnchor.MiddleCenter;
            selectedStyle.normal.textColor = Color.white;
            selectedStyle.fontStyle = FontStyle.Bold;
            selectedStyle.wordWrap = true;

            switch (Type)
            {
                case SentenceType.start:
                    normalStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6.png") as Texture2D;
                    normalStyle.border = new RectOffset(12, 12, 12, 12);

                    selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6 on.png") as Texture2D;
                    selectedStyle.border = new RectOffset(12, 12, 12, 12);
                    break;

                default:
                    normalStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
                    normalStyle.border = new RectOffset(12, 12, 12, 12);

                    selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0 on.png") as Texture2D;
                    selectedStyle.border = new RectOffset(12, 12, 12, 12);
                    break;
            }

            var str = Type == SentenceType.start ? "start" : Text.Substring(0, Mathf.Min(Text.Length, 25)).Replace('\n', '\0');

            if(Type == SentenceType.displayPlayable)
                str = $"Memo : {str}";
            GUI.Box(rect, str, isSelected ? selectedStyle : normalStyle);
        }

        public void Drag(Event e)
        {
            rect.position += e.delta;
            EditorUtility.SetDirty(target);
        }

        public void EventHandler(Event e)
        {
            switch (e.type)
            {
                case EventType.Used:
                    isSelected = false;
                    GUI.changed = true;
                    break;

                case EventType.MouseDown:
                    if (rect.Contains(e.mousePosition))
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

                case EventType.MouseUp:
                    if(isSelected)
                    {
                        EditorUtility.SetDirty(target);
                    }
                    break;

                case EventType.MouseDrag:
                    if (isSelected)
                    {
                        Drag(e);
                        GUI.changed = true;
                        e.Use();
                    }
                    break;
            }
        }

        public void ShowContextMenu()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Make transition"), false, () => { transitionAction?.Invoke(this); isSelected = false; });
            if (Type != SentenceType.start) menu.AddItem(new GUIContent("Remove sentence"), false, () => removeAction?.Invoke(this));
            menu.ShowAsContext();
        }
#endif
    }
}
