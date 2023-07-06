using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HP.Core.Mapping;
using HP.Data.Entity.Pagination;
using HP.Utility.Data;
using HP.Utility.Extensions;
using HP.Web.Api;
using HP.Web.Mvc.Extensions;
using HP.Web.Mvc.Interceptor;
using HP.Web.Mvc.Pagination;
using HPC.BaseService.Contracts;
using HPC.BaseService.Dtos;
using HPC.BaseService.Models;

namespace DF.Web.Areas.BaseApi.Controllers
{
    [Description("通用字典")]
    public class DictionaryController : BaseApiController
    {
        public IDictionaryContract DictionaryContract { set; get; }
        public IMapper Mapper { set; get; }


        #region 查询

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        public HttpResponseMessage GetDictionaryList([FromUri]MvcPageCondition pageCondition)
        {
            var query = DictionaryContract.Dictionaries;
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Name");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value)||p.Name.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var pageResult = query.ToPage(pageCondition).ToMvcJson();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, pageResult);
            return response;
        }


        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="pageCondition"></param>
        /// <returns></returns>
        public HttpResponseMessage GetDictionaryTypeList([FromUri]MvcPageCondition pageCondition)
        {
            var query = DictionaryContract.DictionaryTypes.Where(a=>string.IsNullOrEmpty(a.ParentCode));
            var filterRule = pageCondition.FilterRuleCondition.Find(a => a.Field == "Name");
            if (filterRule != null)
            {
                string value = filterRule.Value.ToString();
                query = query.Where(p => p.Code.Contains(value)||p.ParentCode.Contains(value)||p.Name.Contains(value));
                pageCondition.FilterRuleCondition.Remove(filterRule);

            }
            var pageResult = query.ToPage(pageCondition);//.ToMvcJson();

            List<DictionaryType> typeList = new List<DictionaryType>();
            List<string> sqlStr = new List<string>();
            sqlStr = pageResult.Rows.Select(a => a.Code).ToList();
          
            typeList = DictionaryContract.DictionaryTypes.Where(a => sqlStr.Contains(a.ParentCode)).ToList();
            foreach (var item in pageResult.Rows)
            {
                typeList.Add(item);
            }
            var list = InitDictionTypeTree(typeList, "");
            PageResult<DictionartTypeTreeOutputDto> result = new PageResult<DictionartTypeTreeOutputDto>();
            result.Rows = list;
            result.Pages = pageResult.Pages;
            result.PageSize = pageResult.PageSize;
            result.PageIndex = pageResult.PageIndex;
            result.Total = pageResult.Total;

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.ToMvcJson());



            return response;
        }

        public HttpResponseMessage GetDictionaryTypeList1()
        {
            var query = DictionaryContract.DictionaryTypes.ToList();
      

            var list = InitDictionTypeTree(query, "");


            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, list.ToMvcJson());



            return response;
        }


        /// <summary>
        /// 根据类别获取字典编码
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public HttpResponseMessage GetDictionaryByType(string type)
        {
            return Request.CreateResponse(HttpStatusCode.OK, DictionaryContract.Dictionaries.Where(a => a.TypeCode == type && a.Enabled).ToList().ToMvcJson()); ;
        }

        [HttpPost]
        [LogFilter(Name = "创建字典")]
        public HttpResponseMessage PostDoCreate(Dictionary entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, DictionaryContract.CreateDictionary(entity).ToMvcJson());
        }
        [HttpPost]
        [AuthorizationFilter(ActionName = "Edit")]
        [LogFilter(Name = "字典编辑")]
        public HttpResponseMessage PostDoEdit(Dictionary entity)
        {
            //   return DictionaryContract.EditDictionary(entity).ToMvcJson();
            return Request.CreateResponse(HttpStatusCode.OK, DictionaryContract.EditDictionary(entity).ToMvcJson());
        }


        [HttpPost]
        [LogFilter(Name = "创建字典类别")]
        public HttpResponseMessage PostDoCreateType(DictionaryType entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, DictionaryContract.CreateDictionaryType(entity).ToMvcJson());
        }

        [HttpPost]
        [AuthorizationFilter(ActionName = "Edit")]
        [LogFilter(Name = "字典类别编辑")]
        public HttpResponseMessage PostDoEditType(DictionaryType entity)
        {
         //   return DictionaryContract.EditDictionary(entity).ToMvcJson();
            return Request.CreateResponse(HttpStatusCode.OK, DictionaryContract.EditDictionaryType(entity).ToMvcJson());
        }

        [Description("获取字典l类别树")]
        public HttpResponseMessage GetDictionaryTreeList()
        {
            var dictionaries = DictionaryContract.DictionaryTypes.OrderBy(a=>a.Sort).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, InitTypeTree(dictionaries, null).ToMvcJson());
        }

        [Description("获取字典树信息")]
        public HttpResponseMessage GetTreeByType(string typeCode)
        {
            var dictionaries = DictionaryContract.Dictionaries.Where(a => a.TypeCode == typeCode).OrderBy(a => a.Sort)
                .ToList();
            return Request.CreateResponse(HttpStatusCode.OK, InitTree(dictionaries, null).ToMvcJson());
        }

        private List<DictionaryTree> InitTree(List<Dictionary> list, string parentCode)
        {
            List<DictionaryTree> trees = new List<DictionaryTree>();
            foreach (var type in list.Where(a => a.ParentCode == parentCode))
            {
                DictionaryTree tree = Mapper.MapTo<DictionaryTree>(type);
          //      tree.State = list.Any(a => a.ParentCode == type.Code) ? TreeStateEnum.Closed.ToString().ToLower() : TreeStateEnum.Open.ToString().ToLower();
                tree.TreeId = type.Code;
                tree.TreeText = type.Name;
                tree.Children = InitTree(list, type.Code);
                trees.Add(tree);
            }
            return trees;
        }

        private List<DictionaryTree> InitTypeTree(List<DictionaryType> list, string parentCode)
        {
            List<DictionaryTree> trees = new List<DictionaryTree>();
            if (string.IsNullOrEmpty(parentCode))
            {
                foreach (var type in list.Where(a => string.IsNullOrEmpty(a.ParentCode)))
                {
                    DictionaryTree tree = Mapper.MapTo<DictionaryTree>(type);
                    //      tree.State = list.Any(a => a.ParentCode == type.Code) ? TreeStateEnum.Closed.ToString().ToLower() : TreeStateEnum.Open.ToString().ToLower();
                    tree.TreeId = type.Code;
                    tree.TreeText = type.Name;
                    tree.Children = InitTypeTree(list, type.Code);
                    trees.Add(tree);
                }
            }
            else
            {
                foreach (var type in list.Where(a => a.ParentCode==parentCode))
                {
                    DictionaryTree tree = Mapper.MapTo<DictionaryTree>(type);
                    //      tree.State = list.Any(a => a.ParentCode == type.Code) ? TreeStateEnum.Closed.ToString().ToLower() : TreeStateEnum.Open.ToString().ToLower();
                    tree.TreeId = type.Code;
                    tree.TreeText = type.Name;
                    tree.Children = InitTypeTree(list, type.Code);
                    trees.Add(tree);
                }
            }
            return trees;
        }

        [HttpPost]
        [LogFilter(Name = "删除字典类别")]
        public HttpResponseMessage PostDodDeleteType(DictionaryType entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, DictionaryContract.RemoveDictionaryType(entity.Id).ToMvcJson());
        }
        [HttpPost]
        [LogFilter(Name = "删除字典")]
        public HttpResponseMessage PostDodDelete(Dictionary entity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, DictionaryContract.RemoveDictionary(entity.Id).ToMvcJson());
        }


        /// <summary>
        /// 初始化模块功能树
        /// </summary>
        /// <param name="modules">权限模块</param>
        /// <param name="parentCode">上级模块编码</param>
        /// <returns></returns>
        public List<DictionartTypeTreeOutputDto> InitDictionTypeTree(List<DictionaryType> types, string parentCode)
        {
            List<DictionartTypeTreeOutputDto> result = new List<DictionartTypeTreeOutputDto>();

            List<DictionaryType> tmpModules = null;
            if (parentCode.IsNullOrEmpty())
            {
                tmpModules = types.FindAll(a => string.IsNullOrEmpty(a.ParentCode));
            }
            else
            {
                tmpModules = types.FindAll(a => a.ParentCode == parentCode);
            }

            foreach (var module in tmpModules)
            {
                DictionartTypeTreeOutputDto tree =new DictionartTypeTreeOutputDto ()
                {
                    Id = module.Id,
                    Code = module.Code,
                    Name = module.Name,
                    ParentCode = module.ParentCode,
                    Remark =module.Remark,
                    CreatedTime = module.CreatedTime,
                    CreatedUserCode=module.CreatedUserCode,
                    CreatedUserName=module.CreatedUserName,
                    Sort = module.Sort,
                    Enabled = module.Enabled


                };

                if (types.Exists(a => a.ParentCode == module.Code))
                {
                    tree.State = TreeStateEnum.Open.ToString().ToLower();
                    tree.children = InitDictionTypeTree(types, module.Code);
                }
                else
                {
                    tree.State = TreeStateEnum.Open.ToString().ToLower();
                    tree.Checked = module.Enabled;
                }

                result.Add(tree);
            }

            return result;
        }
        #endregion
    }
}
