namespace yyLibWeb
{
    public class Program
    {
        public static void Main (string [] args)
        {
            var xBuilder = WebApplication.CreateBuilder (args);

            xBuilder.Services.AddRazorPages ();

            var xApp = xBuilder.Build ();

            if (xApp.Environment.IsDevelopment () == false)
            {
                xApp.UseExceptionHandler ("/Error");
                xApp.UseHsts ();
            }

            xApp.UseHttpsRedirection ();
            xApp.UseStaticFiles ();
            xApp.UseRouting ();
            xApp.UseAuthorization ();
            xApp.MapRazorPages ();

            xApp.Run ();
        }
    }
}
