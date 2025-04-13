using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.TextCore;
using System.Reflection;
using System.Linq;

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

		int emojiIndex = 0;

		int totalWidth = 0;
		foreach (Emoji emoji in dialogueData.emojies)
		{
			Texture2D tex = await DownloadTextureAsync(emoji.url);
			if (tex != null)
			{
				/*if (tex.height != 64)
				{
					Texture2D scaled = new Texture2D(64 * tex.width / tex.height, 64, TextureFormat.RGBA32, false);
					Graphics.ConvertTexture(tex, scaled);
					tex = scaled;
				}*/
				emoji.texture = tex;
				emoji.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
				totalWidth += tex.width;
			}
			else
			{
				Debug.LogError("Emoji couldn't be downloaded: " + emoji.name);
			}
		}

		var atlas = new Texture2D(totalWidth, 128, TextureFormat.RGBA32, false);
		atlas.SetPixels(new Color[totalWidth * 128]); // clear

		var spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
		spriteAsset.name = "SimpleEmojiAsset";
		spriteAsset.spriteSheet = atlas;
		spriteAsset.material = new Material(Shader.Find("TextMeshPro/Sprite")) { mainTexture = atlas };

		var versionField = typeof(TMP_SpriteAsset).GetField("m_Version", BindingFlags.Instance | BindingFlags.NonPublic);
		if (versionField != null)
		{
			versionField.SetValue(spriteAsset, "1.1.0"); // TMP expects version >= 1
		}

		int currentX = 0;

		foreach (Emoji emoji in dialogueData.emojies)
		{
			if(emoji.texture != null)
			{
				atlas.SetPixels(currentX, 0, emoji.texture.width, emoji.texture.height, emoji.texture.GetPixels());
				emoji.rect = new Rect(currentX, 0, emoji.texture.width, emoji.texture.height);
				currentX += emoji.texture.width;

				var sprite = Sprite.Create(atlas, emoji.rect, new Vector2(0.5f, 0.5f), 100f);
				emoji.sprite = sprite;

				var glyph = new TMP_SpriteGlyph
				{
					index = (uint)spriteAsset.spriteGlyphTable.Count,
					sprite = sprite,
					metrics = new GlyphMetrics(emoji.rect.width, emoji.rect.height, 0, emoji.rect.height * 0.9f, emoji.rect.width),
					glyphRect = new GlyphRect((int)emoji.rect.x, (int)emoji.rect.y, (int)emoji.rect.width, (int)emoji.rect.height)
				};

				var character = new TMP_SpriteCharacter(glyph.index, glyph)
				{
					name = emoji.name,
					glyph = glyph,
					glyphIndex = glyph.index
				};

				spriteAsset.spriteGlyphTable.Add(glyph);
				spriteAsset.spriteCharacterTable.Add(character);
			}
		}

		atlas.Apply();

		spriteAsset.UpdateLookupTables();

		dialogueData.emojiSpriteAsset = spriteAsset;

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
