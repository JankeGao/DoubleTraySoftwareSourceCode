// 提供基于 BarTender 的客户端打印功能。
/* eslint-disable */
window.BarTenderPrintClient = (function () {

    var BarTenderPrintClient = {}

    BarTenderPrintClient.ClientPrintExceptionMessage =
    {
        // BarTender ActiveX 控件未安装。
        BarTenderPrintClientNotInstalled: "BarTender ActiveX 控件未安装。"
    }

    BarTenderPrintClient.ClientPrintExceptionErrorCode =
    {
        // BarTender ActiveX 控件未安装
        BarTenderPrintClientNotInstalled: 0,

        // 许可信息获取失败
        PrintLicenseCreationFailed: 1,

        // 打印失败
        SpoolingFailed: 2
    }

    BarTenderPrintClient.ClientPrintException = function (message, errorCode) {
        /// <signature>
        ///   <summary>客户端打印异常。</summary>
        ///   <param name="message" type="String">错误信息。</param>
        ///   <param name="errorCode" type="ClientPrintExceptionErrorCode">错误码。</param>
        /// </signature>

        this.Message = message;
        this.ErrorCode = errorCode;
    }

    BarTenderPrintClient.CreatePrinterObject = function () {
        /// <signature>
        ///   <summary>创建 Printer 对象。</summary>
        ///   <returns type="ActiveXObject"/>
        /// </signature>

        var printer;

        try {
            printer = new ActiveXObject("BarTenderPrintClient.Printer");
            if (!printer)
                throw (0);
        }
        catch (ex) {
            throw new BarTenderPrintClient.ClientPrintException(BarTenderPrintClient.ClientPrintExceptionMessage.BarTenderPrintClientNotInstalled, BarTenderPrintClient.ClientPrintExceptionErrorCode.BarTenderPrintClientNotInstalled);
        }

        return printer;
    }

    BarTenderPrintClient.CreatePrintersObject = function () {
        /// <signature>
        ///   <summary>创建 Printers 对象。</summary>
        ///   <returns type="ActiveXObject"/>
        /// </signature>

        var printers;

        try {
            printers = new ActiveXObject("BarTenderPrintClient.Printers");
            if (!printers)
                throw (0);
        }
        catch (ex) {
            throw new BarTenderPrintClient.ClientPrintException(BarTenderPrintClient.ClientPrintExceptionMessage.BarTenderPrintClientNotInstalled, BarTenderPrintClient.ClientPrintExceptionErrorCode.BarTenderPrintClientNotInstalled);
        }

        return printers;
    }

    BarTenderPrintClient.GetPrinters = function () {
        /// <signature>
        ///   <summary>获取客户端打印机列表。</summary>
        ///   <returns type="String[]" />
        /// </signature>

        var printers = this.CreatePrintersObject();

        try {
            var printerNames = printers.GetPrinterNames(false);

            return printerNames.split("\n");
        }
        catch (ex) {
            throw new BarTenderPrintClient.ClientPrintException(BarTenderPrintClient.ClientPrintExceptionMessage.BarTenderPrintClientNotInstalled, BarTenderPrintClient.ClientPrintExceptionErrorCode.BarTenderPrintClientNotInstalled);
        }

    }

    BarTenderPrintClient.GetDefaultPrinter = function () {
        /// <signature>
        ///   <summary>获取客户端默认打印机名称。</summary>
        ///   <returns type="String" />
        /// </signature>

        var printers = this.CreatePrintersObject();

        try {
            return printers.GetDefaultPrinterName();
        }
        catch (ex) {
            throw new BarTenderPrintClient.ClientPrintException(BarTenderPrintClient.ClientPrintExceptionMessage.BarTenderPrintClientNotInstalled, BarTenderPrintClient.ClientPrintExceptionErrorCode.BarTenderPrintClientNotInstalled);
        }
    }

    BarTenderPrintClient.GetLicense = function (printerName) {
        /// <signature>
        ///   <summary>获取指定打印机的许可信息。</summary>
        ///   <param name="printerName" type="String">打印机名称。</param>
        ///   <returns type="String" />
        /// </signature>

        var printer = this.CreatePrinterObject();
        var printLicense = printer.CreatePrintToFileLicenseScriptSafe(printerName);
        var error = printer.GetLastErrorMessage();

        if (error != "") {
            throw new BarTenderPrintClient.ClientPrintException(error, BarTenderPrintClient.ClientPrintExceptionErrorCode.PrintLicenseCreationFailed);
        }

        return printLicense;
    }

    BarTenderPrintClient.Print = function (printerName, jobName, printCode) {
        /// <signature>
        ///   <summary>打印指定内容。</summary>
        ///   <param name="printerName" type="String">打印机名称。</param>
        ///   <param name="jobName" type="String">作业名称。</param>
        ///   <param name="printCode" type="String">打印内容。</param>
        /// </signature>

        var printer = this.CreatePrinterObject();
        var success = printer.SendPrintCode(printerName, jobName, printCode);

        if (success == false) {
            var error = printer.GetLastErrorMessage();
            throw new BarTenderPrintClient.ClientPrintException(error, BarTenderPrintClient.ClientPrintExceptionErrorCode.SpoolingFailed);
        }
    }

    return BarTenderPrintClient;
})();