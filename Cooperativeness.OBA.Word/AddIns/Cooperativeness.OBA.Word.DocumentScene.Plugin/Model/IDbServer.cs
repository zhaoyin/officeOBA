
namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Model
{
    public interface IDbServer
    {
        int ConnectTimeout { get; set; }
        string DataBaseName { get; set; }
        string DataSourceProvider { get; set; }
        string Password { get; set; }
        string ServerName { get; set; }
        string UserName { get; set; }
        bool UseTrusted { get; set; }
    }
}
