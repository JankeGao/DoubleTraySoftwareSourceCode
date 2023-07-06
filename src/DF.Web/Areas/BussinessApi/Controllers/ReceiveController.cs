using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Bussiness.Entitys;
using Bussiness.Enums;
using HP.Core.Logging;
using HP.Core.Sequence;
using HP.Data.Entity.Pagination;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using HPC.BaseService.Contracts;
using HPC.BaseService.Models;

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Description("模块领用订单管理")]
    public class ReceiveController : BaseApiController
    {
        /// <summary>
        /// 领用单数据库操作
        /// </summary>
        /// 
        public Bussiness.Contracts.IReceiveContract ReceiveContract { get; set; }
       /// <summary>
       /// 获取物料信息
       /// </summary>
        public Bussiness.Contracts.IMaterialContract MaterialContract { set; get; }
        
        /// <summary>
        /// 模具
        /// </summary>
        public Bussiness.Contracts.IMouldInformationContract MouldInformationContract { set; get; }

        /// <summary>
        ///自动生成编码
        /// </summary>
        public ISequenceContract SequenceContract { set; get; }

        /// <summary>
        /// 获取仓库
        /// </summary>
        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }

        /// <summary>
        /// 用户
        /// </summary>
        public IIdentityContract IdentityContract { get; set; }

        /// <summary>
        /// 获取领用单数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取领用单信息信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri] MvcPageCondition pageCondition)
        {
            var query = ReceiveContract.ReceiveDtos.Where(a => a.IsDeleted == false);
            // 查询条件，根据领用编码查询
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Code");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value)|| p.Code.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Status");
            if (filterRule != null)
            {
                int value = Convert.ToInt32(filterRule.Value.ToString());
                query = query.Where(p => p.Status == value);
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            //以倒叙方式查询显示
            var proList = query.OrderByDesc(a => a.CreatedTime).ToPage(pageCondition);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, proList.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取仓库信息")]
        [HttpGet]
        public HttpResponseMessage GetWareHouseList()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, WareHouseContract.WareHouses.ToList().ToMvcJson());
            return response;
        }


        /// <summary>
        /// 获取领用明细信息
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取领用模具明细信息")]
        [HttpGet]
        public HttpResponseMessage GetReceiveMaterialList(string Code)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ReceiveContract.ReceiveDetailDtos.Where(a => a.ReceiveCode == Code).ToList().ToMvcJson());
            var va = ReceiveContract.ReceiveDetailDtos.Where(a => a.ReceiveCode == Code).ToList();
            return response;
        }


        /// <summary>
        /// 获取物料信息
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取物料信息")]
        [HttpGet]
        public HttpResponseMessage GetMaterialList(string KeyValue)
        {
            var list = MaterialContract.Materials.Where(a => a.Code.Contains(KeyValue) || a.Name.Contains(KeyValue));
            var aa = list.Take(20).ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, aa.ToMvcJson());
            return response;
        }

        /// <summary>
        /// 接口同步入库单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "同步领用库单")]
        public HttpResponseMessage GetInterfaceReceive()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ReceiveContract.CreateReceiveInterFace().ToMvcJson());
            return response;
        }

        /// <summary>
        /// 导入领用单信息
        /// </summary>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "导入入库信息")]
        [HttpPost]
        public HttpResponseMessage DoUpLoadInInfo()
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            HttpPostedFile file = files[0];//取得第一个文件
            if (file == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请选择导入文件！").ToMvcJson());
            }

            var extensionName = System.IO.Path.GetExtension(file.FileName);
            if (extensionName != ".xls" && extensionName != ".xlsx")
            {
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请导入Excel文件！").ToMvcJson());
            }

            try
            {
                var tb = Bussiness.Common.ExcelHelper.ReadExeclToDataTable("sheet1", 0, file.InputStream);
                if (tb.Rows.Count <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("Excel无内容").ToMvcJson());
                }

                Bussiness.Entitys.Receive ReceiveEntity = new Bussiness.Entitys.Receive();

                ReceiveEntity.BillCode= tb.Rows[0]["来源单号"].ToString();
                ReceiveEntity.WareHouseCode = tb.Rows[0]["仓库编码"].ToString();
                ReceiveEntity.LastTimeReceiveName = tb.Rows[0]["领用人"].ToString();
                ReceiveEntity.ReceiveType =  tb.Rows[0]["领用类型"].ToInt();
                ReceiveEntity.Remarks = tb.Rows[0]["领用备注"].ToString();
                ReceiveEntity.LastTimeReceiveDatetime = Convert.ToDateTime(tb.Rows[0]["领用时间"].ToString());

                // 核查领用类型
                if (ReceiveEntity.Status >= 3 )
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("领用类型不正确，请导入[0-2]之间的数字，领用类型：" + ReceiveEntity.Status).ToMvcJson());
                }
                // 核查领用时间
                if (string.IsNullOrEmpty(ReceiveEntity.LastTimeReceiveDatetime.ToString()))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("领用时间格式不正确，领用时间：" + ReceiveEntity.LastTimeReceiveDatetime).ToMvcJson());
                }
                ReceiveEntity.PredictReturnTime = Convert.ToDateTime(tb.Rows[0]["预计归还时间"].ToString());
                // 核查预计归还时间
                if (string.IsNullOrEmpty(ReceiveEntity.PredictReturnTime.ToString()))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("预计归还时间格式不正确，预计归还时间：" + ReceiveEntity.PredictReturnTime).ToMvcJson());
                }
                if (!WareHouseContract.WareHouses.Any(a => a.Code == ReceiveEntity.WareHouseCode))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("仓库编码:" + ReceiveEntity.WareHouseCode + "系统不存在").ToMvcJson());
                }
                if (!IdentityContract.Users.Any(a => a.Name == ReceiveEntity.LastTimeReceiveName))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("用户:" + ReceiveEntity.LastTimeReceiveName + "系统不存在").ToMvcJson());
                }
                User user = IdentityContract.Users.FirstOrDefault(a => a.Name == ReceiveEntity.LastTimeReceiveName);
                ReceiveEntity.LastTimeReceiveName = user.Code;
                ReceiveEntity.CreatedTime = DateTime.Now;
                ReceiveEntity.IsDeleted = false;
                ReceiveEntity.Status = 0;
                ReceiveEntity.AddMaterial = new List<Bussiness.Entitys.ReceiveDetail>();
                ReceiveEntity.Code = SequenceContract.Create(ReceiveEntity.GetType());  // 自动生成编码

                foreach (DataRow item in tb.Rows)
                {
                    Bussiness.Entitys.ReceiveDetail inmaterial = new Bussiness.Entitys.ReceiveDetail();
                    
                    inmaterial.ReceiveCode = ReceiveEntity.Code;
                    inmaterial.IsDeleted = false;
                    inmaterial.Status = ReceiveEntity.Status;
                    inmaterial.LocationCode = item["货柜/储位"].ToString();
                    inmaterial.MaterialLabel = item["模具条码"].ToString();
                    if (MouldInformationContract.MouldInformations.FirstOrDefault(a => a.MaterialLabel == inmaterial.MaterialLabel) == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("模具条码:" + inmaterial.MaterialLabel + "系统不存在").ToMvcJson());
                    }                                     
                    inmaterial.Quantity = Convert.ToDecimal(item["数量"].ToString());
                    // 核查导入数量
                    if (inmaterial.Quantity <= 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请输入物料数量，物料：" + inmaterial.Quantity).ToMvcJson());
                    }
                    inmaterial.Status = 0;
                    ReceiveEntity.AddMaterial.Add(inmaterial);
                }
                return Request.CreateResponse(HttpStatusCode.OK, ReceiveContract.CreateReceive(ReceiveEntity).ToMvcJson());              
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Equals("输入字符串的格式不正确。"))
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        DataProcess.Failure("数量字段中含有非法字符，请核查数量格式后重新导入").ToMvcJson());
                }
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(ex.Message).ToMvcJson());
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "删除领用单")]
        [HttpPost]
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.Receive entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ReceiveContract.RemoveReceive(entity.Id).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 添加数据时调用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public HttpResponseMessage PostDoCreate(Receive entity)
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ReceiveContract.CreateReceive(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 编辑领用单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "编辑领用单")]
        [HttpPost]
        public HttpResponseMessage PostDoUpdate(Bussiness.Entitys.Receive entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ReceiveContract.EditReceive(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 作废领用单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "作废领用单")]
        [HttpPost]
        public HttpResponseMessage PostDoCancellatione(Bussiness.Entitys.Receive entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, ReceiveContract.CancellationeReceive(entity).ToMvcJson());
            return response;
        }

        /// <summary>
        /// 从WebAPI下载文件领用单模板
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DoDownLoadTemp()
        {
            try
            {
                string path = "/Assets/themes/Excel/Receive.xlsx";
                string filePath = HP.Utility.Files.FileHelper.GetAbsolutePath(path);
                var stream = new FileStream(filePath, FileMode.Open);
                //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                //response.Content = new StreamContent(stream);
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                //{
                //    FileName = "领用单的导入模版.xlsx"
                //};
                //return response;
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = $"领用单导入模版.xlsx";
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}

