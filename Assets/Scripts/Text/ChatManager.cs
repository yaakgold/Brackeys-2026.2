using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace Text
{
    public class ChatManager : NetworkBehaviour
    {
        public static ChatManager Instance;

        private void Awake()
        {
            Instance = this;
        }
        
        [SerializeField] private PanelRenderer panelRenderer;
    }
}
