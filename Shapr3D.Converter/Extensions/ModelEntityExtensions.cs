using Shapr3D.Converter.Datasource;
using Shapr3D.Converter.ViewModels;

namespace Shapr3D.Converter.Extensions
{
    public static class ModelEntityExtensions
    {
        public static FileViewModel ToFileViewModel(this ModelEntity modelEntity) => new FileViewModel(
                    modelEntity.Id,
                    modelEntity.OriginalPath,
                    modelEntity.ObjConverted,
                    modelEntity.StepConverted,
                    modelEntity.StlConverted,
                    modelEntity.FileSize,
                    modelEntity.FileBytes,
                    modelEntity.StlFileBytes,
                    modelEntity.ObjFileBytes,
                    modelEntity.StepFileBytes);
    }
}
