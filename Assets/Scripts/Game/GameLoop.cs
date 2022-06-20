using SteinCo.Comms;
using SteinCo.Intercomms;
using SteinCo.Ivisa.RoulettePremium.API;
using SteinCo.Ivisa.RoulettePremium.API.Classes;
using SteinCo.Ivisa.RoulettePremium.API.Utils;
//using SteinCo.Ivisa.RoulettePremium.Attract;
using SteinCo.Ivisa.RoulettePremium.Chips;
using SteinCo.Ivisa.RoulettePremium.Sound;
using SteinCo.Ivisa.RoulettePremium.UI;
using SteinCo.Utils;
using System.Collections.Generic;
using System.Globalization;
//using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SteinCo.Ivisa.RoulettePremium.Menu;
using System.IO;
using System.Collections;
//using System.Diagnostics;

namespace SteinCo.Ivisa.RoulettePremium.Game
{
    public class GameLoop : MonoBehaviour
    {
        #region Object references

        public FadeImage racetrack;

        public Highlighter highlighter;
        public CylinderController cylinderController;
        public FadeImage rouletteResultCamera;
        public BetController betController;
        public ChipsController chipsController;
        public RemoveLosingChips removeLosingChips;
        public AddWinningChips addWinningChips;
        public InfoMessage infoMessage;

        public BallSoundFX ballSoundFX;
        public ChipsSoundFX chipsSoundFX;
        public NumberHistory numberHistory;
        public RouletteType rouletteType;
        public Tooltips tooltips;
        public HelperButtons helperButtons;
    
        public PseudoBall pseudoBall;
        public MeshRenderer ballMR;

        public HistoryGlow historyGlow;
        public TimeAlert timeAlert;
        public BetPrizeGlow betPrizeGlow;
        public ChipGUITexts chipGUITexts;

        public Button cancelAllBets;
        public Button cancelLastBet;
        public Button repeatLastBet;
        public Button spin;

        public GameObject betWinningsPanel;
        public GameObject betWinningsPanelWebGL;
        public Text betText;
        public Text winningsText;

        public Text creditsTextWebGL;
        public Text betTextWebGL;
        public Text winningsTextWebGL;


        public FadeImage timeLimit;
        public Text timeLimitText;

        public Button previousPlaysButton;

        public GameObject payoutTable;

        public IServerLayer serverLayer;
        public Component serverLayerComponent;
        public ServerCommsGetVersion serverComms;

        public GameObject payoutPanel;
        public GameObject audioVolumePanel;
        public GameObject musicVolumePanel;
        public GameObject exitButton;

        public Version version;
        public GameObject table3D;

        public StandaloneInputModule standaloneInputModule;

        #endregion

#pragma warning disable 618
        // Obsolete but useful to avoid the "autoclick" when the UPS is connected pinging the machine via USB
        public TouchInputModule touchInputModule;

        public Button payoutTableButton;

        #region Game vars

        //private TCPSocket socketComms;
        private int time;
        public static CultureInfo cultureInfo;
        private bool firstBetOfRound = false;
        private bool losingChipsAnimationEnded = false;
        private bool winningChipsAnimationEnded = false;

        private bool retryMonedero = false;

        private bool flagUnknownMessage = false;

        public static bool appIsWebGL = false;
        private bool initialized = false;

        private float timeCounter = 0.0f;

        public ChipsPile[] chanceBets;

        #endregion

        #region GameState vars

        private enum GameStates
        {
            LocalDataAcquisition,
            WaitingLocalDataAcquisition,
            Init,
            WaitingInit,
            Bet,
            Spin,
            SpinTimeOut,
            FakeSpin,
            Result,
            PreviousPlays,
        }

        private GameStates gameState = GameStates.LocalDataAcquisition;

        private string URI;

        #endregion

        #region Event handler vars

        private bool independentTimeCountdown = false;
        private float startingIndependentTime = 0.0f;

        #endregion

        #region Intercommm vars

        //private Thread connectToMonederoThread;
        private string monederoURI;
        private int monederoPort;
        private bool flagTimeScaleOff = false;
        private bool flagTimeScaleOn = false;

        #endregion

        #region Monobehaviour

        private System.Text.StringBuilder text = new System.Text.StringBuilder();

        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return;
            }
            string path = ".\\Logs\\App";
            string logFileName = "\\LogApp" + System.DateTime.Today.ToString("yyyyMMdd") + ".txt";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            text.Length = 0;
            
            switch (type)
            {
                case LogType.Error:
                    text.Append("[ERROR] ");
                    break;
                case LogType.Assert:
                    text.Append("[ASSERT] ");
                    break;
                case LogType.Warning:
                    text.Append("[WARNING] ");
                    break;
                case LogType.Log:
                    text.Append("[INFO] ");
                    break;
                case LogType.Exception:
                    text.Append("[EXCEPTION] ");
                    break;
                default:
                    text.Append("[LOG] ");
                    break;
            }

            text.AppendLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            text.Append("[CONDITION] ");
            text.AppendLine(condition);

            text.AppendLine(stackTrace);

            StreamWriter sw = new StreamWriter(File.Open(path + logFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.UTF8);
            sw.WriteLine(text);
            sw.Flush();
            sw.Close();
        }

        void Awake()
        {
            /*Process simuladorMonedero = new Process();
            simuladorMonedero.StartInfo.FileName = Application.dataPath + "/SimuladorMonedero.exe"; //change the path
            simuladorMonedero.Start();*/

            Input.multiTouchEnabled = false;

            PlayerPrefs.DeleteAll();

            Application.RegisterLogCallback(HandleLog);

            appIsWebGL = Application.platform == RuntimePlatform.WebGLPlayer;

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                appIsWebGL = false;
            }

            ////DebugWithDate.Log("JUEGO ARRANCADO: Ruleta 3D " + version.version);
            serverLayer = serverLayerComponent.GetComponent<IServerLayer>();

            serverComms.TimeOut = float.Parse(WorkingData.GetData("Comms Time Out", (5.0f).ToString()));
            serverComms.RetryPeriod = 2.0f;

            cylinderController.OnSpinReady += OnSpinReady;
            betController.OnBetChanged += OnBetChanged;
            BettingButton.OnBetMade += OnBetMade;

            removeLosingChips.OnFinished += OnLosingChipsAnimationEnded;
            addWinningChips.OnFinished += OnWinningChipsAnimationEnded;

            serverLayer.OnInit += OnInit;
            serverLayer.OnInitWeb += OnInitWeb;
            serverLayer.OnCredits += OnCredits;
            serverLayer.OnSpin += OnSpin;
            serverLayer.OnSessionEnd += OnSessionEnd;

            //socketComms = new TCPSocket();

            //socketComms.OnMessageReceived += OnMessageReceived;
            //socketComms.OnError += OnError;

            //attractLoop.OnAttractStarted += OnAttractStarted;

            //table3D.SetActive(true);
			//table2D.SetActive(false);

            betWinningsPanel.SetActive(true);
            betWinningsPanelWebGL.SetActive(false);

            standaloneInputModule.enabled = false;
            touchInputModule.enabled = true;

            Application.runInBackground = true;

            if (GameLoop.appIsWebGL)
            {
				//table3D.SetActive(false);
				//table2D.SetActive(true);

                betWinningsPanel.SetActive(false);
                betWinningsPanelWebGL.SetActive(true);

                standaloneInputModule.enabled = true;
                touchInputModule.enabled = false;

                serverComms.TimeOut = 15.0f;

                Application.runInBackground = false;

                ChangeState(GameStates.WaitingLocalDataAcquisition);
                Application.ExternalCall("JuegoComenzado");

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    //var idsesionusuario = "007300030051000056";
                    var idsesionusuario = "000000655040900012";

                    var URL = "http://200.42.89.158:59165/RuletaWebService/rws";

                    // Enviamos la configuración necesaria para la ruleta
                    // Hay que juntar los dos parámetros en un solo string, separado por # porque 
                    // podemos enviar un único parámetro solamente
                    OnInitWeb(idsesionusuario + "#" + URL);
                }
            }
            else
            {
                ChangeState(GameStates.Init);

                //ConnectToMonedero();

                //////DebugWithDate.Log("Socket started");
            }
        }

        void OnDestroy()
        {
            cylinderController.OnSpinReady -= OnSpinReady;
            betController.OnBetChanged -= OnBetChanged;
            BettingButton.OnBetMade -= OnBetMade;

            removeLosingChips.OnFinished -= OnLosingChipsAnimationEnded;
            addWinningChips.OnFinished -= OnWinningChipsAnimationEnded;

            serverLayer.OnInit -= OnInit;
            serverLayer.OnInitWeb -= OnInitWeb;
            serverLayer.OnCredits -= OnCredits;
            serverLayer.OnSpin -= OnSpin;
            serverLayer.OnSessionEnd -= OnSessionEnd;

            //socketComms.OnMessageReceived -= OnMessageReceived;
            //socketComms.OnError -= OnError;

			//attractLoop.OnAttractStarted -= OnAttractStarted;
        }

        void OnApplicationFocus(bool focusStatus)
        {
            if (initialized && GameLoop.appIsWebGL)
            {
                if (focusStatus)
                {
                    serverLayer.CallCredits();
                }
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (initialized && GameLoop.appIsWebGL)
            {
                if (!pauseStatus)
                {
                    serverLayer.CallCredits();
                }
            }
        }

        private void OnPreviousPlaysOn()
        {
            ChangeState(GameStates.PreviousPlays);
            ClearGUI();
        }

        private void OnPreviousPlaysOff()
        {
            ChangeState(GameStates.Bet);
            ClearGUI();
        }

        void Update()
        {

            if (flagTimeScaleOn)
            {
                flagTimeScaleOn = false;
                Time.timeScale = 1.0f;
            }
            if (flagTimeScaleOff)
            {
                flagTimeScaleOff = false;
                Time.timeScale = 0.0f;
            }

            if (retryMonedero)
            {
                retryMonedero = false;
                //ConnectToMonedero();
            }

            if (flagUnknownMessage)
            {
                flagUnknownMessage = false;
            }


            //debugText.text = "Creditos: " + ConvertAmountToMoney(betController.credits);

            if (independentTimeCountdown)
            {
                if ((Time.realtimeSinceStartup - startingIndependentTime) > 180.0f)
                {
                    payoutPanel.SetActive(false);
                    audioVolumePanel.SetActive(false);
                    musicVolumePanel.SetActive(false);
                    //exitButton.SetActive(true);
                    OnResumeTime();
                    ballSoundFX.OnResume();
                }
            }

            previousPlaysButton.interactable = false;
			
            switch (gameState)
            {
                case GameStates.LocalDataAcquisition:
                    LocalDataAcquisition();
                    break;
                case GameStates.Init:
                    Init();
                    break;
                case GameStates.WaitingInit:
                    break;
                case GameStates.Bet:
                    ProcesarTimerApuesta();
                    break;
                case GameStates.Spin:
                    break;
                case GameStates.FakeSpin:
                    break;
                case GameStates.Result:
                    break;
            }
        }

        #endregion

        #region GameStates

        private void ChangeState(GameStates state)
        {
            gameState = state;
        }

        private void LocalDataAcquisition()
        {
            URI = (string)Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Ivisa\\RoulettePremium", "URL", string.Empty);

            if (URI.Equals(string.Empty))
            {
            }
            else
            {
                //SendSocketMessage("TERMINAL#");
            }
            ChangeState(GameStates.WaitingLocalDataAcquisition);
        }

        private void Init()
        {
            ballMR.enabled = false;
            //rouletteResultCamera.ShowNow();
			racetrack.ShowNow();
            highlighter.ClearHighlights();
            UpdateButtonsState();

            ChangeState(GameStates.WaitingInit);

            if (GameLoop.appIsWebGL)
            {
                serverLayer.CallInitWeb();
            }
            else
            {
                serverLayer.CallInit();
            }


			infoMessage.ShowMessage(InfoMessage.MessageTypes.PleaseBet, false);
        }

        private void ProcesarTimerApuesta()
        {
            if (time == 0)
            {
                return;
            }

            timeCounter += Time.fixedUnscaledDeltaTime;

            System.DateTime dt = new System.DateTime();
            string displayTime = string.Empty;
            string suffix = " segundos";

            if (timeCounter >= time)
            {
                displayTime = dt.Second.ToString("00");

                if (betController.CurrentBetAmount > 0) // Si hay fichas en mesa:
                {
                    ////DebugWithDate.Log("Girando tras timeout con fichas en mesa. Juega por: " + ConvertAmountToMoney(betController.CurrentBetAmount));

                    Spin();
                }
                else
                {
                    ////DebugWithDate.Log("Girando tras timeout sin fichas en mesa.");

                    FakeSpin();
                }
            }
            else
            {
                int remainingSeconds = Mathf.CeilToInt(time - timeCounter);
                dt = dt.AddSeconds(remainingSeconds);

                if (remainingSeconds <= 10)
                {
                    timeAlert.Begin();
                }

                if (time >= 60)
                {
                    displayTime = dt.Minute.ToString("0") + ":" + dt.Second.ToString("00");

                    if (dt.Minute > 0)
                    {
                        suffix = " minutes";
                    }
                }
                else
                {
                    displayTime = dt.Second.ToString("00");
                }
            }

            timeLimitText.text = displayTime + suffix;
        }

        private IEnumerator Result(SpinAnswer spinAnswer)
        {
            yield return new WaitUntil(() => losingChipsAnimationEnded && winningChipsAnimationEnded);

            losingChipsAnimationEnded = false;
            winningChipsAnimationEnded = false;

            if (spinAnswer.msgPremioMayor != null && !spinAnswer.msgPremioMayor.Equals(string.Empty))
            {
                OnSuspendTime();
                ballSoundFX.OnPause();

                spinAnswer.msgPremioMayor = string.Empty;
            }
            else
            {
                chipGUITexts.SetCanvasToOverlay();
                firstBetOfRound = true;
                timeCounter = 0.0f;

                if (time > 0)
                {
                    timeLimit.Show();
                }

                betController.CancelAllBets();

                ChangeState(GameStates.Bet);

                UpdateButtonsState();
                addWinningChips.ResetTexts();

                payoutTableButton.interactable = true;
            }
        }

        #endregion

        #region Game Functions

        private void Spin()
        {
            bool spinIniciado = false;

            if (gameState == GameStates.Bet)
            {
                ////DebugWithDate.Log("Estado de juego: Spin");

                ChangeState(GameStates.Spin);

                spinIniciado = true;
            }

            if (spinIniciado)
            {
                timeLimit.Hide();

                AnySpinCommands();

                TrueSpinCommands();
            }
        }

        private void FakeSpin()
        {
            bool fakeSpinIniciado = false;

            if (gameState == GameStates.Bet || gameState == GameStates.Spin || gameState == GameStates.SpinTimeOut)
            {
                ////DebugWithDate.Log("Estado de juego: FakeSpin");

                ChangeState(GameStates.FakeSpin);

                fakeSpinIniciado = true;
            }

            if (fakeSpinIniciado)
            {
                AnySpinCommands();

                SpinAnswer spinAnswer = new SpinAnswer();

                spinAnswer.numero = Random.Range(0, 37);
                spinAnswer.saldoInt = betController.credits;
                spinAnswer.premiosTotalInt = 0;
                spinAnswer.mensajeError = string.Empty;

                ////DebugWithDate.Log("Giro sin fichas tras timeout. Bolilla cayendo al número " + spinAnswer.numero);

                OnSpin(spinAnswer);
            }
        }

        private void AnySpinCommands()
        {
            UpdateButtonsState();

            timeAlert.Stop();

            ClearGUI();
        }

        private void TrueSpinCommands()
        {
            //SendSocketMessage("INICIOJUGADA#");

            serverLayer.CallSpin(betController);
        }

        private void UpdateButtonsState()
        {
            if (gameState == GameStates.Bet)
            {
                spin.interactable = true;
                cancelAllBets.interactable = true;
                cancelLastBet.interactable = true;
                repeatLastBet.interactable = true;

                payoutTableButton.interactable = true;
                previousPlaysButton.interactable = true;

                /*
				if (betController.AreThereBetsLeft)
				{
					spin.interactable = true;
					cancelAllBets.interactable = true;
					cancelLastBet.interactable = true;
				}
				else
				{
					spin.interactable = false;
					cancelAllBets.interactable = false;
					cancelLastBet.interactable = false;
				}

				repeatLastBet.interactable = betController.PreviousBetPresent;
				*/
            }
            else
            {
                cancelAllBets.interactable = false;
                cancelLastBet.interactable = false;
                repeatLastBet.interactable = false;
                spin.interactable = false;
            }
        }

        public static string ConvertAmountToMoney(int amount)
        {
            //System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.GetCultureInfo("es-AR");
            return (amount / 100.0f).ToString("C", cultureInfo);
        }

        private void CheckIfFirstBet()
        {
            if (firstBetOfRound)
            {
                ClearGUI();

                betController.CancelAllBets();

                firstBetOfRound = false;
            }
        }

        private void ClearGUI()
        {
            if (firstBetOfRound)
            {
                chipGUITexts.HideAll();
            }
            tooltips.Hide();
			infoMessage.ShowMessage(InfoMessage.MessageTypes.PleaseBet);
            //rouletteResultCamera.Show();
			rouletteResultCamera.Hide();
			racetrack.Show();
            highlighter.ClearHighlights();
            historyGlow.Stop();
            betPrizeGlow.Stop();
            addWinningChips.ClearTable();
            winningsText.text = ConvertAmountToMoney(0);
            winningsTextWebGL.text = ConvertAmountToMoney(0);
        }

        #endregion

        #region GUI Input

        public void OnSpin()
        {
            ////DebugWithDate.Log("Botón Girar presionado. Evaluando apuesta.");

            ClearGUI();

            if (!betController.AreThereBetsLeft)
            {
                chipsSoundFX.OnError();
                infoMessage.ShowMessage(InfoMessage.MessageTypes.NoBets);

                ////DebugWithDate.Log("Apuesta inválida: no hay fichas.");
            }
            else if (!AllChanceBetsMinReached())
            {
                chipsSoundFX.OnError();
                infoMessage.ShowMessage(InfoMessage.MessageTypes.AllChanceBetsMinNotReached);

                ////DebugWithDate.Log("Apuesta inválida: mínimo de chance no alcanzado.");
            }
            else if (!betController.AllSimpleBetsMinReached())
            {
                chipsSoundFX.OnError();
                infoMessage.ShowMessage(InfoMessage.MessageTypes.AllSimpleBetsMinNotReached);

                ////DebugWithDate.Log("Apuesta inválida: mínimo simple no alcanzado.");
            }
            else if (!TableMinReached())
            {
                chipsSoundFX.OnError();
                infoMessage.ShowMessage(InfoMessage.MessageTypes.MinBetNotReached);

                ////DebugWithDate.Log("Apuesta inválida: mínimo de mesa no alcanzado.");
            }
            else
            {
                ////DebugWithDate.Log("Apuesta aprobada. Juega por: " + ConvertAmountToMoney(betController.CurrentBetAmount));

                betController.SetBetAsMade();

                Spin();
            }
        }

        private bool AllChanceBetsMinReached ()
		{
			for (int i = 0; i < chanceBets.Length; i++)
			{
				if (chanceBets[i].previousAmount != 0 && chanceBets[i].previousAmount < betController.apMinChance)
				{
					return false;
				}
			}

			return true;
		}

		private bool TableMinReached ()
		{
			return betController.CurrentBetAmount >= betController.MinimumTableBetAmount;
		}

        public void OnRepeatBet()
        {
            ClearGUI();

			if (betController.FirstBet)
			{
				chipsSoundFX.OnError();
				infoMessage.ShowMessage(InfoMessage.MessageTypes.NothingToRepeat);
			}
            else if (!betController.PreviousBetPresent)
            {
                chipsSoundFX.OnError();
                infoMessage.ShowMessage(InfoMessage.MessageTypes.CantRepeat);
            }
            else
            {
                CheckIfFirstBet();
                betController.RepeatBet();
                //Spin();
            }
        }

        public void OnCancelAllBets()
        {
            ClearGUI();

            if (!betController.AreThereBetsLeft)
            {
                chipsSoundFX.OnError();
                infoMessage.ShowMessage(InfoMessage.MessageTypes.NoBetsToClear);
            }
            else
            {
                betController.CancelAllBets();
                chipsSoundFX.OnCancelAll();
            }
        }

        public void OnCancelLastBet()
        {
            ClearGUI();

            if (!betController.AreThereBetsLeft)
            {
                chipsSoundFX.OnError();
                infoMessage.ShowMessage(InfoMessage.MessageTypes.NoBetsToClear);
            }
            else
            {
                betController.CancelLastBet();
                chipsSoundFX.OnCancelPrevious();
            }
        }

        public void OnBetMade(BettingButton bettingButton)
        {
            if (gameState != GameStates.Bet && gameState != GameStates.PreviousPlays)
            {
                return;
            }

            if (gameState == GameStates.Bet)
            {
                CheckIfFirstBet();
                UpdateButtonsState();
                betController.OnBetMade(bettingButton);
            }

            if (gameState == GameStates.PreviousPlays)
            {
                betController.OnShowTooltipPreviousPlay(bettingButton);
            }
        }

        public void OnQuit()
        {
            ////DebugWithDate.Log("SALIR PRESIONADO");

            //SendSocketMessage("CERRARSESION#");

            helperButtons.OnTurnOffPanels();

            if (GameLoop.appIsWebGL)
            {
                Application.ExternalCall("CerrarSesion");
            }
            else
            {
                Application.Quit();
            }
        }

        #endregion

        #region Debug

        public void OnBetALot()
        {
            foreach (BetController.BetTypes type in System.Enum.GetValues(typeof(BetController.BetTypes)))
            {
                int total = API2ID.RegisteredPiles(type);

                for (int i = 0; i < total; i++)
                {
                    //int id = API2ID.ConvertToId(i.ToString("00") + "#", type);

                    BettingButton button = API2ID.GetButton(type, i);
                    if (button.isActiveAndEnabled)
                    {
                        for (int j = 0; j < 1; j++)
                        {
                            button.OnBet();
                        }
                    }
                }
            }
        }

        public void OnDebug()
        {
            //////DebugWithDate.Log(betController.Bets);
            //numberHistory.AddNumber(Mathf.FloorToInt(slider.value));
            //rouletteType.ChangeType();
            //API2ID.DebugMe();

            //serverLayer.CallCredits();
            serverLayer.CallEndSession();
            //SendSocketMessage("CREDITOS#");

        }

        public void OnDebug2()
        {
            //socketComms.StartAsClient();
        }

        #endregion

        #region Server Event Handlers

        private void OnInit(InitAnswer initAnswer)
        {
            if (!initAnswer.mensajeError.Equals(string.Empty))
            {
                //SendSocketMessage("CREDITOS#");
            }
            else
            {
                initialized = true;

                rouletteType.ChangeToType(initAnswer.cero);

                chipsController.SetUp(initAnswer.Fichas);

                for (int i = 0; i < chipsController.SlotAmount; i++)
                {
                    float chanceFloat = initAnswer.apMaximaChance / (float)chipsController.GetValue(i);

                    if (chanceFloat - Mathf.FloorToInt(chanceFloat) > 0.0f)
                    {
                        ////DebugWithDate.LogWarning("La ficha de " + ConvertAmountToMoney(chipsController.GetValue(i)) + " no es divisor del máximo de apuesta chance");
                    }
                }

                for (int i = 0; i < chipsController.SlotAmount; i++)
                {
                    float simpleFloat = initAnswer.apMaximaSimple / (float)chipsController.GetValue(i);

                    if (simpleFloat - Mathf.FloorToInt(simpleFloat) > 0.0f)
                    {
                        ////DebugWithDate.LogWarning("La ficha de " + ConvertAmountToMoney(chipsController.GetValue(i)) + " no es divisor del máximo de apuesta simple");
                    }
                }

                for (int i = 0; i < chipsController.SlotAmount; i++)
                {
                    float mesaFloat = initAnswer.apuestaMaxima / (float)chipsController.GetValue(i);

                    if (mesaFloat - Mathf.FloorToInt(mesaFloat) > 0.0f)
                    {
                        ////DebugWithDate.LogWarning("La ficha de " + ConvertAmountToMoney(chipsController.GetValue(i)) + " no es divisor del máximo de mesa");
                    }
                }

                for (int i = 0; i < chipsController.SlotAmount; i++)
                {
                    float minimaFloat = initAnswer.apuestaMinima / (float)chipsController.GetValue(i);

                    if (minimaFloat - Mathf.FloorToInt(minimaFloat) > 0.0f)
                    {
                        ////DebugWithDate.LogWarning("La ficha de " + ConvertAmountToMoney(chipsController.GetValue(i)) + " no es divisor del mínimo de mesa");
                    }
                }

                betController.SetUp(initAnswer);
                creditsTextWebGL.text = ConvertAmountToMoney(initAnswer.Credito);

                time = initAnswer.timeout;

                //System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.GetCultureInfo("es-AR");
                //cultureInfo = CultureInfo.GetCultureInfo(initAnswer.culture);
                cultureInfo = CultureInfo.GetCultureInfo("es-AR");

                if (time == 0)
                {
                    timeLimit.HideNow();
                }

                if (!initAnswer.mensajeError.Equals(string.Empty))
                {
                    serverLayer.CallEndSession();
                }

                timeCounter = 0.0f;
                firstBetOfRound = true;

                //SendSocketMessage("CREDITOS#");

                ChangeState(GameStates.Bet);
                UpdateButtonsState();
            }
        }

        private void OnInitWeb(InitAnswerWeb initAnswer)
        {
            if (!initAnswer.mensajeError.Equals(string.Empty))
            {
                //SendSocketMessage("CREDITOS#");
            }
            else
            {
                initialized = true;
                rouletteType.ChangeToType(initAnswer.cero);

                chipsController.SetUp(initAnswer.Fichas);

                for (int i = 0; i < chipsController.SlotAmount; i++)
                {
                    float chanceFloat = initAnswer.apMaximaChance / (float)chipsController.GetValue(i);

                    if (chanceFloat - Mathf.FloorToInt(chanceFloat) > 0.0f)
                    {
                        ////DebugWithDate.LogWarning("La ficha de " + ConvertAmountToMoney(chipsController.GetValue(i)) + " no es divisor del máximo de apuesta chance");
                    }
                }

                for (int i = 0; i < chipsController.SlotAmount; i++)
                {
                    float simpleFloat = initAnswer.apMaximaSimple / (float)chipsController.GetValue(i);

                    if (simpleFloat - Mathf.FloorToInt(simpleFloat) > 0.0f)
                    {
                        ////DebugWithDate.LogWarning("La ficha de " + ConvertAmountToMoney(chipsController.GetValue(i)) + " no es divisor del máximo de apuesta simple");
                    }
                }

                for (int i = 0; i < chipsController.SlotAmount; i++)
                {
                    float mesaFloat = initAnswer.apuestaMaxima / (float)chipsController.GetValue(i);

                    if (mesaFloat - Mathf.FloorToInt(mesaFloat) > 0.0f)
                    {
                        ////DebugWithDate.LogWarning("La ficha de " + ConvertAmountToMoney(chipsController.GetValue(i)) + " no es divisor del máximo de mesa");
                    }
                }

                for (int i = 0; i < chipsController.SlotAmount; i++)
                {
                    float minimaFloat = initAnswer.apuestaMinima / (float)chipsController.GetValue(i);

                    if (minimaFloat - Mathf.FloorToInt(minimaFloat) > 0.0f)
                    {
                        ////DebugWithDate.LogWarning("La ficha de " + ConvertAmountToMoney(chipsController.GetValue(i)) + " no es divisor del mínimo de mesa");
                    }
                }

                betController.SetUp(initAnswer);
                creditsTextWebGL.text = ConvertAmountToMoney(initAnswer.Credito);

                time = initAnswer.timeout;

                //System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.GetCultureInfo("es-AR");
                //cultureInfo = CultureInfo.GetCultureInfo(initAnswer.culture);
                cultureInfo = CultureInfo.GetCultureInfo("es-AR");

                if (time == 0)
                {
                    timeLimit.HideNow();
                }

                if (!initAnswer.mensajeError.Equals(string.Empty))
                {
                    serverLayer.CallEndSession();
                }

                timeCounter = 0.0f;
                firstBetOfRound = true;

                //SendSocketMessage("CREDITOS#");

                ChangeState(GameStates.Bet);
                UpdateButtonsState();
            }
        }

        private void OnCredits(CreditAnswer creditAnswer)
        {
            if (!creditAnswer.mensajeError.Equals(string.Empty))
            {
                serverLayer.CallEndSession();
            }
            else
            {
                if (creditAnswer.msgPremioMayor != null && !creditAnswer.msgPremioMayor.Equals(string.Empty))
                {
                    OnSuspendTime();
                    ballSoundFX.OnPause();
                }
            }

            if (creditAnswer.Credito < betController.credits)
            {
                firstBetOfRound = true;
                ClearGUI();
                betController.CancelAllBets();
            }
            betController.credits = creditAnswer.Credito;
            creditsTextWebGL.text = ConvertAmountToMoney(creditAnswer.Credito);
        }

        private void OnSpin(SpinAnswer spinAnswer)
        {
            if (gameState == GameStates.Spin || gameState == GameStates.SpinTimeOut || gameState == GameStates.FakeSpin)
            {
                if (!spinAnswer.mensajeError.Equals(string.Empty))
                {
                    //SendSocketMessage("CREDITOS#");

                    serverLayer.CallEndSession();
                }
                else
                {
                    ballMR.enabled = false;
                    ballSoundFX.OnSpin();
                    pseudoBall.Begin();
                    infoMessage.ShowMessage(InfoMessage.MessageTypes.Spinning, true, false);

                    StartCoroutine(SaveSpinAnswer(spinAnswer));

                    cylinderController.PreAnimate(spinAnswer.numero);

                    /*if (spinAnswer.msgPremioMayor != null && !spinAnswer.msgPremioMayor.Equals(string.Empty))
                    {
                        OnSuspendTime();
                        ballSoundFX.OnPause();
                        legalMessage.SetActive(true);
                        legalMessageText.text = spinAnswer.msgPremioMayor;
                    }
                    else
                    {
                        OnLaunchPreAnimate(spinAnswer.numero);
                    }*/
                }
            }
            else
            {
                return;
            }
        }

        private IEnumerator SaveSpinAnswer(SpinAnswer spinAnswer)
        {
            yield return new WaitUntil(() => cylinderController.spinEnded);

            cylinderController.spinEnded = false;

            SpinEnded(spinAnswer);
        }

        private void OnSessionEnd(EndSessionAnswer endSessionAnswer)
        {
            //errorDisplay.OnSessionEnded(endSessionAnswer.mensajeError);
        }

        #endregion

        #region Event Handlers

        private void OnAttractStarted()
        {
            ClearGUI();
        }

        private void OnSpinReady()
        {
            ballMR.enabled = true;

            pseudoBall.Stop();
        }

        private void SpinEnded(SpinAnswer spinAnswer)
        {
            if (CheckForSpinOk(spinAnswer))
            {
                ////DebugWithDate.Log("Respuesta de giro de ruleta aprobada.");

                highlighter.Hightlight(spinAnswer.numero);

                if (gameState == GameStates.Spin || gameState == GameStates.SpinTimeOut)
                {
                    ////DebugWithDate.Log("Añadiendo el número " + spinAnswer.numero + " a la tabla de últimos resultados.");

                    numberHistory.AddNumber(spinAnswer.numero);
                    historyGlow.Begin(0);
                }
                else if (gameState == GameStates.FakeSpin)
                {
                    ////DebugWithDate.Log("Número " + spinAnswer.numero + " no añadido a la tabla de últimos resultados por ser un giro falso.");
                }
            }
            else
            {
                ////DebugWithDate.Log("Respuesta de giro de ruleta inválida.");
            }

            racetrack.Hide();
            rouletteResultCamera.Show();

            ChangeState(GameStates.Result);

            StartCoroutine(Result(spinAnswer));

            UpdateButtonsState();

            betController.credits = spinAnswer.saldoInt;

            creditsTextWebGL.text = ConvertAmountToMoney(spinAnswer.saldoInt);

            winningsText.text = ConvertAmountToMoney(spinAnswer.premiosTotalInt);
            winningsTextWebGL.text = ConvertAmountToMoney(spinAnswer.premiosTotalInt);

            if (spinAnswer.premiosTotalInt > 0)
            {
                betPrizeGlow.Begin();
            }

            removeLosingChips.Remove(new Dictionary<BetController.BetTypes, string>() {
            {  BetController.BetTypes.Pleno, spinAnswer.PerdPlenoarray },
            {  BetController.BetTypes.Semipleno, spinAnswer.PerdSemiplenoarray },
            {  BetController.BetTypes.Calle, spinAnswer.PerdCallearray },
            {  BetController.BetTypes.Cuadrado, spinAnswer.PerdCuadroarray },
            {  BetController.BetTypes.Linea, spinAnswer.PerdLineaarray },
            {  BetController.BetTypes.ParImpar, spinAnswer.PerdParimpararray },
            {  BetController.BetTypes.MenorMayor, spinAnswer.Perdmayormenorarray },
            {  BetController.BetTypes.Docena, spinAnswer.PerdDocenaarray },
            {  BetController.BetTypes.Columna, spinAnswer.PerdColumnaarray },
            {  BetController.BetTypes.Color, spinAnswer.PerdColor },
            {  BetController.BetTypes.TransversalConCero, spinAnswer.PerdTransversalCero }
        });

            addWinningChips.Add(new Dictionary<BetController.BetTypes, string>() {
            {  BetController.BetTypes.Pleno, spinAnswer.Premplenoarray },
            {  BetController.BetTypes.Semipleno, spinAnswer.Premsemiplenoarray},
            {  BetController.BetTypes.Calle, spinAnswer.Premcallearray },
            {  BetController.BetTypes.Cuadrado, spinAnswer.Premcuadroarray},
            {  BetController.BetTypes.Linea, spinAnswer.Premlineaarray },
            {  BetController.BetTypes.ParImpar, spinAnswer.Premparimpararray },
            {  BetController.BetTypes.MenorMayor, spinAnswer.Premmayormenorarray },
            {  BetController.BetTypes.Docena, spinAnswer.Premdocenaarray },
            {  BetController.BetTypes.Columna, spinAnswer.Premcolumnaarray },
            {  BetController.BetTypes.Color, spinAnswer.PremColor },
            {  BetController.BetTypes.TransversalConCero, spinAnswer.PremtransversalCero }
        }, spinAnswer.premiosTotalInt);

            //SendSocketMessage("CREDITOS#");
        }

        private bool CheckForSpinOk(SpinAnswer spinAnswer)
        {
            int length = 0;

            length += spinAnswer.msgPremioMayor == null || spinAnswer.msgPremioMayor.Length == 0 ? 0 : 1;
            length += spinAnswer.PerdCallearray == null || spinAnswer.PerdCallearray.Length == 0 ? 0 : 1;
            length += spinAnswer.PerdColor == null || spinAnswer.PerdColor.Length == 0 ? 0 : 1;
            length += spinAnswer.PerdColumnaarray == null || spinAnswer.PerdColumnaarray.Length == 0 ? 0 : 1;
            length += spinAnswer.PerdCuadroarray == null || spinAnswer.PerdCuadroarray.Length == 0 ? 0 : 1;
            length += spinAnswer.PerdDocenaarray == null || spinAnswer.PerdDocenaarray.Length == 0 ? 0 : 1;
            length += spinAnswer.PerdLineaarray == null || spinAnswer.PerdLineaarray.Length == 0 ? 0 : 1;
            length += spinAnswer.Perdmayormenorarray == null || spinAnswer.Perdmayormenorarray.Length == 0 ? 0 : 1;
            length += spinAnswer.PerdParimpararray == null || spinAnswer.PerdParimpararray.Length == 0 ? 0 : 1;
            length += spinAnswer.PerdPlenoarray == null || spinAnswer.PerdPlenoarray.Length == 0 ? 0 : 1;
            length += spinAnswer.PerdSemiplenoarray == null || spinAnswer.PerdSemiplenoarray.Length == 0 ? 0 : 1;
            length += spinAnswer.PerdTransversalCero == null || spinAnswer.PerdTransversalCero.Length == 0 ? 0 : 1;
            length += spinAnswer.Premcallearray == null || spinAnswer.Premcallearray.Length == 0 ? 0 : 1;
            length += spinAnswer.PremColor == null || spinAnswer.PremColor.Length == 0 ? 0 : 1;
            length += spinAnswer.Premcolumnaarray == null || spinAnswer.Premcolumnaarray.Length == 0 ? 0 : 1;
            length += spinAnswer.Premcuadroarray == null || spinAnswer.Premcuadroarray.Length == 0 ? 0 : 1;
            length += spinAnswer.Premdocenaarray == null || spinAnswer.Premdocenaarray.Length == 0 ? 0 : 1;
            length += spinAnswer.Premlineaarray == null || spinAnswer.Premlineaarray.Length == 0 ? 0 : 1;
            length += spinAnswer.Premmayormenorarray == null || spinAnswer.Premmayormenorarray.Length == 0 ? 0 : 1;
            length += spinAnswer.Premparimpararray == null || spinAnswer.Premparimpararray.Length == 0 ? 0 : 1;
            length += spinAnswer.Premplenoarray == null || spinAnswer.Premplenoarray.Length == 0 ? 0 : 1;
            length += spinAnswer.Premsemiplenoarray == null || spinAnswer.Premsemiplenoarray.Length == 0 ? 0 : 1;
            length += spinAnswer.PremtransversalCero == null || spinAnswer.PremtransversalCero.Length == 0 ? 0 : 1;

            if (spinAnswer.PerdCallearray == null)
            {
                length = 0;
            }

            return length > 0;
        }

        private void OnBetChanged(int amount)
        {
            //////DebugWithDate.Log("Bet changed: " + amount);

            betText.text = ConvertAmountToMoney(betController.CurrentBetAmount);
            betTextWebGL.text = ConvertAmountToMoney(betController.CurrentBetAmount);

            if (amount > 0)
            {
                chipsSoundFX.OnBet();
            }
            else
            {
                chipsSoundFX.OnBetFailed();
            }

            UpdateButtonsState();
        }

        private void OnLosingChipsAnimationEnded()
        {
            losingChipsAnimationEnded = true;
        }

        private void OnWinningChipsAnimationEnded()
        {
            winningChipsAnimationEnded = true;
        }

        public void OnSuspendTime()
        {
            Time.timeScale = 0.0f;
        }

        public void OnResumeTime()
        {
            Time.timeScale = 1.0f;
        }

        public void OnCloseAfterSpecifiedTime()
        {
            independentTimeCountdown = true;
            startingIndependentTime = Time.realtimeSinceStartup;
        }

        #endregion

        #region Javascript WebGL Events

        public void OnInitWeb(string data)
        {
            if (data == null)
            {
                ////DebugWithDate.LogWarning("OnInitWeb: el mensaje desde javascript es null");
                return;
            }

            string idsesionusuario = string.Empty; // "16161616";
            string URL = string.Empty; // "http://200.42.89.158:59165/RuletaWebService/rws";

            string[] dataParts = data.Split(new string[] { "#" }, System.StringSplitOptions.RemoveEmptyEntries);

            if (data.Length >= 2)
            {
                idsesionusuario = dataParts[0];
                URL = dataParts[1];

                if (idsesionusuario.Length > 0 && URL.Length > 0)
                {
                    serverLayer.SetUp(URL, "0", idsesionusuario);
                    ChangeState(GameStates.Init);
                }
                else
                {
                    ////DebugWithDate.LogWarning("OnInitWeb: no hay usuario o URL");
                }
            }
            else
            {
                ////DebugWithDate.LogWarning("OnInitWeb: no hay usuario o URL");
            }

            ////DebugWithDate.Log("OnInitWeb: [" + URL + "] [" + idsesionusuario + "]");
        }

        #endregion

        #region Intercomms

        /*private void ConnectToMonedero()
        {
            monederoURI = WorkingData.GetData("URL Monedero", "127.0.0.1");
            monederoPort = int.Parse(WorkingData.GetData("Port Monedero", (12288).ToString()));

            if (connectToMonederoThread == null)
            {
                connectToMonederoThread = new Thread(ConnectToMonederoThread);
                connectToMonederoThread.Start();
            }
        }

        private void ConnectToMonederoThread()
        {
            socketComms.StartAsClient(monederoURI, monederoPort);

            Thread temp = connectToMonederoThread;
            connectToMonederoThread = null;
            temp.Abort();
        }*/

        /*private void SendSocketMessage(string message)
        {
            socketComms.SendMessage(message);
        }

        private void OnMessageReceived(string message)
        {
            errorMonederoStatus = false;

            flagTimeScaleOn = true;

            ////DebugWithDate.Log("Mensaje de socket recibido: " + message);

            string[] messages = message.Split(new string[] { "#" }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string individualMessage in messages)
            {
                string[] messageParts = individualMessage.Split(new string[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);

                if (messageParts[0].StartsWith("TERMINAL"))
                {
                    if (terminal.Equals(string.Empty))
                    {
                        terminal = messageParts[1];
                        serverLayer.SetUp(URI, terminal, "");
                        ChangeState(GameStates.Init);
                    }
                }
                else if (messageParts[0].StartsWith("CREDITO"))
                {
                    if (gameState != GameStates.Spin || (gameState == GameStates.FakeSpin))
                    {
                        ////DebugWithDate.Log("Solicitando créditos al servidor");

                        serverLayer.CallCredits();
                    }
                    else
                    {
                        ////DebugWithDate.Log("Ruleta girando, no se piden créditos");
                    }

                }
                else if (messageParts[0].StartsWith("CONNECTED"))
                {
                    if (gameState == GameStates.WaitingLocalDataAcquisition)
                    {
                        ChangeState(GameStates.LocalDataAcquisition);
                    }
                }
                else if (messageParts[0].StartsWith("IGNORE"))
                {

                }
                else
                {
                    flagUnknownMessage = true;
                    unknownMessage = message;
                }
            }
        }*/

        private void OnError(string message)
        {
            flagTimeScaleOff = true;
            ////DebugWithDate.LogError(message);

            retryMonedero = true;
        }

        #endregion
    }
}