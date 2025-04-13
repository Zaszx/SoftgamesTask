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
	}
	private async Task<DialogueData> FetchDialogueData(string url)
	{
		using var request = UnityWebRequest.Get(url);
		var op = request.SendWebRequest();

		while (!op.isDone)
			await Task.Yield();

		if (request.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError($"Failed to fetch JSON: {request.error}");
			return null;
		}

		string json = request.downloadHandler.text;
		return JsonUtility.FromJson<Wrapper>($"{{\"data\":{json}}}").data;
	}

	private async Task<TMP_SpriteAsset> PrepareEmojiSpriteAsset()
	{
		int totalWidth = 0;
		foreach (Emoji emoji in dialogueData.emojies)
		{
			Texture2D tex = await DownloadTextureAsync(emoji.url);
			if (tex != null)
			{
				if (tex.height != 128)
				{
					tex = RescaleToHeight(tex, 128);
				}
				emoji.texture = tex;
				totalWidth += tex.width;
			}
			else
			{
				Debug.LogError("Emoji couldn't be downloaded: " + emoji.name);
			}
		}

		var atlas = new Texture2D(totalWidth, 128, TextureFormat.RGBA32, false);
		atlas.SetPixels(new Color[totalWidth * 128]);

		var spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
		spriteAsset.name = "SimpleEmojiAsset";
		spriteAsset.spriteSheet = atlas;
		spriteAsset.material = new Material(Shader.Find("TextMeshPro/Sprite")) { mainTexture = atlas };

		var versionField = typeof(TMP_SpriteAsset).GetField("m_Version", BindingFlags.Instance | BindingFlags.NonPublic);
		if (versionField != null)
		{
			versionField.SetValue(spriteAsset, "1.1.0");
		}

		int currentX = 0;

		foreach (Emoji emoji in dialogueData.emojies)
		{
			if (emoji.texture != null)
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

		return spriteAsset;
	}

	async Task LoadDialogueData()
	{
		dialogueData = await FetchDialogueData("https://private-624120-softgamesassignment.apiary-mock.com/v2/magicwords");

		dialogueData.emojiSpriteAsset = await PrepareEmojiSpriteAsset();
		
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

	private Texture2D RescaleToHeight(Texture2D original, int targetHeight)
	{
		if (original == null) return null;

		float aspectRatio = (float)original.width / original.height;
		int targetWidth = Mathf.RoundToInt(targetHeight * aspectRatio);

		RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
		RenderTexture.active = rt;
		Graphics.Blit(original, rt);

		Texture2D scaled = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
		scaled.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
		scaled.Apply();

		RenderTexture.active = null;
		RenderTexture.ReleaseTemporary(rt);

		return scaled;
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
