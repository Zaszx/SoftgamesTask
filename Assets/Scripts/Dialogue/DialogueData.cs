using UnityEngine;
using System.Linq;

[System.Serializable]
public class DialogueEntry
{
	public string name;
	public string text;

	[System.NonSerialized] public Avatar avatar;
}

[System.Serializable]
public class Emoji
{
	public string name;
	public string url;

	[System.NonSerialized] public Sprite sprite;
}

[System.Serializable]
public class Avatar
{
	public string name;
	public string url;
	public string position;

	[System.NonSerialized] public Sprite sprite;
}

[System.Serializable]
public class DialogueData
{
	public DialogueEntry[] dialogue;
	public Emoji[] emojies;
	public Avatar[] avatars;

	public Avatar GetAvatar(string name)
	{
		return avatars.FirstOrDefault(x => x.name == name);
	}

	public Emoji GetEmoji(string name)
	{
		return emojies.FirstOrDefault(x => x.name == name);
	}
}