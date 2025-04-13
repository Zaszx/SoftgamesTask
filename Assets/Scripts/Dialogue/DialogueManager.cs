using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class DialogueManager
{
	private static DialogueManager _instance;
	public static DialogueManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new DialogueManager();
			}
			return _instance;
		}
	}

	public DialogueData dialogueData;

	public async Task Init()
	{
		await LoadDialogueData();
		// Optional: print test
		foreach (var entry in dialogueData.dialogue)
			Debug.Log($"[{entry.name}]: {entry.text}");
	}

	async Task LoadDialogueData()
	{
		TextAsset jsonFile = Resources.Load<TextAsset>("Dialogue/DialogueData");
		if (jsonFile == null)
		{
			Debug.LogError("Could not find DialogueData.json in Resources/Dialogue/");
			return;
		}

		dialogueData = JsonUtility.FromJson<Wrapper>($"{{\"data\":{jsonFile.text}}}").data;

		foreach(Emoji emoji in dialogueData.emojies)
		{
			Texture2D tex = await DownloadTextureAsync(emoji.url);
			if(tex != null)
			{
				emoji.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
			}
			else
			{
				Debug.LogError("Emoji couldn't be downloaded: " + emoji.name);
			}
		}

		foreach (Avatar avatar in dialogueData.avatars)
		{
			Texture2D tex = await DownloadTextureAsync(avatar.url);
			if (tex != null)
			{
				avatar.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
			}
			else
			{
				Debug.LogError("Avatar couldn't be downloaded: " + avatar.name);
			}
		}

		foreach (DialogueEntry dialogueEntry in dialogueData.dialogue)
		{
			dialogueEntry.avatar = dialogueData.GetAvatar(dialogueEntry.name);
		}
	}

	private async Task<Texture2D> DownloadTextureAsync(string url)
	{
		using var uwr = UnityWebRequestTexture.GetTexture(url);
		var op = uwr.SendWebRequest();

		while (!op.isDone)
			await Task.Yield();

		if (uwr.result != UnityWebRequest.Result.Success)
			return null;

		return DownloadHandlerTexture.GetContent(uwr);
	}

	[System.Serializable]
	private class Wrapper
	{
		public DialogueData data;
	}
}
