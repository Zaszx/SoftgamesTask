using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueDisplay : MonoBehaviour
{
    public Image leftAvatar;
    public Image rightAvatar;
    public TMP_Text text;

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
        await DialogueManager.Instance.Init();
        _dialogueState = DialogueState.Ready;
        DisplayDialogue();
    }

    void DisplayDialogue()
	{
        DialogueEntry dialogueEntry = DialogueManager.Instance.dialogueData.dialogue[_dialogueIndex];

        Image avatarImage = dialogueEntry.avatar?.position == "left" ? leftAvatar : rightAvatar;
        leftAvatar.gameObject.SetActive(false);
        rightAvatar.gameObject.SetActive(false);
        avatarImage.gameObject.SetActive(true);

        avatarImage.sprite = dialogueEntry.avatar?.sprite;
        text.text = dialogueEntry.text;
	}

    void Update()
    {
        if(_dialogueState == DialogueState.Ready && Input.GetKeyDown(KeyCode.Mouse0))
		{
            _dialogueIndex++;
            if(_dialogueIndex >= DialogueManager.Instance.dialogueData.dialogue.Length)
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
