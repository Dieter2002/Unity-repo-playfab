using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Party;
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DemoScript : MonoBehaviour {
	public InputField networkIdTextBox;
	public Text output;

	// Start is called before the first frame update
	void Start() {
		PlayFabSettings.TitleId = "5C42A";
		var playFabMultiplayerManager = PlayFabMultiplayerManager.Get();
		// This will turn on verbose logging that is useful when debugging the SDK.
		//playFabMultiplayerManager.LogLevel = PlayFabMultiplayerManager.LogLevelType.Verbose;
		
		// Log into playfab. The SDK will use the logged in user when connecting to the network.
		var request = new LoginWithCustomIDRequest
			{ CustomId = UnityEngine.Random.value.ToString(), CreateAccount = true };
		PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

		// Listen for this event to know when the SDK has joined the network and can start sending messages.
		playFabMultiplayerManager.OnNetworkJoined += OnNetworkJoined;
		
		// The simple way to receive data messages.
		// playFabMultiplayerManager.OnDataMessageReceived += OnDataMessageReceived;

		// A more complex, but no copy way of recieving data messages.
		// playFabMultiplayerManager.OnDataMessageNoCopyReceived += OnDataMessageNoCopyReceived;

		playFabMultiplayerManager.OnChatMessageReceived += OnChatMessageReceived;
		playFabMultiplayerManager.OnRemotePlayerLeft += OnRemotePlayerLeft;

		playFabMultiplayerManager.OnDataMessageReceived += OnDataMessageReceived;

		playFabMultiplayerManager.OnRemotePlayerJoined += OnRemotePlayerJoined;
		
	}

	private void OnRemotePlayerJoined(object sender, PlayFabPlayer player) {
		// This player will not be able to send text or voice communication.
		// Data messages can still be sent.
		player.IsMuted = false;
		player.VoiceLevel = 1;
		Debug.Log("new player");		
		Debug.Log(player.ChatState);

	}
	private void OnRemotePlayerLeft(object sender, PlayFabPlayer player) {
		// This player will not be able to send text or voice communication.
		// Data messages can still be sent.
		player.IsMuted = false;
		Debug.Log("Player disconnected");
	}


	private void OnDataMessageReceived(object sender, PlayFabPlayer from, byte[] buffer) {
		Debug.Log("Got a message (simple).");
		Debug.Log(Encoding.Default.GetString(buffer));
		output.text += "\r\n Got a message (simple).";
	}

	private void OnChatMessageReceived(object sender, PlayFabPlayer from, string message, ChatMessageType type) {
		Debug.Log(message);
		output.text += "\r\n Got a message (simple).";
	}

	public void CreateAndJoinToNetwork() {
		// Create and join a network
		PlayFabMultiplayerManager.Get().CreateAndJoinNetwork();
	}

	public void JoinNetwork() {
		PlayFabMultiplayerManager.Get().JoinNetwork(networkIdTextBox.text);
		PlayFabMultiplayerManager.Get().LocalPlayer.IsMuted = false;
		PlayFabMultiplayerManager.Get().LocalPlayer.VoiceLevel = 1;
		Debug.Log(PlayFabMultiplayerManager.Get().LocalPlayer.IsMuted);
		Debug.Log(PlayFabMultiplayerManager.Get().LocalPlayer.VoiceLevel);
		
		// Debug.Log("Send a message");

		// sendMessage();
	}

	private void sendMessage() {
		// Simple send message.
		byte[] buffer = Encoding.ASCII.GetBytes("Hello world (simple message).");
		PlayFabMultiplayerManager.Get().SendDataMessageToAllPlayers(buffer);

		// Send a data message. There is a simpler version of this API available as well.
		byte[] buffer2 = Encoding.ASCII.GetBytes("Hello world (no garbage collection method).");
		IntPtr unmanagedPointer = Marshal.AllocHGlobal(buffer2.Length);
		Marshal.Copy(buffer2, 0, unmanagedPointer, buffer2.Length);
		PlayFabMultiplayerManager.Get().SendDataMessage(unmanagedPointer, (uint)buffer2.Length,
			PlayFabMultiplayerManager.Get().RemotePlayers, DeliveryOption.BestEffort);
		Marshal.FreeHGlobal(unmanagedPointer);

		Debug.Log("Send a message");
	}

	// private void OnDataMessageReceived(object sender, PlayFabPlayer from, byte[] buffer)
	// {
	//     Debug.Log("Got a message (simple).");
	//     output.text += "\r\n Got a message (simple).";
	// }

	private void OnDataMessageNoCopyReceived(object sender, PlayFabPlayer from, System.IntPtr buffer, uint bufferSize) {
		Debug.Log("Got a message (no copy).");
		output.text += "\r\n Got a message (no copy).";
	}

	private void OnNetworkJoined(object sender, string networkId) {
		Debug.Log("Joined the network.");
		output.text += "\r\n Joined the network.";
		networkIdTextBox.text = networkId;
		
	}

	private void OnLoginSuccess(LoginResult result) {
		Debug.Log("Logged into PlayFab.");
		output.text += "\r\n Logged into PlayFab.";
	}

	private void OnLoginFailure(PlayFabError error) {
		Debug.Log("Error logging into PlayFab: " + error.ErrorMessage);
		output.text += "\r\n Error logging into PlayFab: " + error.ErrorMessage;
	}

	// Update is called once per frame
	void Update() {
	}
}