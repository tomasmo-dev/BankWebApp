namespace BankWebApp.env;

/// <summary>
/// The Envs static class contains environment variables for the application.
/// </summary>
public static class Envs
{
    /// <summary>
    /// The ConnectionString is used to connect to the database.
    /// It includes the server address, database name, user id, password, 
    /// trust server certificate and multiple active result sets settings.
    /// The Connection String is set in Program.cs from appsettings.json.
    /// </summary>
    public static string ConnectionString;
}