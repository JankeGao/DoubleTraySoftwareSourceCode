using System;
using HP.Core.Data;
using Bussiness.Entitys;
using HP.Data.Orm;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HP.Utility.Files;
using System.IO;
namespace Bussiness.Services
{
    class FileLibraryServer : Contracts.IFileLibraryContract
    {
        /// <summary>
        /// 文件库仓储
        /// </summary>
        public IRepository<FileLibrary, int> FileLibraryRepository { set; get; }

        /// <summary>
        /// 文件库查询
        /// </summary>
        public IQuery<FileLibrary> FileLibrarys
        {
            get { return FileLibraryRepository.Query(); }
        }


        /// <summary>
        /// 文件库文件上传
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult FileLibraryUpload(FileLibrary entity)
        {
            var result = ValidateFileLibrary(entity);
            if (!result.Success) return result;
            try
            {
                //if (!FileLibraryRepository.Insert(entity))
                //{
                //    return DataProcess.Failure("文件上传失败！");
                //}
            }
            catch (Exception)
            {
            }
            //添加到数据库中
            //FileLibrary file = new FileLibrary();
            //file = entity;
            FileLibraryRepository.Insert(entity);
            return DataProcess.Success("文件上传成功！", entity);
        }

        /// <summary>
        /// 文件库文件编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataResult EditFileLibrary(FileLibrary entity)
        {
            entity.CheckNotNull("entity");
            if (entity.Id == 0)
            {
                return DataProcess.Failure("文件编码无效！");
            }

            if (FileLibraryRepository.Update(a => new FileLibrary
            {
                FileName = entity.FileName,
            }, a => a.Id == entity.Id) == 0)
            {
                return DataProcess.Failure("文件({0})更新失败！".FormatWith(entity.Id));
            }

            return DataProcess.Success("文件({0})更新成功！".FormatWith(entity.Id));
        }

        /// <summary>
        /// 文件库文件移除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public DataResult RemoveFileLibrary(int Id)
        {
            if (Id == 0)
            {
                return DataProcess.Failure("文件编码无效！");
            }
            FileLibraryRepository.UnitOfWork.TransactionEnabled = true;
            FileLibrary orEntity = FileLibraryRepository.GetEntity(Id);
            if (orEntity != null)
            {
                if (FileLibraryRepository.Delete(orEntity) == 0)
                {
                    return DataProcess.Failure("文件移除失败！");
                }
                //获取图片的保存位置
                var fileAbsolutePath = FileHelper.GetAbsolutePath(orEntity.FilePath);
                if (File.Exists(fileAbsolutePath))
                {
                    try
                    {
                        File.Delete(fileAbsolutePath);
                    }
                    catch (Exception)
                    {
                        return DataProcess.Failure("文件移除失败！");
                    }
                }
            }
            FileLibraryRepository.UnitOfWork.Commit();
            return DataProcess.Success("文件移除成功！");
        }

        /// <summary>
        /// 数据验证
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private DataResult ValidateFileLibrary(FileLibrary entity)
        {
            entity.CheckNotNull("entity");
            if (entity.FileName.IsNullOrEmpty())
            {
                return DataProcess.Failure("文件名不能为空！");
            }
            if (entity.ExtensionName.IsNullOrEmpty())
            {
                return DataProcess.Failure("文件拓展名不能为空！");
            }
            if (entity.ContentType.IsNullOrEmpty())
            {
                return DataProcess.Failure("文件类型不能为空！");
            }
            if (entity.Size <= 0)
            {
                return DataProcess.Failure("文件大小数据无效！");
            }
            if (entity.FilePath.IsNullOrEmpty())
            {
                return DataProcess.Failure("文件路径不能为空！");
            }

            return DataProcess.Success();
        }
    }
}
