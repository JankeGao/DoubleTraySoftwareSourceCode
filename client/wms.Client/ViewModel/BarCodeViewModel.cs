using System;
using System.Collections.ObjectModel;
using System.Drawing;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using wms.Client.LogicCore.Common;
using System.Windows;
using Bussiness.Contracts;
using Bussiness.Entitys;
using HP.Core.Dependency;
using HP.Utility;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Seagull.BarTender.Print;
using wms.Client.Core.Interfaces;
using wms.Client.LogicCore.Configuration;
using wms.Client.Model.Entity;

namespace wms.Client.ViewModel
{
    /// <summary>
    /// 操作人员登录
    /// </summary>
    public class BarCodeViewModel : ViewModelBase
    {

        /// <summary>
        /// 条码契约
        /// </summary>
        private readonly ILabelContract LabelContract;


        private readonly IMaterialContract MaterialContract;


        public BarCodeViewModel()
        {
            LabelContract = IocResolver.Resolve<ILabelContract>();
            MaterialContract= IocResolver.Resolve<IMaterialContract>();
            this.init();
        }


        private string _btw_path = System.IO.Path.Combine(Environment.CurrentDirectory, "Demo.btw");

        /// <summary>
        /// 当前操作储位
        /// </summary>
        private int _Number = 1;
        public int Number
        {
            get { return _Number; }
            set { _Number = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 进度报告
        /// </summary>
        private string _Report;
        public string Report
        {
            get { return _Report; }
            set { _Report = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 实体
        /// </summary>
        public LabelClient LabelEntity { get; set; } = new LabelClient();



        /// <summary>
        /// 打印机分组
        /// </summary>
        private ObservableCollection<Printer> _PrintGroups = new ObservableCollection<Printer>();

        /// <summary>
        /// 已加载模块
        /// </summary>
        public ObservableCollection<Printer> PrintGroups
        {
            get { return _PrintGroups; }
            set { _PrintGroups = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 搜索条码
        /// </summary>
        private string selectPrint = string.Empty;
        public string SelectPrint
        {
            get { return selectPrint; }
            set { selectPrint = value; RaisePropertyChanged(); }
        }

        #region 命令(Binding Command)

        private RelayCommand _printCommand;

        public RelayCommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new RelayCommand(() => PrintBarCode());
                }
                return _printCommand;
            }
        }

        private RelayCommand _selectPrintCommand;

        public RelayCommand SelectPrintCommand
        {
            get
            {
                if (_selectPrintCommand == null)
                {
                    _selectPrintCommand = new RelayCommand(() => SelectPrintFnc());
                }
                return _selectPrintCommand;
            }
        }

        private RelayCommand _exitCommand;

        public RelayCommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(() => ApplicationShutdown());
                }
                return _exitCommand;
            }
        }

        private RelayCommand _MinusCommand;

        public RelayCommand MinusCommand

        {
            get
            {
                if (_MinusCommand == null)
                {
                    _MinusCommand = new RelayCommand(() => Minus());
                }
                return _MinusCommand;
            }
        }

        private RelayCommand _AddCommand;

        public RelayCommand AddCommand

        {
            get
            {
                if (_AddCommand == null)
                {
                    _AddCommand = new RelayCommand(() => Add());
                }
                return _AddCommand;
            }
        }
        private RelayCommand _BarCodeCloseDialogCommand;

        public RelayCommand BarCodeCloseDialogCommand

        {
            get
            {
                if (_BarCodeCloseDialogCommand == null)
                {
                    _BarCodeCloseDialogCommand = new RelayCommand(() => CloseForm());
                }
                return _BarCodeCloseDialogCommand;
            }
        }

        #endregion

        #region Print


        public async void init()
        {
            Printers printers = new Printers();
            foreach (Printer printer in printers)
            {
                PrintGroups.Add(printer);
            }

            if (printers.Count > 0)
            {
                SelectPrint = printers.Default.PrinterName;
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        public async void Add()
        {
            Number = Number + 1;
        }
        /// <summary>
        /// 减少
        /// </summary>
        public async void Minus()
        {
            if (Number - 1 == 0)
            {
                return;
            }
            Number = Number - 1;
        }
        public async void SelectPrintFnc()
        {

        }

        /// <summary>
        /// 标签打印
        /// </summary>
        public async void PrintBarCode()
        {
            try
            {
                if (SelectPrint == "")
                {
                    this.Report = "请选择打印机";
                    return;
                }

                this.Report = "条码打印中";

                using (Engine btEngine = new Engine(true))
                {
                    LabelFormatDocument labelFormat = btEngine.Documents.Open(_btw_path);
                    var material = MaterialContract.Materials.FirstOrDefault(a => a.Code == LabelEntity.MaterialCode);
                    for (var i = 0; i < Number; i++)
                    {
                        if (String.IsNullOrEmpty(LabelEntity.LabelCode))
                        {
                            // 生成条码
                            var label = new Label()
                            {
                                MaterialCode = LabelEntity.MaterialCode,
                                BatchCode = LabelEntity.BatchCode,
                                SupplierCode = LabelEntity.SupplyCode,
                                Quantity = (decimal)LabelEntity.Quantity
                            };

                            // 生成条码
                            var labelService = ServiceProvider.Instance.Get<ILabelService>();

                            // 物料实体映射
                            var createLabel = await labelService.PostCreateLabel(label);
                            if (createLabel.Success)
                            {
                                Label entity = JsonHelper.DeserializeObject<Label>(createLabel.Data.ToString());

                                try
                                {
                                    labelFormat.SubStrings.SetSubString("MaterialName", LabelEntity.MaterialName);
                                    labelFormat.SubStrings.SetSubString("MaterialCode", LabelEntity.MaterialCode);
                                    labelFormat.SubStrings.SetSubString("Quantity", LabelEntity.Quantity.ToString());
                                    //labelFormat.SubStrings.SetSubString("BatchCode", LabelEntity.BatchCode);
                                    labelFormat.SubStrings.SetSubString("MaterialLabel", entity.Code);
                                    labelFormat.SubStrings.SetSubString("MaterialUnit", material.Unit);
                                }

                                catch (Exception ex)
                                {
                                    this.Report = "未获取到条码信息";
                                    return;
                                }

                                labelFormat.PrintSetup.PrinterName = SelectPrint;
                                labelFormat.Print("BarPrint" + DateTime.Now, 3 * 1000);
                            }
                            else
                            {
                                this.Report = createLabel.Message;
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                labelFormat.SubStrings.SetSubString("MaterialName", LabelEntity.MaterialName);
                                labelFormat.SubStrings.SetSubString("MaterialCode", LabelEntity.MaterialCode);
                                labelFormat.SubStrings.SetSubString("Quantity", LabelEntity.Quantity.ToString());
                                //labelFormat.SubStrings.SetSubString("SupplyName", LabelEntity.SupplyName);
                                //labelFormat.SubStrings.SetSubString("BatchCode", LabelEntity.BatchCode);
                                labelFormat.SubStrings.SetSubString("MaterialLabel", LabelEntity.LabelCode);
                                labelFormat.SubStrings.SetSubString("MaterialUnit", material.Unit);
                            }

                            catch (Exception ex)
                            {
                                this.Report = "未获取到条码信息";
                                return;
                            }

                            labelFormat.PrintSetup.PrinterName = SelectPrint;
                            labelFormat.Print("BarPrint" + DateTime.Now, 3 * 1000);

                        }
                    }

                    this.Report = "";
                }
             //   DialogHost.CloseDialogCommand.Execute(null, null);

                #endregion
            }
            catch (Exception ex)
            {
                this.Report = ex.Message;
            }
        }

        public async void CloseForm()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        /// <summary>
        /// 关闭系统
        /// </summary>
        public void ApplicationShutdown()
        {
            Messenger.Default.Send("", "ApplicationShutdown");
        }


    }
}
