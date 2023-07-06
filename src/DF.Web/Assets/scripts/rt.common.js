function formatEnabled(val) {
    var icon = 'fa-remove';
    if (val === true || val === 'true') {
        icon = 'fa-check';
    }
    return '<span class="fa ' + icon + '"></span>';
}

function formatUnEnabled(val) {
    var icon = 'fa-remove';
    if (val === false || val === 'false') {
        icon = 'fa-check';
    }
    return '<span class="fa ' + icon + '"></span>';
}

function findByJson(data, param) {
    if (param != null) {
        for (var i = 0; i < data.length; i++) {
            var node = data[i];
            var valid = true;
            for (var key in param) {
                if (param[key] != node[key]) {
                    valid = false;
                    break;
                }
            }
            if (valid === true) return node;
            //if (node.children != null && node.children.length > 0) {
            //    var result = findByJson(node.children, param);
            //    if (result != null) return result;
            //}
        }
        return null;
    }
    return data;
}

function formatSuccess(val, index, row) {
    if (val === true) {
        return '<label class="green">成功</label>';
    } else {
        return '<label class="red">失败</label>';
    }
}

function formatDate(val, index, row) {
    if (val!=null&&val.length > 8) {
        return val.substring(0,10);
    }
    return val;
}

function formatLongText(value) {
    if (value && value.length > 20) {
        return '<span title="' + value + '">' + value.substring(0, 20) + '...</span>';
    }
    return value;
}

function s4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}
function getGuid() {
    return (s4() + s4() + "-" + s4() + "-" + s4() + "-" + s4() + "-" + s4() + s4() + s4());
}

function getDateDiff(startDate, endDate,isAbs) {
    var startTime = new Date(Date.parse(startDate.replace(/-/g, "/"))).getTime();
    var endTime = new Date(Date.parse(endDate.replace(/-/g, "/"))).getTime();
    var dates = 0;
    if (isAbs === true) {
        dates= Math.abs((startTime - endTime)) / (1000 * 60 * 60 * 24);
    } else {
        dates = (endTime - startTime) / (1000 * 60 * 60 * 24);
    }
    return dates;
}

/**
 * 参数：保留小数位数，货币符号，整数部分千位分隔符，小数分隔符
    // Default usage and custom precision/symbol :
    var revenue = 12345678;
    alert(revenue.formatMoney()); // $12,345,678.00
    alert(revenue.formatMoney(0, "HK$ ")); // HK$ 12,345,678

    // European formatting:
    var price = 4999.99;
    alert(price.formatMoney(2, "€", ".", ",")); // €4.999,99

    // It works for negative values, too:
    alert((-500000).formatMoney(0, "£ ")); // £ -500,000
 * @param {} places 
 * @param {} symbol 
 * @param {} thousand 
 * @param {} decimal 
 * @returns {} 
 */
Number.prototype.formatMoney = function (places, symbol, thousand, decimal) {
        places = !isNaN(places = Math.abs(places)) ? places : 2;
        symbol = symbol !== undefined ? symbol : "$";
        thousand = thousand || ",";
        decimal = decimal || ".";
        var number = this,
            negative = number < 0 ? "-" : "",
            i = parseInt(number = Math.abs(+number || 0).toFixed(places), 10) + "",
            j = (j = i.length) > 3 ? j % 3 : 0;
        return symbol + negative + (j ? i.substr(0, j) + thousand : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousand) + (places ? decimal + Math.abs(number - i).toFixed(places).slice(2) : "");
};

function layoutRegionRefresh(layout, region, url) {
    var panel = $(layout).layout('panel', region);
    panel.panel('refresh', url);
}

/**
 *
 * @desc 判断两个数组是否相等
 * @param {Array} arr1
 * @param {Array} arr2
 * @return {Boolean}
 */
function arrayEqual(arr1, arr2) {
    // 首先要判断是否是数组,传进来的非数组,返回false
    if (!(arr1 instanceof Array) || !(arr2 instanceof Array)) {
        return false;
    }
    if (arr1 === arr2) return true;
    if (arr1.length != arr2.length) return false;
    for (var i = 0; i < arr1.length; ++i) {
        if (arr1[i] !== arr2[i]) return false;
    }
    return true;
}

/**
 * @desc 获取浏览器类型和版本
 * @return {String}
 */
function getExplore() {
    var sys = {},
        ua = navigator.userAgent.toLowerCase(),
        s;
    (s = ua.match(/rv:([\d.]+)\) like gecko/)) ? sys.ie = s[1] :
        (s = ua.match(/msie ([\d\.]+)/)) ? sys.ie = s[1] :
        (s = ua.match(/edge\/([\d\.]+)/)) ? sys.edge = s[1] :
        (s = ua.match(/firefox\/([\d\.]+)/)) ? sys.firefox = s[1] :
        (s = ua.match(/(?:opera|opr).([\d\.]+)/)) ? sys.opera = s[1] :
        (s = ua.match(/chrome\/([\d\.]+)/)) ? sys.chrome = s[1] :
        (s = ua.match(/version\/([\d\.]+).*safari/)) ? sys.safari = s[1] : 0;

    // 根据关系进行判断
    if (sys.ie) return ('IE: ' + sys.ie);
    if (sys.edge) return ('EDGE: ' + sys.edge);
    if (sys.firefox) return ('Firefox: ' + sys.firefox);
    if (sys.chrome) return ('Chrome: ' + sys.chrome);
    if (sys.opera) return ('Opera: ' + sys.opera);
    if (sys.safari) return ('Safari: ' + sys.safari);
    return 'Unkonwn';
}

/**
 *
 * @desc 获取操作系统类型
 * @return {String}
 */
function getOS() {
    var userAgent = 'navigator' in window
        && 'userAgent' in navigator
        && navigator.userAgent.toLowerCase() || '';
    var vendor = 'navigator' in window
        && 'vendor' in navigator
        && navigator.vendor.toLowerCase() || '';
    var appVersion = 'navigator' in window
        && 'appVersion' in navigator
        && navigator.appVersion.toLowerCase() || '';

    if (/mac/i.test(appVersion))
        return 'MacOSX';
    if (/win/i.test(appVersion))
        return 'windows';
    if (/linux/i.test(appVersion))
        return 'linux';
    if (/iphone/i.test(userAgent) || /ipad/i.test(userAgent) || /ipod/i.test(userAgent))
        return 'ios';
    if (/android/i.test(userAgent))
        return 'android';
    if (/win/i.test(appVersion) && /phone/i.test(userAgent))
        return 'windowsPhone';
}

/**
 * @desc 深拷贝，支持常见类型
 * @param {Any} values
 */
function deepClone(values) {
    var copy;

    // Handle the 3 simple types, and null or undefined
    if (null == values || "object" != typeof values) return values;

    // Handle Date
    if (values instanceof Date) {
        copy = new Date();
        copy.setTime(values.getTime());
        return copy;
    }

    // Handle Array
    if (values instanceof Array) {
        copy = [];
        for (var i = 0, len = values.length; i < len; i++) {
            copy[i] = deepClone(values[i]);
        }
        return copy;
    }

    // Handle Object
    if (values instanceof Object) {
        copy = {};
        for (var attr in values) {
            if (values.hasOwnProperty(attr)) copy[attr] = deepClone(values[attr]);
        }
        return copy;
    }

    throw new Error("Unable to copy values! Its type isn't supported.");
}

/**
 *
 * @desc   判断`obj`是否为空
 * @param  {Object} obj
 * @return {Boolean}
 */
function isEmptyObject(obj) {
    if (!obj || typeof obj !== 'object' || Array.isArray(obj))
        return false
    return !Object.keys(obj).length
}

/**
 *
 * @desc 生成指定范围随机数
 * @param  {Number} min
 * @param  {Number} max
 * @return {Number}
 */
function randomNum(min, max) {
    return Math.floor(min + Math.random() * (max - min));
}

/**
 *
 * @desc 随机生成颜色
 * @return {String}
 */
function randomColor() {
    return '#' + ('00000' + (Math.random() * 0x1000000 << 0).toString(16)).slice(-6);
}

/**
 *
 * @desc   现金额转大写
 * @param  {Number} n
 * @return {String}
 */
function digitUppercase(n) {
    var fraction = ['角', '分'];
    var digit = [
        '零', '壹', '贰', '叁', '肆',
        '伍', '陆', '柒', '捌', '玖'
    ];
    var unit = [
        ['元', '万', '亿'],
        ['', '拾', '佰', '仟']
    ];
    var head = n < 0 ? '欠' : '';
    n = Math.abs(n);
    var s = '';
    for (var i = 0; i < fraction.length; i++) {
        s += (digit[Math.floor(n * 10 * Math.pow(10, i)) % 10] + fraction[i]).replace(/零./, '');
    }
    s = s || '整';
    n = Math.floor(n);
    for (var i = 0; i < unit[0].length && n > 0; i++) {
        var p = '';
        for (var j = 0; j < unit[1].length && n > 0; j++) {
            p = digit[n % 10] + unit[1][j] + p;
            n = Math.floor(n / 10);
        }
        s = p.replace(/(零.)*零$/, '').replace(/^$/, '零') + unit[0][i] + s;
    }
    return head + s.replace(/(零.)*零元/, '元')
        .replace(/(零.)+/g, '零')
        .replace(/^整$/, '零元整');
}

/**
 *
 * @desc   判断是否为邮箱地址
 * @param  {String}  str
 * @return {Boolean}
 */
function isEmail(str) {
    return /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/.test(str);
}

/**
 *
 * @desc   判断是否为手机号
 * @param  {String|Number} str
 * @return {Boolean}
 */
function isPhoneNum(str) {
    return /^(0|86|17951)?(13[0-9]|15[012356789]|17[678]|18[0-9]|14[57])[0-9]{8}$/.test(str);
}