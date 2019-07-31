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
using System.IO;

namespace DotNetStorage.Net.Helpers
{
    internal static class FileHelpers
    {
        internal static string GetLocalStoreFilePath(string filename)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
        }

        internal static string GetLocalStoreFilePath(string baseDirectory, string filename)
        {
            return Path.Combine(baseDirectory, filename);
        }
    }
}
