﻿using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Piously.Game.Configuration;

namespace Piously.Game.IO
{
    public class PiouslyStorage : MigratableStorage
    {
        /// <summary>
        /// Indicates the error (if any) that occurred when initialising the custom storage during initial startup.
        /// </summary>
        public readonly PiouslyStorageError Error;

        /// <summary>
        /// The custom storage path as selected by the user.
        /// </summary>
        [CanBeNull]
        public string CustomStoragePath => storageConfig.Get<string>(StorageConfig.FullPath);

        /// <summary>
        /// The default storage path to be used if a custom storage path hasn't been selected or is not accessible.
        /// </summary>
        [NotNull]
        public string DefaultStoragePath => defaultStorage.GetFullPath(".");

        private readonly GameHost host;
        private readonly StorageConfigManager storageConfig;
        private readonly Storage defaultStorage;

        public override string[] IgnoreDirectories => new[] { "cache" };

        public override string[] IgnoreFiles => new[]
        {
            "framework.ini",
            "storage.ini"
        };

        public PiouslyStorage(GameHost host, Storage defaultStorage)
            : base(defaultStorage, string.Empty)
        {
            this.host = host;
            this.defaultStorage = defaultStorage;

            storageConfig = new StorageConfigManager(defaultStorage);

            if (!string.IsNullOrEmpty(CustomStoragePath))
                TryChangeToCustomStorage(out Error);
        }

        /// <summary>
        /// Resets the custom storage path, changing the target storage to the default location.
        /// </summary>
        public void ResetCustomStoragePath()
        {
            storageConfig.SetValue(StorageConfig.FullPath, string.Empty);
            storageConfig.Save();

            ChangeTargetStorage(defaultStorage);
        }

        /// <summary>
        /// Attempts to change to the user's custom storage path.
        /// </summary>
        /// <param name="error">The error that occurred.</param>
        /// <returns>Whether the custom storage path was used successfully. If not, <paramref name="error"/> will be populated with the reason.</returns>
        public bool TryChangeToCustomStorage(out PiouslyStorageError error)
        {
            Debug.Assert(!string.IsNullOrEmpty(CustomStoragePath));

            error = PiouslyStorageError.None;
            Storage lastStorage = UnderlyingStorage;

            try
            {
                Storage userStorage = host.GetStorage(CustomStoragePath);

                if (!userStorage.ExistsDirectory(".") || !userStorage.GetFiles(".").Any())
                    error = PiouslyStorageError.AccessibleButEmpty;

                ChangeTargetStorage(userStorage);
            }
            catch
            {
                error = PiouslyStorageError.NotAccessible;
                ChangeTargetStorage(lastStorage);
            }

            return error == PiouslyStorageError.None;
        }

        protected override void ChangeTargetStorage(Storage newStorage)
        {
            base.ChangeTargetStorage(newStorage);
            Logger.Storage = UnderlyingStorage.GetStorageForDirectory("logs");
        }

        public override void Migrate(Storage newStorage)
        {
            base.Migrate(newStorage);
            storageConfig.SetValue(StorageConfig.FullPath, newStorage.GetFullPath("."));
            storageConfig.Save();
        }
    }

    public enum PiouslyStorageError
    {
        /// <summary>
        /// No error.
        /// </summary>
        None,

        /// <summary>
        /// Occurs when the target storage directory is accessible but does not already contain game files.
        /// Only happens when the user changes the storage directory and then moves the files manually or mounts a different device to the same path.
        /// </summary>
        AccessibleButEmpty,

        /// <summary>
        /// Occurs when the target storage directory cannot be accessed at all.
        /// </summary>
        NotAccessible,
    }
}
