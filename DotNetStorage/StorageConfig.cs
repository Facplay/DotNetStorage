#region Derechos Reservados
// ===================================================
// Desarrollado por          : Fabián C. Sánchez
// Fecha de creación         : 2019/06/28
// Modificado por            : Fabián C. Sánchez
// Fecha de Modificacion     : 2019/07/28
// Empresa                   : S.
// ===================================================
#endregion
using DotNetStorage.Net.Helpers;

namespace DotNetStorage.Net
{
    /// <summary>
    /// Provides options to configure LocalStorage to behave just like you want it.
    /// </summary>
    public class StorageConfig : IStorageConfig
    {
        /// <summary>
        /// Indicates if LocalStorage should automatically load previously persisted state from disk, when it is initialized (defaults to true).
        /// </summary>
        /// <remarks>
        /// Requires manually to call Load() when disabled.
        /// </remarks>
        public bool AutoLoad { get; set; } = Configurations.AutoLoad() != null ? Configurations.AutoLoad().Value : true;

        /// <summary>
        /// Indicates if LocalStorage should automatically persist the latest state to disk, on dispose (defaults to true).
        /// </summary>
        /// <remarks>
        /// Disabling this requires a manual call to Persist() in order to save changes to disk.
        /// </remarks>
        public bool AutoSave { get; set; } = Configurations.AutoSave() != null ? Configurations.AutoSave().Value : true;

        /// <summary>
        /// Indicates if LocalStorage should encrypt its contents when persisting to disk.
        /// </summary>
        public bool EnableEncryption { get; set; } = Configurations.EnableEncryption() != null ? Configurations.EnableEncryption().Value : false;

        /// <summary>
        /// [Optional] Add a custom salt to encryption, when EnableEncryption is enabled.
        /// </summary>
        public string EncryptionSalt { get; set; } = Configurations.EncryptionSalt() != null ? Configurations.EncryptionSalt() : ".localstorage";

        /// <summary>
        /// Filename for the persisted state on disk (defaults to ".localstorage").
        /// </summary>
        public string Filename { get; set; } = Configurations.Filename() != null ? Configurations.Filename() : ".localstorage";

        /// <summary>
        /// Base Directory for the persisted state on disk.
        /// </summary>
        public string BaseDirectory { get; set; } = Configurations.BaseDirectory() != null ? Configurations.BaseDirectory() : string.Empty;

        /// <summary>                                                                                       
        /// Number of Minutes for the expiry default.
        /// </summary>
        public int MinutExpiryDefault { get; set; } = Configurations.MinutExpiryDefault() != null ? Configurations.MinutExpiryDefault().Value : 1;
    }
}
