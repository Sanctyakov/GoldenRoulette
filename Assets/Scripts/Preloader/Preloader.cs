using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;

public class Preloader : MonoBehaviour
{
	public Image slider;

	private AsyncOperation gameLoadingAO;
	void Start()
	{
		gameLoadingAO = SceneManager.LoadSceneAsync("Game");
	}

	// Update is called once per frame
	void Update()
	{
		if (!gameLoadingAO.isDone)
		{
			slider.fillAmount = gameLoadingAO.progress;
		}
	}
}
