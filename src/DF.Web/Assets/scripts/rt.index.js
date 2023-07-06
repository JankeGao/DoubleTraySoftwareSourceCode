function tabClose() {
    var $tabs = $('#tabs');
    var currTab = $tabs.tabs('getSelected');
    var title = currTab.panel('options').title;
    if (title != '首页') {
        $tabs.tabs('close', title);
    }
}

function tabCloseAll() {
    $('.tabs-inner span').each(function (i, n) {
        var t = $(n).text();
        if (t !== '首页') {
            $('#tabs').tabs('close', t);
        }
    });
}

function tabCloseOther() {
    var prevall = $('.tabs-selected').prevAll();
    var nextall = $('.tabs-selected').nextAll();
    var $tabs = $('#tabs');
    if (prevall.length > 0) {
        prevall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            if (t !== '首页') {
                $tabs.tabs('close', t);
            }
        });
    }
    if (nextall.length > 0) {
        nextall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            if (t !== '首页') {
                $tabs.tabs('close', t);
            }
        });
    }
}

function tabCloseLeft() {
    var prevall = $('.tabs-selected').prevAll();
    if (prevall.length === 0) {
        return false;
    }
    prevall.each(function (i, n) {
        var t = $('a:eq(0) span', $(n)).text();
        if (t !== '首页') {
            $('#tabs').tabs('close', t);
        }
    });
    return false;
}

function tabCloseRight() {
    var nextall = $('.tabs-selected').nextAll();
    if (nextall.length === 0) {
        return false;
    }
    nextall.each(function (i, n) {
        var t = $('a:eq(0) span', $(n)).text();
        if (t !== '首页') {
            $('#tabs').tabs('close', t);
        }
    });

    return false;
}

function tabRefresh() {
    var $tabs = $('#tabs');
    var currTab = $tabs.tabs('getSelected');
    var url = $(currTab.panel('options').content).attr('src');
    if (url != undefined && currTab.panel('options').title != '首页') {
        $tabs.tabs('update', {
            tab: currTab,
            options: {
                content: createFrame(url)
            }
        });
    }
}

function addTab(title, url, icon) {
    var $tabs = $('#tabs');
    if ($tabs.tabs('exists', title)) {
        $tabs.tabs('select', title); //选中并刷新
        var currTab = $tabs.tabs('getSelected');
        if (url != undefined && currTab.panel('options').title !== '首页') {
            $tabs.tabs('update', {
                tab: currTab,
                options: {
                    content: createFrame(url)
                }
            });
        }
    } else {
        var content = createFrame(url);
        if (icon == null || icon == '') {
            icon = 'fa fa-file-text-o';
        }
        $('#tabs').tabs('add', {
            title: title,
            content: content,
            iconCls: icon,
            closable: true
        });
    }
}

function createFrame(url) {
    var s = '<iframe scrolling="auto" frameborder="0" border="0"  src="' + url + '" style="width:100%;height:100%;"></iframe>';
    return s;
}

function accordionOnSelect(title, index) {
    var panel = $(this).accordion('getPanel', index);
    var $tree = $(panel).find("ul").eq(0);
    if ($tree.hasClass('tree')) {
        return;
    }
    $tree.tree({
        animate: true,
        url: $tree.attr('url'),
        onSelect: function (node) {
            switch (node.attributes.Target) {
            case 'Expand':
                    $(this).tree('toggle', node.target);
                break;
            case 'Page':
                if (node.attributes.Address != null && node.attributes.Address !== '') {
                    addTab(node.text, node.attributes.Address, node.iconCls);
                }
                break;
            case 'Href':
                if (node.attributes.Address != null && node.attributes.Address !== '') {
                    window.location.href = node.attributes.Address;
                }
                break;
            case 'Blank':
                if (node.attributes.Address != null && node.attributes.Address !== '') {
                    window.open(node.attributes.Address);
                }
                break;
            }
        }
    });
}

function switchSkin(skin) {
    $('#swicth-style').attr('href', themeRootPath + '/' + skin + '.css');
}

var _url = undefined;

function switchMenu(url) {
    if (_url == url) return;
    var $divMenu = $('#divMenu');
    if (url == null || url == '') {
        rt.get('/Home/GetChildModules',
            function(result) {
                if (result != null) {
                    var html =
                        '<div class="easyui-accordion" fit="true" border="0" data-options="onSelect:accordionOnSelect">';
                    for (var i = 0; i < result.length; i++) {
                        var data = result[i];
                        var address = data.attributes.Address;
                        if (address == null || address == '') {
                            address = '/Home/GetChildModules?id=' + data.id;
                        }
                        html = html +
                            '<div title="' +
                            data.text +
                            '" class="menu-module" data-options="iconCls:\'' +
                            data.iconCls +
                            '\'">' +
                            '<ul animate="true" url="' +
                            address +
                            '"></ul>' +
                            '</div>';
                    }
                    html = html + '</div>';
                    $divMenu.html(html);
                    $.parser.parse($divMenu);
                    $('.menu-module').mCustomScrollbar();
                }
            });
    } else {
        rt.get(url,
            function(result) {
                if (result != null) {
                    var address = result.attributes.Address;
                    if (address == null || address == '') {
                        address = '/Home/GetChildModules?id=' + result.id;
                    }
                    var html =
                        '<div class="easyui-accordion" fit="true" border="0" data-options="onSelect:accordionOnSelect">' +
                            '<div title="' +
                            result.text +
                            '" class="menu-module" data-options="iconCls:\'' +
                            result.iconCls +
                            '\'">' +
                            '<ul animate="true" url="' +
                            address +
                            '"></ul>' +
                            '</div>' +
                            '</div>';
                    $divMenu.html(html);
                    $.parser.parse($divMenu);
                    $('.menu-module').mCustomScrollbar();
                }
            });
    }
    _url = url;
}

var _ushow = false;
function hideUserHeader(obj) {
    var $this = $(obj);
    var $menu = $('.user-simple');
    $menu.animate({
        top: '-98px',
        opacity: '0'
    }, 'fast', function () {
        $menu.hide();
        $('body').unbind('click');
    });
    $this.removeClass('user-info-header-hover');
    _ushow = false;
}
function showUserHeader(obj) {
    var $this = $(obj);
    var $menu = $('.user-simple');
    $menu.show().animate({
        top: '52px',
        opacity: '0.95'
    }, 'fast', function () {
        $this.unbind('mouseout');
        $this.addClass('user-info-header-hover');
        $('body').one('click', function () {
            hideUserHeader(obj);
        });
    });
    _ushow = true;
}

$(function () {

    //$('#mModule').click(function () {
    //    switchMenu();
    //});

    $('.mToolbox').click(function () {
        var $this = $(this);
        var target = $this.attr('target');
        var url = $this.attr('address');
        var title = $this.attr('Title');
        var icon = $this.attr('Icon');
        switch (target) {
            case 'Expand':
                switchMenu(url);
                break;
            case 'IFrame':
                addTab(title, url, icon);
                break;
            case 'Href':
                window.location.href = url;
                break;
            case 'Blank':
                window.open(url);
                break;
        }
        return false;
    });

    $('#user').click(function () {
        if (_ushow) {
            hideUserHeader(this);
        } else {
            showUserHeader(this);
        }
    });

    $('#btnProfile').click(function () {
        addTab('个人信息', '/User/Info', 'fa fa-user');
    });
});

(function ($) {
    $(document).ready(function () {
        $('.menu-module').mCustomScrollbar();
        $('.menu-module-float').mCustomScrollbar();
        //$('.datagrid-body').mCustomScrollbar();
    });
})(jQuery);