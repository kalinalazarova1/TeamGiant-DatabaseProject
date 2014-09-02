//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using VehicleVendor.Data;
//using Newtonsoft.Json;
////using MongoDB.Bson;
//using System.IO;

//namespace VehicleVendor.Reports.JsonReportSQLServerGenerator
//{
//    public class ObjectIdConverter
//    {
//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            serializer.Serialize(writer, value.ToString());

//        }

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            throw new NotImplementedException();
//        }

//        public override bool CanConvert(Type objectType)
//        {
//            return typeof(ObjectId).IsAssignableFrom(objectType);
//            //return true;
//        }
//    }
//}
