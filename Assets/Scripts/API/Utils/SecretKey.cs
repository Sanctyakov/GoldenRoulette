using System.Security.Cryptography;
using System.Text;

namespace SteinCo.Ivisa.RoulettePremium.API.Utils
{
	public static class SecretKey
	{
		public static string CalculateSecretKey(string texto)
		{
			using (var md5 = MD5.Create())
			{
				byte[] textToHash = Encoding.Default.GetBytes(texto);
				byte[] result = md5.ComputeHash(textToHash);
				return System.BitConverter.ToString(result).Replace("-", string.Empty).ToLower();
			}
		}
	}
}