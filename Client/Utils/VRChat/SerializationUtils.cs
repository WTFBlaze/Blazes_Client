using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Utils.VRChat
{
    internal static class SerializationUtils
    {
        internal static byte[] ToByteArray(Il2CppSystem.Object obj)
        {
            if (obj == null) return null;
            var bf = new Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var ms = new Il2CppSystem.IO.MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        internal static byte[] ToByteArray(object obj)
        {
            if (obj == null) return null;
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var ms = new System.IO.MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        internal static T FromByteArray<T>(byte[] data)
        {
            if (data == null) return default(T);
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using var ms = new System.IO.MemoryStream(data);
            var obj = bf.Deserialize(ms);
            return (T)obj;
        }

        internal static T IL2CPPFromByteArray<T>(byte[] data)
        {
            if (data == null) return default(T);
            var bf = new Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var ms = new Il2CppSystem.IO.MemoryStream(data);
            object obj = bf.Deserialize(ms);
            return (T)obj;
        }

        internal static T FromIL2CPPToManaged<T>(Il2CppSystem.Object obj)
        {
            return FromByteArray<T>(ToByteArray(obj));
        }

        internal static T FromManagedToIL2CPP<T>(object obj)
        {
            return IL2CPPFromByteArray<T>(ToByteArray(obj));
        }

        internal static object[] FromIL2CPPArrayToManagedArray(Il2CppSystem.Object[] obj)
        {
            var Parameters = new object[obj.Length];
            for (var i = 0; i < obj.Length; i++)
                if (obj[i].GetIl2CppType().Attributes == Il2CppSystem.Reflection.TypeAttributes.Serializable)
                    Parameters[i] = FromIL2CPPToManaged<object>(obj[i]);
                else
                    Parameters[i] = obj[i];
            return Parameters;
        }

        internal static Il2CppSystem.Object[] FromManagedArrayToIL2CPPArray(object[] obj)
        {
            Il2CppSystem.Object[] Parameters = new Il2CppSystem.Object[obj.Length];
            for (var i = 0; i < obj.Length; i++)
            {
                if (obj[i].GetType().Attributes == System.Reflection.TypeAttributes.Serializable)
                    Parameters[i] = FromManagedToIL2CPP<Il2CppSystem.Object>(obj[i]);
                else
                    Parameters[i] = (Il2CppSystem.Object)obj[i];
            }
            return Parameters;
        }
    }
}
