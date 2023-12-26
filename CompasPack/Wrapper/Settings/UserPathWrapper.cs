using CompasPack.Settings;
using System.Collections.Generic;
using System.IO;

namespace CompasPack.Wrapper
{
    public class UserPathWrapper : ModelWrapper<UserPath>
    {
        public string PathFolderPrograms
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string PathFolderImageProgram
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public UserPathWrapper() : base(null)
        {
        }
        public UserPathWrapper(UserPath model) : base(model)
        {
        }
     
        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(PathFolderPrograms):
                    if (!Directory.Exists(PathFolderPrograms))
                    {
                        yield return "Не знайдено шлях до папки";
                    }
                    break;
                case nameof(PathFolderImageProgram):
                    if (!Directory.Exists(PathFolderImageProgram))
                    {
                        yield return "Не знайдено шлях до папки";
                    }
                    break;
            }
        }
    }
}
