using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DialogueDisplay : MonoBehaviour
{
    public Image leftAvatar;
    public Image rightAvatar;
    public TMP_Text dialogueText;

    int _dialogueIndex = 0;

    enum DialogueState
	{
        Initializing,
        Ready,
        Finished,
	}
    DialogueState _dialogueState = DialogueState.Initializing;

    async void Start()
    {
		leftAvatar.gameObject.SetActive(false);
		rightAvatar.gameObject.SetActive(false);
        dialogueText.text = "Initializing...";
		await DialogueManager.Instance.Init();
        _dialogueState = DialogueState.Ready;
        DisplayDialogue();
    }

    void DisplayDialogue()
	{
        DialogueEntry dialogueEntry = DialogueManager.Instance.DialogueData.dialogue[_dialogueIndex];

        Image avatarImage = dialogueEntry.avatar?.position == "left" ? leftAvatar : rightAvatar;
        leftAvatar.gameObject.SetActive(false);
        rightAvatar.gameObject.SetActive(false);
        avatarImage.gameObject.SetActive(true);

        if(dialogueEntry.avatar == null)
		{
            Debug.LogError("Avatar not found: " + dialogueEntry.name);
		}

        avatarImage.sprite = dialogueEntry.avatar?.sprite;
		string formatted = Regex.Replace(dialogueEntry.text, @"\{(.*?)\}", "<sprite name=$1>");
        dialogueText.spriteAsset = DialogueManager.Instance.DialogueData.emojiSpriteAsset;
        dialogueText.text = formatted;
	}

    void Update()
    {
        if(_dialogueState == DialogueState.Ready && Input.GetKeyDown(KeyCode.Mouse0))
		{
            _dialogueIndex++;
            if(_dialogueIndex >= DialogueManager.Instance.DialogueData.dialogue.Length)
			{
                gameObject.SetActive(false);
                _dialogueState = DialogueState.Finished;
            }
            else
			{
				DisplayDialogue();
			}
		}
    }
}
