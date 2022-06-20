using UnityEngine;
using UnityEngine.UI;
using SteinCo.Utils;
using SteinCo.Comms;
using SteinCo.Ivisa.RoulettePremium.Game;
using SteinCo.Ivisa.RoulettePremium.API.Classes;

namespace SteinCo.Ivisa.RoulettePremium.API.Utils
{
    public class ServerLayer : MonoBehaviour, IServerLayer
    {
        public CommsContentManager ccm;
        public GameObject[] deactivateOnError;
        public GameObject commsError;
        public GameObject retryButtonCollider; //
        public GameObject resetButtonCollider; //
        public GameObject resetButtonCollider2; //
        public GameObject retryButton;
        public Text commsErrorText;
        public Version version;

        public event Credits OnCredits;
        public event Init OnInit;
        public event InitWeb OnInitWeb;
        public event Spin OnSpin;
        public event EndSession OnSessionEnd;
        public event CheckPassword OnCheckPassword;

        public string secretKey;

        private string URI = string.Empty;
        private string terminal = "0";

        private string sessionID;
        private string keyOut;

        private int maxRetries = 3;
        private float creditsRetryDelay = 5.0f * 60.0f;

        private bool isInitWeb;

        /*private enum MessageTypes
		{
			Init,
			InitWeb,
			Credits,
			Spin,
			EndSession,
			CheckPassword,
			PreviousPlay,
		}

		private MessageTypes messageType;*/

        void Awake()
        {
            ccm.OnRequestFailed += OnRequestFailed;
            ccm.OnRequestSuccess += OnRequestSuccess;
        }

        void OnDestroy()
        {
            ccm.OnRequestFailed -= OnRequestFailed;
            ccm.OnRequestSuccess -= OnRequestSuccess;
        }

        void Start()
        {
            maxRetries = int.Parse(WorkingData.GetData("Maximum Retries", (3).ToString()));
            creditsRetryDelay = float.Parse(WorkingData.GetData("Credits Ping Delay", (5.0f * 60.0f).ToString()));
        }

        public void SetUp(string URI, string terminal, string sessionID)
        {
            this.URI = URI;
            this.terminal = terminal;
            this.sessionID = sessionID;
        }


        /*
		 //Constante SecretKey= "c96e651946818e0787d6296f69549fe1"

Ejemplo:

// sk: Constante SecretKey
// Fecha/Hora actual del Servidor: "30/05/2016 13:45:26"
// Comando: "ini"
// textHash = sk + "." + dateHash + "." + command;
string skActual = CalculateSecretKey("c96e651946818e0787d6296f69549fe1.2016.05.30.13.ini");
// skActual= "087a1ced761d3bf36a1358046e188996"
		 * */

        private string CalculateHash(string command)
        {
            return SecretKey.CalculateSecretKey(string.Join(".", new string[] { secretKey, System.DateTime.Now.ToString("yyyy.MM.dd.HH"), command }));
        }

        public void CallInit()
        {
            //messageType = MessageTypes.Init;

            //https://200.42.89.158:59165/RuletaWebService/isb?command=ini&idterminal=16161616
            //http://200.42.89.158:59165//RuletaWebService/rws?command=ini&idTerminal=16161616&version=
            FormContents form = new FormContents(URI, 0);
            form.Add("command", "ini");
            form.Add("idterminal", terminal);
            form.Add("version", version.gameIdExe + version.version);
            form.Add("secretKey", CalculateHash("ini"));

            ccm.AddRequest(form);
        }

        public void CallInitWeb()
        {
            //messageType = MessageTypes.InitWeb;

            isInitWeb = true;

            //https://200.42.89.158:59165/RuletaWebService/isb?command=ini&idterminal=16161616
            //http://200.42.89.158:59165//RuletaWebService/rws?command=ini&idTerminal=16161616&version=
            //http://200.42.89.158:59165/ruletawebservice/rws?command=inicioSesionDomiciliario&idsesionusuario=007300030051000056&version=
            FormContents form = new FormContents(URI, 0);
            form.Add("command", "inicioSesionDomiciliario");
            //form.Add("idterminal", terminal);
            form.Add("idsesionusuario", sessionID);
            form.Add("version", version.gameIdWebGL + version.version);
            form.Add("secretKey", CalculateHash("inicioSesionDomiciliario"));

            ccm.AddRequest(form);
        }

        public void CallCredits()
        {
            //messageType = MessageTypes.Credits;

            //https://200.42.89.158:59165/RuletaWebService/isb?command=cre&idTerminal=16161616&idsession=000300000001000006&keyOut=AA925F58-D5F2-466E-9606-0F87CA36F640

            FormContents form = new FormContents(URI, 0);
            form.Add("command", "cre");
            form.Add("idTerminal", terminal);
            form.Add("idsession", sessionID);
            form.Add("keyOut", keyOut);
            form.Add("secretKey", CalculateHash("cre"));

            ccm.AddRequest(form);
        }

        public void CallEndSession()
        {
            //messageType = MessageTypes.EndSession;

            //https://200.42.89.158:59165//RuletaWebService/rws?command=closeSession&idTerminal=16161616&idsession=001000639000000548

            FormContents form = new FormContents(URI, 0);
            form.Add("command", "closeSession");
            form.Add("idTerminal", terminal);
            form.Add("idsession", sessionID);
            form.Add("secretKey", CalculateHash("closeSession"));

            ccm.AddRequest(form);
        }

        private int roundID;

        public void CallSpin(BetController betController)
        {
            //messageType = MessageTypes.Spin;

            //https://200.42.89.158:59165/RuletaWebService/rws?command=apu&idTerminal=16161616&idsession=001000639000000548&PlenoArray=15%23%231000

            FormContents form = new FormContents(URI, 0);
            form.Add("command", "apu");
            form.Add("idTerminal", terminal);
            form.Add("idsession", sessionID);
            form.Add("keyOut", keyOut);

            form.Add("PlenoArray", betController.GetBets(BetController.BetTypes.Pleno));
            form.Add("SemiplenoArray", betController.GetBets(BetController.BetTypes.Semipleno));
            form.Add("CalleArray", betController.GetBets(BetController.BetTypes.Calle));
            form.Add("CuadroArray", betController.GetBets(BetController.BetTypes.Cuadrado));
            form.Add("LineaArray", betController.GetBets(BetController.BetTypes.Linea));
            form.Add("Color", betController.GetBets(BetController.BetTypes.Color));
            form.Add("ParImparArray", betController.GetBets(BetController.BetTypes.ParImpar));
            form.Add("MayorMenorArray", betController.GetBets(BetController.BetTypes.MenorMayor));
            form.Add("DocenaArray", betController.GetBets(BetController.BetTypes.Docena));
            form.Add("ColumnaArray", betController.GetBets(BetController.BetTypes.Columna));
            form.Add("TransversalCero", betController.GetBets(BetController.BetTypes.TransversalConCero));

            /*
			 * roundID varchar(38),    --'yyyymmdd' + idsession + 'nnnnnn':secuencia de apuesta del día

nnnnnn es el número de secuencia de la apuesta desde que arrancó la sesión (es decir, el juego)

Al reintentar la apuesta, hay que mandar el mismo identificador.

Cuando se finaliza la apuesta, ya sea en forma exitosa o si dio error, se debe incrementar el campo nnnnnn
para que en la siguiente llamada este campo venga con otro valor.*/

            form.Add("roundID", System.DateTime.Today.ToString("yyyyMMdd") + sessionID + roundID.ToString("000000"));
            // roundID=20160712000000060810000700000000&secretKey=a74e6ec01d74fe940b29e405b63312c9&
            form.Add("secretKey", CalculateHash("apu"));

            roundID++;
            ccm.AddRequest(form);
        }

        public void CallCheckPassword(string password)
        {
            //rws? command = validarclaveadmin & terminal = &idsession = &clave =

            //messageType = MessageTypes.CheckPassword;

            FormContents form = new FormContents(URI, 0);
            form.Add("command", "validarclaveadmin");
            form.Add("terminal", terminal);
            form.Add("idsession", sessionID);
            form.Add("keyOut", keyOut);
            form.Add("clave", password);
            form.Add("secretKey", CalculateHash("validarclaveadmin"));

            ccm.AddRequest(form);
        }

        public void CallPreviousPlay(int IDApuestaData, int offset)
        {
            /*
			
rws?command=DatosApu&idTerminal=&idsession=&idApuesta=&sigID=&keyOut=

idTerminal: Terminal que envía el pedido
idsession: Sesion iniciada en la terminal
idApuesta= Id de la apuesta que se pide. Si es = 0 devuelve el ID y los datos de la ultima apuesta de la terminal.
sigID: Se tiene en cuenta en caso que idApuesta <> 0.
        Si es -1 se devuelve el ID y los datos correspondientes a la apuesta anterior al ID recibido.
        Si es 0 se devuelve el ID y los datos correspondientes a la apuesta con el ID recibido.
        Si es 1 se devuelve el ID y los datos correspondientes a la apuesta posterior al ID recibido.
keyOut: Es el key recibido en la última transaccion realizada.

El procedimiento devuelve los siguientes datos:

{"Plenoarray":"","Semiplenoarray":"","Callearray":"","Color":"","Columnaarray":"","Cuadroarray":"","Docenaarray":"","Lineaarray":"","Parimpararray":"","TransversalCero":"",
"mayormenorarray":"","PerdCallearray":"","PerdColor":"","PerdColumnaarray":"","PerdCuadroarray":"","PerdDocenaarray":"","PerdLineaarray":"","PerdParimpararray":"",
"PerdPlenoarray":"","PerdSemiplenoarray":"","PerdTransversalCero":"","Perdmayormenorarray":"","PremColor":"","Premcallearray":"","Premcolumnaarray":"",
"Premcuadroarray":"","Premdocenaarray":"","Premlineaarray":"","Premmayormenorarray":"","Premparimpararray":"","Premplenoarray":"","Premsemiplenoarray":"",
"PremtransversalCero":"","numero":"","premiosTotalInt":"","proxKeyOut":"","mensajeError":""}*/

            //messageType = MessageTypes.PreviousPlay;

            FormContents form = new FormContents(URI, 0);
            form.Add("command", "DatosApu");
            form.Add("idTerminal", terminal);
            form.Add("idsession", sessionID);
            form.Add("keyOut", keyOut);
            form.Add("idApuesta", IDApuestaData.ToString());
            form.Add("idProxApuesta", offset.ToString());
            form.Add("secretKey", CalculateHash("DatosApu"));

            ccm.AddRequest(form);
        }

        private int retries = 0;

        private FormContents retryForm;

        private void OnRequestFailed(int id, string data, FormContents form)
        {
            ////DebugWithDate.Log("Error de conexión " + data + ". Mostrando pantalla de error. Intento: " + retries);

            foreach (GameObject go in deactivateOnError)
            {
                go.SetActive(false);
            }

            commsError.SetActive(true);
            retryButtonCollider.SetActive(true); //
            resetButtonCollider.SetActive(true); //
            commsErrorText.text = "ERROR DE CONEXIÓN\n" + data;
            retryForm = form;

            retries++;

            if (retries > maxRetries)
            {
                ////DebugWithDate.Log("Error de conexión " + data + ": máximo de reintentos excedido. Mostrando sólo opción Reiniciar.");

                retryButton.SetActive(false);
                retryButtonCollider.SetActive(false); //
                resetButtonCollider.SetActive(false); //
                resetButtonCollider2.SetActive(true); //
                InvokeRepeating("PingRetry", creditsRetryDelay, creditsRetryDelay);
            }
        }

        private void PingRetry()
        {
            commsError.SetActive(false);
            retryButtonCollider.SetActive(false); //
            resetButtonCollider.SetActive(false); //
            CallCredits();
        }

        public void OnRetry()
        {
            commsError.SetActive(false);
            retryButtonCollider.SetActive(false); //
            resetButtonCollider.SetActive(false); //
            ccm.AddRequest(retryForm);
        }

        private void OnRequestSuccess(int id, string data)
        {
            //retryButton.SetActive(true); //
            //retryButtonCollider.SetActive(true); //

            retries = 0;
            CancelInvoke("CallCredits");

            int messageNum = 0;

            //Init 1
            //InitWeb 2
            //Credits 3
            //Spin 4
            //EndSession 5
            //CheckPassword 6
            //PreviousPlay 7

            if (data.StartsWith("{\"Credito\""))
            {
                if (data.Contains("\"Fichas\""))
                {
                    if (!isInitWeb)
                    {
                        messageNum = 1;
                    }
                    else
                    {
                        messageNum = 2;
                    }
                }
                else
                {
                    messageNum = 3;
                }
            }
            else if (data.StartsWith("{\"PerdCallearray\""))
            {
                messageNum = 4;
            }
            else if (data.StartsWith("{\"mensajeError\""))
            {
                if (!data.Contains("\"proxKeyOut\""))
                {
                    messageNum = 5;
                }
                else
                {
                    messageNum = 6;
                }
            }
            else if (data.StartsWith("{\"Color\""))
            {
                messageNum = 7;
            }
            else
            {
                messageNum = 0;
            }

            ////DebugWithDate.Log(id + " Server answer: " + data);

            //switch (messageType)
            switch (messageNum)
            {
                //case MessageTypes.Init:
                case 1:
                    InitAnswer initAnswer = SerializationHelper.Deserialize<InitAnswer>(data);
                    sessionID = initAnswer.idsession;
                    keyOut = initAnswer.keyOut;
                    OnInit.Invoke(initAnswer);
                    break;
                //case MessageTypes.InitWeb:
                case 2:
                    InitAnswerWeb initAnswerWeb = SerializationHelper.Deserialize<InitAnswerWeb>(data);
                    sessionID = initAnswerWeb.idsession;
                    keyOut = initAnswerWeb.keyOut;
                    OnInitWeb.Invoke(initAnswerWeb);
                    break;
                //case MessageTypes.Credits:
                case 3:
                    CreditAnswer creditAnswer = SerializationHelper.Deserialize<CreditAnswer>(data);
                    keyOut = creditAnswer.proxKeyOut;
                    OnCredits.Invoke(creditAnswer);
                    break;
                //case MessageTypes.Spin:
                case 4:
                    SpinAnswer spinAnswer = SerializationHelper.Deserialize<SpinAnswer>(data);
                    keyOut = spinAnswer.proxKeyOut;
                    ////DebugWithDate.Log("Invocando evento de giro de ruleta. Número recibido: " + spinAnswer.numero);
                    OnSpin.Invoke(spinAnswer);
                    break;
                //case MessageTypes.EndSession:
                case 5:
                    EndSessionAnswer endSessionAnswer = new EndSessionAnswer();
                    /*EndSessionAnswer endSessionAnswer = SerializationHelper.Deserialize<EndSessionAnswer>(data);
					if (endSessionAnswer.mensajeError.Equals(string.Empty))
					{
						roundID = 0;
					}
					*/
                    roundID = 0;
                    OnSessionEnd.Invoke(endSessionAnswer);
                    break;
                //case MessageTypes.CheckPassword:
                case 6:
                    CheckPasswordAnswer checkPasswordAnswer = SerializationHelper.Deserialize<CheckPasswordAnswer>(data);
                    keyOut = checkPasswordAnswer.proxKeyOut;
                    OnCheckPassword.Invoke(checkPasswordAnswer);
                    break;
                //case MessageTypes.PreviousPlay:
                case 7:
                    break;
                case 0:
                    EndSessionAnswer endSessionAnswer2 = new EndSessionAnswer();
                    roundID = 0;
                    OnSessionEnd.Invoke(endSessionAnswer2);
                    break;
                default:
                    EndSessionAnswer endSessionAnswer3 = new EndSessionAnswer();
                    roundID = 0;
                    OnSessionEnd.Invoke(endSessionAnswer3);
                    break;
            }
        }
    }
}