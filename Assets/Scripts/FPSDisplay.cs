using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
	public TMP_Text fpsText;
	public float updateInterval = 1f;

	private int frameCount = 0;
	private float elapsedTime = 0f;

	void Update()
	{
		frameCount++;
		elapsedTime += Time.unscaledDeltaTime;

		if (elapsedTime >= updateInterval)
		{
			float fps = frameCount / elapsedTime;
			fpsText.text = $"FPS: {fps:F1}";
			frameCount = 0;
			elapsedTime = 0f;
		}
	}
}
