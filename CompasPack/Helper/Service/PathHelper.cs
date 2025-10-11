using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper
{
    public static class PathHelper
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
