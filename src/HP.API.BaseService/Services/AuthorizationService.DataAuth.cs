namespace HPC.BaseService.Services
{
    //public partial class AuthorizationService
    //{
    //    public IQuery<DataAuth> DataAuths
    //    {
    //        get { return DataAuthRepository.Query(); }
    //    }

    //    /// <summary>
    //    /// 数据授权
    //    /// </summary>
    //    /// <param name="type">类别</param>
    //    /// <param name="typeCode">类别Code</param>
    //    /// <param name="auth">数据权限类型</param>
    //    /// <returns></returns>
    //    private DataResult DataAuthorization(int type, string typeCode, DataAuth auth)
    //    {
    //        //是否存在该数据授权类型
    //        if (DataAuths.Any(a => a.Type == type && a.TypeCode == typeCode))
    //        {
    //            //如果是无配置，则移除数据库数据授权类型
    //            if (auth.FilterType == (int) EntityDataAuthTypeEnum.None)
    //            {
    //                if (DataAuthRepository.Delete(a => a.Type == type && a.TypeCode == typeCode) ==
    //                    0)
    //                {
    //                    DataProcess.Failure(
    //                        "{0}({1})原始数据权限移除失败！".FormatWith(EnumHelper.GetCaption(typeof (AuthorizationTypeEnum),
    //                            type), typeCode));
    //                }
    //            }
    //            else
    //            {
    //                if (DataAuthRepository.Update(a => new DataAuth()
    //                {
    //                    FilterType = auth.FilterType
    //                }, a => a.Type == type && a.TypeCode == typeCode) == 0)
    //                {
    //                    return
    //                        DataProcess.Failure(
    //                            "{0}({1})数据权限更新失败！".FormatWith(EnumHelper.GetCaption(typeof(AuthorizationTypeEnum),
    //                                type), typeCode));
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (!DataAuthRepository.Insert(new DataAuth()
    //            {
    //                Type = type,
    //                TypeCode = typeCode,
    //                FilterType = auth.FilterType
    //            }))
    //            {
    //                return
    //                    DataProcess.Failure(
    //                        "{0}({1})数据权限创建失败！".FormatWith(EnumHelper.GetCaption(typeof(AuthorizationTypeEnum),
    //                            type), typeCode));
    //            }
    //        }

    //        return DataProcess.Success();
    //    }
    //}
}
