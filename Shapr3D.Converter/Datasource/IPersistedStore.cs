using System.Collections.Generic;
using System.Threading.Tasks;
using Shapr3D_Converter.Models;

namespace Shapr3D.Converter.Datasource
{
    public interface IPersistedStore
    {
        Task AddOrUpdateAsync(ModelEntity model);
        Task DeleteAllAsync();
        Task<List<ModelEntity>> GetAllAsync();
        Task InitAsync();
    }
}