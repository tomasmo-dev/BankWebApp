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
    /// </summary>
    public const string ConnectionString = "Server=127.0.0.1,6661;Database=FakeBank;User Id=SA;Password=Heslo1234.;TrustServerCertificate=true;MultipleActiveResultSets=true";
}