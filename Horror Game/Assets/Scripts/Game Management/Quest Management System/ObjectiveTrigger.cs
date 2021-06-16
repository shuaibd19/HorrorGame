using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

//combines a quest, an objective in the quest, and an objective status to use
[System.Serializable]
public class ObjectiveTrigger 
{
    //the quest that we're reffering to
    public Quest quest;

    //the status we want to apply to the objective
    public Quest.Status statusToApply;

    //the location of this objective in the quest's objective list
    public int objectiveNumber;

    public void Invoke()
    {
        //find the quest manager
        var manager = Object.FindObjectOfType<QuestManager>();

        //and tell it to update our objective
        manager.UpdateObjectiveStatus(quest, objectiveNumber, statusToApply);
    }
}

#if UNITY_EDITOR
//custom property drawers override how a type of property appears in the inspector
[CustomPropertyDrawer(typeof(ObjectiveTrigger))]
public class ObjectiveTriggerDrawer: PropertyDrawer
{
    //called when unity needs to draw an objectivetirgger property in the inspector
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //wrap this in begin/endproperty to ensure that undo works on the entire objectivetrigger property
        EditorGUI.BeginProperty(position, label, property);

        //get a reference to the three properties in the objectivetrigger
        var questProperty = property.FindPropertyRelative("quest");
        var statusProperty = property.FindPropertyRelative("statusToApply");
        var objectiveNumberProperty = property.FindPropertyRelative("objectiveNumber");

        ///we want to display three controls:
        /// - an object field for dropping a quest object into
        /// - a popup field to selecting a quest.status from
        /// - it should show the name of the objective
        /// 
        /// if no quest has been specified, or if the quest has no
        /// objectives, the objective pop up should be empty and 
        /// disabled
        /// 

        //calculate the rectangles in which we're displaying
        var lineSpacing = 2;

        //calculate the rectangle for the first time
        var firstLinePosition = position;

        firstLinePosition.height = base.GetPropertyHeight(questProperty, label);

        //and for the second line (same as the first line but shifted down one line)
        var secondLinePosition = position;
        secondLinePosition.y = firstLinePosition.y + firstLinePosition.height + lineSpacing;
        secondLinePosition.height = base.GetPropertyHeight(statusProperty, label);

        //repeat for the third line (same as the second line but shifted down)
        var thirdLinePosition = position;
        thirdLinePosition.y = secondLinePosition.y + secondLinePosition.height + lineSpacing;
        thirdLinePosition.height = base.GetPropertyHeight(objectiveNumberProperty, label);

        //draw the quest and status properties using the automatic property fields
        EditorGUI.PropertyField(firstLinePosition, questProperty, new GUIContent("Quest"));
        EditorGUI.PropertyField(secondLinePosition, statusProperty, new GUIContent("Status"));

        //now we draw our custom property for the object 
        //draw a label on the left hand side and get a new rectangle to draw the pop up in
        thirdLinePosition = EditorGUI.PrefixLabel(thirdLinePosition, new GUIContent("Objective"));

        //draw the ui for choosing a property
        var quest = questProperty.objectReferenceValue as Quest;

        //only draw this if we have a quest, and it has objectives
        if (quest != null && quest.objectives.Count > 0)
        {
            //get the name of every objective as an array
            var objectiveNames = quest.objectives.Select(o => o.name).ToArray();

            //get the index of the currently selected objective
            var selectedObjective = objectiveNumberProperty.intValue;

            //if we somehow are referring to an object that's not present in the list, reset
            //it to the first objective
            if (selectedObjective >= quest.objectives.Count)
            {
                selectedObjective = 0;
            }

            //draw the pop up and get back the new selection
            var newSelectedObjective = EditorGUI.Popup(thirdLinePosition, selectedObjective, objectiveNames);

            //if it was different, store it in the property
            if (newSelectedObjective != selectedObjective)
            {
                objectiveNumberProperty.intValue = newSelectedObjective;
            }
        }
        else
        {
            //draw a disabled pop up as a visual placeholder
            using (new EditorGUI.DisabledGroupScope(true))
            {
                //show a pop up with a single entry: the string "-"
                //ignore the return value
                EditorGUI.Popup(thirdLinePosition, 0, new[] { "-" });
            }
        }

        EditorGUI.EndProperty();
    }

    //called by unity to figure out the height of this property
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //the number of lines in this property
        var lineCount = 3;

        //the number of pixels in between each line
        var lineSpacing = 2;

        //the height of each line
        var lineHeight = base.GetPropertyHeight(property, label);

        //the height of this property is the number of lines times the 
        //height of each line, plus the spacing in between each line

        return (lineHeight * lineCount) + (lineSpacing * (lineCount - 1));
    }
}
#endif
