using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using MLNET.RealWorld.Db;
using MLNET.RealWorld.Models;

namespace MLNET.RealWorld.Services
{
    public class DataLoader
    {
        private readonly SpamDetectorDbContext _dbContext;
        private readonly MLContext _mlContext;

        public DataLoader(SpamDetectorDbContext dbContext, MLContext mlContext) {
            _dbContext = dbContext;
            _mlContext = mlContext;
        }

        public async Task<IDataView> Load() {
            await Task.CompletedTask;
            var databaseLoader = _mlContext.Data.CreateDatabaseLoader<SpamInput>();
            var dataView = databaseLoader.Load(
                new DatabaseSource(SqlClientFactory.Instance,
                _dbContext.Database.GetDbConnection().ConnectionString,
                "SELECT IsSpam, Message FROM SpamRecords"));

            return dataView;
        }
    }
}
