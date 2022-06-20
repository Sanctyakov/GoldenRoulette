using UnityEngine;
using UnityEngine.UI;
using SteinCo.Ivisa.RoulettePremium.Game;
using SteinCo.Ivisa.RoulettePremium.Chips;

namespace SteinCo.Ivisa.RoulettePremium.Tools
{
	public class BetStressTest : MonoBehaviour
	{
		public GameObject rootBetButtons;
		public BetController betController;
		public ChipsController chipsController;
		public GameLoop gameLoop;
		public CylinderController cylinderController;
		public Button spinButton;
		public Slider slider;
		public int cancelThreshold = 100;
		public int perFrameAmount = 100;
		public int cancelTimes = 10;
		public bool started = false;

		public float slotChangeRate = 1.0f;
		public float spinRate = 1.0f;

		private BettingButton[] buttons;

		void Awake()
		{
			cylinderController.OnSpinEnded += OnSpinEnded;
		}

		void OnDestroy()
		{
			cylinderController.OnSpinEnded -= OnSpinEnded;
		}

		void Start()
		{
			buttons = rootBetButtons.GetComponentsInChildren<BettingButton>();
		}

		private int chipCounter = 1;
		private void ChangeChip()
		{
			chipsController.SelectValue(chipCounter++);

			if (chipCounter >= chipsController.SlotAmount)
			{
				chipCounter = 0;
			}
		}

		private int counter = 0;
		private bool undo = false;
		private float timeCounter = 0.0f;
		private int cancelCounter = 0;

		void Update()
		{
			if (!started)
			{
				return;
			}

			timeCounter += Time.deltaTime;
			spinCounter += Time.deltaTime;

			if (spinCounter > spinRate && spinButton.interactable)
			{
				int targetValue = Mathf.FloorToInt(slider.value);
				targetValue++;

				if (targetValue > slider.maxValue)
				{
					targetValue = 0;
				}
				slider.value = targetValue;

				gameLoop.OnSpin();
			}

			if (timeCounter > slotChangeRate)
			{
				timeCounter = 0.0f;

				ChangeChip();
			}

			if (undo)
			{
				for (int i = 0; i < perFrameAmount; i++)
				{
					betController.CancelLastBet();

					counter--;
				}
			}
			else
			{
				for (int i = 0; i < perFrameAmount; i++)
				{
					int button = Random.Range(0, buttons.Length);

					buttons[button].OnBet();

					counter++;
				}
			}

			if (counter > cancelThreshold || counter <= 0)
			{
				cancelCounter++;

				if (cancelCounter >= cancelTimes && !undo)
				{
					cancelCounter = 0;
					counter = 0;
					betController.CancelAllBets();
				}
				else
				{
					undo = !undo;
				}
			}
		}

		public void ToggleStarted()
		{
			started = !started;

			if (!started)
			{
				betController.CancelAllBets();
				cancelCounter = 0;
				counter = 0;
				timeCounter = 0.0f;
				spinCounter = 0.0f;
				undo = false;
			}
		}

		private float spinCounter = 0.0f;

		public void OnSpinEnded()
		{
			spinCounter = 0.0f;
		}
	}
}