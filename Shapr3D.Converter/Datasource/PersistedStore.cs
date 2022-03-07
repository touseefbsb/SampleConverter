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
        public byte[] FileBytes { get; set; }
        public byte[] StlFileBytes { get; set; }
        public byte[] ObjFileBytes { get; set; }
        public byte[] StepFileBytes { get; set; }
    }

    public class DummyStore : IPersistedStore
    {
        private readonly Dictionary<Guid, ModelEntity> data = new Dictionary<Guid, ModelEntity>();
        public Task InitAsync() => Task.CompletedTask;

        public Task AddOrUpdateAsync(ModelEntity model)
        {
            data[model.Id] = model;
            return Task.CompletedTask;
        }

        public Task<List<ModelEntity>> GetAllAsync() => Task.FromResult(data.Values.ToList());

        public Task DeleteAllAsync()
        {
            data.Clear();
            return Task.CompletedTask;
        }
    }
}
