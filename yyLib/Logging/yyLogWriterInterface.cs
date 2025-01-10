namespace yyLib
{
    public interface yyLogWriterInterface
    {
        void Write (DateTime createdAtUtc, string key, string value);
    }
}
