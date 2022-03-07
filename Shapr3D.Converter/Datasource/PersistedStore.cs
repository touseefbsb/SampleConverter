using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shapr3D.Converter.Helpers;
using Shapr3D_Converter.Models;

namespace Shapr3D.Converter.Datasource
{
    public class DummyStore : IPersistedStore
    {
        private readonly Dictionary<Guid, ModelEntity> data = new Dictionary<Guid, ModelEntity>();
        public async Task InitAsync()
        {
            try
            {
                using (var db = new Shapr3dDbContext())
                {
                    data.Clear();
                    foreach (var item in await db.ModelEntities.ToListAsync())
                    {
                        data.Add(item.Id, item);
                    }
                }
            }
            catch (Exception ex)
            {
                await AppCenterHelper.TrackExceptionAndShowErrorDialogAsync("Getting data failed", ex, "Unable to get data from database. Please contact administrator or try again later.");
            }
        }

        public async Task AddOrUpdateAsync(ModelEntity model)
        {
            try
            {
                using (var db = new Shapr3dDbContext())
                {
                    if (data.ContainsKey(model.Id))
                    {
                        db.ModelEntities.Update(model);
                    }
                    else
                    {
                        db.ModelEntities.Add(model);
                    }

                    if (await db.SaveChangesAsync() > 0)
                    {
                        data[model.Id] = model;
                    }
                }
            }
            catch (Exception ex)
            {
                await AppCenterHelper.TrackExceptionAndShowErrorDialogAsync("Add/Update failed", ex, "Unable to Add/Update data from database. Please contact administrator or try again later.");
            }
        }

        public Task<List<ModelEntity>> GetAllAsync() => Task.FromResult(data.Values.ToList());

        public async Task DeleteAllAsync()
        {
            try
            {
                using (var db = new Shapr3dDbContext())
                {
                    if (data.Values.ToList() is IEnumerable<ModelEntity> listToRemove)
                    {
                        db.ModelEntities.RemoveRange(listToRemove);
                        if (await db.SaveChangesAsync() > 0)
                        {
                            data.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await AppCenterHelper.TrackExceptionAndShowErrorDialogAsync("Delete failed", ex, "Unable to Delete data from database. Please contact administrator or try again later.");
            }
        }
    }
}
