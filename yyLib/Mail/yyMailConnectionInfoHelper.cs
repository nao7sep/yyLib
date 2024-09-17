using MailKit;

namespace yyLib
{
    public static class yyMailConnectionInfoHelper
    {
        public static async Task ConnectAsync (this IMailService service, yyMailConnectionInfo connectionInfo, CancellationToken? cancellationToken = null) =>
            await service.ConnectAsync (connectionInfo.Host, connectionInfo.Port!.Value, connectionInfo.SecureSocketOptions!.Value, cancellationToken ?? CancellationToken.None);

        public static async Task AuthenticateAsync (this IMailService service, yyMailConnectionInfo connectionInfo, CancellationToken? cancellationToken = null) =>
            await service.AuthenticateAsync (connectionInfo.UserName, connectionInfo.Password, cancellationToken ?? CancellationToken.None);
    }
}
