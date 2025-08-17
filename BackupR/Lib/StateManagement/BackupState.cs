using System;
using System.Collections.Generic;

namespace Tekook.BackupR.Lib.StateManagement
{
    internal class BackupState : CommandState<BackupTask>
    {
        public List<Exception> Errors { get; set; } = [];
    }
}