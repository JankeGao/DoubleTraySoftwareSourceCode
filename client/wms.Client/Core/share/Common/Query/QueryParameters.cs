namespace wms.Client.Core.share.Common.Query
{
    /// <summary>
    /// 搜索基类
    /// </summary>
    public class QueryParameters
    {
        private int _pageIndex = 0;
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value;
        }

        private int _pageSize = 10;
        public virtual int PageSize
        {
            get => _pageSize;
            set => _pageSize = value;
        }

        public string Search { get; set; }

    }
}
