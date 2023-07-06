/*
 * 文件名称：hc-core.js
 * 描述说明：系统框架扩展脚本

 * ========================================================================
*/
//window.onerror = function (message, source, lineno, colno, error) {
//    var msg = "There was an error on this page.\n\n";
//    msg += "Error: " + message + "\n";
//    msg += "URL: " + source + "\n";
//    msg += "Line: " + lineno + "\n\n";
//    msg += "Column: " + colno + "\n\n";
//    msg += "ErrorType: " + error + "\n\n";
//    msg += "Click OK to continue.\n\n";
//    $.messager.alert('脚本错误', msg, 'error');
//    return false;
//}
var rt = {
    defaults: {
        window: undefined,
        datagrid: undefined,
        form: undefined,
        refreshs: undefined,
        url: undefined,
        formUrl: undefined,
        idField: 'Id',
        clearSelection: true
    },
    ajax: function (param) {
        var options = {
            url: '',
            type: 'post',
            data: null,
            dataType: 'json',
            timeout: 30000,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            onSuccess: null,
            onError: null,
            onBeforeSend: null
        };
        options = $.extend({}, options, param);
        if (options == null || options.url === '') {
            return;
        }
        $.ajax({
            url: options.url,
            type: options.type,
            dataType: options.dataType,
            data: options.data,
            contentType: options.contentType,
            timeout: options.timeout,
            cache: false,
            success: function (result) {
                if (options.onSuccess != null) options.onSuccess(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (options.onError != null) options.onError(XMLHttpRequest, textStatus, errorThrown);
                else {
                    rt.progress(false);
                    var msg = "status：" + XMLHttpRequest.status + '<br />readyState:' + XMLHttpRequest.readyState + '<br />statusText：' + XMLHttpRequest.statusText;
                    $.messager.alert('系统错误', msg, 'error');
                }
            },
            beforeSend: function (data) {
                if (options.onBeforeSend) options.onBeforeSend(data);
            }
        });
    },
    post: function (url, data, onSuccess) {
        var options = { type: 'post', url: url, data: data, onSuccess: onSuccess };
        rt.ajax(options);
    },
    get: function (url, onSuccess) {
        var options = { type: 'get', url: url, onSuccess: onSuccess };
        rt.ajax(options);
    },
    html: function (url, onSuccess) {
        var options = { type: 'get', dataType: 'html', url: url, onSuccess: onSuccess };
        rt.ajax(options);
    },
    message: function (content, success) {
        var color = 'red';
        if (success === true) {
            color = 'green';
        }

        if (success == true) {
            $.messager.show({
                title: '成功',
                showType: 'fade',
                style: {
                    right: '',
                    bottom: ''
                },
                msg: '<label class="' + color + '">' + content + '</label>'
            });
        } else {
            $.messager.alert('失败', '<label class="' + color + '">' + content + '</label>', 'error');
        }
    },
    window: function (options) {

        var $window;

        var defaultOptions = {
            //title: '添加',
            url: null,
            type: null,
            collapsible: false,
            maximizable: false,
            minimizable: false,
            closable: true,
            draggable: true,
            modal: true,
            constrain: true,
            closed:true,
            buttons: [{
                text: '确定',
                iconCls: 'fa fa-check',
                handler: function () {
                    rt.progress(true);
                    if (options.enabled === false) {
                        rt.progress(false);
                        return;
                    }
                    options.enabled = false;
                    if (options.onSubmit != null) {
                        options.onSubmit();
                        options.enabled = true;
                        rt.progress(false);
                    } else {
                        var $form;
                        if (options.form != null && options.form != '') {
                            $form = $(options.form);
                        } else {
                            var $win = $window.window('window');
                            $form = $win.find('form');
                        }
                        $form.form('submit', {
                            url: options.formUrl,
                            onSubmit: function () {
                                if (options.onBeforeSubmit != null) {
                                    var result = options.onBeforeSubmit();
                                    if (result == null || result === false) {
                                        options.enabled = true;
                                        rt.progress(false);
                                        return false;
                                    }
                                }
                                var valid = $(this).form('enableValidation').form('validate');
                                if (valid === false) {
                                    options.enabled = true;
                                    rt.progress(false);
                                }
                                return valid;
                            },
                            success: function (result) {
                                try {
                                    result = $.parseJSON(result);
                                } catch (e) {
                                    rt.progress(false);
                                    options.enabled = true;
                                    var msg = 'status：500<br />readyState:4<br />statusText：Internal Server Error';
                                    $.messager.alert('系统错误', msg, 'error');
                                    return;
                                }
                                if (options.onSuccess != null) {
                                    var onSuccessResult = options.onSuccess(result, options);
                                    if (onSuccessResult == null || onSuccessResult === false) {
                                        rt.progress(false);
                                        options.enabled = true;
                                        return;
                                    }
                                }
                                if (result.Success === true) {
                                    $window.dialog('close');
                                    if (options.refreshs != null && options.refreshs !== '') {
                                        var refreshs = options.refreshs.split(',');
                                        for (var i = 0; i < refreshs.length; i++) {
                                            var $refresh = $(refreshs[i]);
                                            if ($refresh.datagrid('options').type === 'datagrid') {
                                                $refresh.datagrid('reload');
                                            } else {
                                                $refresh.treegrid('reload');
                                            }
                                        }
                                    }
                                }
                                rt.progress(false);
                                rt.message(result.Message, result.Success);
                                options.enabled = true;
                            }
                        });
                    }
                }
            }, {
                text: '取消',
                handler: function() {
                    $window.window("close");
                }
            }],
            onBeforeSubmit: undefined,
            onSubmit: undefined,
            onSuccess: undefined,
            onLoad: undefined,
            onBeforeLoad: undefined
        };

        options = $.extend({}, rt.defaults, defaultOptions, options);

        if (options.window == null || options.window == '') {
            $.messager.alert('警告', 'window is null！', 'warning');
            return false;
        }

        $window = $(options.window);

        if ($window == null || $window.length == 0) {
            var id = options.window;
            if (id.substr(0, 1) == '#') {
                id = id.substr(1, id.length - 1);
            }
            $('body').append('<div id="' + id + '"></div>');
            $window = $(options.window);
        }

        if (options.onBeforeLoad != null && options.onBeforeLoad != '') {
            var fn = eval(options.onBeforeLoad);
            var result = fn(options);
            if (result != true) {
                return false;
            }
            options.onBeforeLoad = undefined;
        }

        options.href = options.url;
        options.iconCls = options.icon;

        $window.dialog(options);

        $window.dialog("open");
        return false;
    },
    detail: function (param) {

        var options = $.extend({}, rt.defaults, { title: '详情', icon: 'fa fa-newspaper-o' }, param);

        var row = $(options.datagrid).datagrid('getSelected');
        if (row == null) {
            $.messager.alert('警告', '请选择一条记录！', 'warning');
            return;
        }
        if (options.url.indexOf('?') > -1) {
            options.url = options.url + '&';
        } else {
            options.url = options.url + '?';
        }
        options.url = options.url + options.idField + '=' + row[options.idField];
        options.buttons = null;

        rt.window(options);
    },
    create: function (param) {
        var options = { title: '创建', icon: 'fa fa-plus' };
        if (typeof param === 'string') {
            options.url = param;
        } else {
            options = $.extend({}, options, param);
        }
        rt.window(options);
    },
    edit: function (param) {

        var options = $.extend({}, rt.defaults, { title: '编辑', icon: 'fa fa-pencil' }, param);

        var row = $(options.datagrid).datagrid('getSelected');
        if (row == null) {
            $.messager.alert('警告', '请选择一条记录！', 'warning');
            return;
        }
        if (options.url.indexOf('?') > -1) {
            options.url = options.url + '&';
        } else {
            options.url = options.url + '?';
        }

        options.url = options.url + options.idField + '=' + row[options.idField];

        rt.window(options);
    },
    remove: function (param) {

        var options = $.extend({}, { title: '移除', icon: 'fa fa-trash' }, param);

        rt.exec(options);
    },
    exec: function (param) {

        var options = $.extend({}, rt.defaults, param);

        var row = $(options.datagrid).datagrid('getSelected');
        if (row == null) {
            $.messager.alert('警告', '请选择一条记录！', 'warning');
            return;
        }
        var id = row[options.idField];

        $.messager.confirm('确认', '确定' + options.title + '该记录？', function (r) {
            if (r) {
                rt.progress(true);
                var json = {};
                json[options.idField] = id;
                rt.post(options.url, json, function (result) {
                    if (options.onSuccess != null) {
                        var onSuccessResult = options.onSuccess(result, options);
                        if (onSuccessResult == null || onSuccessResult === false) {
                            rt.progress(false);
                            return;
                        }
                    }
                    if (result.Success === true) {
                        if (options.clearSelection == true) {
                            var $datagrid = $(options.datagrid);
                            if ($datagrid.datagrid('options').type === 'datagrid') {
                                $datagrid.datagrid('clearSelections');
                            } else {
                                $datagrid.treegrid('unselectAll');
                            }
                        }
                        if (options.refreshs != null && options.refreshs !== '') {
                            var refreshs = options.refreshs.split(',');
                            for (var i = 0; i < refreshs.length; i++) {
                                var $refresh = $(refreshs[i]);
                                if ($refresh.datagrid('options').type === 'datagrid') {
                                    $refresh.datagrid('reload');
                                } else {
                                    $refresh.treegrid('reload');
                                }
                            }
                        }
                    }
                    rt.progress(false);
                    rt.message(result.Message, result.Success);
                });
            }
        });
    },
    datagrid: function (param) {
        var defaultOptions = {
            idField: 'Id',
            pagination: true,
            striped: true,
            filterBtnIconCls: 'fa fa-filter',
            //sortName: "CreateTime",
            //sortOrder: "desc",
            remoteFilter: false,
            multiSort: true,
            selectOnCheck: false,
            checkOnSelect: false,
            pageSize: 20,
            pageList: [10, 20, 50, 100],
            toolbar: null,
            fit: true,
            singleSelect: true,
            rownumbers: true,
            url: null,
            columns: null,
            type: 'datagrid',
            dataModelColumns: null
        };
        var options = $.extend({}, rt.defaults, defaultOptions, param);
        $(options.datagrid).datagrid(options);
    },
    treegrid: function (param) {

        var defaultOptions = {
            idField: 'Id',
            treeField: 'Id',
            toolbar: null,
            striped: true,
            fit: true,
            singleSelect: true,
            selectOnCheck: false,
            checkOnSelect: false,
            rownumbers: true,
            url: null,
            columns: null,
            type: 'treegrid'
        };
        var options = $.extend({}, rt.defaults, defaultOptions, param);
        $(options.datagrid).treegrid(options);
    },
    combogrid: function (param) {

        var defaultOptions = {
            striped: true,
            pageSize: 20,
            pageList: [10, 20, 50, 100],
            pagination: true,
            fit: false,
            panelWidth: 450,
            selectOnCheck: false,
            checkOnSelect: false,
            idField: 'Id',
            singleSelect: true,
            rownumbers: true,
            url: null,
            columns: null
        };
        var options = $.extend({}, rt.defaults, defaultOptions, param);
        $(options.datagrid).combogrid(options);
    },
    query: function (obj, params) {
        var $datagrid = $(obj);
        var dc = $datagrid.data('datagrid').dc;
        var div = dc.view.children('.datagrid-filter-cache');
        if (!div.length && dc.header1.add(dc.header2).find('tr.datagrid-filter-row').length === 0) {
            $datagrid.datagrid('enableFilter', params);
        } else if (dc.header1.add(dc.header2).find('tr.datagrid-filter-row').length === 0) {
            $datagrid.datagrid('showFilter');
        } else {
            $datagrid.datagrid('disableFilter');
        }
    },
    exp: function (url, data) {
        var form = $("<form>");
        form.attr("style", "display:none");
        form.attr("target", "");
        form.attr("method", "post");
        form.attr("action", url);

        if (data == null) {
            data = { FilterRules: [] };
        }
        for (var key in data) {
            var input = $("<input>");
            input.attr("type", "hidden");
            input.attr("name", key);
            input.attr("value", JSON.stringify(data[key]));
            form.append(input);
        }

        $("body").append(form);
        form.submit();
        form.remove();
    },
    progress: function (enabled, title, msg) {
        if (enabled === true) {
            if (title == null) {
                title = '请等待';
            }
            if (msg == null) {
                msg = '数据处理中...';
            }
            $.messager.progress({
                title: title,
                msg: msg
            });
        } else {
            $.messager.progress('close');
        }
    }
};
