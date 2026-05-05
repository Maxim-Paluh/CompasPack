using System;
using System.IO;

namespace CompasPack.Helper.Service
{
    public static class PathService
    {
        public static void SetRootPath(string pathRoot, object obj)
        {
            foreach (var item in obj.GetType().GetProperties())
            {
                if (item.PropertyType == typeof(string))
                {
                    var tempValue = (string)item.GetValue(obj);
                    if (!string.IsNullOrEmpty(tempValue))
                        item.SetValue(obj, Path.Combine(pathRoot, tempValue));
                }
                else
                    SetRootPath(pathRoot, item.GetValue(obj));
            }
        }
    }
}
