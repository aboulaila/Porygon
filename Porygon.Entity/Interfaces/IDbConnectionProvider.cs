using System.Data;
namespace Porygon.Entity.Interfaces
{
    public interface IDbConnectionProvider
	{
        IDbConnection GetConnection();
    }
}

