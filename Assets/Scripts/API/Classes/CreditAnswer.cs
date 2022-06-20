namespace SteinCo.Ivisa.RoulettePremium.API.Classes
{
	public struct CreditAnswer
	{
		/*
		Parámetros a enviar
		- ID de terminal
		_IDSesión
		_KeyOut
		Parámetros a recibir
		- Crédito
		_@mensajeError varchar(255) output
		*/

		/*
		{
			"Credito": 0,
			"mensajeError": "Error en la secuencia de KeyOut",
			"proxKeyOut": "AA925F58-D5F2-466E-9606-0F87CA36F640"
		}
		*/

		public int Credito;
		public string mensajeError;
		public string msgPremioMayor;
		public string proxKeyOut;
	}
}