using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Transfer.PagedList
{
    public class PagedListDTO<T> where T : class
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Item { get; set; }
    }

}
