using SteinCo.Ivisa.RoulettePremium.Game;
using SteinCo.Ivisa.RoulettePremium.API.Classes;

namespace SteinCo.Ivisa.RoulettePremium.API
{
	public delegate void Init(InitAnswer init);
	public delegate void InitWeb(InitAnswerWeb init);
	public delegate void Credits(CreditAnswer credit);
	public delegate void Spin(SpinAnswer spin);
	public delegate void EndSession(EndSessionAnswer endSession);
	public delegate void CheckPassword(CheckPasswordAnswer checkPasswordAnswer);

	public interface IServerLayer
	{
		void SetUp(string URI, string terminal, string sessionID);
		void CallInit();
		void CallInitWeb();
		void CallCredits();
		void CallSpin(BetController betController);
		void CallEndSession();
		void CallCheckPassword(string password);
		void CallPreviousPlay(int IDApuestaData, int offset);

		event Init OnInit;
		event InitWeb OnInitWeb;
		event Credits OnCredits;
		event Spin OnSpin;
		event EndSession OnSessionEnd;
		event CheckPassword OnCheckPassword;
	}
}
