using UnityEngine;
using System.Collections;

public class PrizeGlow : MonoBehaviour
{
	public float duration = 1.0f;

	private bool started = false;
	private bool show = false;
	private Material material;
	private float timeCounter = 0.0f;

	private float alpha = 0.0f;

	// Use this for initialization
	void Start()
	{
		material = GetComponent<MeshRenderer>().material;
	}

	// Update is called once per frame
	void Update()
	{
		if (!started)
		{
			return;
		}

		timeCounter += Time.deltaTime;

		if (timeCounter > duration)
		{
			started = false;
		}

		

		if (show)
		{
			alpha += Time.deltaTime / duration;
		}
		else
		{
			alpha -= Time.deltaTime / duration;
		}

		alpha = Mathf.Clamp01(alpha);

		material.SetColor("_TintColor", new Color(1.0f, 1.0f, 1.0f, alpha));
		//material.color = new Color(1.0f, 1.0f, 1.0f, alpha);
	}

	public void Show()
	{
		started = true;
		show = true;
		timeCounter = 0.0f;
	}

	public void Hide()
	{
		started = true;
		show = false;
		timeCounter = 0.0f;
	}

	public void HideNow()
	{
		material.SetColor("_TintColor", new Color(1.0f, 1.0f, 1.0f, 0.0f));
		//material.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
	}
}
