using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Dialogue;

public class SituationEditorWindow : EditorWindow
{
    public static void Open(Situation target)
    {
        var window = EditorWindow.GetWindow<SituationEditorWindow>("Situation Editor Window");

        window.target = target;
        window.Show();
    }

    private Situation target;
    private Vector2 offset;
    private Sentence select_s;
    private Connection select_c;
    private Sentence begin;

    private void OnInspectorUpdate() 
    {
        Repaint();
    }

    private void OnGUI()
    {
        if (target == null)
        {
            Close();
            return;
        }

        if(EditorUtility.IsDirty(target)) this.titleContent = new GUIContent("Situation Editor Window*");
        else this.titleContent = new GUIContent("Situation Editor Window");

        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        MakeTransition(Event.current);
        DrawConnections();
        DrawSentences();

        SentenceEventHandler(Event.current);
        ConnectionEventHandler(Event.current);
        EventHandler(Event.current);

        if
        (
            GUI.Button(new Rect(0, 0, 100, 20), "Save") ||
            (Event.current.keyCode == KeyCode.LeftControl && Event.current.keyCode == KeyCode.S)
        )
            AssetDatabase.SaveAssetIfDirty(target);

        if (GUI.changed) Repaint();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void Drag(Event e)
    {
        offset += e.delta;
        foreach (var sentence in target.sentences) sentence.Drag(e);
        GUI.changed = true;
    }

    private void DrawSentences()
    {
        foreach (var sentence in target.sentences) sentence.Draw();
    }

    private void DrawConnections()
    {
        foreach (var connection in target.connections) connection.Draw();
    }

    private void SentenceEventHandler(Event e)
    {
        for (int i = target.sentences.Count - 1; i >= 0; i--)
        {
            var index = i;
            var sentence = target.sentences[i];

            sentence.selectAction = (target) =>
            {
                select_c = null;
                select_s = target;

                Selection.activeObject = target;
            };
            sentence.removeAction = (target) =>
            {
                for (int k = this.target.connections.Count - 1; k >= 0; k--)
                {
                    if (this.target.connections[k].begin == target || this.target.connections[k].end.next == target)
                    {
                        Object.DestroyImmediate(this.target.connections[k].end, true);
                        this.target.connections[k].begin.Nexts.Remove(this.target.connections[k].end);
                        Object.DestroyImmediate(this.target.connections[k], true);
                        this.target.connections.RemoveAt(k);
                    }
                }

                this.target.sentences.RemoveAt(index);
                Object.DestroyImmediate(target, true);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this.target));

                UpdateConnectionOrder();
                EditorUtility.SetDirty(this.target);
            };
            sentence.transitionAction = (target) =>
            {
                select_s = null;
                begin = target;
            };

            sentence.EventHandler(e);
        }
    }

    private void ConnectionEventHandler(Event e)
    {
        for (int i = target.connections.Count - 1; i >= 0; i--)
        {
            var index = i;
            var connection = target.connections[i];

            connection.selectAction = (target) =>
            {
                select_s = null;
                select_c = target;

                Selection.activeObject = target.end;
            };
            connection.removeAction = (target) =>
            {
                target.begin.Nexts.Remove(target.end);
                Object.DestroyImmediate(target.end, true);
                this.target.connections.RemoveAt(index);
                Object.DestroyImmediate(target, true);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this.target));

                UpdateConnectionOrder();
                EditorUtility.SetDirty(this.target);
            };

            connection.EventHandler(e);
        }
    }

    private void EventHandler(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    select_s = null;
                    begin = null;
                    Selection.activeObject = null;
                }

                if (e.button == 1)
                {
                    ShowContextMenu(e);
                }
                break;

            case EventType.MouseDrag:
                Drag(e);
                break;
        }
    }

    private void ShowContextMenu(Event e)
    {
        var menu = new GenericMenu();

        menu.AddItem(new GUIContent("Sentence/Normal"), false, () =>
        {
            var s = ScriptableObject.CreateInstance<Sentence>();
            s.hideFlags = HideFlags.HideInHierarchy;
            s.rect.position = e.mousePosition;
            s.Type = SentenceType.normal;
            s.target = target;
            target.sentences.Add(s);

            AssetDatabase.AddObjectToAsset(s, target);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));

            EditorUtility.SetDirty(target);
        });

        menu.AddItem(new GUIContent("Sentence/Question"), false, () =>
        {
            var s = ScriptableObject.CreateInstance<Sentence>();
            s.hideFlags = HideFlags.HideInHierarchy;
            s.rect.position = e.mousePosition;
            s.Type = SentenceType.question;
            s.target = target;
            target.sentences.Add(s);

            AssetDatabase.AddObjectToAsset(s, target);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));

            EditorUtility.SetDirty(target);
        });

        menu.AddItem(new GUIContent("Sentence/Display Playable"), false, () =>
        {
            var s = ScriptableObject.CreateInstance<Sentence>();
            s.hideFlags = HideFlags.HideInHierarchy;
            s.rect.position = e.mousePosition;
            s.Type = SentenceType.displayPlayable;
            s.target = target;
            target.sentences.Add(s);

            AssetDatabase.AddObjectToAsset(s, target);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));

            EditorUtility.SetDirty(target);
        });
        menu.ShowAsContext();
    }   

    private void UpdateConnectionOrder()
    {
        foreach(var connection in target.connections)
            connection.end.order = connection.begin.Nexts.IndexOf(connection.end);
    }

    private void MakeTransition(Event e)
    {
        if (begin != null)
        {
            Handles.DrawLine(begin.rect.center, e.mousePosition);

            var middle = (begin.rect.center + e.mousePosition) / 2;
            var dir = (e.mousePosition - begin.rect.center);
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
            GUI.changed = true;

            if (select_s != null)
            {
                if (select_s != begin && select_s.Type != SentenceType.start)
                {
                    var contains = false;
                    foreach (var connection in target.connections) if (connection.begin == begin && connection.end.next == select_s) contains = true;

                    if (!contains)
                    {
                        var nextInfo = ScriptableObject.CreateInstance<NextSentenceInfo>();
                        nextInfo.hideFlags = HideFlags.HideInHierarchy;
                        nextInfo.next = select_s;

                        AssetDatabase.AddObjectToAsset(nextInfo, target);
                        begin.Nexts.Add(nextInfo);

                        var newConnection = ScriptableObject.CreateInstance<Connection>();
                        newConnection.hideFlags = HideFlags.HideInHierarchy;
                        newConnection.target = target;
                        newConnection.begin = begin;
                        newConnection.end = nextInfo;
                        target.connections.Add(newConnection);

                        AssetDatabase.AddObjectToAsset(newConnection, target);
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));

                        UpdateConnectionOrder();
                        EditorUtility.SetDirty(target);
                    }
                }
                begin = null;
            }
        }
    }
}
#endif