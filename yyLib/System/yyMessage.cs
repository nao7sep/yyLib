namespace yyLib
{
    // A simple class to ensure messages embedded in code will always be easy to find.

    public class yyMessage
    {
        public static yyMessage Create (string content) => new (content);

        public string Content { get; private set; }

        public yyMessage (string content) => Content = content;
    }
}
