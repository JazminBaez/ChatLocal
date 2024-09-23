using ChatLocalCliente;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

      
        Application.Run(new UserForm());
    }
}
