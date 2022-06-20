using FullSerializer;

namespace SteinCo.Utils
{
	public static class SerializationHelper
	{
		public static string Serialize<T>(T value, bool compressed = true)
		{
			fsData data;
			(new fsSerializer()).TrySerialize(typeof(T), value, out data).AssertSuccessWithoutWarnings();

			string res = string.Empty;

			if (compressed)
			{
				res = fsJsonPrinter.CompressedJson(data);
			}
			else
			{
				res = fsJsonPrinter.PrettyJson(data);
			}

			return res;
		}

		public static T Deserialize<T>(string serializedState)
		{
			object deserialized = null;
			fsData data = null;

			fsResult result = fsJsonParser.Parse(serializedState, out data);

			if (!result.Failed)
			{
				(new fsSerializer()).TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();
			}

			return (T)deserialized;
		}
	}
}