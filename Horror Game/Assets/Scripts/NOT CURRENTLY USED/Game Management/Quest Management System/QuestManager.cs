using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//represents the player's current progress through a quest
public class QuestStatus
{
    //the underlying data object that describes the quest
    public Quest questData;

    //the map of objective identifiers
    public Dictionary<int, Quest.Status> objectiveStatuses;

    //the constructor - pass a quest to set it up
    public QuestStatus(Quest questData)
    {
        this.questData = questData;

        //create a map of objective numbers to their quests
        objectiveStatuses = new Dictionary<int, Quest.Status>();

        for (int i = 0; i < questData.objectives.Count; i++)
        {
            var objectiveData = questData.objectives[i];

            objectiveStatuses[i] = objectiveData.initialStatus;
        }
    }

    //returns the state of the entire quest
    //if all non-optional objectives are complete - quest complete
    //if any non-optional objectives is failed - quest failed
    //otherwise the quest is non yet complete
    public Quest.Status questStatus
    {
        get
        {
            for (int i = 0; i < questData.objectives.Count; i++)
            {
                var objectiveData = questData.objectives[i];

                //optional objectives do not matter to the overall quest status
                if (objectiveData.optional)
                {
                    continue;
                }

                var objectiveStatus = objectiveStatuses[i];

                //this is the mandatory objective
                if (objectiveStatus == Quest.Status.Failed)
                {
                    //if a mandatory objective is failed then the whole mission is failed
                    return Quest.Status.Failed;
                }
                else if (objectiveStatus != Quest.Status.Complete)
                {
                    //if a mandatory objective is not yet complete then the whole quest is incomplete
                    return Quest.Status.NotYetComplete;
                }
            }

            //all mandatory objectives are complete, so this quest is complete
            return Quest.Status.Complete;
        }
    }

    //returns a string containing the list of objectives. their statuses
    //and the status of the quest
    public override string ToString()
    {
        var stringBuilder = new System.Text.StringBuilder();

        for (int i = 0; i < questData.objectives.Count; i++)
        {
            //get the objective and its status
            var objectiveData = questData.objectives[i];
            var objectiveStatus = objectiveStatuses[i];

            //don't show hidden objectives that haven't been finsihed
            if (objectiveData.visible == false && objectiveStatus == Quest.Status.NotYetComplete)
            {
                continue;
            }

            //if this objective is optional then display optional after its name
            if (objectiveData.optional)
            {
                stringBuilder.AppendFormat("{0} (Optional) - {1}\n", objectiveData.name, objectiveStatus.ToString());
            }
            else
            {
                stringBuilder.AppendFormat("{0} - {1}\n", objectiveData.name, objectiveStatus.ToString());
            }
        }

        //add a blank line followed by the quest status
        stringBuilder.AppendLine();
        stringBuilder.AppendFormat("Status: {0}", this.questStatus.ToString());

        return stringBuilder.ToString();
    }
}

//manages a quest
public class QuestManager : MonoBehaviour
{
    //the quest that starts when the game starts 
    [SerializeField] private Quest startingQuest = null;

    //a label to show the state of the quest in
    [SerializeField] private UnityEngine.UI.Text objectiveSummary = null;

    //tracks the state of the current quest
    private QuestStatus activeQuest;

    //start a new quest when the game starts
    private void Start()
    {
        if (startingQuest != null)
        {
            StartQuest(startingQuest);
        }
    }

    private void StartQuest(Quest quest)
    {
        activeQuest = new QuestStatus(quest);

        UpdateObjectiveSummaryText();

        Debug.LogFormat("Starting quest {0}", activeQuest.questData.name);
    }

    //updates the label that displays the status of the quest and its objectives
    private void UpdateObjectiveSummaryText()
    {
        string label;

        if (activeQuest == null)
        {
            label = "No active quest.";
        }
        else
        {
            label = activeQuest.ToString();
        }

        objectiveSummary.text = label;
    }

    //called by other objects to indicate that an objective has changed status
    public void UpdateObjectiveStatus(Quest quest, int objectiveNumber, Quest.Status status)
    {
        if (activeQuest == null)
        {
            Debug.LogError("Tried to set an objective status, but not quest is active");
            return;
        }

        if (activeQuest.questData != quest)
        {
            Debug.LogWarningFormat("Tried to set an objective status for quest {0},"
                + " but this is not the active quest. Ignoring.", quest.questName);
            return;
        }

        //update the objective status
        activeQuest.objectiveStatuses[objectiveNumber] = status;

        //update the display label
        UpdateObjectiveSummaryText();
    }
}
