using Shapr3D.Converter.ViewModels;
using Shapr3D_Converter.Models;

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
                    modelEntity.StepFileBytes,
                    modelEntity.ThumbnailBytes,
                    modelEntity.StlConversionTime,
                    modelEntity.ObjConversionTime,
                    modelEntity.StepConversionTime);
    }
}
