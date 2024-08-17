namespace Tekook.BackupR.Lib.StateManagement
{
    internal class CleanupTask : StateTask
    {
        /// <summary>
        /// How many file are remaining in the container?
        /// </summary>
        public int RemainingFiles { get; set; } = -1;

        /// <summary>
        /// Configuration <see cref="Config.IContainerConfig.MaxSize"/>
        /// </summary>
        public string MaxSizeConfig { get; set; } = null;

        /// <summary>
        /// Number of deleted files base of container size.
        /// </summary>
        public int MaxSizeDeletedFiles { get; set; } = -1;

        /// <summary>
        /// The size of the deleted files.
        /// </summary>
        public double MaxSizeDeletedFilesSize { get; set; } = -1;

        /// <summary>
        /// Configuration <see cref="Config.IContainerConfig.MaxFiles"/>
        /// </summary>
        public long? MaxFilesConfig { get; set; } = null;

        /// <summary>
        /// Number of deleted files base of container file count.
        /// </summary>
        public int MaxFilesDeletedFiles { get; set; } = -1;

        /// <summary>
        /// The size of the deleted files.
        /// </summary>
        public double MaxFilesDeletedFilesSize { get; set; } = -1;

        /// <summary>
        /// Total number of deleted files.
        /// </summary>
        public int TotalDeletedFiles { get; set; } = -1;

        /// <summary>
        /// Total size of the deleted files.
        /// </summary>
        public double TotalDeletedFilesSize { get; set; } = -1;
    }
}