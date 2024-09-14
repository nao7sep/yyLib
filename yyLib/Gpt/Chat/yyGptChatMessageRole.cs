using yyLib;

namespace yyGptLib
{
    // Could be an enum, but I want to make sure the value is returned as a lowercase string.

    public class yyGptChatMessageRole (string value): IEquatable <yyGptChatMessageRole>
    {
        public static yyGptChatMessageRole System { get; } = new yyGptChatMessageRole ("system");

        public static yyGptChatMessageRole User { get; } = new yyGptChatMessageRole ("user");

        public static yyGptChatMessageRole Assistant { get; } = new yyGptChatMessageRole ("assistant");

        public static yyGptChatMessageRole Parse (string? value)
        {
            if ("system".Equals (value, StringComparison.OrdinalIgnoreCase))
                return System;

            else if ("user".Equals (value, StringComparison.OrdinalIgnoreCase))
                return User;

            else if ("assistant".Equals (value, StringComparison.OrdinalIgnoreCase))
                return Assistant;

            else throw new yyArgumentException ($"'{nameof (value)}' is invalid: {value.GetVisibleString ()}");
        }

        public string Value { get; private set; } = value;

        public bool Equals (yyGptChatMessageRole? role) => Value.Equals (role?.Value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals (object? obj) => Equals (obj as yyGptChatMessageRole);

        public override int GetHashCode () => Value.GetHashCode ();

        public override string ToString () => Value;
    }
}
