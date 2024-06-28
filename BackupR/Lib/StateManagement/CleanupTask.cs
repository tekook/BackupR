namespace Tekook.BackupR.Lib.StateManagement
{
    internal class CleanupTask : StateTask
    {
        public int RemainingFiles { get; set; } = -1;
        public string? MaxSizeConfig { get; set; } = null;
        public int MaxSizeDeletedFiles { get; set; } = -1;
        public double MaxSizeDeletedFilesSize { get; set; } = -1;
        public long? MaxFilesConfig { get; set; } = null;
        public int MaxFilesDeletedFiles { get; set; } = -1;
        public double MaxFilesDeletedFilesSize { get; set; } = -1;
        public int TotalDeletedFiles { get; set; } = -1;
        public double TotalDeletedFilesSize { get; set; } = -1;
    }
}