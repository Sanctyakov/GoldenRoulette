namespace SteinCo.Ivisa.RoulettePremium.API.Classes
{
	public struct InitAnswer
	{
		/*
		Parámetros a enviar
	- ID de terminal

	Parámetros a recibir
	- Crédito
	- Lista de valores para fichas(la cantidad indica cuántas van a aparecer)
	- Tiempo máximo(con 0 para infinito)
	- Un cero o doble cero
	- Apuesta máxima(ídem)
	- (otros que vayan apareciendo)
	_IDSesión(identifica la sesión para la máquina en forma unívoca)
	_KeyOut
	_@mensajeError varchar(255) output
	*/
		/* {
			"Credito": 17124,
			"Fichas": [
				"1",
				"2",
				"3"
				],
			"apMaximaChance": 2000,
			"apMaximaSimple": 500,
			"apMinimaChance": 200,
			"apMinimaSimple": 10,
			"apuestaMaxima": 20000,
			"apuestaMinima": 100,
			"cero": "0",
			"idsession": "001000639000000548",
			"keyOut": "FA387DE0-FB15-44D1-A101-5E0EDF0FEE60",
			"mensajeError": "",
			"timeout": 300
		}
		*/

		public int Credito;
		public int[] Fichas;
		public string cero;
		public int apMaximaChance;
		public int apMaximaSimple;
		public int apMinimaChance;
		public int apMinimaSimple;
		public int apuestaMaxima;
		public int apuestaMinima;
		public string idsession;
		public string keyOut;
		public string mensajeError;
		public int timeout;
	}
}