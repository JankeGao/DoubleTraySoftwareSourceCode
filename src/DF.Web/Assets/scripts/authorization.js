var $datagridEntity;
var $treegridRule;
var _dataRules = [];
var _index = 0;
var editingId = undefined;
var _groupOperates = [{
    Key: 'And',
    Value: 1
}, {
    Key: 'Or',
    Value: 2
}];
var _ruleOperates = [{
    Key: 'Equal',
    Value: 3
}, {
    Key: 'NotEqual',
    Value: 4
}, {
    Key: 'Less',
    Value: 5
}, {
    Key: 'LessOrEqual',
    Value: 6
}, {
    Key: 'Greater',
    Value: 7
}, {
    Key: 'GreaterOrEqual',
    Value: 8
}, {
    Key: 'StartsWith',
    Value: 9
}, {
    Key: 'EndsWith',
    Value: 10
}, {
    Key: 'Contains',
    Value: 11
}, {
    Key: 'NotContains',
    Value: 12
}, {
    Key: 'In',
    Value: 13
}];

function validateGroup() {
    if (editingId == undefined) return true;
    var editor = $treegridRule.treegrid('getEditor', { id: editingId, field: 'Op' });
    if (editor == null) return true;

    var op = $(editor.target).combobox('getValue');
    if (op == '') {
        return false;
    }
    return true;
}

function validateRule() {
    if (editingId == undefined) return true;
    var editor = $treegridRule.treegrid('getEditor', { id: editingId, field: 'Op' });
    if (editor == null) return true;

    var op = $(editor.target).combobox('getValue');
    if (op == '') {
        return false;
    }

    editor = $treegridRule.treegrid('getEditor', { id: editingId, field: 'Field' });
    if (editor == null) return true;

    var field = $(editor.target).textbox('getValue');
    if (field == '') {
        return false;
    }

    return true;
}

function validateRow() {
    var row = $treegridRule.treegrid('getSelected');
    if (row.children == null) {
        return validateRule();
    }
    else {
        return validateGroup();
    }
    return false;
}

function endEditing() {
    if (editingId == undefined) {
        setEntityCache();
        return true;
    }
    if (validateRow()) {
        $treegridRule.treegrid('endEdit', editingId);
        $treegridRule.treegrid('unselect', editingId);
        setEntityCache();

        editingId = undefined;
        return true;
    } else {
        return false;
    }
}

function setEntityCache() {
    var row = $datagridEntity.datagrid('getSelected');
    if (row){
        var root = $treegridRule.treegrid('getRoot');
        var entityId = row.Id;
        var dataRuleJson = JSON.stringify(createEntityDataRule(root));
        var entityInfoObj = getDataRuleByEntityId(entityId);
        if (entityInfoObj != null) {
            entityInfoObj.DataFilterRule = dataRuleJson;
        }
        else {
            _dataRules.push({
                DataFilterRule: dataRuleJson,
                EntityInfoId: entityId
            });
        }
    }
}

function createEntityDataRule(row) {
    var result = { Rules: [], Groups: [], Operate: row.Op };
    if (row.children != null && row.children.length > 0) {
        for (var i = 0; i < row.children.length; i++) {
            var r = row.children[i];
            if (r.children == null) {
                result.Rules.push({
                    Field: r.Field,
                    Op: r.Op,
                    Value: r.Value
                });
            }
            else if (r.children != null) {
                result.Groups.push(createEntityDataRule(r));
            }
        }
    }

    return result;
}

function getDataRuleByEntityId(entityId) {
    if (_dataRules != null && _dataRules.length > 0) {
        for (var i = 0; i < _dataRules.length; i++) {
            var dataRule = _dataRules[i];
            if (dataRule.EntityInfoId == entityId) {
                return dataRule;
            }
        }
    }
    return null;
}

function initTreegridData(row) {
    var roots = [];
    var root = { Id: 'root', Field: 'Root', Op: 1, Value: '',children:[] };
    var dataFilterRule = null;
    var entityInfoObj = getDataRuleByEntityId(row.Id);
    if (entityInfoObj == null) {
        if (row.DataFilterRule != null && $.trim(row.DataFilterRule) !== '') {
            dataFilterRule = JSON.parse(row.DataFilterRule);
        }
    }
    else {
        if (entityInfoObj.DataFilterRule != null && $.trim(entityInfoObj.DataFilterRule) !== '') {
            dataFilterRule = JSON.parse(entityInfoObj.DataFilterRule);
        }
    }
    if (dataFilterRule != null) {
        root.Op = dataFilterRule.Operate;
        root.children = setDataFilter(root.Id, dataFilterRule);
    }
    roots.push(root);
    $treegridRule.treegrid('loadData', { total: roots.length, rows: roots });
    editingId = undefined;
}

function remove() {
    var row = $treegridRule.treegrid('getSelected');
    if (row == null || row.Id == 'root') {
        $.messager.alert('警告', '根节点无法移除！', 'warning');
        return;
    }
    $treegridRule.treegrid('remove', row.Id);
    editingId = undefined;
}

function confirm() {
    if (!endEditing()) {
        $treegridRule.treegrid('select', editingId);
    }
}

function edit() {
    var row = $treegridRule.datagrid('getSelected');
    if (editingId !== row.Id) {
        if (row && endEditing()) {
            $treegridRule.treegrid('select', row.Id)
                .treegrid('beginEdit', row.Id);
            editingId = row.Id;
            initEditor(row.children != null);
        } else {
            setTimeout(function () {
                $treegridRule.treegrid('select', editingId);
            }, 0);
        }
    }
}

function initEditor(isGroup) {
    if (!isGroup) {
        setFields();
        var editor = $treegridRule.treegrid('getEditor', {
            id: editingId,
            field: 'Op'
        });
        if (editor != null) {
            $(editor.target).combobox('loadData', _ruleOperates);
        }
    }
    else {
        var editor = $treegridRule.treegrid('getEditor', {
            id: editingId,
            field: 'Field'
        });
        if (editor != null) {
            $(editor.target).combobox('disable');
        }

        editor = $treegridRule.treegrid('getEditor', {
            id: editingId,
            field: 'Value'
        });
        if (editor != null) {
            $(editor.target).combobox('disable');
        }

        var editor = $treegridRule.treegrid('getEditor', {
            id: editingId,
            field: 'Op'
        });
        if (editor != null) {
            $(editor.target).combobox('loadData', _groupOperates);
        }
    }
}

function checkAll(obj) {
    var $this = $(obj);
    var $inputs = $this.siblings('input');
    $inputs.prop("checked", $this.prop("checked"));
}

function setFields() {
    var editor = $treegridRule.treegrid('getEditor', {
        id: editingId,
        field: 'Field'
    });
    if (editor != null) {
        var row = $datagridEntity.datagrid('getSelected');
        $(editor.target).combobox('loadData', JSON.parse(row.PropertyNamesJson));
    }
}

function functionFormatter(val, row, index) {
    var html = '';
    if (row.Functions.length > 0) {
        var allId = row.Functions[0].ModuleCode + '_all';
        html = '<input id="' + allId + '" type="checkbox" onclick="checkAll(this)" /><label for="' + allId + '" style="margin-left:3px;">全选</label>';
        for (var i = 0; i < row.Functions.length; i++) {
            var func = row.Functions[i];
            var id = func.ModuleCode + func.Code;
            html = html +
                '<input style="margin-left:10px;" name="functionCodes" moduleCode="' +
                func.ModuleCode +
                '" code="' +
                func.Code +
                '" type="checkbox" id="' +
                id +
                '" ' +
                (func.Enabled === true ? "checked" : "") +
                '/>';
            html = html + '<i class="' + func.Icon + '" style="margin-left:3px;"></i>';
            html = html + '<label for="' + id + '" style="margin-left:3px;">' + func.Name + '</label>';
        }
    }

    return html;
}

function checkFunc(row, checked, obj) {
    $('input[moduleCode="' + row.Code + '"]').prop("checked", checked);
    var children = $(obj).treegrid('getChildren', row.Code);
    if (children != null && children.length > 0) {
        for (var i = 0; i < children.length; i++) {
            var child = children[i];
            checkFunc(child, checked, obj);
        }
    }
}

function getModuleAuthJson(json, nodes) {
    for (var i = 0; i < nodes.length; i++) {
        var node = nodes[i];
        if (node.checkState === 'checked' || node.checkState === 'indeterminate') {
            json.push({
                ModuleCode: node.Code
            });
            if (node.children != null && node.children.length > 0) {
                getModuleAuthJson(json, node.children);
            }
        }
    }
}

function createRuleGroup() {
    var selectedNode = $treegridRule.treegrid('getSelected');

    if (selectedNode.children == null) {
        $.messager.alert('警告', '该记录无法创建规则组！', 'warning');
        return;
    }

    if (!endEditing()) return;

    editingId = _index++;
    $treegridRule.treegrid('append', {
        parent: selectedNode.Id,  // the node has a 'id' value that defined through 'idField' property
        data: [{
            Id: editingId,
            Field: 'Group',
            Op: '',
            Value: '',
            children: []
        }]
    });

    $treegridRule.treegrid('select', editingId);
    $treegridRule.treegrid('beginEdit', editingId);
    initEditor(true);
}

function createRule() {
    var selectedNode = $treegridRule.treegrid('getSelected');

    if (selectedNode.children == null) {
        $.messager.alert('警告', '该记录无法创建规则！', 'warning');
        return;
    }

    if (!endEditing()) return;

    editingId = _index++;
    var forInsertNodeId = getForInsertGroupId(selectedNode.children);
    if (forInsertNodeId == null) {
        $treegridRule.treegrid('append', {
            parent: selectedNode.Id,  // the node has a 'id' value that defined through 'idField' property
            data: [{
                Id: editingId,
                Field: '',
                Op: '',
                Value: ''
            }]
        });
    }
    else {
        $treegridRule.treegrid('insert', {
            before: forInsertNodeId,
            data: {
                Id: editingId,
                Field: '',
                Op: '',
                Value: '',
            }
        });
    }
    $treegridRule.treegrid('select', editingId);
    $treegridRule.treegrid('beginEdit', editingId);
    initEditor(false);
}

//获取待插入的节点
function getForInsertGroupId(children) {
    for (var i = 0; i < children.length; i++) {
        var child = children[i];
        if (child.children != null) {
            return child.Id;
        }
    }
    return null;
}

function setDataFilter(parentId,inputFilterGroup) {
    var result = [];
    if (inputFilterGroup == null) return result;
    //规则
    if (inputFilterGroup.Rules != null && inputFilterGroup.Rules.length > 0) {
        for (var i = 0; i < inputFilterGroup.Rules.length; i++) {
            var filterRule = inputFilterGroup.Rules[i];
            filterRule.Id = _index++;
            filterRule._parentId = parentId;
            result.push(filterRule);
        }
    }
    //组
    if (inputFilterGroup.Groups != null && inputFilterGroup.Groups.length > 0) {
        for (var i = 0; i < inputFilterGroup.Groups.length; i++) {
            var filterGroup = inputFilterGroup.Groups[i];
            var group = {
                Id: _index++,
                Field: 'Group',
                Op: filterGroup.Operate,
                Value: '',
                _parentId:parentId
            };
            group.children = setDataFilter(group.Id,filterGroup);
            result.push(group);
        }
    }
    return result;
}

function setAuthorization(options) {

    var row = options.row;

    var window = '#winAuthorization';
    rt.window({
        url: options.url,
        title: options.authTypeName + '授权(' + row.Name + ')',
        window: window,
        fit: true,
        onSubmit: function () {
            if (!endEditing()) {
                $.messager.alert('警告', '数据规则验证失败！', 'warning');
                return false;
            }

            var type = options.authType;
            var $gridModuleFunction = $('#gridModuleFunctionAuth');

            //模块授权
            var moduleJsons = [];
            var roots = $gridModuleFunction.treegrid('getRoots');
            if (roots != null && roots.length > 0) {
                getModuleAuthJson(moduleJsons, roots);
            }

            //功能授权
            var funcJsons = [];
            var $functionCodes = $('input:checkbox[name=functionCodes]:checked');
            for (var j = 0; j < $functionCodes.length; j++) {
                var $functionCode = $functionCodes.eq(j);
                var moduleCode = $functionCode.attr('moduleCode');
                var functionCode = $functionCode.attr('code');
                funcJsons.push({
                    ModuleCode: moduleCode,
                    FunctionCode: functionCode
                });
            }

            //数据规则授权
            var dataRuleJsons = [];
            if (_dataRules != null && _dataRules.length > 0) {
                for (var i = 0; i < _dataRules.length; i++) {
                    var r = _dataRules[i];
                    if (r.DataFilterRule != null && $.trim(r.DataFilterRule) !== '') {
                        dataRuleJsons.push({
                            DataFilterRule: r.DataFilterRule,
                            EntityInfoId: r.EntityInfoId
                        });
                    }
                }
            }

            rt.post(options.submitUrl, {
                Type: type,
                TypeCode: row.Code,
                ModuleAuthJson: JSON.stringify(moduleJsons),
                FunctionAuthJson: JSON.stringify(funcJsons),
                DataRuleJson: JSON.stringify(dataRuleJsons)
            }, function (result) {
                if (result.Success === true) {
                    $(window).dialog('close');
                }
                rt.message(result.Message, result.Success);
            });
        }
    });

    return false;
}