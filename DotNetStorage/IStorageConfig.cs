#region Derechos Reservados
// ===================================================
// Desarrollado por          : Fabián C. Sánchez
// Fecha de creación         : 2019/06/28
// Modificado por            : Fabián C. Sánchez
// Fecha de Modificacion     : 2019/07/28
// Empresa                   : S.
// ===================================================
#endregion
namespace DotNetStorage.Net
{
    public interface IStorageConfig
    {
        /// <summary>
        /// Indicates if LocalStorage should automatically load previously persisted state from disk, when it is initialized (defaults to true).
        /// </summary>
        /// <remarks>
        /// Requires manually to call Load() when disabled.
        /// </remarks>
        bool AutoLoad { get; set; }

        /// <summary>
        /// Indicates if LocalStorage should automatically persist the latest state to disk, on dispose (defaults to true).
        /// </summary>
        /// <remarks>
        /// Disabling this requires a manual call to Persist() in order to save changes to disk.
        /// </remarks>
        bool AutoSave { get; set; }

        /// <summary>
        /// Indicates if LocalStorage should encrypt its contents when persisting to disk.
        /// </summary>
        bool EnableEncryption { get; set; }

        /// <summary>
        /// [Optional] Add a custom salt to encryption, when EnableEncryption is enabled.
        /// </summary>
        string EncryptionSalt { get; set; }

        /// <summary>
        /// Filename for the persisted state on disk (defaults to ".localstorage").
        /// </summary>
        string Filename { get; set; }

        /// <summary>
        /// Base Directory for the persisted state on disk.
        /// </summary>
        string BaseDirectory { get; set; }

        /// <summary>                                                                                       
        /// Number of Minutes for the expiry default.
        /// </summary>
        int MinutExpiryDefault { get; set; }


    }
}