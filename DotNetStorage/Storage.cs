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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DotNetStorage.Net.Helpers;
using Newtonsoft.Json;

namespace DotNetStorage.Net
{
    /// <summary>
    /// A simple and lightweight tool for persisting data in dotnet (core) apps.
    /// </summary>
    public class Storage : IDisposable
    {
        /// <summary>
        /// Gets the number of elements contained in the LocalStorage.
        /// </summary>
        public int Count => StorageData.Count;

        /// <summary>
        /// Configurable behaviour for this LocalStorage instance.
        /// </summary>
        private readonly IStorageConfig _config;

        /// <summary>
        /// User-provided encryption key, used for encrypting/decrypting values.
        /// </summary>
        private readonly string _encryptionKey;

        /// <summary>
        /// Most current actual, in-memory state representation of the LocalStorage.
        /// </summary>
        private Dictionary<string, string> StorageData { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Most current actual, in-memory state representation of the elementos de espiracion LocalStorage.
        /// </summary>
        private Dictionary<string, string> StorageExpiry { get; set; } = new Dictionary<string, string>();

        private static readonly object writeLock = new object();

        public Storage() : this(new StorageConfig(), string.Empty) { }

        public Storage(IStorageConfig configuration) : this(configuration, string.Empty) { }

        public Storage(IStorageConfig configuration, string encryptionKey)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));

            if (_config.EnableEncryption) {
                if (string.IsNullOrEmpty(encryptionKey)) throw new ArgumentNullException(nameof(encryptionKey), "When EnableEncryption is enabled, an encryptionKey is required when initializing the LocalStorage.");
                _encryptionKey = encryptionKey;
            }

            if (_config.AutoLoad && string.IsNullOrEmpty(_config.BaseDirectory))
            {
                Load();
            }
            else if(_config.AutoLoad)
            {
                Load(_config.BaseDirectory);
            }
        }

        /// <summary>
        /// Clears the in-memory contents of the LocalStorage, but leaves any persisted state on disk intact.
        /// </summary>
        /// <remarks>
        /// Use the Destroy method to delete the persisted file on disk.
        /// </remarks>
        public void Clear()
        {
            StorageData.Clear();
            StorageExpiry.Clear();
        }

        /// <summary>
        /// Deletes the persisted file on disk, if it exists, but keeps the in-memory data intact.
        /// </summary>
        /// <remarks>
        /// Use the Clear method to clear only the in-memory contents.
        /// </remarks>
        public void Destroy()
        {

            var filepath = string.IsNullOrEmpty(_config.BaseDirectory) ? 
                           FileHelpers.GetLocalStoreFilePath(_config.Filename) : 
                           FileHelpers.GetLocalStoreFilePath(_config.BaseDirectory,_config.Filename);

            if (File.Exists(filepath))
                File.Delete(filepath);
        }

        /// <summary>
        /// Deletes the persisted file on disk, if it exists, but keeps the in-memory data intact.
        /// </summary>
        /// <remarks>
        /// Use the Clear method to clear only the in-memory contents.
        /// </remarks>
        public void Destroy(string baseDirectory)
        {
            var filepath = FileHelpers.GetLocalStoreFilePath(baseDirectory,_config.Filename);
            if (File.Exists(filepath))
                File.Delete(filepath);
        }
        /// <summary>
        /// Determines whether this LocalStorage instance contains the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return StorageData.ContainsKey(key: key);
        }

        /// <summary>
        /// Gets an object from the LocalStorage, without knowing its type.
        /// </summary>
        /// <param name="key">Unique key, as used when the object was stored.</param>
        public object Get(string key)
        {
            return Get<object>(key);
        }

        /// <summary>
        /// Gets a strong typed object from the LocalStorage.
        /// </summary>
        /// <param name="key">Unique key, as used when the object was stored.</param>
        public T Get<T>(string key)
        {
            var succeeded = StorageData.TryGetValue(key, out string raw);
            var expiryStatus = StorageExpiry.TryGetValue(key + "Expiry", out string exp);
            if (!succeeded && !expiryStatus && !((DateTime.Parse(exp) - DateTime.Now) > TimeSpan.Zero)) throw new ArgumentNullException($"Could not find key '{key}' in the LocalStorage.");

            if (_config.EnableEncryption)
                raw = CryptographyHelpers.Decrypt(_encryptionKey, _config.EncryptionSalt, raw);

            return JsonConvert.DeserializeObject<T>(raw);
        }

        /// <summary>
        /// Gets a collection containing all the keys in the LocalStorage.
        /// </summary>
        public ReadOnlyCollection<string> Keys()
        {
            return new ReadOnlyCollection<string>(StorageData.Keys.OrderBy(x => x).ToList());
        }

        /// <summary>
        /// Loads the persisted state from disk into memory, overriding the current memory instance.
        /// </summary>
        /// <remarks>
        /// Simply doesn't do anything if the file is not found on disk.
        /// </remarks>
        public void Load()
        {
            if (!File.Exists(FileHelpers.GetLocalStoreFilePath(_config.Filename))
                || !File.Exists(FileHelpers.GetLocalStoreFilePath(_config.Filename + "Expiry"))) return;

            var serializedContent = File.ReadAllText(FileHelpers.GetLocalStoreFilePath(_config.Filename));
            var serializedContentExpiry = File.ReadAllText(FileHelpers.GetLocalStoreFilePath(_config.Filename + "Expiry"));

            if (string.IsNullOrEmpty(serializedContent) || string.IsNullOrEmpty(serializedContentExpiry)) return;

            StorageExpiry.Clear();
            StorageExpiry = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedContentExpiry);
            StorageExpiry = StorageExpiry.Where(x => ((DateTime.Parse(x.Value) - DateTime.Now) > TimeSpan.Zero)).ToDictionary(x => x.Key, x => x.Value);

            StorageData.Clear();
            StorageData = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedContent);
            StorageData = StorageData.Where(x => StorageExpiry.Keys.Contains(x.Key + "Expiry")).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Loads the persisted state from disk into memory, overriding the current memory instance.
        /// </summary>
        /// <remarks>
        /// Simply doesn't do anything if the file is not found on disk.
        /// </remarks>
        public void Load(string baseDirectory)
        {
            if (!File.Exists(FileHelpers.GetLocalStoreFilePath(baseDirectory, _config.Filename))
                || !File.Exists(FileHelpers.GetLocalStoreFilePath(baseDirectory,_config.Filename + "Expiry"))) return;

            var serializedContent = File.ReadAllText(FileHelpers.GetLocalStoreFilePath(baseDirectory,_config.Filename));
            var serializedContentExpiry = File.ReadAllText(FileHelpers.GetLocalStoreFilePath(baseDirectory,_config.Filename + "Expiry"));

            if (string.IsNullOrEmpty(serializedContent) || string.IsNullOrEmpty(serializedContentExpiry)) return;

            StorageExpiry.Clear();
            StorageExpiry = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedContentExpiry);
            StorageExpiry = StorageExpiry.Where(x => ((DateTime.Parse(x.Value) - DateTime.Now) > TimeSpan.Zero)).ToDictionary(x => x.Key, x => x.Value);

            StorageData.Clear();
            StorageData = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedContent);
            StorageData = StorageData.Where(x => StorageExpiry.Keys.Contains(x.Key + "Expiry")).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Stores an object into the LocalStorage.
        /// </summary>
        /// <param name="key">Unique key, can be any string, used for retrieving it later.</param>
        /// <param name="instance"></param>
        public void Store<T>(string key, T instance,TimeSpan? expiry=null)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var value = JsonConvert.SerializeObject(instance);


            var valueExpiry = (expiry != null ? DateTime.Now.Add(expiry.Value) : DateTime.Now.Add(TimeSpan.FromMinutes(_config.MinutExpiryDefault))).ToString();

            if (StorageData.Keys.Contains(key))
            {
                StorageData.Remove(key);
                StorageExpiry.Remove(key + "Expiry");
            }

            
            if (_config.EnableEncryption)
            {
                value = CryptographyHelpers.Encrypt(_encryptionKey, _config.EncryptionSalt, value);
                valueExpiry = CryptographyHelpers.Encrypt(_encryptionKey, _config.EncryptionSalt, valueExpiry);
            }

            StorageData.Add(key, value);
            StorageExpiry.Add(key + "Expiry", valueExpiry);
        }

        /// <summary>
        /// Syntax sugar that transforms the response to an IEnumerable<T>, whilst also passing along an optional WHERE-clause. 
        /// </summary>
        public IEnumerable<T> Query<T>(string key, Func<T, bool> predicate = null)
        {
            var collection = Get<IEnumerable<T>>(key);
            return predicate == null ? collection : collection.Where(predicate);
        }

        /// <summary>
        /// Persists the in-memory store to disk.
        /// </summary>
        public void Persist()
        {
            if (!string.IsNullOrEmpty(_config.BaseDirectory))
            {
                Persist(_config.BaseDirectory);
                return;
            }

            var serialized = JsonConvert.SerializeObject(StorageData, Formatting.Indented);
            var serializedExpiry = JsonConvert.SerializeObject(StorageExpiry, Formatting.Indented);

            var writemode = File.Exists(FileHelpers.GetLocalStoreFilePath(_config.Filename))
                ? FileMode.Truncate
                : FileMode.Create;

            var writemodeExpiry = File.Exists(FileHelpers.GetLocalStoreFilePath(_config.Filename + "Expiry"))
                ? FileMode.Truncate
                : FileMode.Create;

            lock (writeLock)
            {
                try
                {
                    using (var fileStream = new FileStream(FileHelpers.GetLocalStoreFilePath(_config.Filename),
                        mode: writemode,
                        access: FileAccess.Write))
                    {
                        using (var writer = new StreamWriter(fileStream))
                        {
                            writer.Write(serialized);
                        }
                    }

                    using (var fileStream = new FileStream(FileHelpers.GetLocalStoreFilePath(_config.Filename + "Expiry"),
                        mode: writemodeExpiry,
                        access: FileAccess.Write))
                    {
                        using (var writer = new StreamWriter(fileStream))
                        {
                            writer.Write(serializedExpiry);
                        }
                    }
                }
                catch
                {
                    //Para deteccion de algun tipo de  se dbe poner codigo aquí
                }
            }
        }

        /// <summary>
        /// Persists the in-memory store to disk.
        /// </summary>
        public void Persist(string baseDirectory)
        {
            var serialized = JsonConvert.SerializeObject(StorageData, Formatting.Indented);
            var serializedExpiry = JsonConvert.SerializeObject(StorageExpiry, Formatting.Indented);

            var writemode = File.Exists(FileHelpers.GetLocalStoreFilePath(baseDirectory, _config.Filename))
                ? FileMode.Truncate
                : FileMode.Create;

            var writemodeExpiry = File.Exists(FileHelpers.GetLocalStoreFilePath(baseDirectory,_config.Filename + "Expiry"))
                ? FileMode.Truncate
                : FileMode.Create;

            lock (writeLock)
            {
                using (var fileStream = new FileStream(FileHelpers.GetLocalStoreFilePath(baseDirectory, _config.Filename),
                    mode: writemode,
                    access: FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        writer.Write(serialized);
                    }
                }
            


                using (var fileStream = new FileStream(FileHelpers.GetLocalStoreFilePath(baseDirectory, _config.Filename + "Expiry"),
                    mode: writemodeExpiry,
                    access: FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        writer.Write(serializedExpiry);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_config.AutoSave && string.IsNullOrEmpty(_config.BaseDirectory))
            {
                Persist();
            }
            else if(_config.AutoSave)
            {
                Persist(_config.BaseDirectory);
            }
        }
    }
}
