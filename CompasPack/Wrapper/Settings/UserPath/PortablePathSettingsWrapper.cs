using CompasPack.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Wrapper
{
    public class PortablePathSettingsWrapper : ModelWrapper<PortablePathSettings>
    {
        public string RarPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string KMSAutoPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string KMSAutoRarPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public PortablePathSettingsWrapper(PortablePathSettings model) : base(model)
        {
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(RarPath):
                    if (!Directory.Exists(RarPath))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;
                case nameof(KMSAutoPath):
                    if (!Directory.Exists(RarPath))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;
                case nameof(KMSAutoRarPath):
                    if (!Directory.Exists(RarPath))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;

            }
        }
    }
}
