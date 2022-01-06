using System;

namespace Tekook.BackupR.Lib.State
{
    public interface IBackupState
    {
        public DateTime BackupEnd { get; set; }
        public DateTime BackupStart { get; set; }
        public bool BackupSuccessful { get; set; }
        public long TotalBackupSize { get; set; }
    }
}