namespace yyLib
{
    public interface yyILogWriter
    {
        void Write (DateTime createdAtUtc, string key, string value);
    }
}
