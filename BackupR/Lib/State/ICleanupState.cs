using System;

namespace Tekook.BackupR.Lib.State
{
    public interface ICleanupState
    {
        public DateTime CleanupStart { get; set; }
        public DateTime CleanupStop { get; set; }
        public bool CleanupSuccessful { get; set; }
    }
}