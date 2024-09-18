using EwEMSPLink;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace MEL.Converters;

    class cPressureConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(cPressure);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            if (jsonObject.ContainsKey("bIsEcological"))
            {
                return new cFishingEcoPressure(
                    jsonObject["Name"].ToObject<string>(),
                    jsonObject["bIsEcological"].ToObject<bool>()
                );
            }

            if (jsonObject.ContainsKey("EffortScalar"))
            {
                return new cFishingEffortPressure(
                    jsonObject["Name"].ToObject<string>(),
                    jsonObject["EffortScalar"].ToObject<float>()
                );
            }

            JArray valuesArray = (JArray)jsonObject["Grid"]["Cell"];

            int rows = valuesArray.Count;
            int cols = valuesArray[0].Count();
            double[,] doubleArray = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    doubleArray[i, j] = (double)valuesArray[i][j];
                }
            }

            return new cEnvironmentalPressure(
                jsonObject["Name"].ToObject<string>(),
                jsonObject["Grid"]["Width"].ToObject<int>(),
                jsonObject["Grid"]["Height"].ToObject<int>(),
                doubleArray
            );
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
