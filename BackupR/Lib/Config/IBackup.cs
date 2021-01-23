namespace Tekook.BackupR.Lib.Config
{
    public interface IBackup
    {
        public string Name { get; }
        public string UploadPath { get; }
        public string UploadName { get; }
    }
}