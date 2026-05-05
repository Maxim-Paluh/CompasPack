using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Extension
{
    public static class WhereAsyncExtensions
    {
        // просто спосіб скоротити такі записи
        //var activeAntiviruses = (await Task.WhenAll(_antiviruses.Select(async av => new {
        //    Antivirus = av,
        //    IsActive = av.IsControlled && await av.GetRealTimeMonitoringStatus() != AntivirusStatusEnum.Disabled
        //}))).Where(x => x.IsActive).Select(x => x.Antivirus).ToList();
        public static async Task<IEnumerable<T>> WhereAsync<T>(this IEnumerable<T> source, Func<T, Task<bool>> predicate)
        {
            var results = await Task.WhenAll(source.Select(async x => (Item: x, IsMatch: await predicate(x))));
            return results.Where(x => x.IsMatch).Select(x => x.Item);
        }
    }
}
