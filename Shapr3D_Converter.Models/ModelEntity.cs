using System;

namespace Shapr3D_Converter.Models
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
        public byte[] ThumbnailBytes { get; set; }
        public TimeSpan? StlConversionTime { get; set; }
        public TimeSpan? ObjConversionTime { get; set; }
        public TimeSpan? StepConversionTime { get; set; }
    }
}
