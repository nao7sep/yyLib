using MailKit;

namespace yyLib
{
    public static class yyMailServiceInterfaceHelper
    {
        public static async Task ConnectAsync (this IMailService service, yyMailConnectionInfo connectionInfo, CancellationToken cancellationToken = default) =>
            await service.ConnectAsync (connectionInfo.Host, connectionInfo.Port!.Value, connectionInfo.SecureSocketOptions!.Value, cancellationToken).ConfigureAwait (false);

        public static async Task AuthenticateAsync (this IMailService service, yyMailConnectionInfo connectionInfo, CancellationToken cancellationToken = default) =>
            await service.AuthenticateAsync (connectionInfo.UserName, connectionInfo.Password, cancellationToken).ConfigureAwait (false);
    }
}
