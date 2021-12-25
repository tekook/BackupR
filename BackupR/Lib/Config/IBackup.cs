namespace Tekook.BackupR.Lib.Config
{
    public interface IBackup
    {
        public bool Disabled { get; }
        public string Name { get; }
        public string UploadName { get; }
        public string UploadPath { get; }
    }
}