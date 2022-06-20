using UnityEngine;
using UnityEngine.UI;
//using SteinCo.Ivisa.RoulettePremium.Attract;

namespace SteinCo.Ivisa.RoulettePremium.Game
{
	public class CylinderController : MonoBehaviour
	{
		[System.Serializable]
		public struct BallAnimation
		{
			public AnimationClip animation;
			public int endingCell;
		}

		public float speed = -60.0f;
		public Transform cylinderTransform;
		public Transform cylinderTransform2;
		public Animation animationComponent;
		public Transform ballTransform;
		public Transform ballRootTransform;

		public Transform resultCamera;
		public float resultCameraOffsetAngle = 0.0f;

		public Transform pseudoBallRotationAxis;
		public float launchThreshold = 10.0f;

		public BallAnimation[] animationsSingleZero;
		public BallAnimation[] animationsDoubleZero;

		private BallAnimation[] currentAnimations;
		private BallAnimation currentBallAnimation;

		private enum AnimationStates
		{
			Stopped,
			Calculating,
			Preanimating,
			Animating,
			Ending,
		}
		private AnimationStates animationState = AnimationStates.Stopped;

		private float timeCounter = 0.0f;

		private Transform ballOriginalParentTransform;
		private Vector3 ballOriginalPosition;
		private Quaternion ballOriginalRotation;
		private Vector3 ballOriginalScale;

		private float cellAngle;
		private int[] numberOrder;

		// Debug
		public bool started = false;
        public bool spinEnded = false;
        public int target = 0;

		public delegate void SpinEnded();
		public event SpinEnded OnSpinEnded;

		public delegate void SpinReady();
		public event SpinReady OnSpinReady;

		public bool Paused
		{
			get;
			set;
		}

		void Awake()
		{
			RouletteType.OnRouletteTypeChanged += OnRouletteTypeChanged;
		}

		void OnDestroy()
		{
			RouletteType.OnRouletteTypeChanged -= OnRouletteTypeChanged;
		}

		void Start()
		{
			ballOriginalPosition = ballTransform.position;
			ballOriginalRotation = ballTransform.rotation;
			ballOriginalScale = ballTransform.localScale;
			ballOriginalParentTransform = ballTransform.parent;
		}

		public float CellAmount
		{
			get
			{
				return cellAmount;
			}
		}

		private float cellAmount = 37.0f;

		private void OnRouletteTypeChanged(RouletteType.Types type)
		{
			switch (type)
			{
				case RouletteType.Types.SingleZero:
					numberOrder = new int[] { 0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26 };
					//numberOrder = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36 };
					cellAmount = 37.0f;
					currentAnimations = animationsSingleZero;
					break;
				case RouletteType.Types.DoubleZero:
					//numberOrder = new int[] { 0, 28, 9, 26, 30, 11, 7, 20, 32, 17, 5, 22, 34, 15, 3, 24, 36, 13, 1, 37, 27, 10, 25, 29, 12, 8, 19, 31, 18, 6, 21, 33, 16, 4, 23, 35, 13, 2 };
					numberOrder = new int[] { 37, 27, 10, 25, 29, 12, 8, 19, 31, 18, 6, 21, 33, 16, 4, 23, 35, 14, 2, 0, 28, 9, 26, 30, 11, 7, 20, 32, 17, 5, 22, 34, 15, 3, 24, 36, 13, 1 };
					//numberOrder = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37 };
					cellAmount = 38.0f;
					currentAnimations = animationsDoubleZero;
					break;
			}

			cellAngle = 360.0f / cellAmount;
		}

		void Update()
		{
			if (started)
			{
				started = false;
				Animate(target);
			}

			int animId = animationId;

			if (animId == 0)
			{
				animId = 5;
			}

			timeCounter += Time.deltaTime;

			// Rotate cylinder

			if (Paused)
			{
				//cylinderTransform.localRotation = Quaternion.Euler(Vector3.up * speed * AttractLoop.currentTime);
				cylinderTransform2.localRotation = cylinderTransform.localRotation;
			}
			else
			{
				cylinderTransform.localRotation = Quaternion.Euler(Vector3.up * speed * Time.time);
				cylinderTransform2.localRotation = cylinderTransform.localRotation;
			}


            // Animate ball
            switch (animationState)
            {
                case AnimationStates.Stopped:
                    break;
                case AnimationStates.Calculating:

                    float difference = CalculateIfReadyToLaunch();

                    if (Mathf.Abs(difference - pseudoBallRotationAxis.localRotation.eulerAngles.y) < launchThreshold)
                    {
                        Animate(difference);
                        OnSpinReady.Invoke();
                    }
                    break;
                case AnimationStates.Preanimating:
                    timeCounter = 0.0f;

                    // Reset ball position, and reattach to the animation root node
                    ballTransform.parent = ballOriginalParentTransform;
                    ballTransform.position = ballOriginalPosition;
                    ballTransform.rotation = ballOriginalRotation;
                    ballTransform.localScale = ballOriginalScale;

                    // Start the animation
                    animationComponent.AddClip(currentBallAnimation.animation, currentBallAnimation.animation.name);
                    animationComponent.clip = currentBallAnimation.animation;
                    animationComponent.Rewind();
                    animationComponent.Play();

                    animationState = AnimationStates.Animating;

                    break;
                case AnimationStates.Animating:
                    if (timeCounter > animationComponent.clip.length)
                    {
                        animationState = AnimationStates.Ending;

                        float diff = timeCounter - animationComponent.clip.length;

                        // Update Result Camera
                        resultCamera.localRotation = Quaternion.Euler(0.0f, -lastTargetPos * cellAngle + resultCameraOffsetAngle - 60.0f * diff, 0.0f);

                        OnSpinEnded.Invoke();

                        spinEnded = true;
                    }

                    break;
                case AnimationStates.Ending:
                    // Attach the ball to the cylinder, so it keeps its rotation in place
                    ballTransform.parent = cylinderTransform;

                    float angle = (FindPosition(target) * -cellAngle - cellAngle / 2.0f) * Mathf.Deg2Rad;
                    ////DebugWithDate.Log(cylinderTransform.localRotation.eulerAngles + " " + angle);

                    ballTransform.localPosition = new Vector3(Mathf.Cos(angle), 0.0081f, Mathf.Sin(angle)) * distance;

                    animationState = AnimationStates.Stopped;
                    break;
            }
        }

		public float distance = 1.0f;

		private int CurrentPosition()
		{
			return Mathf.FloorToInt(cylinderTransform.rotation.eulerAngles.y / cellAngle);
		}

		private float CurrentPositionRemaider()
		{
			return cylinderTransform.rotation.eulerAngles.y / cellAngle - CurrentPosition();
		}

		private int targetPos;
		private int endCell;
		public int lala = 0;

        public void PreAnimate(int targetNumber)
        {
            if ((CellAmount == 38.0f && targetNumber > 37) || (CellAmount == 37.0f && targetNumber > 36) || targetNumber < 0)
            {
                //DebugWithDate.LogError("El número " + targetNumber + " no corresponde a la ruleta");
            }
            else
            {
                //DebugWithDate.Log("Animación de giro de ruleta: bolilla cayendo al número " + targetNumber);

                target = targetNumber;

                // Pick animation at random
                animationId = Random.Range(0, currentAnimations.Length);

                currentBallAnimation = currentAnimations[animationId];

                if (animationId >= currentAnimations.Length)
                {
                    animationId = 0;
                }

                animationId = lala;

                // Given current plate position, calculate how much we should rotate the animation

                targetPos = FindPosition(targetNumber, true);
                endCell = currentBallAnimation.endingCell;

                animationState = AnimationStates.Calculating;
            }
        }

        private float CalculateIfReadyToLaunch()
		{
			float differenceInDegrees = (endCell - targetPos + CurrentPositionRemaider() + CurrentPosition()) * cellAngle;

			return differenceInDegrees;
		}

		private int lastTargetPos;
		public int animationId = -1;

		private void Animate (float differenceInDegrees)
		{
			// Rotate the animation
			ballRootTransform.rotation = Quaternion.Euler(Vector3.up * differenceInDegrees);

			// Start animation
			animationState = AnimationStates.Preanimating;

			lastTargetPos = targetPos;
		}

		private int FindPosition(int value, bool reverse = false)
		{
			bool found = false;
			int counter = 0;

			while (!found)
			{
				if (numberOrder[counter] == value)
				{
					found = true;
				}
				else
				{
					counter++;

					if (counter >= numberOrder.Length)
					{
						//DebugWithDate.LogError("FindPos: " + value + " not found in Roulette");
						break;
					}
				}
			}

			if (reverse)
			{
				return numberOrder.Length - counter;
			}

			return counter;
		}
	}
}