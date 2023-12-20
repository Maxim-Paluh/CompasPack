using CompasPack.Settings;
using CompasRTF.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CompasPack.Wrapper
{
    public class UserPathWrapper : ModelWrapper<UserPath>
    {
        public UserPathWrapper() : base(null)
        {
        }
        public UserPathWrapper(UserPath model) : base(model)
        {
        }
        public string rtfFolderPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string ArchiveFolderPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(rtfFolderPath):
                    if (!Directory.Exists(rtfFolderPath))
                    {
                        yield return "Не знайдено шлях до папки";
                    }
                    break;
                case nameof(ArchiveFolderPath):
                    if (!Directory.Exists(ArchiveFolderPath))
                    {
                        yield return "Не знайдено шлях до папки";
                    }
                    break;
            }
        }
    }
}
