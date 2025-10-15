using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CompasPack.Helper.Service;

namespace CompasPack.Data.Providers
{
    public interface ISettings<T>
    {
        Task LoadFromFile();
        Task<T> LoadDefault();
        Task<bool> Save();
    }
    public class SettingsFileProviderBase<T> : ISettings<T> where T : class, new()
    {
        #region Properties
        private readonly IFileSystemReaderWriter _fileSystemReaderWriter;
        private IMessageDialogService _messageDialogService;
        public T Settings { get; set; }
        public string SettingsPathFolder { get; private set; }
        public string SettingsPathFile { get; private set; }
        public string SettingsPathDefaultFile { get; set; }
        public bool IsLoad { get; set; }
        #endregion

        #region Constructors
        public SettingsFileProviderBase(IFileSystemReaderWriter fileSystemReaderWriter, IMessageDialogService messageDialogService, string fileName)
        {
            _fileSystemReaderWriter = fileSystemReaderWriter;
            _messageDialogService = messageDialogService;
            SettingsPathFolder = Path.Combine(Directory.GetCurrentDirectory(), "Settings");
            SettingsPathFile = Path.Combine(SettingsPathFolder, $"{fileName}.json");
            SettingsPathDefaultFile = string.Empty;
            IsLoad = false;
        }
        #endregion

        #region Motods
        public virtual async Task LoadFromFile()
        {
            if(string.IsNullOrWhiteSpace(SettingsPathFile))
                throw new ArgumentException(nameof(SettingsPathFile));
            if (File.Exists(SettingsPathFile))
            {
                try
                {
                    Settings = JsonConvert.DeserializeObject<T>(await _fileSystemReaderWriter.ReadAllTextAsync(SettingsPathFile), new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error });
                    IsLoad = true;
                }
                catch (Exception exp)
                {
                    IsLoad = false;
                    Settings = null;
                    _messageDialogService.ShowInfoDialog($"Помилка в завантаженні файлу налаштувань {SettingsPathFile}\n\n" + $"Помилка:\n{exp.Message}", "Помилка!");
                }
            }
            else // якщо файла налаштувань нема
            {
                Settings = await LoadDefault(); // створюємо новий файл за замовчуванням
                IsLoad = true;
                await Save();                   // зберігаємо його, щоб в наступний раз він був навіть якщо користувач нічого не вносив в нього
            }
        }
        public virtual async Task<T> LoadDefault()
        {
            if (!string.IsNullOrWhiteSpace(SettingsPathDefaultFile) && File.Exists(SettingsPathDefaultFile))    // якщо вказано файл налаштувань за замовчуванням і він існує тоді завантажуємо його
                return JsonConvert.DeserializeObject<T>(await _fileSystemReaderWriter.ReadAllTextAsync(SettingsPathDefaultFile), new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error });
            else                                                                                                // інакше створуємо новий з конструтором по замовчуванню
                return new T();
        }
        public async Task<bool> Save()
        {
            if (Settings != null)
            {
                var settingsJson = JsonConvert.SerializeObject(Settings, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });
                await _fileSystemReaderWriter.WriteAllTextAsync(SettingsPathFile, settingsJson);
                return true;
            }
            else
                return false;
        }
        #endregion
    }
}
