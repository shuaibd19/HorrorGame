using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//A Quest stores information about a quest - it's name, and its objectives

//CreateAssetMenu makes the create Asset menu contain an entry that creates a new Quest Asset
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest", order = 100)]
public class Quest : ScriptableObject
{
    //represents the status of objectives and quests
    public enum Status
    {
        NotYetComplete, //the objective has not yet bee completed
        Complete,       //the objective has been completed
        Failed          //the objective or quest has been failed
    }

    //the name of the quest
    public string questName;

    //the list of objectives that form this quest
    public List<Objective> objectives;

    //Objectives are the specific tasks that make up a quest
    [System.Serializable]
    public class Objective
    {
        //the visible name that's shown the player
        public string name = "New Objective";

        //if true, this objective doesn't need to be complete in order
        //for the quest to be considered complete (optional quest)
        public bool optional = false;

        //if false the objective will not be shown to the user it it's not yet complete
        //i.e. it will only be shown if complete or failed
        public bool visible = true;

        //the status of the objective when the quests begins
        public Status initialStatus = Status.NotYetComplete;
    }
}

#if UNITY_EDITOR
//draw a custom editor that lets you build the list of objectives
[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor
{
    //called when unity wants to draw the inspector for a quest
    public override void OnInspectorGUI()
    {
        //ensure that the object we're displaying has had any pending changes done
        serializedObject.Update();

        //draw the name of the quest
        EditorGUILayout.PropertyField(serializedObject.FindProperty("questName"),
            new GUIContent("Name"));

        //draw a header for the list of objectives
        EditorGUILayout.LabelField("Objectives");

        //get the property that contains the list objectives
        var objectiveList = serializedObject.FindProperty("objectives");

        //indent the objectives
        EditorGUI.indentLevel += 1;

        //for each objective in the list, draw an entry
        for (int i = 0; i < objectiveList.arraySize; i++)
        {
            //draw a single line of controls
            EditorGUILayout.BeginHorizontal();

            //draw the objective itself (its name and its flags)
            EditorGUILayout.PropertyField(
                objectiveList.GetArrayElementAtIndex(i),
                includeChildren: true);

            //draw a button that moves the item up in the list
            if (GUILayout.Button("Up", EditorStyles.miniButtonLeft, GUILayout.Width(25)))
            {
                objectiveList.MoveArrayElement(i, i - 1);
            }

            //draw a button that moves the item down in the list
            if (GUILayout.Button("Down", EditorStyles.miniButtonMid, GUILayout.Width(40)))
            {
                objectiveList.MoveArrayElement(i, i + 1);
            }

            //draw a button that removes the item from the list
            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(25)))
            {
                objectiveList.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        //remove the indentation
        EditorGUI.indentLevel -= 1;

        //draw a button that adds a new objective to the list
        if (GUILayout.Button("Add Objective"))
        {
            objectiveList.arraySize += 1;
        }

        //save the changes
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
