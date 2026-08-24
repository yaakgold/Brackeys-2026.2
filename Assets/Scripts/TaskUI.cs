using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TaskUI : MonoBehaviour
{
    [SerializeField] private Button interactButton;
    [SerializeField] private Image taskBackground;
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private Color easyColor, mediumColor, hardColor;
    
    private GameTask _task;

    public void SetTask(GameTask t)
    {
        _task = t;
        
        interactButton.onClick.AddListener(DoAction);

        taskBackground.color = _task.difficulty switch
        {
            ETaskDifficulty.Easy => easyColor,
            ETaskDifficulty.Medium => mediumColor,
            ETaskDifficulty.Hard => hardColor,
            _ => throw new ArgumentOutOfRangeException()
        };

        interactText.text = $"{_task.title}\n{_task.description}";
    }

    private void DoAction()
    {
        switch (_task.taskType)
        {
            case ETaskType.Sanity:
                if (NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject()
                    .TryGetComponent(out SanityController sc))
                {
                    sc.UpdateSanity(_task.effectAmount);
                }
                break;
            case ETaskType.Quality:
                ProjectManager.Instance.UpdateQualityRpc(_task.effectAmount);
                break;
            case ETaskType.Sabotage:
                break;
            case ETaskType.Other:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        ProjectManager.Instance.UpdateTimeRpc(_task.timeToComplete);
        
        //TODO: Setup the minigame system and remove the temp code above
        // if (_task.minigamePrefab == null) return;
        //
        // Instantiate(_task.minigamePrefab);
    }
}