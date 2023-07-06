using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Bussiness.Common
{
    public class ExcelHelper
    {
        /// <summary>
        /// Excel转换为DataTable(忽视空行)
        /// </summary>
        /// <param name="sheetName">Excel表名</param>
        /// <param name="startRowIndex">开始行(从零开始计数)()</param>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public static DataTable ReadExeclToDataTable(string sheetName, int startRowIndex, Stream fileStream)
        {
            try
            {
                IWorkbook doc = WorkbookFactory.Create(fileStream);
                ISheet sheet = doc.GetSheet(sheetName);

                if (sheet == null)
                {
                    throw new Exception("工作表名：" + sheetName + "，不存在！请将需要导入数据所在的工作表名改为" + sheetName + "！");
                }

                DataTable dt = new DataTable();

                #region 获取表头

                IRow headerRow = sheet.GetRow(startRowIndex);
                int cellCount = headerRow.LastCellNum;

                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn dc = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    dt.Columns.Add(dc);
                }

                #endregion

                #region 获取数据

                int rowCount = sheet.LastRowNum;
                for (int i = (startRowIndex + 1); i <= rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    DataRow dr = dt.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            dr[j] = row.GetCell(j).ToString();
                        }
                    }
                    dt.Rows.Add(dr);
                }

                #endregion

                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// 将List转为Excel
        /// </summary>
        /// <param name="list">数据</param>
        /// <param name="fileName">文件名</param>
        /// <param name="dicFields">字段名称字典</param>
        /// <returns></returns>
        public static HSSFWorkbook ListToExecl(IList list, string fileName, Dictionary<string, string> dicFields)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            var sheet = hssfworkbook.CreateSheet("Sheet1");

            int length = dicFields.Count;
            string[] fieldname = new string[length];
            int i;

            #region 定义字体

            IFont font = hssfworkbook.CreateFont();//普通数据字体
            font.FontName = "宋体";
            font.FontHeightInPoints = 11;

            IFont fontTitle = hssfworkbook.CreateFont();//标题字体
            fontTitle.FontName = "宋体";
            fontTitle.FontHeightInPoints = 11;
            fontTitle.Boldweight = (short)FontBoldWeight.Bold;

            #endregion

            #region 定义单元格

            ICellStyle style = hssfworkbook.CreateCellStyle();
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Left;

            ICellStyle styleTitle = hssfworkbook.CreateCellStyle();
            styleTitle.SetFont(fontTitle);
            styleTitle.Alignment = HorizontalAlignment.Center;
            ICellStyle styleDecimal = hssfworkbook.CreateCellStyle();
            IDataFormat formatDecimal = hssfworkbook.CreateDataFormat();
            styleDecimal.DataFormat = formatDecimal.GetFormat("0.0000");

            #endregion

            #region 全局

            sheet.DefaultColumnWidth = 14;          //全局列宽
            sheet.DefaultRowHeightInPoints = 15;    //全局行高
            sheet.CreateFreezePane(0,2, 0, 2);     //冻结前两行

            #endregion

            #region 大标题

            var rowBigTitle = sheet.CreateRow(0);
            var cell = rowBigTitle.CreateCell(0);
            cell.SetCellValue(fileName.Split('.')[0]);
            cell.CellStyle = styleTitle;
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, length - 1));

            #endregion

            #region 添加表头

            var rowTitle = sheet.CreateRow(1);

            if (length > 0)
            {
                i = 0;
                foreach (var kvp in dicFields)
                {
                    fieldname[i] = kvp.Key;
                    rowTitle.CreateCell(i).SetCellValue(kvp.Value);
                    
                    rowTitle.Cells[i].CellStyle = styleTitle;
                    i++;
                }
            }
            else
            {
                return null;//TODO 报错或者全部从list取
            }

            #endregion

            #region 添加数据

            int k = 2;
            foreach (var obj in list)
            {
                var rowData = sheet.CreateRow(k);
                for (int j = 0; j < length; j++)
                {
                    var pi = obj.GetType().GetProperty(fieldname[j]);
                    if (pi != null)
                    {
                        var value = pi.GetValue(obj, null);
                        if (value != null)
                        {
                            rowData.CreateCell(j).SetCellValue(value.ToString());
                        }
                    }
                }
                k++;
            }

            #endregion

            #region 自适应列宽

            for (i = 0; i < length; i++)
            {
                sheet.AutoSizeColumn(i);
            }
            //第一列特殊，不能用自适应，原因可能是第一行合并多列，强制设置宽度
            sheet.SetColumnWidth(0, 5000);

            #endregion

            return hssfworkbook;
        }

        /// <summary>
        /// 下载输出
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <param name="list">数据</param>
        /// <param name="fileName">文件名</param>
        /// <param name="dicFields">字段名称字典</param>
        public static void DownExcel(HttpResponseBase response, HttpRequestBase request, IList list, string fileName, Dictionary<string, string> dicFields)
        {
            var excelFile = ListToExecl(list, fileName, dicFields);

            response.ContentType = "application/vnd.ms-excel";
            //通知浏览器下载文件而不是打开
            if (request.UserAgent != null && request.UserAgent.ToLower().IndexOf("firefox", StringComparison.Ordinal) > 0)
            {
                response.AddHeader("Content-Disposition", "attachment;  filename=\"" + fileName + "\"");
                //response.AddHeader("Content-Disposition", "attachment;  filename*=utf-8'zh_cn'" + fileName);
            }
            else
            {
                fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
                response.AddHeader("Content-Disposition", "attachment;  filename=" + fileName);
            }
            excelFile.Write(response.OutputStream);
            response.OutputStream.Flush();
            response.OutputStream.Close();
            response.Flush();
            response.End();
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <param name="path"></param>
        public static void DownloadExcelTemplate(HttpResponseBase response, HttpRequestBase request, string path)
        {
            string fileName = Path.GetFileName(path);

            FileStream fs = File.Open(path, FileMode.Open);

            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            response.ContentType = "application/vnd.ms-excel";
            //通知浏览器下载文件而不是打开
            if (request.UserAgent != null && request.UserAgent.ToLower().IndexOf("firefox", StringComparison.Ordinal) > 0)
            {
                response.AddHeader("Content-Disposition", "attachment;  filename=\"" + fileName + "\"");
                //response.AddHeader("Content-Disposition", "attachment;  filename*=utf-8'zh_cn'" + fileName);
            }
            else
            {
                fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
                response.AddHeader("Content-Disposition", "attachment;  filename=" + fileName);
            }
            response.BinaryWrite(bytes);
            response.Flush();
            response.End();
        }


        #region DataTable列名转换

        /// <summary>
        /// DataTable列名转换
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static DataTable TransformColumnName(DataTable dataTable, Dictionary<string, string> dictionary)
        {
            foreach (var keyValuePair in dictionary)
            {
                if (dataTable.Columns.Contains(keyValuePair.Key))
                {
                    dataTable.Columns[keyValuePair.Key].ColumnName = keyValuePair.Value;
                }
            }

            return dataTable;
        }

        #endregion

        #region DataTableToObjectList
        //TODO：需要移到正确位置

        /// <summary>
        /// DataTable数据转成ListEntity
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="dt">数据集</param>
        /// <returns></returns>
        public static List<T> DataTableToObjectList<T>(DataTable dt) where T : new()
        {
            var list = new List<T>();

            //初始化propertyinfo
            Type objtype = new T().GetType();
            PropertyInfo[] propertyInfos = objtype.GetProperties();

            foreach (DataRow dr in dt.Rows)
            {
                T entity = new T();

                //填充entity类的属性
                foreach (var propertyInfo in propertyInfos)
                {
                    if (dt.Columns.Contains(propertyInfo.Name))
                    {
                        string value = dr[propertyInfo.Name].ToString();
                        if (!string.IsNullOrEmpty(value) && propertyInfo.PropertyType != typeof(bool?))
                        {
                            if (propertyInfo.PropertyType == typeof(DateTime?))
                            {
                                propertyInfo.SetValue(entity, (DateTime?)DateTime.Parse(value), null);
                            }
                            else if (propertyInfo.PropertyType == typeof(int?))
                            {
                                propertyInfo.SetValue(entity, Convert.ToInt32(value), null);
                            }
                            else if (propertyInfo.PropertyType == typeof(decimal?))
                            {
                                propertyInfo.SetValue(entity, Convert.ToDecimal(value), null);
                            }
                            else
                            {
                                propertyInfo.SetValue(entity, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                            }
                        }
                        else if (propertyInfo.PropertyType == typeof(bool?))
                        {
                            if (value == "否" || value == "0" || value == "")
                            {
                                propertyInfo.SetValue(entity, false, null);
                            }
                            else
                            {
                                propertyInfo.SetValue(entity, true, null);
                            }
                        }
                    }
                }

                list.Add(entity);
            }

            return list;
        }

        #endregion
    }
}