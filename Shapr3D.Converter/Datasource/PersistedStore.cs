using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shapr3D.Converter.Datasource
{
    public class ModelEntity
    {
        public Guid Id { get; set; }
        public string OriginalPath { get; set; }
        public ulong FileSize { get; set; }
        public bool ObjConverted { get; set; }
        public bool StlConverted { get; set; }
        public bool StepConverted { get; set; }
    }

    public class DummyStore : IPersistedStore
    {
        private Dictionary<Guid, ModelEntity> data = new Dictionary<Guid, ModelEntity>();
        public Task InitAsync()
        {
            return Task.CompletedTask;
        }

        public Task AddOrUpdateAsync(ModelEntity model)
        {
            data[model.Id] = model;
            return Task.CompletedTask;
        }

        public Task<List<ModelEntity>> GetAllAsync()
        {
            return Task.FromResult(data.Values.ToList());
        }

        public Task DeleteAllAsync()
        {
            data.Clear();
            return Task.CompletedTask;
        }
    }
}
