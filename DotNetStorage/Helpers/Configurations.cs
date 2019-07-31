#region Derechos Reservados
// ===================================================
// Desarrollado por          : Fabián C. Sánchez
// Fecha de creación         : 2019/06/28
// Modificado por            : Fabián C. Sánchez
// Fecha de Modificacion     : 2019/07/28
// Empresa                   : S.
// ===================================================
#endregion
using System;
using System.Collections.Specialized;

namespace DotNetStorage.Net.Helpers
{
    public class Configurations
    {
        #region Session de la configuración
        /// <summary>
        /// Session de la configuración
        /// </summary>
        private static string Seccion(string Nombre)
        {
            try
            {
                NameValueCollection ConfigComfama = new NameValueCollection();
                ConfigComfama = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("LocalStorage.Configuration");
                return ConfigComfama[Nombre].ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Aplicación

        /// <summary>
        /// Indicates if LocalStorage should automatically load previously persisted state from disk, when it is initialized (defaults to true).
        /// </summary>
        /// <remarks>
        /// Requires manually to call Load() when disabled.
        /// </remarks>
        public static bool? AutoLoad() { bool? nullValue=null; return (bool.TryParse(Seccion("AutoLoad"),out bool val) ? val : nullValue); }

        /// <summary>
        /// Indicates if LocalStorage should automatically persist the latest state to disk, on dispose (defaults to true).
        /// </summary>
        /// <remarks>
        /// Disabling this requires a manual call to Persist() in order to save changes to disk.
        /// </remarks>
        public static bool? AutoSave() {bool? nullValue = null; return (bool.TryParse(Seccion("AutoSave"), out bool val) ? val : nullValue); }

        /// <summary>
        /// Indicates if LocalStorage should encrypt its contents when persisting to disk.
        /// </summary>
        public static bool? EnableEncryption() {bool? nullValue = null; return (bool.TryParse(Seccion("EnableEncryption"), out bool val) ? val : nullValue); }

        /// <summary>
        /// [Optional] Add a custom salt to encryption, when EnableEncryption is enabled.
        /// </summary>
        public static string EncryptionSalt() { return Seccion("EncryptionSalt"); }

        /// <summary>
        /// Filename for the persisted state on disk (defaults to ".localstorage").
        /// </summary>
        public static string Filename() { return Seccion("Filename"); }

        /// <summary>
        /// Base Directory for the persisted state on disk.
        /// </summary>
        public static string BaseDirectory() { return Seccion("BaseDirectory"); }  
        
        /// <summary>                                                                                       
        /// Number of Minutes for the expiry default.
        /// </summary>
        public static int? MinutExpiryDefault() { int? nullValue = null; return (int.TryParse(Seccion("MinutExpiryDefault"), out int val) ? val : nullValue);}
        #endregion
    }
}
