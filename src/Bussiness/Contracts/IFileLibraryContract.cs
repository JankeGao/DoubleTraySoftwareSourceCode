using Bussiness.Entitys;
using HP.Core.Dependency;
using HP.Data.Orm;
using HP.Utility.Data;

namespace Bussiness.Contracts
{
    public interface IFileLibraryContract : IScopeDependency
    {

        #region 文件操作

        /// <summary>
        /// 文件库查询
        /// </summary>
        IQuery<FileLibrary> FileLibrarys { get; }



        /// <summary>
        /// 文件库文件上传
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult FileLibraryUpload(FileLibrary entity);

        /// <summary>
        /// 文件库文件编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DataResult EditFileLibrary(FileLibrary entity);

        /// <summary>
        /// 文件库文件删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        DataResult RemoveFileLibrary(int Id);

        /// <summary>
        /// 文件库文件转移
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
       // DataResult TransferFileLibrary(FileLibrary entity);

        #endregion
    }
}
