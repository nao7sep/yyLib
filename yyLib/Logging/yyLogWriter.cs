namespace yyLib
{
    public interface yyLogWriter
    {
        void Write (DateTime createdAtUtc, string key, string value);
    }
}
