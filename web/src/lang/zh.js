export default {
  // 常用按钮
  baseBtn: {
    importBtn: '导入', // 导入按钮
    addBtn: '添加',
    queryBtn: '查询',
    templateBtn: '模版下载',
    exportBtn: '导出',
    exportBtnWhole: '全部导出',
    exportBtnCondition: '条件导出',
    editBtn: '编辑',
    removeBtn: '移除',
    Operation: '操作',
    Synchronization: '同步',
    cancel: '取消',
    confirm: '确认',
    SelectPicture: '选择图片',
    Template: '模板',
    issue: '下发',
    toVoid: '作废',
    putOnTheShelf: '上架',
    establish: '创建',
    allExport: '全部导出',
    queryExport: '按条件导出',
    implement: '执行',
    return: '归还',
    details: '详情',
    close: '关闭',
    updateBtn: '更新',
    submit: '提交',
    startThe: '启动',
    putOut: '熄灭',
    delete: '删除',
    permissions: '权限',
    takeOutThe: '取出',
    deposit: '存入',
    confirmDelete: '确认删除',
    cancelDelete: '取消删除',
    authorization: '授权',
    reset: '重置',
    stop: '停止',
    empty: '清空',

  },
  // 常见提示文字
  messageTips: {
    messageSucceed: '创建成功',
    messageFailure: '创建失败',

    editSucceed: '编辑成功',
    editFailure: '编辑失败',

    deleteTips: '此操作将永久删除该数据, 是否继续?',
    theAbolitionOf: '此操作将作废该入库单, 是否继续?, 提示',
    Tips: '提示',
    messageCancel: '已取消',
    cancelledDelete: '已取消删除',
    deleteSucceed: '删除成功',
    deleteFailure: '删除失败',

    ImportSucceed: '导入成功',
    ImportFailure: '导入失败',
    ImportJudge: '当前限制选择 1 个文件，请删除后继续上传',

    Succeed: '成功',
    Failure: '失败',

    Export: '您将导出所有的所有的数据是否继续?'
  },
  navbar: {
    dashboard: '首页',
    github: '项目地址',
    logOut: '退出登录',
    profile: '个人中心',
    theme: '换肤',
    size: '布局大小'
  },
  login: {
    title: '朗杰智能仓库管理系统',
    logIn: '登录',
    username: '账号',
    password: '密码',
    any: '随便填',
    thirdparty: '第三方登录',
    thirdpartyTips: '本地不能模拟，请结合自己业务进行模拟！！！'
  },
  // 常用数据 
  commonData: {
    receiptNo: '入库单号',
    quantity: '数量',
  },
  // 常见的输入框提示
  imputBox: {
    docNoOperatorAndMaterial: '单号、操作人、物料',
    startDate: '开始日期',
    endDate: '结束日期',
    to: '至',
    locationCode: '请输入库位编码',
    materialCodeNameAndBarcode: '物料编码、物料名称、物料条码 ',
    materialOrName: '请输入关键字(物料编码或名称)',
    pleaseEnterTheSupplierCode: '请输入供应商编码',
    pleaseEnterVendorName: '请输入供应商名称',
    pleaseEnterContacts: '请输入联系人',
    pleaseEnterContactInformation: '请输入联系方式',
    pleaseEnterSupplierAddress: '请输入供应商地址',
    supplierRemarks: '供应商备注',
  },
  // 常见表头 
  headerInformation: {
    state: '状态',
    materialName: '物料名称',
    remarks: '备注',
    operation: '操作',
    createdTime: '添加时间',
    materialCode: '物料编码',
    materialDescription: '物料描述',
    warehousingType: '入库类型',
    warehouse: '仓库',
    printing: '打印',
    preparationDate: '制单日期',
    supplierName: '供应商名称',
    supplierCode: '供应商编码',
    dateOfManufacture: '生产日期',
    batch: '批次',
    printBarcode: '打印条码',
    category: '类别',
    dueDate: '到期日期',
    issuedQuantity: '已下发数量',
    warehouseCode: '仓库编码',
    sourceNo: '来源单号',
    specificDescription: '具体描述',
    selectDate: '选择日期',
    materialCategory: '物料类别',
    name: '名称',
    container: '货柜',
    shelfStorage: '上架储位',
    vehicleName: '载具名称',
    picture: '图片',
    materialBarcode: '物料条码',
    company: '单位',
    warehouseName: '仓库名称',
    operator: '操作人',
    quantityToBePutOnShelves: '待上架数量',
    shelfLocation: '上架库位',
    deliveryOrderNo: '出库单号',
    issueType: '出库类型',
    sourceDocNo: '来源单据号',
    pickedQuantity: '已拣选数量',
    locationAddress: '库位地址',
    pickingQuantity: '拣选数量',
    tray: '托盘',
    storage: '储位',
    issuedBy: '下发人',
    issuedTime: '下发时间',
    bill: '单据',
    palletToBePicked: '待拣货托盘',
    storageSpaceForGoodsToBePicked: '待拣货储位',
    quantityToBePicked: '待拣货数量',
    pickedquantity: '已拣货数量',
    axisX: 'X轴灯号',
    axisY: 'Y轴灯号',
    xAxis: '---X轴',
    yAxis: '---Y轴',
    pickingStorage: '拣货储位',
    receivedQuantity: '已入库数量',
    locationCode: '库位码',
    address: '地址',
    contact: '联系方式',
    theContact: '联系人',
    departmentHead: '部门负责人'
  },

  // 载具管理
  box: {
    queryBack: '载具箱名称、载具箱编号',
    // 表头
    Code: '载具箱编码',
    Picture: '图片',
    Colour: '颜色',
    Name: '载具名称',
    Type: '类别',
    width: '深度',
    length: '宽度',
    IsVirtual: '虚拟载具',
    Yes: '是',
    No: '否',
    Introduce: '介绍',
    CreatedUserName: '添加信息',
    CreatedTime: '添加时间',

    // 创建编辑
    update: '编辑载具箱',
    create: '创建载具箱',
    // 创建输入框提示
    inputBoxCode: '请输入载具箱编码',
    inputBoxName: '请输入载具箱名称',
    inputBoxlength: '请输入载具箱宽度',
    inputBoxwidth: '请输入载具箱深度',
    inputBoxType: '请选择种类',
    inputBoxColour: '请选择颜色',
    inputBoxRemarks: '介绍'
  },
  // 设备型号
  EquipmentType: {
    // 设备型号表头
    Code: '设备型号',
    Picture: '图片',
    Remark: '型号描述',
    brand: '品牌',
    Type: '类型',
    CreatedUserName: '添加信息',
    CreatedTime: '添加时间',

    // 页面查找输入框提示
    choiceEquipmentType: '请选择类型',
    choiceEquipmentBrand: '请选择品牌',

    // 创建输入框提示
    inputEquipmentCode: '请输入设备型号',
    inputEquipmentRemark: '请输入设备描述',

    // 创建编辑
    update: '编辑设备型号',
    create: '创建设备型号',

    // 品牌
    whole: '全部',
    NamgyaiVoluntarily: '朗杰',
    Kardex: '卡迪斯',
    Hanel: '享乃尔',

    // 类型
    GoUpDecline: '升降库',
    Rotation: '回转库'
  },

  // 供应商
  Supply: {
    // 页面查找输入框提示
    SupplierSearch: '供应商编码、供应商名称',

    // 表头
    Code: '供应商编码',
    Name: '供应商名称',
    Linkman: '联系人',
    Phone: '联系方式',
    Address: '地址',
    Remark: '备注'

  },

  documentation: {
    documentation: '文档',
    github: 'Github 地址'
  },
  permission: {
    addRole: '新增角色',
    editPermission: '编辑权限',
    roles: '你的权限',
    switchRoles: '切换权限',
    tips: '在某些情况下，不适合使用 v-permission。例如：Element-UI 的 el-tab 或 el-table-column 以及其它动态渲染 dom 的场景。你只能通过手动设置 v-if 来实现。',
    delete: '删除',
    confirm: '确定',
    cancel: '取消'
  },
  guide: {
    description: '引导页对于一些第一次进入项目的人很有用，你可以简单介绍下项目的功能。本 Demo 是基于',
    button: '打开引导'
  },
  components: {
    documentation: '文档',
    tinymceTips: '富文本是管理后台一个核心的功能，但同时又是一个有很多坑的地方。在选择富文本的过程中我也走了不少的弯路，市面上常见的富文本都基本用过了，最终权衡了一下选择了Tinymce。更详细的富文本比较和介绍见',
    dropzoneTips: '由于我司业务有特殊需求，而且要传七牛 所以没用第三方，选择了自己封装。代码非常的简单，具体代码你可以在这里看到 @/components/Dropzone',
    stickyTips: '当页面滚动到预设的位置会吸附在顶部',
    backToTopTips1: '页面滚动到指定位置会在右下角出现返回顶部按钮',
    backToTopTips2: '可自定义按钮的样式、show/hide、出现的高度、返回的位置 如需文字提示，可在外部使用Element的el-tooltip元素',
    imageUploadTips: '由于我在使用时它只有vue@1版本，而且和mockjs不兼容，所以自己改造了一下，如果大家要使用的话，优先还是使用官方版本。'
  },
  table: {
    dynamicTips1: '固定表头, 按照表头顺序排序',
    dynamicTips2: '不固定表头, 按照点击顺序排序',
    dragTips1: '默认顺序',
    dragTips2: '拖拽后顺序',
    title: '标题',
    importance: '重要性',
    type: '类型',
    remark: '点评',
    search: '搜索',
    add: '添加',
    export: '导出',
    reviewer: '审核人',
    id: '序号',
    date: '时间',
    author: '作者',
    readings: '阅读数',
    status: '状态',
    actions: '操作',
    edit: '编辑',
    publish: '发布',
    draft: '草稿',
    delete: '删除',
    cancel: '取 消',
    confirm: '确 定'
  },
  example: {
    warning: '创建和编辑页面是不能被 keep-alive 缓存的，因为keep-alive 的 include 目前不支持根据路由来缓存，所以目前都是基于 component name 来进行缓存的。如果你想类似的实现缓存效果，可以使用 localStorage 等浏览器缓存方案。或者不要使用 keep-alive 的 include，直接缓存所有页面。详情见'
  },
  errorLog: {
    tips: '请点击右上角bug小图标',
    description: '现在的管理后台基本都是spa的形式了，它增强了用户体验，但同时也会增加页面出问题的可能性，可能一个小小的疏忽就导致整个页面的死锁。好在 Vue 官网提供了一个方法来捕获处理异常，你可以在其中进行错误处理或者异常上报。',
    documentation: '文档介绍'
  },
  excel: {
    export: '导出',
    selectedExport: '导出已选择项',
    placeholder: '请输入文件名(默认excel-list)'
  },
  zip: {
    export: '导出',
    placeholder: '请输入文件名(默认file)'
  },
  pdf: {
    tips: '这里使用   window.print() 来实现下载pdf的功能'
  },
  theme: {
    change: '换肤',
    documentation: '换肤文档',
    tips: 'Tips: 它区别于 navbar 上的 theme-pick, 是两种不同的换肤方法，各自有不同的应用场景，具体请参考文档。'
  },
  tagsView: {
    refresh: '刷新',
    close: '关闭',
    closeOthers: '关闭其它',
    closeAll: '关闭所有'
  },
  settings: {
    title: '系统布局配置',
    theme: '主题色',
    tagsView: '开启 Tags-View',
    fixedHeader: '固定 Header',
    sidebarLogo: '侧边栏 Logo'
  },
  // 单据入库
  bill: {
    createdUserName: '添加信息',
  },
  // 单据/下拉列表
  select: {
    whole: '全部',
    toBeIssued: '待下发',
    partialDistribution: '部分下发',
    issueAll: '全部下发',
    completed: '已完成',
    toVoid: '以作废'
  },
  // 单据/ 创建/编辑弹出框
  popup: {
    // 输入框提示
    pleaseEntertheSourceDocumentNumber: '请输入来源单据号',
    // 输入框提示
    warehousingRemarks: '入库备注',
    // 输入框提示

    // 输入框提示
    pleaseEnterSupplierName: '请输入供应商名称',
  },

  // 单据/打印条码
  printBar: {
    totalWarehousingQuantity: '入库总数量',
    // 输入框提示
    minimumQuantityOfSinglePackage: '最小单包数量',
    barcodeQuantity: '条码数量',
    touchTool: '每个模具生成唯一码,数量为1，进行模具生命周期管理',
    numberOfGenerated: '生成个数',
    // 加载中
    justAMomentPlease: '正在生成条码信息，请稍等~',
    generate: '生成',
    historyBarcodeGenerationRecord: '历史条码生成记录',
    // 选择框提示
    selectAnotherPrinter: '选择其他打印机',
    barCode: '条码编码',
    founder: '创建人',
    createTime: '创建时间',
  },
  // 单据/打印
  documentPrinting: {
    receiptDocDetails: '入库单明细',
    receiptInformation: '入库单信息',
  },
  // 历史入库
  historicalWarehousing: {
    warehousingTime: '入库时间',
  },
  //  入库任务
  inTask: {
    warehousingTaskNo: '入库任务号',
    receiptDocNo: '入库单据号',

  },
  // 入库/单据/下拉列表
  inTaskSelect: {
    whole: '全部',
    toBeImplemented: '待执行',
    pendingExecution: '执行中',
    completed: '已完成',
    voided: '已作废'
  },
  // 入库/行项目
  inTaskLineItem: {
    tray: '待上架托盘',
    storagePosition: '待上架储位',
    axisX: 'X-轴灯号',
    axisY: 'Y-轴灯号',
    quantityOnShelves: '已上架数量',
    shelfTime: '上架时间',
    implement: '执行'
  },
  // 入库/创建编辑弹出框
  inTaskPopup: {
    materialWarehousing: '物料入库',
    // 输入框提示
    enterMaterialBarcode: '请扫描或输入物料条码',
    receiptQuantity: '入库数量',
    // 输入框提示
    enterReceiptQuantity: '请输入入库数量',
    axisX: '---X轴',
    axisY: '---Y轴',

    // 输入框提示

    startContainer: '启动货柜',
    depositInContainer: '存入货柜',
  },
  // 入库/单据打印
  intaskDocumentPrinting: {
    warehousingTask: '入库任务',
  },
  // 材料帮助
  materialShelf: {
    shelfWarehouse: '上架仓库',
    shelfTime: '上架时间'
  },
  // 货架
  shelf: {
    region: '区域',
    shelfCode: '货架编码',
    endTime: '结束时间',
    forcedCompletion: '强制完成',
    locationCode: '库位码',
    supplier: '供应商',
    batchNumber: '批号',
    // 标题
    smtMaterialWarehousing: 'SMT 物料入库',
    scanning: '扫描',
    generate: '生成',
    // 输入框提示
    pleaseEnterMaterialBarcode: '请输入物料条码',
    pleaseEnterkeywordsLocationCode: '请输入关键词(库位编码)',
    pleaseEnterSupplierName: '请输入供应商名称',
    pleaseEnterQuantity: '请输入数量'
  },

  // 出库单据
  outBill: {
    addTime: '添加时间',
  },
  // 出库单据/创建编辑 弹出框
  outBillPopup: {
    // 输入框提示
    enterDocumentNumber: '请输入来源单据号',
    specificDescription: '具体描述',
    exWarehouseRemarks: '出库备注',
    lineItemNo: '行项目号',
    availableStock: '可用库存',
  },
  // 出库/单据/下拉列表
  handPickSelect: {
    '-1': '全部',
    '0': '待发送',
    '1': '已发送',
    '2': '执行中',
    '3': '手动下架',
    '4': '已下架',
    '5': '已复核',
  },
  // 出库/手动练选
  manualSelection: {
    manualPicking: '手动练选',
    picked: '已拣',
    numberOfLocks: '锁定数量',
    inventoryLock: '盘点锁定',
  },
  // 出库/复核
  toReview: {
    toReview: '复核',
    quantityToBePicked: '应拣数量',
  },
  // 出库/退料
  returnMaterial: {
    returnMaterial: '退料',
    complete: '完成',
    barCode: '条码',
    barcodeQuantity: '条码数量',
    currentLocation: '当前库位',
    warehouseReturn: '仓库退料',
  },
  // 出库/任务
  issueTask: {
    // 输入框提示
    issueTaskNo: '出库任务号',
    issueDocNo: '出库单据号',

  },
  // 推荐出库物料
  recommendedOutboundMaterials: {
    outboundMaterial: '出库物料',
    materialSuggestion: '物料建议',
    pickingTime: '拣货时间',
    implement: '执行'
  },
  // 执行出库
  executeDelivery: {
    executeDelivery: '执行出库',
    outboundQuantity: '出库数量',
    startContainer: '启动货柜',
    depositInContainer: '存入货柜',
    // 输入框提示
    pleaseEnterTheIssueQuantity: '请输入出库数量',
    pleaseOutputTheLocationCode: '请输出库位编码'
  },
  // 出库单据打印
  documentPrinting: {
    issueTask: '出库任务',
    container: '容器'
  },
  // 历史出库
  historicalDelivery: {
    shelfTray: '上架托盘',
    deliveryTime: '出库时间',
  },

  // 模具管理/数据
  moduleManagement: {
    // 模具信息
    moldName: '模具名称',
    floorNumber: '层号',
    dieCode: '模具编码',
    lastPayee: '上次领用人',
    lastCollectionTime: '上次领用时间',
    lastCollectionCategory: '上次领用类别',
    lastReturnedBy: '上次归还人',
    lastReturnTime: '上次归还时间',
    durationOfCollection: '领用时长',
    // 输入框提示
    pleaseEnterTheMoldCode: '请输入模具编码',
    pleaseEnterTheWarehouseCode: '请输入仓库编码',
    pleaseEnterTheReceiver: '请输入领用人',
    pleaseEnterTheReturnee: '请输入归还人',
    pleaseEnterTheCollectionTime: '请输入领用时长',
    selectCollectingTime: '选择领用时间',
    selectReturnTime: '选择归还时间',
    moldInformationRemarks: '模具信息备注',

    // 领用单据
    collectionType: '领用类型',
    receiver: '领用人',
    collectionTimet: '领用时间',
    estimatedReturnTime: '预计归还时间',
    sourceDocument: '来源单据',
    receivingRemarks: '领用备注',
    toolDescription: '模具描述',
    receivedQuantity: '领用数量',
    moldBarCode: '模具条码',
    Xaxis: 'X轴',
    Yaxis: 'Y轴',
    returnee: '归还人',
    returnTime: '归还时间',
    serviceTime: '使用时间(min)',
    // 输入框提示
    requisitionNo: '领用单号',
    pleaseEnterTheSourceDocumentNumber: '请输入来源单据号',
    requisitionDescription: '领用描述',
    pleaseSelectCollectingTime: '请选择领用时间',
    pleaseEstimatedReturnTime: '请选择预计归还时间',

    // 领用历史
    taskCode: '任务编码',
    collectingTask: '领用任务',
    container: '容器',
    // 输入框提示
    collectingMoNo: '领用任务单号',
    pleaseSelectTheReturnee: '请选择归还人',
  },
  // 库存管理
  stockManagement: {
    // 单核库存
    // 输入框提示
    warehouseCodeAndName: '仓库编码、仓库名称',
    containerCode: '货柜编码',
    palletNumber: '托盘编号',
    storageCode: '储位编码',
    supplierCodeSupplierName: '供应商编码、供应商名称',

    // 物料库存
    // 输入框提示
    theTotalNumber: '总数量',
    lockTheNumber: '锁定数量',
    theNumberOfAvailable: '可用数量',
    bomNumberAndBomName: '物料编码、物料名称',

    // 库存上线下预警
    alarmStatus: '报警状态',
    upperLimitOfTheInventory: '库存上限',
    inventoryFloor: '库存下限',
    inventoryQuantity: '库存数量',
    expiryDate: '有效期到期时间',
    palletCoding: '托盘编码',
    locationAddress: '库位地址',
    expiryDateDays: '有效期（天）',
    theBarcode: '条码',
  },

  // 设备管理
  equipmentManagement: {
    SynchronizeEditing: '同步编辑',
    SynchronizeDeleted: '同步删除',
    equipmentManagement: '设备管理',
    equipmentInformation: '设备信息',
    equipmentAddress: '设备地址',
    thePortNumber: '端口号',
    describe: '描述',
    brandCategory: '品牌类别',
    locationCoding: '库位编码',
    empty: '空',
    lessHalf: '少半',
    forTheMostPart: '多半',
    isFull: '已满',
    forAVehicle: '存放载具',
    theLengthOfThe: '长度',
    theWidthOfThe: '宽度',
    currentStorageMaterials: '当前存放物料',
    currentStorageQuantity: '当前存放数量',
    maximumStorageQuantity: '最多存放数量',
    equipmentCoding: '设备编码',
    theEquipmentBrand: '设备品牌',
    equipmentCategory: '设备类别',
    deviceIPAddress: '设备IP地址',
    equipmentPort: '设备端口',
    deviceDescription: '设备描述',
    // 输入框提示
    device: '请描述下设备具体功能',

    // 设备报警
    containerCodeInformation: '货柜编码信息',
    containerNumber: '货柜编号',
    thePoliceDescribed: '报警描述',
    timeOfOccurrence: '发生时间',
    duration: '延续时间(min)',
    equipmentAlarmCode: '设备报警编码',
    // 输入框提示
    pleaseEnterTheAlarmCodeOfTheDevice: '请输入设备报警编码',
    pleaseEnterTheDeviceCodeName: '请输入设备编码名称',
    pleaseEnterTheDeviceStatus: '请输入设备状态',
    pleaseEnterAnAlarmDescription: '请输入报警描述',
    pleaseEnterTheWarehouseCode: '请输入仓库编码',
  },

  // 盘点管理/ 物料库存
  inventoryManagement: {
    // 盘点单据
    inventoryNumber: '盘点单号',
    inventoryType: '盘点类型',
    startTime: '开始时间',
    selectTheAreaForCounting: '选择盘点的区域',
    inventoryWarehouse: '盘点的仓库',
    containersCounted: '盘点的货柜',
    inventoryPallet: '盘点的托盘',
    inventorySheetDetails: '盘点单详情',
    physicalInventory: '盘点数量',
    stockist: '盘点人',
    inventoryTime: '盘点时间',
    selectInventoryContainer: '选择盘点货柜',
    inventoryListDetails: '盘点单详情',
    storeACode: '储位码',
    containerYard: '货柜码',

    // 输入框提示
    warehouseCodeAndName: '仓库编码、仓库名称',
    pleaseSelectContainer: '请选择货柜',
    pleaseEnterTheLocationCode: '请输入库位编码',
    pleaseSelectInventoryType: '请选择盘点类型',

    // 物料库存
    areaCode: '区域编码',
    inventoryToComplete: '盘点完成',
    thePalletNo: '托盘号',
    manualInventory: '手动盘点',
    addInventoryBarCode: '新增盘点条码',
    regionalDetails: '区域详情',
  },

  // 数据报表
  dataReport: {
    // 库龄报表
    allWarehouse: '全部仓库',
    libraryOfAge: '库龄',

    // 呆滞料报表
    dullNumber: '呆滞数量',
    // 输入框提示
    dullDays: '呆滞天数',
    dullMaterial: '呆滞物料',
    dateOfLastShipment: '最近一次出库日期',
    dateOfLastWarehousing: '最近一次入库日期',

    // 物料状态报表
    materialUnit: '物料单位',
    normalNumber: '正常数量',
  },

  // 基础信息
  basicInformation: {
    // 仓库信息
    warehouseInformation: '仓库信息',
    devicePortNumber: '设备端口号',
    model: '型号',
    vehiclewidth: '载具宽度(mm)',
    theLengthOfTheVehicle: '载具长度(mm)',
    transversePut: '横向摆放',
    longitudinalPut: '纵向摆放',
    xAxisStorageQuantity: 'X轴存放数量',
    yAxisStorageQuantity: 'Y轴存放数量',
    addLoadWith: '添加载具',
    theXaxisOfTheOriginIsShifted: '原点X轴平移',
    theYaxisOfTheOriginIsShifted: '原点Y轴平移',
    currentMaintenanceTray: '当前维护托盘',
    layer: '层',
    lights: '灯号',
    theBindingMaterial: '绑定物料',
    bindingAVehicle: '绑定载具',
    theScaleOf1: '比例尺 1',
    setUserGroupPermission: '用户组权限设置-',
    selectAUserGroup: '选择用户组',
    workNumber: '工号',
    listOfOperating: '操作名单',
    deviceType: '设备型号',
    deviceTypeCoding: '设备类型编码',

    // 输入框提示
    pleaseEnterTheStorageCode: '请输入储位编码',
    pleaseEnterBomNumberOrName: '请输入物料编码或名称',
    pleaseEnterVehicleCodeOrName: '请输入载具编码或名称',
    pleaseEnterWarehouseCategory: '请输入仓库类别',
    pleaseEnterTheWarehouseAddress: '请输入仓库地址',
    pleaseEnterTheQuantityOfXAxisStorage: '请输入X轴存放数量',
    pleaseEnterTheQuantityOfYAxisStorage: '请输入y轴存放数量',
    selectABindingMaterial: '请选择绑定物料',
    materialCodeOrMaterialName: '物料编码或物料名称',

    pleaseEnterTheWarehouseName: '请输入仓库名称',
    enterTheDevicePortNumber: '请输入设备端口号',
    pleaseEnterTheDeviceTrayNumber: '请输入设备托盘编码',
    pleaseSelectTheVehicleYouWantToStore: ' 请选择需要存放的载具',
    pleaseSelectVehicleType: '请选择载具类别',

    pleaseWaitWhileLoadingLayoutInformation: '正在加载布局信息，请稍等',
    maintainAStorage: '维护储位',
    deleteAllStorageLocationsUnderTheTray: '删除该托盘下所有库位',
    theExportLocation: '导出库位',
    warehouseCategory: '仓库类别',
    theWarehouseAddress: '仓库地址',
    containerBrand: '货柜品牌',
    containerType: '货柜型号',
    theIpAddress: 'Ip地址',
    theSerialNumber: '序列号',
    trayBearingCapacity: '托盘承重(Kg)',
    widthOfTheTray: '托盘宽度（mm）',
    palletDepth: '托盘深度（mm）',
    numberOfXAxisLights: 'X轴灯个数',
    numberOfYAxisLights: 'Y轴灯个数',
    storeTheBitWidth: '储位宽度(mm)',
    storeALength: '储位长度(mm)',
    toEnableThe: '启用',
    selectADeviceModel: '请选择设备型号',
    pleaseEnterTheUniqueDeviceSerialNumber: '请输入设备唯一序列号',
    numberOfEquipmentTrays: '设备托盘数',
    pleaseEnterTheNumberOfEquipmentTrays: '请输入设备托盘数量',
    singlePalletBearing: '单托盘承重(Kg)',
    disabled: '为0不开启托盘承重校验 ',
    pleaseEnterTheEquipmentTrayWidth: '请输入设备托盘宽度',
    pleaseEnterTheEquipmentTrayDepth: '请输入设备托盘深度',
    theXAxisLampNumber: 'X轴灯数',
    theYAxisLampNumber: 'Y轴灯数',
    pleaseEnterTheNumberOfXAxisLightsOfTheDevice: '请输入设备X轴灯数',
    pleaseEnterTheNumberOfYAxisLightsOfTheDevice: '请输入设备Y轴灯数',
    enterTheIPAddressOfTheDevice: '请输入设备IP地址',

    // 字典明细
    theDictionaryClass: '字典类别',
    dictionaryCodeOrName: '字典编码或名称',
    theDictionaryCoding: '字典编码',
    nameOfTheDictionary: '字典名称',
    extensionValue: '扩展值',
    theCategoryCode: '类别编码',
    toEnableThe: '启用',
    theSorting: '排序',
    theDictionaryNote: '字典备注',
    // 输入框提示
    pleaseEnterTheDictionaryCode: '请输入字典编码',
    pleaseEnterADictionaryName: '请输入字典名称',
    pleaseEnterADictionaryExtensionValue: '请输入字典扩展值',
    pleaseSelectADictionaryCategory: '请选择字典类别',

    // 字典类别
    categoryName: '类别名称',
    theSuperiorCoding: '上级编码',
    //输入框提示
    pleaseEnterTheCategoryCode: '请输入类别编码',
    pleaseEnterACategoryName: '请输入类别名称',
    pleaseEnterTheSuperiorCode: '请输入上级编码',
    dictionaryCategoryCodeOrName: '字典类别编码或名称',

    // 物料属性组
    attributeGroupName: '属性组名称',
    largestInventory: '最大库存',
    theMinimumInventory: '最小库存',
    firstInFirstOut: '先进先出',
    precision: '精度',
    batchesOfMaterial: '批次料',
    storeLocked: '存储锁定',
    singlePackageBarCode: '单包条码',
    whetherTheMixedBatchOf: '是否混批',
    costCenter: '成本中心',
    agingTimeDays: '老化时间(天)',
    expiryDateDays: '有效期(天)',
    agingTime: '老化时间',
    shelfLife: '库存有效期',
    day: '天',
    expiryDate: '有效期',
    // 输入框提示
    nameOfTheMaterialPropertyGroup: '物料属性组名称',
    pleaseEnterTheMaterialPropertyGroupName: '请输入物料属性组名称',
    pleaseInputTheMaximumStockOfMaterials: '请请输入物料最大库存',
    pleaseEnterTheMinimumInventoryOfMaterials: '请输入物料最小库存',
    pleaseSelectFifO: '请选择先进先出',
    pleaseSelectFifOAccuracy: '请选择先进先出精度',
    pleaseEnterTheValidityPeriod: '请输入有效期天数',
    pleaseEnterAgingTime: '请输入老化时间-天',
    pleaseEnterMaterialCostCenter: '请输入物料成本中心',
    attributeGroup: '请说明下物料属性组的具体功能 ',

    // 条码模板
    personalCenter: '个人中心',
    uploadThePicture: '上传头像',

    // 客户
    theCustomerName: '客户名称',
    // 输入框提示
    customerCodeAndCustomerName: '客户编码、客户名称',
    pleaseEnterTheCustomerCode: '请输入客户编码',
    pleaseEnterACustomerName: '请输入客户名称',
    pleaseEnterContacts: '请输入联系人',
    pleaseEnterContactInformation: '请输入联系方式',
    pleaseEnterCustomerAddress: '请输入客户地址',
    customerRemarks: '客户备注',

    // 货柜初始化
    verticalStrokeLearning: '垂直行程学习',
    horizontalStrokeLearning: '水平行程学习',
    automaticDoorTravelLearning: '自动门行程学习',
    trayScanned: '托盘扫描',
    automaticAccessTray: '自动存取托盘',
    addTheTray: '添加托盘',
    deleteTheTray: '删除托盘',
    defragmentStorageSpace: '整理存储空间',
    theNextStep: '下一步',
    numberOfFrontBrackets: '前部托架数',
    numberOfRearBrackets: '后部托架数',
    numberOfFrontPallets: '前部托盘数',
    numberOfRearTrays: '后部托盘数',
    thePalletNo: '托盘号',
    bracket: '托架号',
    height: '高度',
    //标题
    step1: '步骤 1',
    step2: '步骤 2',
    step3: '步骤 3',
    step4: '步骤 4',
    step5: '步骤 5',
    step6: '步骤 6',
    step7: '步骤 7',
    numberOfFrontAndRearBrackets: '前部和后部托架数',
    numberOfTraysInFrontAndRear: '前部和后部托盘数',
    startdefiningWriteTrayNumber: '开始定义 写入托盘号',
    writeTheDestinationTrayNumber: '写入目标托盘号',
    theBracketNumberShownInD651: 'D651中显示的托架号',
    theHeightShownInD652: 'D652中显示的高度',
    writeTheDestinationTrayNumber: '写入目标托盘号',
    targetTrayNumber: '写入需要删除的目标托盘号',
    spaceUtilization: '空间利用率',
    // 描述
    beginVerticalStrokeLearning: '开始垂直行程学习',
    monitoringTheM340: '监视M340 on表示学习结束,并弹框显示前部托架数和后部托架数 ',
    endOfTheVertical: '确认垂直行程学习结束',
    theLevelOfLearning: '开始水平行程学习',
    monitoringLevelLearning: '监视水平学习状态',
    endOfTheLevel: ' 确认水平行程学习结束',
    automaticLearning: '开始自动门行程学习',
    automaticLearningState: '监视自动门学习状态',
    automaticLearningEnd: ' 确认自动门行程学习结束',
    startTrayScan: '开始托盘扫描',
    monitorTrayScanStatus: ' 监视托盘扫描状态,当扫描结束时,显示前后部托盘数',
    startDefining: '开始定义',
    monitorTheTray: '监视托盘到达位标示位M392 ON后弹出对话框,输入托盘号,点击确认后对话框关闭 ',
    defineTheNextOne: '继续定义下一个',
    monitorM395: '监视M395,ON表示全部完成,OFF 后回到第四步',
    completeAllPallets: '确认全部托盘定义完成',
    writeTraySerialNumberToD650: '往D650写入托盘序号',
    queryM650: '查询M650 在D651中显示托盘所在托架号',
    startTheM651: '启动M651',
    displayMaterialHeightInD652: '在D652中显示物料高度',
    writeTheSerialNumberOfTrayD700: ' 往D700写入需要添加的托盘序号',
    buttonM700ToStartExecution: '按钮M700开始执行',
    scanMaterialHeightM701: '扫描物料高度M701,OFF空间不足,ON空间足够 ',
    confirmThatM702IsAdded: '确认添加M702',
    writeTheTraySerialNumberToBeDeletedToD750: '往D750写入需要删除的托盘序号',
    buttonM750ToStartExecution: '按钮M750开始执行',
    displayTrayTrayNumberInD751: '在D751中显示托盘所在托架号',
    startRunningDeviceM751: '开始运行设备M751',
    monitoringTrayInPlaceMarkerM752TRUEInPlace: '监视托盘到位标示位M752 TRUE到位',
    confirmOrCancelTheDeletion: '确认删除或取消删除',
    pressToStartSorting: '按下开始整理',
    monitorM801: '监视M801 TRUE时显示空间利用率',
    confirmFinishing: '确认整理完毕',

    // 物料信息
    singlePackageNumber: '单包数量',
    maximumStorage: '最大存放数',
    costCenter: '成本中心',
    materialType: '物料类型',
    theWeightOfThe: '重量（g）',
    note1: '备注1',
    note2: '价格',
    note3: '用途',
    note4: '备注4',
    note5: '备注5',
    toGenerateTheBarcode: '生成条码',
    basicInformation: '基础信息',
    choosePicture: '选择图片',
    attributeInformation: '属性信息',
    attributeGroup: '属性组',
    numberOfPackages: '包装数量',
    nameOfVehicleBox: '载具箱名称',
    theLengthOfThe: '长度（mm）',
    theWidthOfThe: '宽度（mm）',
    maximumStorageQuantity: '最大存放数量',
    chooseAVehicle: '选择载具',
    toGenerateTheNumber: '生成数量',
    vehicleInformation: '载具信息',
    vehicleCode: '载具编码',
    imageInformation: '图片信息',
    typeOfVehicleBox: '载具箱类别',
    nameOfVehicleBox: '载具箱名称',
    vehicleColor: '载具颜色',
    theBarcodeInformation: '条码信息',
    coding: '编码',
    supplierInformation: '供应商信息',
    // 输入框提示
    pleaseEnterTheMaterialName: '请输入物料名称',
    ifItIsEmpty: ' 为空则系统自动生成',
    pleaseEnterTheMaterialUnit: '请输入物料单位',
    pleaseEnterUnitWeight: '请输入单位重量',
    pleaseEnterMaterialRemarks: '请输入物料备注',
    pleaseEnterMaterialPrice: '请输入物料价格',
    pleaseEnterMaterialPurpose: '请输入物料用途',
    batchMaintenance: '选择属性组批量维护',
    singlePackageManagement: '模具属性物料启动单包管理',
    pleaseEnterPackingQuantity: '请输入包装数量',
    vehicleClassCode: '载具类别编码',
    lengthOfVehicleBox: '载具箱长度',
    widthOfVehicleBox: '载具箱宽度',
    barCodePrintingTask: '打印条码任务正在发送，请稍等'
  },

  // 系统管理
  systemManagement: {

    // 个人中心
    personalCenter: '个人中心',
    uploadingFacePictures: '上传人脸图片',
    facePhotosShouldBeLessThan200K: '人脸照片需小于200k',
    confirmPassword: '确认密码',
    male: '男',
    female: '女',
    theName: '姓名',
    gender: '性别',
    mobilePhone: '手机',
    // 输入框提示
    pleaseEnterYourName: '请输入姓名',
    PhoneNumber: '请输入手机号码',
    password: '密码',

    // 部门管理
    noteInformation: '备注信息',
    head: '负责人',
    theSuperiorCoding: '上级编码',
    departmentName: '部门名称',
    departmentCode: '部门编码',
    // 输入框提示
    nameOfResponsiblePerson: '请输入负责人姓名',
    pleaseEnterTheDepartmentCode: '请输入部门编码',
    pleaseEnterADepartmentName: '请输入部门名称',
    pleaseEnterTheSuperiorCode: '请输入上级编码',


    // 用户组
    departmentInformation: '部门信息',
    creationDate: '创建日期',
    userGroupAuthorization: '用户组授权',
    // 输入框提示
    userGroupName: '用户组名称',
    pleaseEnterAUserGroupName: '请输入用户组名称',
    pleaseSelectTheDepartmentOnTheLeft: '请在左侧选择部门',
    specificunFction: ' 请说明下用户组的具体功能',

    // 用户管理
    theUserName: '用户名',
    userGroups: '用户组',
    employeeCardNumber: '员工卡号',
    theDefaultPasswordIs: '默认密码为：123456',
    theUserAuthorization: '用户授权',
    // 输入框提示
    pleaseEnterAUserName: '请输入用户名',
    pleaseEntereMplsoyeeCardNumber: '请输入员工卡号',

    // 模块管理
    moduleCoding: '模块编码',
    theNameOfTheModule: '模块名称',
    icon: '图标',
    chooseTheIcon: '选择图标',

    // 编码规则
    codeName: '编码名称',
    resetTheRules: '重置规则',
    stepLength: '步长',
    theSerialNumber: '当前序号',
    theCurrentCode: '当前编码',
    entityMapping: '实体映射',
    createInformation: '创建信息',
    theInputCode: '输入编码',
    enterAName: '输入名称',
    descr: '备注说明',
    ruleProject: '规则项目',
    theRules: '规则',
    regularAssembly: '规则程序集',
    value: '值',
    theSorting: '排序',
    // 输入框提示
    fillMode: '填充模式',
    theNewRules: '新增规则',
    fillingLength: '填充长度',
    fillCharacter: '填充字符',
    fillMode: '填充模式',

    // 日志管理
    theOperationName: '操作名称',
    operatingTime: '操作时间',
    theDurationOfThe: '持续时间',
    theOperationLog: '操作日志',
    logOnToLog: '登录日志',
    readLog: '读取日志',

    // 定时任务
    scheduledTaskName: '定时任务名称',
    taskId: '任务Id',
    theNameOfTheTask: '任务名称',
    theTaskObject: '任务对象',
    taskStatus: '任务状态',
    repeatTheNumber: '重复次数',
    nextExecutionTime: '下次执行时间',
    timeIntervalBetween: '间隔时间',
    intervalType: '间隔类型',
    // 输入框提示
    enterAScheduledTaskName: '请输入定时任务名称',
    selectATaskObject: '请选择任务对象',
    selectDateAndTime: '选择日期时间',
    pleaseEnterTheIntervalForStartingTheTask: '请输入任务启动的间隔时间',
    pleaseSelectAnIntervalType: '请选择间隔类型',

    // 定时对象
    timingObjectCoding: '定时对象编码',
    objectCode: '对象编码',
    contract: '契约',
    methods: '方法',
    theAssembly: '程序集',
  },

  // 精选
  handPick: {
    // 输入框提示
    material: '物料编码、物料名称、物料条码',
    // 输入框提示
    query: '查询',
    pickingTime: '练选时间',
    reviewTime: '复核时间',
    selectPeople: '拣选人',
    reviewer: '复核人',
    state: '状态',
    materialCode: '物料编码',
    materialName: '物料名称',
    pickingQuantity: '拣选数量',
    pickedQuantity: '已拣数量',
    cancel: '取消',
    recheckQuantity: '复核数量',
    company: '单位',
    locationCode: '库位码',
    warehouseName: '仓库名称',
    areaName: '区域名称'
  },

  // 入库单据提示
  presentationOfWarehousingDocuments: {
    pleaseScanTheBarCode: '请扫描条码',
    pleaseEnterTheCorrectNumber: '请输入正确的数字',
    numberOfBarCodesInAPacket: '请输入正确的单包条码数量',
    editTheEntryList: '编辑入库单',
    createAnEntryList: '创建入库单',
    pleaseEnterTheWarehouseEntryNumber: '请输入入库单号',
    pleaseSelectWarehouse: '请选择仓库',
    pleaseEnterTheNumberOfPackets: '请输入单包数量',
    pleaseEnterTheNumberOfCorrectFormats: '请输入正确格式的数量',
    pleaseEnterTheCorrectNumberOfTagGeneration: '请输入正确的标签生成数量',
    pleaseEnterTheCorrectBatch: '请输入正确的批次',
    pleaseAddMaterialDetails: '请添加物料明细',
    itHasBeenCancelledAndInvalidated: '已取消作废',
    theWarehouseEntryWasCancelledSuccessfully: '入库单作废成功',
    theWarehouseEntryWasCancelledFailure: '入库单作废失败',
    pleaseSelectPrinter: '请选择打印机',
    pleaseSelectTheEntryListThatRequiresBarcodePrinting: '请选择需要打印条码的入库单',
    printTaskSentPleaseWait: '打印任务已发送，请稍等',
    songTypeface: '宋体',
    warehousingOrder: '入库单',
    nonEditable: '不可编辑',
    updateSuccessful: '更新成功',
    updateFailure: '更新失败',
    cannotDelete: '不可删除',
    permanentlyDeleteTheReceiptDoc: '此操作将永久删除该入库单, 是否继续?',
    tips: '提示',
    pleaseSelectAMaterial: '请选择物料',
    pleaseEnterTheProductionDate: '请输入生产日期',
    pleaseEnterTheCorrectNumber: ' 请输入正确的数字（包含两位小数的数字或者不包含小数的数字）',
    uploadFile: '当前限制选择 1 个文件，请删除后继续上传',
    pleaseScanOrEnterBarcodeFirst: '请先扫描或输入条码',
    notObtained: '未获取到该条码明细，请核查输入条码信息',
    noMaterialReceipt: '未进行收料操作，无法创建入库单',
    notToBePutOnTheShelf: '不为待上架',
    lightOnTaskSentSuccessfully: '亮灯任务发送成功',
    lightOnTaskSentFailure: '亮灯任务发送失败',

    // 入库任务
    noBarcodeInformationWasObtained: '未获取到条码信息',
    itemDoesNotBelongTo: ' 此物料不属于本行项目入库的物料',
    failedToObtainTheBarCodeInformationOnTheShelf: '获取上架条码信息失败',
    startExecution: '是否开始执行',
    nonExecutable: '不可执行',
    sorry: 'Sorry, you dont have permission to operate this tray',
    manualExecutionCanceled: '已取消手动执行',
    unableToStockIn: ' 未获取到正确的标签数量，无法入库',
    excessQuantity: 'Receipt quantity exceeds receivable quantity | 入库数量超过应收数量',
    successfulExecution: '执行成功',
    failureExecution: '执行失败',

    // 历史入库
    youHaveNotSelectedAnExportCondition: ' 您没有选择导出条件，将导出全部数据, 是否继续?',
    permanentlyDeleteTheVehicleBox: ' 此操作将永久删除该载具箱, 是否继续?',
    thisOperationWillExportAllData: '此操作将导出全部数据共',
    article: '条，是否继续?',

  },

  // 出库单提示
  deliveryPrompt: {
    editIssuedoc: '编辑出库单',
    createIssuedoc: '创建出库单',
    pleaseEnterTheDeliveryOrderNo: '请输入出库单号',
    pleaseSelectAWarehouse: '请选择仓库',
    thisItemIsNotAnIssueItem: '此物料不属于本行项目出库的物料',
    thisBarcodeBatch: '此条码批次与出库任务不符合 ',
    failedToGetPickingBarcodeInformation: '获取拣货条码信息失败',
    trayPermissions: '抱歉，您没有操作该托盘的权限',
    printBarcodeDeliveryOrder: ' 请选择需要打印条码的出库单',
    outboundOrder: '出库单',
    notForGoodsToBePicked: '不为待拣货',
    theBarcodeDetailsWereNotObtained: ' 未获取到该条码明细，请核查输入条码信息',
    pleaseScanOrEnterBarcodeFirst: '请先扫描或输入条码',
    pleaseEnterBatch: '请输入批次',
    pleaseSelectAMaterial: '请选择物料',
    permanentlyDeleteThisMo: '此操作将永久删除该入任务单, 是否继续?',

    // 出库单据
    thisOperationWillVoidTheIssueDoc: ' 此操作将作废该出库单, 是否继续?',
    pickingFailed: '拣选失败',
    pickingSucceed: '拣选成功',
    reviewFailed: '复核失败',
    reviewSucceed: '复核成功',
    pleaseConfirmTheRecheckQuantity: '请确认复核数量',
    theRecheckQuantityMustBe: ' 复核数量必须为小于或等于拣选数量',
    recheckQuantityMustBeNumericOrNot0: '复核数量必须为数值或者不为0',
    deliverySucceeded: '出库成功',
    pickingHasNotStarted: '尚未开始拣货',
    reviewCompleted: '尚未完成拣货或已复核完毕',
    pleaseEnterPickingQuantity: '请输入拣选数量',
    availableStockQuantity: '拣选数量应小于可用库存数量 ',
    lessThanTheRequiredQuantity: '|拣选数量必须为小于需求数量',
    pickingQuantityMustBeNumericOrNot0: '拣选数量必须为数值或者不为0',
    permanentlyDeleteTheIssueDoc: ' 此操作将永久删除该出库单, 是否继续?',
    noBarcodeSelected: '未选择任何条码',
    inProgressOrCompleted: '执行中或已完成',
    issueBarcodeGenerationFailed: '出库条码生成失败',
    issueBarcodeGeneratedSuccessfully: '出库条码生成成功',
    notToBeSent: '不为待发送',
    failedToVoidDeliveryOrder: '出库单作废失败',
    succeedToVoidDeliveryOrder: '出库单作废失败',
  },

  // 摸具管理提示
  dieTips: {
    editMoldInformation: '编辑模具信息',
    createMoldInformation: '创建模具信息',
    pleaseEnterCollectingType: '请输入领用种类',

    // 摸具信息/下拉列表
    whole: '全部',
    production: '生产',
    moldRepair: '修模',
    cancellation: '注销',
    collectionLock: '领用锁定',
    inLibrary: '在库中',

    // 领用单据
    editCollectingDoc: '编辑领用单',
    createCollectingDoc: '创建领用单',
    // 下拉列表
    whole: '全部',
    toBeImplemented: '待执行',
    haveInHand: '进行中',
    completed: '已完成',
    voided: '已作废',
    issuedSuccessfully: '下发成功',
    issuedFailure: '下发失败',
    requisition: '领用单',
    voidedSuccessfully: '作废成功',
    voidedFailure: '作废失败',
    pleaseAddTheMoldInformationToBeCollected: '请添加需要领用的模具信息',
    pleaseSelectCollectingType: '请选择领用类型',
    pleaseSelectTheReceiver: '请选择领用人',

    // 领用任务
    returnFailed: '归还失败',
    returnSucceed: '归还成功',
    doYouWantToReturnThisItemManually: '是否对此物料执行手动归还？手动执行将不驱动货柜运转。',
    doYouWantToManuallyCollectThisMaterial: ' 是否对此物料执行手动领用？手动执行将不驱动货柜运转。',
    pleaseEnterTheCollectingDocNo: '请输入领用单号'

  },

  // 盘点管理提示
  inventoryTips: {
    pleaseEnterANumberInTheCorrectFormat: '请输入正确格式的数字（包含两位小数的数字或者不包含小数的数字）',
    editCountSheet: '编辑盘点单',
    createCountSheet: '创建盘点单',
    pleaseEnterTheCountingBarcode: '请输入盘点条码',
    pleaseEnterTheCountingNumber: '请输入盘点单号',
    monthlyInventory: '月度盘点',
    annualInventory: '年度盘点',
    cycleCount: '周期盘点',
    pleaseSelectTheCountingArea: '请选择盘点区域',
    pleaseSelectCountingType: '请选择盘点类型',
    failedToIssueTheOrder: '下发盘点单任务失败',
    theTaskOfIssuingTheOfferOrderIsSuccessful: '下发盘点单任务成功',
    thisCountSheet: '该盘点单',
    notTheInitialState: '不是初始状态。',
    countingFailed: '盘点失败',
    countingSucceeded: '盘点成功',
    countQuantityMustBeNumeric: '盘点数量必须为数值',
    permanentlyVoidTheCount:'此操作将永久作废该盘点单, 是否继续?',
    submitted:'已提交',

    // 盘点任务
    editCountingTask: '编辑盘点任务',
    createCountingTask: '创建盘点任务',
    inventorySheet: '盘点单',
    inventoryCountingSheetSubmittedSuccessfully: '提交盘点单成功',
    inventoryCountingSheetSubmittedFailure: '提交盘点单失败',
    pleaseEnterTheCountingQuantity: '请输入盘点数量',
    pleaseEnterTheQuantityInTheCorrectFormat:' 请输入正确格式的数量（包含两位小数的数字或者不包含小数的数字）',
    pleaseEnterTheCountingQuantityInTheCorrectFormat:'请输入正确格式的盘点数量（包含两位小数的数字或者不包含小数的数字） ',
    reCountingIsAllowedOnlyWhenTheCountingSheetIsCompleted: '盘点完成的盘点单才允许重盘',
    restartSucceeded:'重盘成功',
    restartFailure:'重盘失败',
    areYouSureToStartTheCheckedArea: '确定启动勾选的区域?',
    ZoneStartFailed: '区域启动失败',
    ZoneStartSucceed: '区域启动成功',
    uncheckedArea: '尚未勾选区域',
    areaCompletionFailed: '区域完成失败',
    areaCompletionSucceed: '区域完成成功',
    DetermineTheAreaToCompleteTheCheck: '确定完成勾选的区域?'


  },

  // 设备管理提示
  deviceTips: {
    allStates: ' 全部状态',
    notEnabled: '未启动',
    inOperation: '运行中',
    fault: '故障',
    editDevice:'编辑设备',
    createDevice:'创建设备',
    loadInStock: '加载库存中',
    permanentlyDeleteDeviceInformation:'  此操作将永久删除设备信息,是否继续?',

    // 设备管理
    callThePolice:'报警',
    reset: '已复位',
    resetSuccessful: '复位成功',
    resetFailure: '复位失败',
    articleContinue: '条, 是否继续?',
    thisOperationWillExportAllDataIncluding: '此操作将导出全部数据，共：',
    youHaveNotSelectedAnExportCondition:' 您没有选择导出条件，将导出全部数据，是否继续?'
  },

  // 库存管理提示
  inventoryTips:{
    pickingLock: '拣货锁定',
    inventoryLock: '盘点锁定',
    agingCycle: '老化周期',
    xAxisNumber: 'X轴编号',
    yAxisNumber: 'Y轴编号',
    pickingListHasNotBeenChecked:'尚未勾选拣货单',
    directOffShelfFailure: '直接下架失败',
    barCodeOffTheShelfSuccessfully:'直接下架条码成功',
    confirmTheBarcodeCheckedOffTheShelf: '确定下架勾选的条码?',
    failureToExtinguish:'熄灭失败',
    succeedToExtinguish:'熄灭成功',
    confirmToLightUpTheCheckedInventory:'确定点亮勾选的库存?',
    
    lightingFailure:'点亮失败',
    lightingSucceed:'点亮成功',
    youHaveNotSelectedAnExportConditionATotalOf:'您没有选择导出条件，将导出共：',
    dataContinue:'条数据, 是否继续?',

    // 物料存储
    noExportCriteriaSelected:'  没有选择导出条件，将导出全部数据, 是否继续?',
    thisOperationWillExportAllData:' 此操作将导出全部数据是否继续',
    youHaveNotEnteredAnExportCondition:' 您没有输入导出条件，将导出共：',
    ifNoExportConditionIsSelectedATotalOf: '没有选择导出条件，将导出共：',
    whole: '全部',
    minimumStockReached: '已达到最小库存',
    maximumStockReached: '已达到最大库存',
    lessThanMinimumStock: '小于最小库存',
    greaterThanMaximumInventory: '大于最大库存',
    hour: '小时',

  },

  // 基础信息提示
  basicTips: {
    // 图片管理
    fileUploadedSuccessfully: '文件上传成功',
    uploadFailed: '上传失败',
    permanentlyDeleteThisPicture: '.  此操作将永久删除该图片, 是否继续?',
    pictureNotDeleted:'图片未删除！',

    // 物料属性组
    editItemAttributeGroup:' 编辑物料属性组',
    createItemAttributeGroup: '创建物料属性组',

    // 仓库信息
    continue: '是否继续?',
    printingFailedPleaseSelectARecord: '打印失败，请选择一条记录',
    printingCanceled: '已取消打印',
    pleaseSelectTheWarehouseLocationForBarcodePrinting: '请选择需要打印条码的仓库区域库位',
    theWarehouseDoesNotContainLocationInformation: '该仓库下未包含库位信息',
    thisOperationWillPrintUnderThiswrehouse: '此操作将打印该仓库下',
    beyondXAxis:' 超出X轴方向可存放数量，最多存放',
    beyondYAxis:' 超出Y轴方向可存放数量，最多存放',
    storageLocationInformation: '储位信息',
    trayInformation:'托盘信息',
    containerInformation: '货柜信息',
    pleaseEnterTheCode: '请输入编码',
    pleaseEnterAName: '请输入名称',
    pleaseSelectADeviceCategory: '请选择设备类别',
    pleaseEnterTrayWidth: '请输入托盘宽度',
    pleaseEnterTrayDepth: '请输入托盘深度',
    pleaseEnterThexAxisLightNumber: '请输入X轴灯号',
    pleaseEnterTheyAxisLightNumber: '请输入Y轴灯号',
    pleaseEnterASinglePalletLoad: '请输入单托盘承重',
    pleaseEnterTheNumberOfDeviceTrays: '请输入设备托盘数',
    afterClearingTheBoundMaterialInformation:'清空绑定物料信息后，请点击右下角确认按钮 ',
    gotIt: '知道了',
    materialsCurrentlyExistInThisStorageLocation: '该储位当前存在物料',
    inventoryPleaseIssueFirst:'库存，请先出库',
    originCoordinates:' 原点坐标大于托盘坐标，请手动移动原点',
    error: '错误',
    pleaseSelectATray: '请选择一个托盘',
    pleaseSelectADeviceCategory: '请选择一个设备类别',
    resetTheContainer: '是否重置该货柜下所有托盘的用户操作权限',
    failedToGetEmployeeInformation: '获取员工信息失败',
    failedToGetWhitelist: '获取白名单失败',
    whitelistSettingSucceeded: '白名单设置成功',
    whitelistSettingFailure: '白名单设置失败',
    pleaseSelectAVehicle: '请选择一条载具',
    addErrorInsufficientTraySpace: '添加错误,托盘空间不足',
    thisOperationWillPermanentlyDeleteThisRecord:'此操作将永久删除该条记录, 是否继续?',
    thisWillPrintUnderThisArea: '此操作将打印该区域下 ',
    locationLabelContinue: '库位标签, 是否继续?',
    individual: '个',
    thePalletDoesNotContainLocationInformation: '该托盘未包含库位信息',
    confirmPrinting: '确认打印?',

    // 物料信息
    editItem: '编辑物料',
    CreateItem: '创建物料',
    pleaseEnterTheUnit: '请输入单位',
    pleaseEnterTheMaterialWeight: '请输入物料重量',
    pleaseEnterTheMaterialName: '请输入物料名称',
    pleaseEnterTheMaximumStorageQuantity: '请输入最大存放数量',
    pleaseSelectVehicle: '请选择载具',
    material: '物料',
    mould:'模具',
    pleaseEnterTheMaterialNameInTheBasicInformationTab:'请在基础信息选项卡中输入物料名称',
    pleaseSelectTheAssociatedVehicle: '请选择关联载具',
    attributeInformation:' 属性信息：最大库存不能小于最小库位',
    pleaseSelectASupplier: '请选择一条供应商',
    permanentlyDeleteTheMaterial:'此操作将永久删除该物料, 是否继续? ',
    OtherwiseTheImportWillFail:'此操作会将系统中存在的物料按照导入文件中的物料编码进行相应替换，请确保导入的物料编码在系统中已存在以及导入的物料名称不能为空，否则将导入失败, 是否继续?',
    importCanceled: '已取消导入',


    // 字典类别/字典类别
    editDictionary: '编辑字典',
    createDictionary: '创建字典',
    pleaseEnterSorting: '请输入排序',
    pleaseEnterAnExtendedValue: '请输入扩展值',
    editDictionaryCategory: '编辑字典类别',
    createDictionaryCategory: '编辑字典类别',
    pleaseEnterTheArticleName: '请输入文章名称',
    rootDirectory: '根目录',
    permanentlyDeleteTheDictionaryCategory:'此操作将永久删除该字典类别, 是否继续?',
    pleaseEnterTheNameOfTheMaterialAttributeGroup: '请输入物料属性组名称',
    nothing: '无',
    warehousingTime: '入库时间',
    shelfLifeOfInventory: '库存保质期',
    second: '秒',
    minute: '分钟',
    permanentlyDeleteTheMaterialAttributeGroup:' 此操作将永久删除该物料属性组, 是否继续?',


    
    //客户管理
    editCustome: '编辑客户',
    createCustome: '编辑客户',
    permanentlyDeleteTheCustomer: ' |此操作将永久删除该客户, 是否继续?',

    // 供应商管理
    editSupplier: '编辑供应商',
    createSupplier: '创建供应商',
    eermanentlyDeleteTheSupplier:' 此操作将永久删除该供应商, 是否继续?',
    

    // 柜货初始化
    failedToStartVerticalLearning: '开始垂直学习失败',
    achievements: '成果' ,
    endVerticalLearningSuccessfully: '结束垂直学习成功',
    endVerticalLearningFailure: '开始垂直学习失败',
    theHorizontalLearningJourneyHasEnded: '水平学习行程已结束',
    failedToStartHorizontalLearningJourney: '开始水平学习行程失败',
    endHorizontalJourneyLearningSuccessful: '结束水平行程学习成功',
    endHorizontalJourneyLearningFailure: '结束水平行程学习失败',
    theAutomaticDoorLearningJourneyHasEnded: '自动门学习行程已结束',
    failedToStartAutomaticDoorLearningJourney: '开始自动门学习行程失败',
    endAutomaticDoorTravelLearningSuccessful: '结束自动门行程学习成功',
    failedToStartTrayScan: '启动托盘扫描失败',
    startDefinitionFailed: '开始定义失败',
    failedToGetM395Status: '获取M395状态失败',
    failedToStartVerticalLearning: '开始垂直学习失败',
    trayScanEndedSuccessfully: '结束托盘扫描成功',
    failedToEndTrayScan: '结束托盘扫描失败',
    trayNumberWrittenSuccessfully: '写入托盘号成功',
    trayNumberWrittenFailure: '写入托盘号失败',
    failedToGetTheCarrierNumber: '获取托架号失败',
    failedToGetStatus: '获取状态失败',
    insufficientSpace: '空间不足',
    failedToGetHeight: '获取高度失败',
    confirmToAddSuccessfully: '确认添加成功',
    confirmToAddFailure: '确认添加失败',
    trayInPlace: '托盘已到位',
    runFailed: '运行失败',
    confirmDeletionSucceeded: '确认删除成功',
    confirmDeletionFailure: '确认删除成功',
    cancelDeletionSucceeded: '取消删除成功',
    cancelDeletionFailure: '取消删除失败',
    failedToStartDefragmentingStorageSpace: '开始整理存储空间失败',
    finishing: '整理完毕',
    failedToConfirmTheCompletionOfSorting: '确认整理完毕失败',


    // 条码信息
    editBarcode: '编辑条码',
    createBarcode: '创建条码',
    pleaseEnterBarcodeCode: '请输入条码编码',
    pleaseEnterBarcodeName: '请输入条码名称',






  },

  // 系统管理提示
  systemManagementTips: {
    pleaseInputAPassword: '请输入密码',
    pleaseEnterThePasswordAgain: '请再次输入密码',
    theTwoPasswordsAreInconsistent: '两次输入密码不一致!',
    pleaseEnterEmployeeName: '请输入员工姓名',
    pleaseEnterTheCorrectMobilePhoneNumber: '请输入正确的手机号码',
    newEmployee: '新建员工',
    CreateEmployee: '编辑员工',
    newEmployeePictureNotDeleted: '图片未删除！',
    personalInformationEditingFailed:'个人信息编辑失败',
    personalInformationEditingSucceed:'个人信息编辑成功',
    failedToGetUserInformation: '获取用户信息失败',

    // 部门管理
    pleaseEnterDepartmentCode: '请输入部门编码',
    pleaseEnterDepartmentName: '请输入部门名称',
    editorialDepartment: '编辑部门',
    createDepartment: '创建部门',
    DepartmentEditedSuccessfully: '部门编辑成功',
    DepartmentEditedFailure: '部门编辑失败',
    departmentCreatedSuccessfully: '部门创建成功',
    departmentCreatedFailure: '部门创建失败',
    permanentlyDeleteThisDepartment: '此操作将永久删除该部门, 是否继续?',

    // 用户组
    editUserGroup: '编辑用户组',
    createUserGroup: '创建用户组',
    pleaseSelectADepartment: '请选择部门',
    permanentlyDeleteThisUserGroup:' 此操作将永久删除该用户组, 是否继续?',
    
    // 用户管理
    passwordNoLessThan5Digits: ' 密码不能小于5位',
    pleaseSelectAtLeastOneRole: '请至少选择一个角色',
    correctMobilePhoneNumber:' 请输入正确手机号码',
    pleaseEnterUserCode: '请输入用户编码',
    pleaseEnterUserName: '请输入用户姓名',
    editUser: '编辑用户',
    createUser: '创建用户',
    permanentlyDeleteTheUser:'此操作将永久删除该用户, 是否继续?',
    userEditedSuccessfully: '用户编辑成功',
    userEditedFailure: '用户编辑失败',
    userCreatedSuccessfully: '用户创建成功',
    userCreatedFailure: '用户创建失败',
    failedToGetRoleInformation: '获取角色信息失败',

    // 模块管理
    eidtModule: '编辑模块',
    createModule: '创建模块',
    pleaseEnterTheModuleCode: '请输入模块编码',
    pleaseEnterModuleName: '请输入模块名称',
    pleaseEnterTheModuleAddress: '请输入模块地址',
    permanentlyDeleteTheModule:' 此操作将永久删除该模块、子模块以及用户授权, 是否继续?',

    // 编码规则
    editCodingRule: '编辑编码规则',
    createCodingRule: '创建编码规则',
    pleaseEnterTheEncodingRuleName: '请输入编码规则名称',
    pleaseSelectResetAssembly: '请选择重置程序集',
    pleaseEnterTheStepSize: '请输入步长',
    PleaseSelectAMappingEntity: '请选择映射实体',
    noFilling:'不填充',
    left: '左边',
    right: '右边',
    permanentlyDeleteTheCodingRule:'此操作将永久删除该编码规则, 是否继续?',

    // 定时任务
    editScheduledTask: '编辑定时任务',
    CreateScheduledTask: '创建定时任务',
    pleaseEnterTheStartTime: '请输入开始时间',
    pleaseEnterATimingName: '请输入定时名称',
    pleaseSelectAScheduledTaskObject: '请选择定时任务对象',
    pleaseSelectTimeType: '请选择时间类型',
    stopFailed: '停止失败',
    stopSucceed: '停止成功',
    startFailed: '启动失败',
    startSucceed: '启动成功',
    permanentlyDelennteTheScheduledTask:' 此操作将永久删除该定时任务, 是否继续?',


    // 定时对象
    editTaskObject: '编辑任务对象',
    createTaskObject: '创建任务对象',
    pleaseEnterTheTaskObjectCode: '请输入任务对象编码',
    pleaseEnterTheTaskObjectContract: '请输入任务对象契约',
    pleaseEnterTheTimedStartMethod: '请输入定时启动方法',
    pleaseEnterATimedStartAssembly: '请输入定时启动程序集',
    permanentlyDeleteTheTaskObject:'此操作将永久删除该任务对象, 是否继续? ',



  }






}
