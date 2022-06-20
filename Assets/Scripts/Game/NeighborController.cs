using UnityEngine;
using UnityEngine.UI;

public class NeighborController : MonoBehaviour {

	public float neighborAmount;
	public Text neighborText;

	void Awake ()
	{
		neighborText.text = "Vecinos: " + neighborAmount;
	}
		
	public void UpdateNeighbors (float newAmount)
	{
		
		neighborAmount = neighborAmount + newAmount;

		if (neighborAmount >= 8)
		{
			neighborAmount = 8;
		}
		else if (neighborAmount <= 0)
		{
			neighborAmount = 0;
		}

		neighborText.text = "Vecinos: " + neighborAmount;
	}
}
