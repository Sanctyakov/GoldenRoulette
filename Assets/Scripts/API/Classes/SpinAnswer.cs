namespace SteinCo.Ivisa.RoulettePremium.API.Classes
{
	public struct SpinAnswer
	{
		/*
			@idTerminal int,
		@idsession char(18) ,
		@plenoarray varchar(255) ,
		@semiplenoarray varchar(255) ,
		@callearray varchar(255) ,
		@cuadroarray varchar(255) ,
		@lineaarray varchar(255) ,
		@color varchar(255),
		@parimpararray varchar(255) ,
		@mayormenorarray varchar(255) ,
		@docenaarray varchar(255) ,
		@columnaarray varchar(255) ,
		@transversalCero varchar(255),


		@keyOut varchar(40) ,
		@saldo money output,
		@premiosTotal money output,

		@Premplenoarray varchar(255) output ,
		@Pemsemiplenoarray varchar(255) output ,
		@Pemcallearray varchar(255) output ,
		@Pemcuadroarray varchar(255) output ,
		@Pemlineaarray varchar(255) output ,
		@PemColor varchar(255) output,
		@Pemparimpararray varchar(255) output,
		@Pemmayormenorarray varchar(255) output ,
		@Pemdocenaarray varchar(255) output ,
		@Pemcolumnaarray varchar(255) output ,
		@PemtransversalCero varchar(255) output,


			@PerdPlenoarray varchar(255),
			@PerdSemiplenoarray varchar(255),
			@PerdCallearray varchar(255),
			@PerdCuadroaarray varchar(255),
			@PerdLineaarray varchar(255),
			@PerdColor varchar(255),
			@PerdParimpararray varchar(1000),
			@PerdMayormenorarray varchar(1000),
			@Perddocenaarray varchar(255),
			@PerdColumnaarray varchar(255),
			@PerdtransversalCero varchar(255),

		@numero int output,
		@mensajeError varchar(255) output
			*/

		/*
		 *
		 {
	"PerdCallearray": "",
	"PerdColor": "02__500",
	"PerdColumnaarray": "",
	"PerdCuadroarray": "",
	"PerdDocenaarray": "",
	"PerdLineaarray": "",
	"PerdParimpararray": "",
	"PerdPlenoarray": "",
	"PerdSemiplenoarray": "",
	"PerdTransversalCero": "",
	"Perdmayormenorarray": "",
	"PremColor": "01__1000",
	"Premcallearray": "",
	"Premcolumnaarray": "",
	"Premcuadroarray": "",
	"Premdocenaarray": "",
	"Premlineaarray": "",
	"Premmayormenorarray": "",
	"Premparimpararray": "",
	"Premplenoarray": "",
	"Premsemiplenoarray": "",
	"PremtransversalCero": "",
	"mensajeError": "",
	"msgPremioMayor": "",
	"numero": 3,
	"premiosTotalInt": 1000,
	"proxKeyOut": "8797B244-EDFB-4A09-A4C5-3108855CACC2",
	"saldoInt": 1712411
}
 
			 */

		public string proxKeyOut;
		public int saldoInt;
		public int premiosTotalInt;
		public int numero;

		public string Premplenoarray;
		public string Premsemiplenoarray;
		public string Premcallearray;
		public string Premcuadroarray;
		public string Premlineaarray;
		public string Premparimpararray;
		public string Premmayormenorarray;
		public string Premdocenaarray;
		public string Premcolumnaarray;
		public string PremColor;
		public string PremtransversalCero;

		public string PerdPlenoarray;
		public string PerdSemiplenoarray;
		public string PerdCallearray;
		public string PerdCuadroarray;
		public string PerdLineaarray;
		public string PerdParimpararray;
		public string Perdmayormenorarray;
		public string PerdDocenaarray;
		public string PerdColumnaarray;
		public string PerdColor;
		public string PerdTransversalCero;

		public string msgPremioMayor;

		public string mensajeError;
	}
}