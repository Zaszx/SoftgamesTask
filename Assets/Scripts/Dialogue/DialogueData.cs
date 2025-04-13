using UnityEngine;
using System.Linq;
using TMPro;

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

	[System.NonSerialized] public Texture2D texture;
	[System.NonSerialized] public Rect rect;
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

	[System.NonSerialized] public TMP_SpriteAsset emojiSpriteAsset;

	public Avatar GetAvatar(string name)
	{
		return avatars.FirstOrDefault(x => x.name == name);
	}
}