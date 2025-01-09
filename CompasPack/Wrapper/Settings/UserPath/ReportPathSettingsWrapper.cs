using CompasPack.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Wrapper
{
    public class ReportPathSettingsWrapper : ModelWrapper<ReportPathSettings>
    {
        public string AidaExeFilePath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string LogInstallRPF
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string ReportRPF
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string MonitorReportRPF
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        //---
        public string LaptopReportPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string PCReportPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string MonitorReportPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        //---
        public string LaptopPricePath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string PCPricePath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string MonitorPricePath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }


        public ReportPathSettingsWrapper(ReportPathSettings model) : base(model)
        {
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(AidaExeFilePath):
                    if (!Directory.Exists(AidaExeFilePath))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;
                case nameof(LogInstallRPF):
                    if (!Directory.Exists(LogInstallRPF))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;
                case nameof(ReportRPF):
                    if (!Directory.Exists(ReportRPF))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;
                case nameof(MonitorReportRPF):
                    if (!Directory.Exists(MonitorReportRPF))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;

                case nameof(LaptopReportPath):
                    if (!Directory.Exists(LaptopReportPath))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;
                case nameof(PCReportPath):
                    if (!Directory.Exists(PCReportPath))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;
                case nameof(MonitorReportPath):
                    if (!Directory.Exists(MonitorReportPath))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;

                case nameof(LaptopPricePath):
                    if (!Directory.Exists(LaptopPricePath))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;
                case nameof(PCPricePath):
                    if (!Directory.Exists(PCPricePath))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;
                case nameof(MonitorPricePath):
                    if (!Directory.Exists(MonitorPricePath))
                    {
                        yield return "Не знайдено шлях до теки";
                    }
                    break;

            }
        }
    }
}
