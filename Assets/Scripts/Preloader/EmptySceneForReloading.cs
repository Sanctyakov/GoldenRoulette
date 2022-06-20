using UnityEngine;
using UnityEngine.SceneManagement;

public class EmptySceneForReloading : MonoBehaviour
{
	public bool load = false;
	void Start()
	{
		//SceneManager.UnloadScene("Game");
		//SceneManager.LoadScene("Game");
	}

	void Update()
	{
		if (load)
		SceneManager.LoadScene("Game");
	}
}
