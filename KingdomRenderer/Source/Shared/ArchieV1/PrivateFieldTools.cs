using System;
using System.Reflection;
using static KingdomRenderer.Shared.ArchieV1.ULogger;

namespace KingdomRenderer.Shared.ArchieV1
{
    public static class PrivateFieldTools
    {
        /// <summary>
        /// Get value of a private field from an Instance
        /// </summary>
        /// <param name="instance">The instance that contains the private field</param>
        /// <param name="fieldName">The private field name</param>
        /// <param name="fieldIsStatic">Is the field static</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when fieldName is not found in instance</exception>
        public static object GetPrivateField(object instance, string fieldName, bool fieldIsStatic = false)
        {
            string exceptionString =
                $"{fieldName} does not correspond to a private {(fieldIsStatic ? "static" : "instance")} field in {instance}";
            object result;
            try
            {
                Type type = instance.GetType();

                FieldInfo fieldInfo = fieldIsStatic ? type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic) : type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

                if (fieldInfo == null) throw new ArgumentException(exceptionString);

                result = fieldInfo.GetValue(instance);
            }
            catch (Exception e)
            {
                ULog(e);
                throw new ArgumentException(exceptionString);
            }

            return result;
        }

        /// <summary>
        /// Get value of a private field from Static Class
        /// </summary>
        /// <param name="type">The Class that contains the private field</param>
        /// <param name="fieldName">The private field name</param>
        /// <param name="fieldIsStatic">Is the field static</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when fieldName is not found in type</exception>
        public static object GetPrivateField(Type type, string fieldName, bool fieldIsStatic = false)
        {
            string exceptionString =
                $"{fieldName} does not correspond to a private {(fieldIsStatic ? "static" : "instance")} field in {type}";
            object result;

            try
            {
                FieldInfo fieldInfo = fieldIsStatic ? type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic) : type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

                if (fieldInfo == null) throw new ArgumentException(exceptionString);

                result = fieldInfo.GetValue(type);
            }
            catch (Exception e)
            {
                ULog(e);
                throw new ArgumentException(exceptionString);
            }

            return result;
        }

        /// <summary>
        /// Sets the value of a private field using reflection.
        /// In normal programming this should not be used. When modding it is sometimes required though.
        /// </summary>
        /// <param name="instance">The object containing the private field</param>
        /// <param name="fieldName">The name of the private field</param>
        /// <param name="newValue">The new value the field will hold</param>
        public static void SetPrivateField(object instance, string fieldName, object newValue)
        {
            Type type = instance.GetType();
            PropertyInfo cellDataProperty = type.GetProperty(fieldName);

            // Set fieldName's value to the NewValue
            cellDataProperty?.SetValue(type, newValue, null);
        }
    }
}