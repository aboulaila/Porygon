using System.Data;
namespace Porygon.Entity.Data
{
    public interface IDbConnectionProvider
	{
        IDbConnection GetConnection();
    }
}

