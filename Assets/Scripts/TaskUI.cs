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
        if (_task.minigamePrefab == null) return;
        
        MinigameController.Instance.StartMinigame(_task.minigamePrefab, _task);
    }
}