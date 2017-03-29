using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commond.Splider
{
    public interface ISpider<T>
    {
        string GetHtmlByUrl(string url);

        List<T> Capture();
    }
}
