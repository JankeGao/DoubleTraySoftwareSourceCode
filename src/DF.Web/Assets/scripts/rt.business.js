function showIconWindow(param) {
    var window = '#windowIcons';
    rt.window({
        window: window,
        url: '/Icon/QueryDialog',
        title: '系统图标',
        width: 900,
        height: 400,
        onSubmit: function () {
            if (typeof param == 'object') {
                var icon = $('.icons .selected').children('i').attr('class');
                $(param).textbox('setValue', icon);
                $(window).window('close');
            }
            else if (typeof param == 'function') {
                param();
            }
            return false;
        }
    });
}