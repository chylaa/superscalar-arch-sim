using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace superscalar_arch_sim.Utilis
{
    internal static class StaticClassSerializer
    {
        const BindingFlags SerializerBindingFlags = (BindingFlags.Static | BindingFlags.Public);
        public static void Save(Type @static, string filename)
        {
            FieldInfo[] fields = @static.GetFields(SerializerBindingFlags);
            PropertyInfo[] properties = @static.GetProperties(SerializerBindingFlags);

            object[,] namevals = new object[fields.Length + properties.Length, 2];
            for (int i = 0; i < fields.Length; i++)
            {
                namevals[i, 0] = fields[i].Name;
                namevals[i, 1] = fields[i].GetValue(null);
            }
            int offset = fields.Length;
            for (int i = offset; i < (offset + properties.Length); i++)
            {
                namevals[i, 0] = properties[i - offset].Name;
                namevals[i, 1] = properties[i - offset].GetValue(null);
            }
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                new BinaryFormatter().Serialize(fs, namevals);
            }
        }

        public static bool Load(Type @static, string filename)
        {
            FieldInfo[] fields = @static.GetFields(SerializerBindingFlags);
            PropertyInfo[] properties = @static.GetProperties(SerializerBindingFlags);

            object[,] namevals;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                namevals = (new BinaryFormatter().Deserialize(fs) as object[,]);
            }

            int namevalslen = namevals.GetLength(0);
            if (namevalslen != (properties.Length + fields.Length))
                Console.WriteLine($"Warning! Non compatibile settings file: {namevalslen}|{(properties.Length + fields.Length)}");

            foreach (FieldInfo field in fields)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    if (field.Name.Equals(namevals[i, 0].ToString()) && false == field.IsLiteral)
                    {
                        field.SetValue(null, namevals[i, 1]);
                    }
                }
            }
            foreach (PropertyInfo property in properties)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    if (property.Name.Equals(namevals[i, 0].ToString()) && property.CanWrite)
                    {
                        property.SetValue(null, namevals[i, 1]);
                    }
                }
            }
            return true;
        }
    }
}
