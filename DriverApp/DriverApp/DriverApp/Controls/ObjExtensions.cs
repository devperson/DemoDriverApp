using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DriverApp
{
    public static class ObjExtensions
    {
        public static T DeepClone<T>(this T oSource)
        {
            T oClone;

            DataContractSerializer dcs = new DataContractSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream())
            {
                dcs.WriteObject(ms, oSource);
                ms.Position = 0;
                oClone = (T)dcs.ReadObject(ms);

                //this line for test purposes.
                //ms.Position = 0;
                //string result = new StreamReader(ms).ReadToEnd();                
            }
            return oClone;
        }
    }
}
