using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;

    public TMP_InputField playerName;
    public TextMeshProUGUI connectionState;
    public TMP_InputField messageInput;
    public TextMeshProUGUI messageArea;

    public GameObject introPanel;
    public GameObject messagePanel;

    public static string dispName;

    private string worldchat;
    [SerializeField] private string userID;

    // Start is called before the first frame update
    void Start()
    {
        dispName = PlayFabControls.dname;
        playerName.text = dispName;

        Application.runInBackground = true;
        if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
        {
            Debug.LogError("No AppID Provided");
            return;
        }

        worldchat = "world";
    }

    // Update is called once per frame
    void Update()
    {
        if(chatClient != null)
        {
            chatClient.Service();
        }
    }

    //method when connection succeeds
    public void GetConnected()
    {
        Debug.Log("Connecting");
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, 
            new Photon.Chat.AuthenticationValues(playerName.text));
    }

    //method that sends the messages
    public void SendMessage()
    {
        chatClient.PublishMessage(worldchat, messageInput.text);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
       
    }

    public void OnDisconnected()
    {

    }

    public void OnConnected()
    {
        introPanel.SetActive(false);
        messagePanel.SetActive(true);
        chatClient.Subscribe(new string[] { worldchat });
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
       //properties of message
       for (int i = 0; i < senders.Length; i++)
        {
            messageArea.text += senders[i] + ": " + messages[i] + "\n";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {

    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach (var channel in channels)
        {
            this.chatClient.PublishMessage(channel, "συνδέθηκε");
        }

        connectionState.text = "Συνδεθήκατε";
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }
}
