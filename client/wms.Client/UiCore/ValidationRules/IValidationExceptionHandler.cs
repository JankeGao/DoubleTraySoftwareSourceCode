namespace wms.Client.UiCore.ValidationRules
{
    public interface IValidationExceptionHandler
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        bool IsValid
        {
            get;
            set;
        }

        /// <summary>
        /// 清空异常
        /// </summary>
        bool IsClear
        {
            get;set;
        }
    }
}
