using CompasPack.Settings;
using System.Collections.Generic;
using System.IO;

namespace CompasPack.Wrapper
{
    //public class UserPathWrapper : ModelWrapper<UserPath>
    //{
    //    private ReportPathSettingsWrapper _reportPathSettingsWrapper;
    //    private PortablePathSettingsWrapper _portablePathSettings;

    //    public string PathFolderPrograms
    //    {
    //        get => GetValue<string>();
    //        set => SetValue(value);
    //    }
    //    public string PathFolderImageProgram
    //    {
    //        get => GetValue<string>();
    //        set => SetValue(value);
    //    }

    //    public string PathExampleFile
    //    {
    //        get => GetValue<string>();
    //        set => SetValue(value);
    //    }

    //    public ReportPathSettingsWrapper ReportPathSettingsWrapper
    //    {
    //        get => _reportPathSettingsWrapper;
    //    }
    //    public PortablePathSettingsWrapper PortablePathSettingsWrapper
    //    {
    //        get => _portablePathSettings;
    //    }

    //    public UserPathWrapper() : base(null) { } // це обовязково треба тут, щоб працювали шаблони в BaseSettingsViewModel для TWrapper
    //    public UserPathWrapper(UserPath model) : base(model)
    //    {
    //        _reportPathSettingsWrapper = new ReportPathSettingsWrapper(model.ReportPathSettings);
    //        _portablePathSettings = new PortablePathSettingsWrapper(model.PortablePathSettings);
    //    }

    //    protected override IEnumerable<string> ValidateProperty(string propertyName)
    //    {
    //        switch (propertyName)
    //        {
    //            case nameof(PathFolderPrograms):
    //                if (!Directory.Exists(PathFolderPrograms))
    //                {
    //                    yield return "Не знайдено шлях до теки";
    //                }
    //                break;
    //            case nameof(PathFolderImageProgram):
    //                if (!Directory.Exists(PathFolderImageProgram))
    //                {
    //                    yield return "Не знайдено шлях до теки";
    //                }
    //                break;
    //            case nameof(PathExampleFile):
    //                if (!Directory.Exists(PathExampleFile))
    //                {
    //                    yield return "Не знайдено шлях до теки";
    //                }
    //                break;
    //        }
    //    }
    //}
}
