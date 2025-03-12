using System.Text;

namespace yyLib
{
    public static class yyEncodingHelper
    {
        public static Encoding OrDefaultEncoding (this Encoding? encoding) => encoding ?? yyEncoding.Default;
    }
}
