using ChatLocalCliente;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var chatForm = new ChatDos();
      
        Application.Run(chatForm);
    }
}
