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

namespace DF.Web.Areas.BussinessApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Description("模块领用订单管理")]
    public class RecipientsOrdersController : BaseApiController
    {
        /// <summary>
        /// 领用单数据库操作
        /// </summary>
        /// 
        public Bussiness.Contracts.IRecipientsOrdersContract RecipientsOrdersContract { get; set; }
        
        /// <summary>
        /// 获取仓库
        /// </summary>
        public Bussiness.Contracts.IWareHouseContract WareHouseContract { set; get; }

        public ISequenceContract SequenceContract { set; get; }

        /// <summary>
        /// 获取领用单数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取领用单信息信息")]
        [HttpGet]
        public HttpResponseMessage GetPageRecords([FromUri] MvcPageCondition pageCondition)
        {
            var query = RecipientsOrdersContract.RecipientsOrderss.Where(a => a.IsDeleted == false);
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
                query = query.Where(p => p.RecipientsOrdersState == value);
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
        /// <param name="InCode"></param>
        /// <returns></returns>
        [LogFilter(Type = LogType.Operate, Name = "获取领用模具信息")]
        [HttpGet]
        public HttpResponseMessage GetRecipientsOrdersMaterialList(string InCode)
        {
            var str = InCode;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, RecipientsOrdersContract.RecipientsOrdersDtoList.Where(a => a.InCode == InCode).ToList().ToMvcJson());
            return response;
        }


        /// <summary>
        /// 导入入库信息
        /// </summary>
        /// <param name="entity"></param>
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
                foreach (DataRow item in tb.Rows)
                {
                    Bussiness.Entitys.RecipientsOrders entity = new Bussiness.Entitys.RecipientsOrders();

                    if (string.IsNullOrEmpty(item["领用单号"].ToString()) || string.IsNullOrEmpty(item["明细编码"].ToString()))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                            DataProcess.Failure("导入的文件中含有”领用单号“或”明细编码“为空的数据，请先确保领用单号或明细编码不为空，再进行导入！"));
                    }
                    entity.Code = item["领用单号"].ToString();
                    var list = RecipientsOrdersContract.RecipientsOrderss.Where(a => a.Code == entity.Code);
                    if (list.Count() > 0)
                    {
                        if (list.Any(a => a.IsDeleted == false))
                        {
                            string result = "领用单号：" + item["领用单号"].ToString() + "已存在";
                            return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure(string.Format(result)));
                        }
                    }

                    entity.Code = item["领用单号"].ToString();
                    entity.LastTimeReceiveName = item["领用人"].ToString();

                    entity.LastTimeReceiveDatetime = item["领用时间"].ToString();
                    // 核查领用时间
                    if (string.IsNullOrEmpty(entity.LastTimeReceiveDatetime))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请输入领用时间格式不正确，领用时间：" + entity.LastTimeReceiveDatetime).ToMvcJson());
                    }
                    entity.PredictReturnTime = item["预计归还时间"].ToString();
                    // 核查预计归还时间
                    if (string.IsNullOrEmpty(entity.PredictReturnTime))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请输入预计归还时间格式不正确，预计归还时间：" + entity.PredictReturnTime).ToMvcJson());
                    }
                    entity.LastTimeReturnName = item["归还人"].ToString();
                    entity.IsDeleted = false;
                    entity.LastTimeReturnDatetime = item["归还时间"].ToString();
                    // 核查归还时间
                    if (string.IsNullOrEmpty(entity.LastTimeReturnDatetime))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请输入归还日期格式不正确，归还日期：" + entity.LastTimeReturnDatetime).ToMvcJson());
                    }
                    RecipientsOrdersContract.RecipientsOrdersRepository.Insert(entity);

                    entity.RecipientsOrdersQuantity = item["领用数量"].ToInt();
                    // 核查导入数量
                    if (entity.RecipientsOrdersQuantity <= 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Failure("请输入领用数量，数量：" + entity.RecipientsOrdersQuantity).ToMvcJson());
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, DataProcess.Success());
                
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
        public HttpResponseMessage PostDoDelete(Bussiness.Entitys.In entity)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, RecipientsOrdersContract.RemoveRecipientsOrders(entity.Id).ToMvcJson());
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
                string path = "/Assets/themes/Excel/RecipientsOrders.xlsx";
                string filePath = HP.Utility.Files.FileHelper.GetAbsolutePath(path);
                var stream = new FileStream(filePath, FileMode.Open);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "领用单的导入模版.xlsx"
                };
                return response;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}

