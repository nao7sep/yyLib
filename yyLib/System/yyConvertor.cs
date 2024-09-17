namespace yyLib
{
    public static class yyConvertor
    {
        public static string EnumToString <T> (T value) where T: struct, Enum
        {
            if (Enum.IsDefined (typeof (T), value) == false)
                throw new yyArgumentException ($"Invalid {typeof (T).Name} value: {value}");

            return value.ToString ();
        }

        public static T StringToEnum <T> (string str, bool ignoreCase = true) where T: struct, Enum
        {
            if (Enum.TryParse <T> (str, ignoreCase: ignoreCase, out T xValue) == false)
                throw new yyArgumentException ($"Invalid {typeof (T).Name} string: {str}");

            return xValue;
        }

        public static bool TryStringToEnum <T> (string str, out T value, bool ignoreCase = true) where T: struct, Enum =>
            Enum.TryParse <T> (str, ignoreCase: ignoreCase, out value);

        public static T ValueToEnum <T> (object value) where T: struct, Enum
        {
            if (Enum.IsDefined (typeof (T), value) == false)
                throw new yyArgumentException ($"Invalid {typeof (T).Name} value: {value}");

            return (T) value;
        }

        public static bool TryValueToEnum <T> (object value, out T enum_value) where T: struct, Enum
        {
            if (Enum.IsDefined (typeof (T), value) == false)
            {
                enum_value = default;
                return false;
            }

            enum_value = (T) value;
            return true;
        }
    }
}
