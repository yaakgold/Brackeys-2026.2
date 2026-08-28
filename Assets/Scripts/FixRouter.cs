using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using WebSocketSharp;
using Random = UnityEngine.Random;

public class FixRouter : Minigame
{
    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private List<string> wordOptions;

    private VisualElement _root;
    private TextField _textInput;
    private Label _label;
    private Label _lblInstructions;

    private void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SendTextInput();
        }
    }

    private void SendTextInput()
    {
        var text = _textInput.text;

        if (text.IsNullOrEmpty()) return;
        if (text == _label.text)
        {
            ProjectManager.Instance.UpdateWordsRpc();
            GetNextWord();
        }

        _textInput.value = string.Empty;
    }

    private void GetNextWord()
    {
        OnNumWordsChanged(ProjectManager.Instance.GetNumWords());
        
        var randomWord = wordOptions[Random.Range(0, wordOptions.Count)];
        _label.text = randomWord;
    }

    void OnEnable()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
        ProjectManager.Instance.onRouterFixed.AddListener(() => onCompleteMinigame.Invoke(0));
        ProjectManager.Instance.onNumWordsChanged.AddListener(OnNumWordsChanged); }

    private void OnNumWordsChanged(int numWords)
    {
        _lblInstructions.text = $"Enter {numWords} more word(s) to reset the router";
    }

    void OnDisable()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }
    
    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _root = rootElement;
        
        _textInput = _root.Q<TextField>("tfInput");
        _label = _root.Q<Label>("lblWordToType");
        _lblInstructions = _root.Q<Label>("lblInstructions");
        GetNextWord();
    }
}
