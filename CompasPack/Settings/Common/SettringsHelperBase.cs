using CompasPack.View.Service;
using CompasPakc.BL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Settings
{
    public interface ISettings<T>
    {
        public Task LoadFromFile();
        public Task<T?> LoadDefault();
        public Task<bool> Save();
    }
    public class SettringsHelperBase<T> : ISettings<T> where T : class, new()
    {
        private readonly IIOManager _ioManager;
        private IMessageDialogService _messageDialogService;
        public T? Settings { get; set; }
        public string SettingsPathFolder { get; private set; }
        public string SettingsPathFile { get; private set; }
        public string SettingsPathDefaultFile { get; set; }
        public bool IsLoad { get; set; }
        public SettringsHelperBase(IIOManager iIOManager, IMessageDialogService messageDialogService, string fileName)
        {
            _ioManager = iIOManager;
            _messageDialogService = messageDialogService;
            SettingsPathFolder = Path.Combine(Directory.GetCurrentDirectory(), "Settings");
            SettingsPathFile = Path.Combine(SettingsPathFolder, $"{fileName}.json");
            SettingsPathDefaultFile = string.Empty;
            IsLoad = false;
        }
        #region Motods
        public virtual async Task LoadFromFile()
        {
            if(string.IsNullOrWhiteSpace(SettingsPathFile))
                throw new ArgumentException(nameof(SettingsPathFile));
            if (File.Exists(SettingsPathFile))
            {
                try
                {
                    Settings = JsonConvert.DeserializeObject<T>(await _ioManager.ReadAllTextAsync(SettingsPathFile), new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error });
                }
                catch (Exception exp)
                {
                    _messageDialogService.ShowInfoDialog($"Помилка в завантаженні файлу налаштувань {SettingsPathFile}\n\n" + $"Помилка:\n{exp.Message}", "Помилка!");
                    Settings = null;
                }
            }
            else // якщо файла налаштувань нема
            {
                Settings = await LoadDefault(); // створюємо новий файл за замовчуванням
                await Save();                   // зберігаємо його, щоб в наступний раз він був навіть якщо користувач нічого не вносив в нього
            }
            IsLoad = true;
        }

        public virtual async Task<T?> LoadDefault()
        {
            if (!string.IsNullOrWhiteSpace(SettingsPathDefaultFile) && File.Exists(SettingsPathDefaultFile))    // якщо вказано файл налаштувань за замовчуванням і він існує тоді завантажуємо його
                return JsonConvert.DeserializeObject<T>(await _ioManager.ReadAllTextAsync(SettingsPathDefaultFile), new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error });
            else                                                                                                // інакше створуємо новий з конструтором по замовчуванню
                return new T();
        }


        public async Task<bool> Save()
        {
            if (Settings != null)
            {
                var settingsJson = JsonConvert.SerializeObject(Settings, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });
                await _ioManager.WriteAllTextAsync(SettingsPathFile, settingsJson);
                return true;
            }
            else
                return false;
        }
        #endregion
    }
}
