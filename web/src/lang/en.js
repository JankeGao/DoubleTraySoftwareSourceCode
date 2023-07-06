export default {
  // 常用按钮
  baseBtn: {
    importBtn: 'Import', // 导入按钮
    addBtn: 'Add',
    queryBtn: 'Query',
    templateBtn: 'TemplateDownload',
    exportBtn: 'Qxport',
    exportBtnWhole: 'ExportAll',
    exportBtnCondition: 'ConditionalExport',
    editBtn: 'Edit',
    removeBtn: 'Remove',
    Operation: 'Operation',
    Synchronization: 'Synchronization',
    cancel: 'Cancel',
    confirm: 'Confirm',
    SelectPicture: 'Select Picture',
    Template: 'Template',
    issue: 'Issue',
    toVoid: 'ToVoid',
    putOnTheShelf: 'PutOnTheShelf',
    shelfLocation: 'ShelfLocation',
    establish: 'Establish',
    allExport: 'ExportAll',
    queryExport: 'QueryExport',
    details: 'Details',
    close: 'Close',
    updateBtn: 'UpdateBtn',
    submit: 'Submit',
    startThe: 'StartThe',
    putOut: 'PutOut',
    delete: 'Delete',
    permissions: 'Permissions',
    takeOutThe: 'TakeOutThe',
    deposit: 'Deposit',
    confirmDelete: 'ConfirmDelete',
    cancelDelete: 'CancelDelete',
    authorization: 'Authorization',
    reset: 'Reset',
    stop: 'Stop',
    empty: 'Empty',

  },
  // 常见提示文字
  messageTips: {
    messageSucceed: 'CreatedSuccessfully',
    messageFailure: 'CreationFailed',

    editSucceed: 'EditSucceeded',
    editFailure: 'EditFailed',

    deleteTips: 'ThisOperationWillPermanentlyDeleteTheDataDoYouWantToContinue?',
    theAbolitionOf: 'ThisOperationWillVoidTheReceiptDocDoYouWantToContinue?,Tips',
    Tips: 'Tips',
    messageCancel: 'ToCancel',
    cancelledDelete: 'CancelledDelete',
    deleteSucceed: 'DeletedSuccessfully',
    deleteFailure: 'DeletedFailed',

    ImportSucceed: 'ImportSucceeded',
    ImportFailure: 'ImportFailed',
    ImportJudge: 'OneFileIsCurrentlyRestricted.PleaseDeleteItAndContinueUploading',

    Succeed: 'Success',
    Failure: 'Error',

    Export: 'YouWillExportAllData.DoYouWantToContinue?'
  },
  // 标题 
  navbar: {
    dashboard: 'HomePage',
    github: 'ProjectAddress',
    logOut: 'LogOut',
    profile: 'PersonalCenter',
    theme: 'Skin',
    size: 'Layout size'
  },
  // 登录
  login: {
    title: 'LangjieIntelligentWarehouseManagementSystem',
    logIn: 'SignIn',
    username: 'AccountNumber',
    password: 'Password',
    any: 'JustFillItIn',
    thirdparty: 'Third party login',
    thirdpartyTips: 'Local cannot be simulated, please simulate in combination with your own business!!!'
  },

  // 载具管理
  box: {
    queryBack: 'Name and number of vehicle box',
    // 表头
    Code: 'VehicleBoxCode',
    Picture: 'Picture',
    Colour: 'Colour',
    Name: 'VehicleName',
    Type: 'Category',
    width: 'Width',
    length: 'Length',
    IsVirtual: 'VirtualVehicle',
    Yes: 'Yes',
    No: 'No',
    Introduce: 'Introduce',
    CreatedUserName: 'AddInformation',
    CreatedTime: 'AddTime',

    // 创建编辑
    update: 'Edit vehicle box',
    create: '创建载具箱',
    // 创建输入框提示
    inputBoxCode: '请输入载具箱编码',
    inputBoxName: '请输入载具箱名称',
    inputBoxlength: '请输入载具箱长度',
    inputBoxwidth: '请输入载具箱宽度',
    inputBoxType: '请选择种类',
    inputBoxColour: '请选择颜色',
    inputBoxRemarks: '介绍'
  },
  // 设备型号
  EquipmentType: {
    // 设备型号表头
    Code: 'EquipmentModel',
    Picture: 'Picture',
    Remark: 'ModelDescription',
    brand: 'Brand',
    Type: 'Type',
    CreatedUserName: 'AddInformation',
    CreatedTime: 'AddTime',

    // 页面查找输入框提示
    choiceEquipmentType: 'PleaseSelectAType',
    choiceEquipmentBrand: 'PleaseSelectBrand',

    // 创建输入框提示
    inputEquipmentCode: 'PleaseEnterTheDeviceModel',
    inputEquipmentRemark: 'PleaseEnterDeviceDescription',

    // 创建编辑
    update: 'EditDeviceModel',
    create: 'CreatingADeviceModel',

    // 品牌
    whole: 'Whole',
    NamgyaiVoluntarily: 'LangJie',
    Kardex: 'Degenerated',
    Hanel: 'EnjoyIs',

    // 类型
    GoUpDecline: 'LiftLibrary',
    Rotation: 'RotaryLibrary'
  },
  // 
  commonData: {
    receiptNo: 'ReceiptNo',
    quantity: 'Quantity',
  },
  // 常见的输入框提示
  imputBox: {
    docNoOperatorAndMaterial: 'DocNoOperatorAndMaterial',
    startDate: 'StartDate',
    endDate: 'EndDate',
    to: 'To',
    locationCode: 'LocationCode',
    materialCodeNameAndBarcode: 'MaterialCode,MaterialNameAndMaterialBarcode ',
    materialOrName: 'PleaseEnterKeyword(MaterialCodeOrName)',
    pleaseEnterTheSupplierCode: 'PleaseEnterTheSupplierCode',
    pleaseEnterVendorName: 'PleaseEnterVendorName',
    pleaseEnterContacts: 'PleaseEnterContacts',
    pleaseEnterContactInformation: 'PleaseEnterContactInformation',
    pleaseEnterSupplierAddress: 'PleaseEnterSupplierAddress',
    supplierRemarks: 'SupplierRemarks',
  },
  // 常见表头 
  headerInformation: {
    state: 'State',
    materialName: 'MaterialName',
    remarks: 'Remarks',
    operation: 'Operation',
    createdTime: 'AddTime',
    materialCode: 'MaterialCode',
    materialDescription: 'MaterialDescription',
    warehousingType: 'WarehousingType',
    warehouse: 'Warehouse',
    printing: 'Printing',
    preparationDate: 'PreparationDate',
    supplierName: 'SupplierName',
    supplierCode: 'SupplierCode',
    dateOfManufacture: 'DateOfManufacture',
    batch: 'Batch',
    printBarcode: 'PrintBarcode',
    category: 'Category',
    dueDate: 'DueDate',
    issuedQuantity: 'IssuedQuantity',
    warehouseCode: 'WarehouseCode',
    sourceNo: 'SourceNo',
    specificDescription: 'SpecificDescription',
    materialCode: 'MaterialCode',
    selectDate: 'SelectDate',
    materialCategory: 'MaterialCategory',
    name: 'Name',
    container: 'Container',
    shelfStorage: 'ShelfStorage',
    vehicleName: 'VehicleName',
    picture: 'Picture',
    materialBarcode: 'MaterialBarcode',
    company: 'Company',
    warehouseName: 'WarehouseName',
    operator: 'Operator',
    quantityToBePutOnShelves: 'QuantityToBePutOnShelves',
    deliveryOrderNo: 'DeliveryOrderNo',
    issueType: 'IssueType',
    sourceDocNo: 'SourceDocNo',
    locationAddress: 'LocationAddress',
    pickedQuantity: 'PickedQuantity',
    pickingQuantity: 'PickingQuantity',
    tray: 'Tray',
    storage: 'Storage',
    issuedBy: 'IssuedBy',
    issuedTime: 'IssuedTime',
    bill: 'Bill',
    palletToBePicked: 'PalletToBePicked',
    storageSpaceForGoodsToBePicked: 'StorageSpaceForGoodsToBePicked',
    quantityToBePicked: 'QuantityToBePicked',
    pickedquantity: 'Pickedquantity',
    axisX: 'X-axisIightSignal',
    axisY: 'Y-axisIightSignal',
    xAxis: '---X-Axis',
    yAxis: '---Y-Axis',
    pickingStorage: 'PickingStorage',
    receivedQuantity: 'ReceivedQuantity',
    locationCode: 'LocationCode',
    address: 'Address',
    contact: 'Contact',
    theContact: 'TheContact',
    departmentHead: 'DepartmentHead'
  },

  // 供应商
  Supply: {
    // 页面查找输入框提示
    SupplierSearch: 'SupplierCodeSupplierName',

    // 表头
    Code: 'SupplierCode',
    Name: 'SupplierName',
    Linkman: 'TheContact',
    Phone: 'Contact',
    Address: 'Address',
    Remark: 'Remarks'

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
    createdUserName: 'AddInformation',

  },
  // 单据/下拉列表
  select: {
    whole: 'Whole',
    toBeIssued: 'ToBeIssued',
    partialDistribution: 'PartialDistribution',
    issueAll: 'IssueAll',
    completed: 'Completed',
    toVoid: 'ToVoid'
  },
  // 单据/下拉列表
  inTaskSelect: {
    whole: 'whole',
    toBeImplemented: 'ToBeImplemented',
    pendingExecution: 'PendingExecution',
    completed: 'Completed',
    voided: 'Voided'
  },
  // 单据/ 创建/编辑弹出框
  popup: {
    // 输入框提示
    pleaseEntertheSourceDocumentNumber: 'PleaseEntertheSourceDocumentNumber',
    // 输入框提示
    warehousingRemarks: 'WarehousingRemarks',
    // 输入框提示


    // 输入框提示
    pleaseEnterSupplierName: 'PleaseEnterSupplierName',
  },

  // 单据/打印条
  printBar: {
    totalWarehousingQuantity: 'TotalWarehousingQuantity',
    // 输入框提示
    minimumQuantityOfSinglePackage: 'MinimumQuantityOfSinglePackage',
    barcodeQuantity: 'BarcodeQuantity',
    touchTool: 'EachMoldGeneratesAUniqueCodeWithAQuantityOf 1 ForMoldLifeCycleManagement',
    numberOfGenerated: 'NumberOfGenerated',
    cancel: 'Cancel',
    // 加载中
    justAMomentPlease: 'BarcodeInformationIsBeingGenerated,PleaseWait~ ',
    generate: 'Generate',
    historyBarcodeGenerationRecord: 'HistoryBarcodeGenerationRecord',
    // 选择框提示
    selectAnotherPrinter: 'SelectAnotherPrinter',
    barCode: 'BarCode',
    founder: 'Founder',
    createTime: 'CreateTime',
  },
  // 单据/打印
  documentPrinting: {
    receiptDocDetails: 'ReceiptDocDetails',
    receiptInformation: 'ReceiptInformation',
  },
  // 历史入库
  historicalWarehousing: {
    shelfTray: 'ShelfTray',
    warehousingTime: 'WarehousingTime',
  },
  //  入库任务
  inTask: {
    warehousingTaskNo: 'WarehousingTaskNo',
    receiptDocNo: 'ReceiptDocNo',

  },
  // 入库/行项目
  inTaskLineItem: {
    tray: 'TrayToBePutOnShelves ',
    storagePosition: 'StorageSpaceToBePutOnTheShelf ',
    axisX: 'X-axisIightSignal ',
    axisY: 'Y-axisIightSignal ',
    quantityOnShelves: 'QuantityOnShelves',
    shelfTime: 'ShelfTime',
    implement: 'Implement'
  },
  // 入库/创建编辑弹出框
  inTaskPopup: {
    materialWarehousing: 'MaterialWarehousing',
    // 输入框提示
    enterMaterialBarcode: 'PleaseScanOrEnterTheMaterialBarcode ',
    receiptQuantity: 'ReceiptQuantity',
    // 输入框提示
    enterReceiptQuantity: 'PleaseEnterTheStockInQuantity ',
    axisX: '---X-Axis',
    axisY: '---Y-Axis',
    startContainer: 'StartContainer',
    depositInContainer: 'DepositInContainer',
  },
  // 入库/单据打印
  intaskDocumentPrinting: {
    warehousingTask: 'WarehousingTask',
  },
  // 材料帮助
  materialShelf: {
    shelfWarehouse: 'ShelfWarehouse',
    shelfTime: 'ShelfTime'
  },
  // 货架
  shelf: {
    region: 'Region',
    shelfCode: 'ShelfCode',
    endTime: 'EndTime',
    forcedCompletion: 'ForcedCompletion',
    locationCode: 'LocationCode',
    supplier: 'Supplier',
    batchNumber: 'BatchNumber',
    // 标题
    smtMaterialWarehousing: 'SMT MaterialWarehousing',
    scanning: 'Scanning',
    generate: 'Generate',
    // 输入框提示
    pleaseEnterMaterialBarcode: 'PleaseEnterMaterialBarcode',
    pleaseEnterkeywordsLocationCode: 'PleaseEnterKeywords(LocationCode)',
    pleaseEnterSupplierName: 'pleaseEnterSupplierName',
    pleaseEnterQuantity: 'PleaseEnterQuantity'
  },

  // 出库任务
  outBill: {
    addTime: 'AddTime',
  },
  // 出库单据/创建编辑 弹出框
  outBillPopup: {
    // 输入框提示
    enterDocumentNumber: 'PleaseEnterTheSourceDocumentNumber ',
    specificDescription: 'SpecificDescription',
    exWarehouseRemarks: 'ExWarehouseRemarks',
    lineItemNo: 'LineItemNo',
    availableStock: 'AvailableStock',
  },
  // 出库/单据/下拉列表
  handPickSelect: {
    '-1': 'All',
    '0': 'ToBeSent',
    '1': 'HasBeenSent',
    '2': 'InExecution',
    '3': 'ManualDismounting',
    '4': 'RemovedFromTheShelf',
    '5': 'Reviewed',
  },
  // 出库/手动练选
  manualSelection: {
    manualPicking: 'ManualPicking',
    picked: 'Picked',
    numberOfLocks: 'NumberOfLocks',
    inventoryLock: 'InventoryLock',

  },
  // 出库/退料
  returnMaterial: {
    returnMaterial: 'ReturnMaterial',
    complete: 'Complete',
    barCode: 'BarCode',
    barcodeQuantity: 'BarcodeQuantity',
    currentLocation: 'CurrentLocation',
    warehouseReturn: 'WarehouseReturn',
  },
  // 出库/任务
  issueTask: {
    // 输入框提示
    issueTaskNo: 'IssueTaskNo',
    issueDocNo: 'IssueDocNo',

  },

  // 推荐出库物料
  recommendedOutboundMaterials: {
    outboundMaterial: 'OutboundMaterial',
    materialSuggestion: 'MaterialSuggestion',
    pickingTime: 'PickingTime',
    implement: 'Implement'
  },
  // 执行出库
  executeDelivery: {
    executeDelivery: 'ExecuteDelivery',
    outboundQuantity: 'OutboundQuantity',
    startContainer: 'StartContainer',
    depositInContainer: 'DepositInContainer',
    // 输入框提示
    pleaseEnterTheIssueQuantity: 'PleaseEnterTheIssueQuantity',
    pleaseOutputTheLocationCode: 'PleaseOutputTheLocationCode'
  },
  // 出库单据打印
  documentPrinting: {
    issueTask: 'IssueTask',
    container: 'Container'
  },
  // 历史出库
  historicalDelivery: {
    shelfTray: 'ShelfTray',
    deliveryTime: 'DeliveryTime',
  },

  // 模具管理/数据
  moduleManagement: {
    moldName: 'MoldName',
    floorNumber: 'FloorNumber',
    dieCode: 'DieCode',
    dieName: 'DieName',
    lastPayee: 'LastPayee',
    lastCollectionTime: 'LastCollectionTime',
    lastCollectionCategory: 'LastCollectionCategory',
    lastReturnedBy: 'LastReturnedBy',
    lastReturnTime: 'LastReturnTime',
    durationOfCollection: 'DurationOfCollection',
    // 输入框提示
    pleaseEnterTheMoldCode: 'PleaseEnterTheMoldCode',
    pleaseEnterTheWarehouseCode: 'PleaseEnterTheWarehouseCode',
    pleaseEnterTheReceiver: 'PleaseEnterTheReceiver',
    pleaseEnterTheReturnee: 'PleaseEnterTheReturnee',
    pleaseEnterTheCollectionTime: 'PleaseEnterTheCollectionTime',
    selectCollectingTime: 'SelectCollectingTime',
    selectReturnTime: 'SelectReturnTime',
    moldInformationRemarks: 'MoldInformationRemarks',

    collectionType: 'CollectionType',
    receiver: 'Receiver',
    collectionTimet: 'CollectionTimet',
    estimatedReturnTime: 'EstimatedReturnTime',
    sourceDocument: 'SourceDocument',
    receivingRemarks: 'ReceivingRemarks',
    toolDescription: 'ToolDescription',
    receivedQuantity: 'ReceivedQuantity',
    moldBarCode: 'MoldBarCode',
    xAxis: 'XAxis',
    yAxis: 'YAxis',
    returnee: 'Returnee',
    returnTime: 'ReturnTime',
    serviceTime: 'ServiceTime(min)',
    // 输入框提示
    requisitionNo: 'RequisitionNo',
    pleaseEnterTheSourceDocumentNumber: 'PleaseEnterTheSourceDocumentNumber',
    requisitionDescription: 'RequisitionDescription',
    pleaseSelectCollectingTime: 'PleaseSelectCollectingTime',
    pleaseEstimatedReturnTime: 'PleaseEstimatedReturnTime',

    taskCode: 'TaskCode',
    collectingTask: 'CollectingTask',
    container: 'Container',
    // 输入框提示
    collectingMoNo: 'CollectingMoNo',
    pleaseSelectTheReturnee: 'PleaseSelectTheReturnee',
  },
  // 库存管理
  stockManagement: {
    // 输入框提示
    warehouseCodeAndName: 'WarehouseCodeAndName',
    containerCode: 'ContainerCode',
    palletNumber: 'PalletNumber',
    storageCode: 'StorageCode',
    supplierCodeSupplierName: 'SupplierCodeSupplierName',

    // 物料库存
    // 输入框提示
    containerCoding: 'ContainerCoding',
    theTotalNumber: 'TheTotalNumber',
    lockTheNumber: 'LockTheNumber',
    theNumberOfAvailable: 'TheNumberOfAvailable',
    bomNumberAndBomName: 'BomNumberAndBomName',

    // 库存上线下预警
    alarmStatus: 'AlarmStatus',
    upperLimitOfTheInventory: 'UpperLimitOfTheInventory',
    inventoryFloor: 'InventoryFloor',
    inventoryQuantity: 'InventoryQuantity',
    expiryDate: 'ExpiryDate',
    palletCoding: 'PalletCoding',
    locationAddress: 'LocationAddress',
    expiryDateDays: 'ExpiryDate(Days)',
    theBarcode: 'TheBarcode',

  },

  // 设备管理
  equipmentManagement: {
    SynchronizeEditing: 'SynchronizeEditing',
    SynchronizeDeleted: 'SynchronizeDeleted',
    equipmentManagement: 'EquipmentManagement',
    equipmentInformation: 'EquipmentInformation',
    equipmentAddress: 'EquipmentAddress',
    thePortNumber: 'ThePortNumber',
    describe: 'Describe',
    brandCategory: 'BrandCategory',
    locationCoding: 'LocationCoding',
    empty: 'Empty',
    lessHalf: 'LessHalf',
    forTheMostPart: 'ForTheMostPart',
    isFull: 'IsFull',
    forAVehicle: 'ForAVehicle',
    theLengthOfThe: 'TheLengthOfThe',
    theWidthOfThe: 'TheWidthOfThe',
    currentStorageMaterials: 'CurrentStorageMaterials',
    currentStorageQuantity: 'CurrentStorageQuantity',
    maximumStorageQuantity: 'MaximumStorageQuantity',
    equipmentCoding: 'EquipmentCoding',
    theEquipmentBrand: 'TheEquipmentBrand',
    equipmentCategory: 'EquipmentCategory',
    deviceIPAddress: 'DeviceIPAddress',
    equipmentPort: 'EquipmentPort',
    deviceDescription: 'DeviceDescription',
    // 输入框提示
    device: 'PleaseDescribeTheSpecificFunctionsOfTheDevice',

    // 设备报警
    containerCodeInformation: 'ContainerCodeInformation',
    containerNumber: 'ContainerNumber',
    thePoliceDescribed: 'ThePoliceDescribed',
    timeOfOccurrence: 'TimeOfOccurrence',
    duration: 'Duration(min)',
    equipmentAlarmCode: 'EquipmentAlarmCode',
    // 输入框提示
    pleaseEnterTheAlarmCodeOfTheDevice: 'PleaseEnterTheAlarmCodeOfTheDevice',
    pleaseEnterTheDeviceCodeName: 'PleaseEnterTheDeviceCodeName',
    pleaseEnterTheDeviceStatus: 'PleaseEnterTheDeviceStatus',
    pleaseEnterAnAlarmDescription: 'PleaseEnterAnAlarmDescription',
    pleaseEnterTheWarehouseCode: 'PleaseEnterTheWarehouseCode',
  },

  // 盘点管理
  inventoryManagement: {
    // 盘点单据
    inventoryNumber: 'InventoryNumber',
    inventoryType: 'InventoryType',
    startTime: 'StartTime',
    selectTheAreaForCounting: 'SelectTheAreaForCounting',
    inventoryWarehouse: 'InventoryWarehouse',
    containersCounted: 'ContainersCounted',
    inventoryPallet: 'InventoryPallet',
    inventorySheetDetails: 'InventorySheetDetails',
    physicalInventory: 'PhysicalInventory',
    stockist: 'Stockist',
    inventoryTime: 'InventoryTime',
    selectInventoryContainer: 'SelectInventoryContainer',
    inventoryListDetails: 'InventoryListDetails',
    storeACode: 'StoreACode',
    containerYard: 'ContainerYard',
    // 输入框提示
    warehouseCodeAndName: 'WarehouseCodeAndName',
    pleaseSelectContainer: 'PleaseSelectContainer',
    pleaseEnterTheLocationCode: 'PleaseEnterTheLocationCode',
    pleaseSelectInventoryType: 'PleaseSelectInventoryType',

    // 盘点任务
    areaCode: 'AreaCode',
    inventoryToComplete: 'InventoryToComplete',
    thePalletNo: 'ThePalletNo',
    manualInventory: 'ManualInventory',
    addInventoryBarCode: 'AddInventoryBarCode',
    regionalDetails: 'RegionalDetails',
  },

  // 数据报表
  dataReport: {
    // 库龄报表
    allWarehouse: 'AllWarehouse',
    libraryOfAge: 'LibraryOfAge',

    //呆滞料报表
    dullNumber: 'DullNumber',
    // 输入框提示
    dullDays: 'DullDays',
    dullMaterial: 'DullMaterial',
    dateOfLastShipment: 'DateOfLastShipment',
    dateOfLastWarehousing: 'DateOfLastWarehousing',

    // 物料状态报表
    materialUnit: 'MaterialUnit',
    normalNumber: 'NormalNumber',
  },

  // 基础信息
  basicInformation: {
    // 仓库信息
    warehouseInformation: 'WarehouseInformation',
    devicePortNumber: 'DevicePortNumber',
    model: 'Model',
    vehiclewidth: 'Vehiclewidth(mm)',
    theLengthOfTheVehicle: 'TheLengthOfTheVehicle(mm)',
    transversePut: 'TransversePut',
    longitudinalPut: 'LongitudinalPut',
    xAxisStorageQuantity: 'XAxisStorageQuantity',
    yAxisStorageQuantity: 'YAxisStorageQuantity',
    addLoadWith: 'AddLoadWith',
    theXaxisOfTheOriginIsShifted: 'TheXaxisOfTheOriginIsShifted',
    theYaxisOfTheOriginIsShifted: 'TheYaxisOfTheOriginIsShifted',
    currentMaintenanceTray: 'CurrentMaintenanceTray',
    layer: 'Layer',
    lights: 'Lights',
    theBindingMaterial: 'TheBindingMaterial',
    bindingAVehicle: 'BindingAVehicle',
    theScaleOf1: 'TheScaleOf1 ',
    setUserGroupPermission: 'SetUserGroupPermission-',
    selectAUserGroup: 'SelectAUserGroup',
    workNumber: 'WorkNumber',
    listOfOperating: 'ListOfOperating',
    deviceType: 'DeviceType',
    deviceTypeCoding: 'DeviceTypeCoding',

    maintainAStorage: 'MaintainAStorage',
    deleteAllStorageLocationsUnderTheTray: 'DeleteAllStorageLocationsUnderTheTray',
    theExportLocation: 'TheExportLocation',
    warehouseCategory: 'WarehouseCategory',
    theWarehouseAddress: 'TheWarehouseAddress',
    containerBrand: 'ContainerBrand',
    containerType: 'ContainerType',
    theIpAddress: 'TheIpAddress',
    theSerialNumber: 'TheSerialNumber',
    trayBearingCapacity: 'TrayBearingCapacity(Kg)',
    widthOfTheTray: 'WidthOfTheTray（mm）',
    palletDepth: 'PalletDepth（mm）',
    numberOfXAxisLights: 'NumberOfX-AxisLights',
    numberOfYAxisLights: 'NumberOfY-AxisLights',
    storeTheBitWidth: 'StoreTheBitWidth(mm)',
    storeALength: 'StoreALength(mm)',
    toEnableThe: 'ToEnableThe',
    // 输入框提示
    pleaseEnterTheStorageCode: 'PleaseEnterTheStorageCode',
    pleaseEnterBomNumberOrName: 'PleaseEnterBomNumberOrName',
    pleaseEnterVehicleCodeOrName: 'PleaseEnterVehicleCodeOrName',
    pleaseEnterWarehouseCategory: 'PleaseEnterWarehouseCategory',
    pleaseEnterTheWarehouseAddress: 'PleaseEnterTheWarehouseAddress',
    pleaseEnterTheQuantityOfXAxisStorage: 'PleaseEnterTheQuantityOfXAxisStorage',
    pleaseEnterTheQuantityOfYAxisStorage: 'PleaseEnterTheQuantityOfYAxisStorage',
    selectABindingMaterial: 'SelectABindingMaterial',
    materialCodeOrMaterialName: 'MaterialCodeOrMaterialName',
    pleaseEnterTheWarehouseName: 'PleaseEnterTheWarehouseName',
    enterTheDevicePortNumber: 'enterTheDevicePortNumber',
    pleaseEnterTheDeviceTrayNumber: 'PleaseEnterTheDeviceTrayNumber',
    pleaseSelectTheVehicleYouWantToStore: ' PleaseSelectTheVehicleYouWantToStore',
    pleaseSelectVehicleType: 'PleaseSelectVehicleType',

    pleaseWaitWhileLoadingLayoutInformation: 'PleaseWaitWhileLoadingLayoutInformation',
    selectADeviceModel: 'SelectADeviceModel',
    pleaseEnterTheUniqueDeviceSerialNumber: 'PleaseEnterTheUniqueDeviceSerialNumber',
    numberOfEquipmentTrays: 'NumberOfEquipmentTrays',
    pleaseEnterTheNumberOfEquipmentTrays: 'PleaseEnterTheNumberOfEquipmentTrays',
    singlePalletBearing: 'SinglePalletBearing(Kg)',
    disabled: 'IfTheValueIs 0,TheBearingCheckOfTheTrayIsDisabled',
    pleaseEnterTheEquipmentTrayWidth: 'PleaseEnterTheEquipmentTrayWidth',
    pleaseEnterTheEquipmentTrayDepth: 'PleaseEnterTheEquipmentTrayDepth',
    theXAxisLampNumber: 'TheXAxisLampNumber',
    theYAxisLampNumber: 'TheYAxisLampNumber',
    pleaseEnterTheNumberOfXAxisLightsOfTheDevice: 'PleaseEnterTheNumberOfXAxisLightsOfTheDevice',
    pleaseEnterTheNumberOfYAxisLightsOfTheDevice: 'PleaseEnterTheNumberOfYAxisLightsOfTheDevice',
    enterTheIPAddressOfTheDevice: 'EnterTheIPAddressOfTheDevice',

    // 字典明细
    theDictionaryClass: 'TheDictionaryClass',
    dictionaryCodeOrName: 'DictionaryCodeOrName',
    theDictionaryCoding: 'TheDictionaryCoding',
    nameOfTheDictionary: 'NameOfTheDictionary',
    extensionValue: 'ExtensionValue',
    theCategoryCode: 'TheCategoryCode',
    toEnableThe: 'ToEnableThe',
    theSorting: 'TheSorting',
    theDictionaryNote: 'TheDictionaryNote',
    // 输入框提示
    pleaseEnterTheDictionaryCode: 'PleaseEnterTheDictionaryCode',
    pleaseEnterADictionaryName: 'PleaseEnterADictionaryName',
    pleaseEnterADictionaryExtensionValue: 'PleaseEnterADictionaryExtensionValue',
    pleaseSelectADictionaryCategory: 'PleaseSelectADictionaryCategory',

    // 字典类别
    categoryName: 'CategoryName',
    theSuperiorCoding: 'TheSuperiorCoding',
    //输入框提示
    pleaseEnterTheCategoryCode: 'PleaseEnterTheCategoryCode',
    pleaseEnterACategoryName: 'PleaseEnterACategoryName',
    pleaseEnterTheSuperiorCode: 'PleaseEnterTheSuperiorCode',
    dictionaryCategoryCodeOrName: 'DictionaryCategoryCodeOrName',

    // 物料属性组
    attributeGroupName: 'AttributeGroupName',
    largestInventory: 'LargestInventory',
    theMinimumInventory: 'TheMinimumInventory',
    firstInFirstOut: 'FirstInFirstOut',
    precision: 'Precision',
    batchesOfMaterial: 'BatchesOfMaterial',
    storeLocked: 'StoreLocked',
    singlePackageBarCode: 'SinglePackageBarCode',
    whetherTheMixedBatchOf: 'WhetherTheMixedBatchOf',
    costCenter: 'costCenter',
    agingTimeDays: 'AgingTime(Days)',
    expiryDateDays: 'ExpiryDate(Days)',
    agingTime: 'AgingTime',
    shelfLife: 'ShelfLife',
    day: 'Day',
    expiryDate: 'ExpiryDate',
    // 输入框提示
    nameOfTheMaterialPropertyGroup: 'NameOfTheMaterialPropertyGroup',
    pleaseEnterTheMaterialPropertyGroupName: 'PleaseEnterTheMaterialPropertyGroupName',
    pleaseInputTheMaximumStockOfMaterials: 'PleaseInputTheMaximumStockOfMaterials',
    pleaseEnterTheMinimumInventoryOfMaterials: 'PleaseEnterTheMinimumInventoryOfMaterials',
    pleaseSelectFifO: 'PleaseSelectFifO',
    pleaseSelectFifOAccuracy: 'PleaseSelectFifOAccuracy',
    pleaseEnterTheValidityPeriod: 'PleaseEnterTheValidityPeriod',
    pleaseEnterAgingTime: 'PleaseEnterAgingTime - Days ',
    pleaseEnterMaterialCostCenter: 'PleaseEnterMaterialCostCenter',
    attributeGroup: 'PleaseDescribeTheSpecificFunctionsOfTheMaterialAttributeGroup ',

    // 条码模板
    personalCenter: 'PersonalCenter',
    uploadThePicture: 'UploadThePicture',

    // 客户
    theCustomerName: 'TheCustomerName',
    // 输入框提示
    customerCodeAndCustomerName: 'CustomerCodeAndCustomerName',
    pleaseEnterTheCustomerCode: 'PleaseEnterTheCustomerCode',
    pleaseEnterACustomerName: 'PleaseEnterACustomerName',
    pleaseEnterContacts: 'PleaseEnterContacts',
    pleaseEnterContactInformation: 'PleaseEnterContactInformation',
    pleaseEnterCustomerAddress: 'PleaseEnterCustomerAddress',
    customerRemarks: 'CustomerRemarks',

    // 货柜初始化
    verticalStrokeLearning: 'VerticalStrokeLearning',
    horizontalStrokeLearning: 'HorizontalStrokeLearning',
    automaticDoorTravelLearning: 'AutomaticDoorTravelLearning',
    trayScanned: 'TrayScanned',
    automaticAccessTray: 'AutomaticAccessTray',
    addTheTray: 'AddTheTray',
    deleteTheTray: 'DeleteTheTray',
    defragmentStorageSpace: 'DefragmentStorageSpace',
    theNextStep: 'TheNextStep',
    numberOfFrontBrackets: 'NumberOfFrontBrackets',
    numberOfRearBrackets: 'NumberOfRearBrackets',
    numberOfFrontPallets: 'NumberOfFrontPallets',
    numberOfRearTrays: 'NumberOfRearTrays',
    thePalletNo: 'ThePalletNo',
    bracket: 'Bracket',
    height: 'Height',
    //标题
    step1: 'Step 1',
    step2: 'Step 2',
    step3: 'Step 3',
    step4: 'Step 4',
    step5: 'Step 5',
    step6: 'Step 6',
    step7: 'Step 7',
    numberOfFrontAndRearBrackets: 'NumberOfFrontAndRearBrackets',
    numberOfTraysInFrontAndRear: 'NumberOfTraysInFrontAndRear',
    startdefiningWriteTrayNumber: 'StartdefiningWriteTrayNumber',
    writeTheDestinationTrayNumber: 'WriteTheDestinationTrayNumber',
    theBracketNumberShownInD651: 'TheBracketNumberShownInD651',
    theHeightShownInD652: 'TheHeightShownInD652',
    writeTheDestinationTrayNumber: 'WriteTheDestinationTrayNumber',
    targetTrayNumber: 'WritesTheDestinationTrayNumberThatYouWantToDelete ',
    spaceUtilization: 'SpaceUtilization',
    // 描述
    beginVerticalStrokeLearning: 'BeginVerticalStrokeLearning',
    monitoringTheM340: 'MonitoringTheM340ONIndicatesThatLearningIsCompleteAndTheNumberOfFrontBracketsAndRearBracketsIsDisplayedInTheFrame ',
    endOfTheVertical: 'VerifyThatVerticalStrokeLearningIsComplete ',
    theLevelOfLearning: 'BeginHorizontalLearning ',
    monitoringLevelLearning: 'MonitorLevelLearningStatus ',
    endOfTheLevel: 'MakeSureTheLevelTripStudyIsOver ',
    automaticLearning: 'StartAutomaticDoorTravelLearning ',
    automaticLearningState: 'MonitorAutomaticDoorLearningStatus ',
    automaticLearningEnd: 'ConfirmTheCompletionOfAutomaticDoorTravelLearning',
    startTrayScan: 'StartTrayScan',
    monitorTrayScanStatus: 'MonitorTheScanningStatusOfTrays,DisplayTheNumberOfTraysInFrontAndBackWhenTheScanningIsOver ',
    startDefining: 'StartDefining',
    monitorTheTray: 'TheDialogBoxPopsUpAfterMonitoringTheTrayArrivalLocationMarkerM392ON.EnterTheTrayNumberAndClick"Confirm"ToCloseTheDialogBox  ',
    defineTheNextOne: 'Let\'sGoAheadAndDefineTheNextOne ',
    monitorM395: 'MonitorM395.ONIndicatesThatAllOperationsAreCompleted.AfterOFF,ReturnToStep4 ',
    completeAllPallets: 'VerifyThatAllTrayDefinitionsAreComplete ',
    writeTraySerialNumberToD650: 'WriteTraySerialNumberToD650',
    queryM650: 'QueryTheTrayNumberDisplayedOnTheD651ScreenOfTheM650 ',
    startTheM651: 'StartTheM651',
    displayMaterialHeightInD652: 'DisplayMaterialHeightInD652',
    writeTheSerialNumberOfTrayD700: 'WriteTheTrayNumberToBeAddedToTheD700 ',
    buttonM700ToStartExecution: 'ButtonM700ToStartExecution',
    scanMaterialHeightM701: 'ScanningMaterialHeightM701,OFFSpaceIsInsufficient,ONSpaceIsSufficient  ',
    confirmThatM702IsAdded: 'ConfirmThatM702IsAdded',
    writeTheTraySerialNumberToBeDeletedToD750: 'WriteTheTraySerialNumberToBeDeletedToD750',
    buttonM750ToStartExecution: 'ButtonM750ToStartExecution',
    displayTrayTrayNumberInD751: 'DisplayTrayTrayNumberInD751',
    startRunningDeviceM751: 'StartRunningDeviceM751',
    monitoringTrayInPlaceMarkerM752TRUEInPlace: 'MonitoringTrayInPlaceMarkerM752TRUEInPlace',
    confirmOrCancelTheDeletion: 'ConfirmOrCancelTheDeletion',
    pressToStartSorting: 'PressToStartSorting',
    monitorM801: 'MonitorM801TRUEToShowSpaceUtilization ',
    confirmFinishing: 'ConfirmFinishing',

    // 物料信息
    singlePackageNumber: 'SinglePackageNumber',
    maximumStorage: 'MaximumStorage',
    costCenter: 'CostCenter',
    materialType: 'MaterialType',
    theWeightOfThe: 'TheWeightOfThe（g）',
    note1: 'Note1',
    note2: 'Price',
    note3: 'Purpose',
    note4: 'Note4',
    note5: 'Note5',
    toGenerateTheBarcode: 'ToGenerateTheBarcode',
    basicInformation: 'BasicInformation',
    choosePicture: 'ChoosePicture',
    attributeInformation: 'AttributeInformation',
    attributeGroup: 'AttributeGroup',
    numberOfPackages: 'NumberOfPackages',
    nameOfVehicleBox: 'NameOfVehicleBox',
    theLengthOfThe: 'TheLengthOfThe（mm）',
    theWidthOfThe: 'TheWidthOfThe（mm）',
    maximumStorageQuantity: 'MaximumStorageQuantity',
    chooseAVehicle: 'ChooseAVehicle',
    toGenerateTheNumber: 'ToGenerateTheNumber',
    vehicleInformation: 'VehicleInformation',
    vehicleCode: 'VehicleCode',
    imageInformation: 'ImageInformation',
    typeOfVehicleBox: 'TypeOfVehicleBox',
    nameOfVehicleBox: 'NameOfVehicleBox',
    vehicleColor: 'VehicleColor',
    theBarcodeInformation: 'TheBarcodeInformation',
    coding: 'Coding',
    supplierInformation: 'SupplierInformation',
    // 输入框提示
    pleaseEnterTheMaterialName: 'PleaseEnterTheMaterialName',
    ifItIsEmpty: 'IfItIsEmpty,TheSystemAutomaticallyGeneratesIt',
    pleaseEnterTheMaterialUnit: 'PleaseEnterTheMaterialUnit',
    pleaseEnterUnitWeight: 'PleaseEnterUnitWeight',
    pleaseEnterMaterialRemarks: 'PleaseEnterMaterialRemarks',
    pleaseEnterMaterialPrice: 'pleaseEnterMaterialPrice',
    pleaseEnterMaterialPurpose: 'pleaseEnterMaterialPurpose',
    batchMaintenance: 'SelectAttributeGroupsForBatchMaintenance',
    singlePackageManagement: 'StartSinglePackageManagementForMouldPropertyMaterials',
    pleaseEnterPackingQuantity: 'PleaseEnterPackingQuantity',
    vehicleClassCode: 'VehicleClassCode',
    lengthOfVehicleBox: 'LengthOfVehicleBox',
    widthOfVehicleBox: 'WidthOfVehicleBox',
    barCodePrintingTask: 'PleaseWaitWhileTheBarCodePrintingTaskIsBeingSent '
  },

  // 系统管理
  systemManagement: {

    // 个人中心
    personalCenter: 'PersonalCenter',
    uploadingFacePictures: 'UploadingFacePictures',
    facePhotosShouldBeLessThan200K: 'FacePhotosShouldBeLessThan200K',
    confirmPassword: 'ConfirmPassword',
    male: 'Male',
    female: 'Female',
    theName: 'TheName',
    gender: 'Gender',
    mobilePhone: 'MobilePhone',
    // 输入框提示
    pleaseEnterYourName: 'PleaseEnterYourName',
    PhoneNumber: ' PleaseEnterYourMobilePhoneNumber',
    password: 'Password',

    // 部门管理
    noteInformation: 'NoteInformation',
    head: 'Head',
    theSuperiorCoding: 'TheSuperiorCoding',
    departmentName: 'DepartmentName',
    departmentCode: 'DepartmentCode',
    // 输入框提示
    nameOfResponsiblePerson: 'PleaseEnterTheNameOfThePersonResponsible ',
    pleaseEnterTheDepartmentCode: 'PleaseEnterTheDepartmentCode',
    pleaseEnterADepartmentName: 'PleaseEnterADepartmentName',
    pleaseEnterTheSuperiorCode: 'PleaseEnterTheSuperiorCode',

    // 用户组
    departmentInformation: 'DepartmentInformation',
    creationDate: 'CreationDate',
    userGroupAuthorization: 'UserGroupAuthorization',
    // 输入框提示
    userGroupName: 'UserGroupName',
    pleaseEnterAUserGroupName: 'PleaseEnterAUserGroupName',
    pleaseSelectTheDepartmentOnTheLeft: 'PleaseSelectTheDepartmentOnTheLeft',
    specificunFction: 'PleaseDescribeTheSpecificFunctionsOfTheUserGroup',

    // 用户管理
    theUserName: 'TheUserName',
    userGroups: 'UserGroups',
    employeeCardNumber: 'EmployeeCardNumber',
    theDefaultPasswordIs: 'TheDefaultPasswordIs：123456',
    theUserAuthorization: 'TheUserAuthorization',
    // 输入框提示
    pleaseEnterAUserName: 'PleaseEnterAUserName',
    pleaseEntereMployeeCardNumber: 'PleaseEntereMployeeCardNumber',

    // 模块管理
    moduleCoding: 'ModuleCoding',
    theNameOfTheModule: 'TheNameOfTheModule',
    icon: 'Icon',
    chooseTheIcon: 'ChooseTheIcon',

    // 编码规则
    codeName: 'CodeName',
    resetTheRules: 'ResetTheRules',
    stepLength: 'StepLength',
    theSerialNumber: 'TheSerialNumber',
    theCurrentCode: 'TheCurrentCode',
    entityMapping: 'EntityMapping',
    createInformation: 'CreateInformation',
    theInputCode: 'TheInputCode',
    enterAName: 'EnterAName',
    descr: 'Descr',
    ruleProject: 'RuleProject',
    theRules: 'TheRules',
    regularAssembly: 'RegularAssembly',
    value: 'Value',
    theSorting: 'TheSorting',
    // 输入框提示
    fillMode: 'FillMode',
    theNewRules: 'TheNewRules',
    fillingLength: 'FillingLength',
    fillCharacter: 'FillCharacter',
    fillMode: 'FillMode',

    // 日志管理
    theOperationName: 'TheOperationName',
    operatingTime: 'OperatingTime',
    theDurationOfThe: 'TheDurationOfThe',
    theOperationLog: 'TheOperationLog',
    logOnToLog: 'LogOnToLog',
    readLog: 'ReadLog',


    // 定时任务
    scheduledTaskName: 'ScheduledTaskName',
    taskId: 'TaskId',
    theNameOfTheTask: 'TheNameOfTheTask',
    theTaskObject: 'TheTaskObject',
    taskStatus: 'TaskStatus',
    repeatTheNumber: 'RepeatTheNumber',
    nextExecutionTime: 'NextExecutionTime',
    timeIntervalBetween: 'TimeIntervalBetween',
    intervalType: 'IntervalType',
    // 输入框提示
    enterAScheduledTaskName: 'EnterAScheduledTaskName',
    selectATaskObject: 'SelectATaskObject',
    selectDateAndTime: 'SelectDateAndTime',
    pleaseEnterTheIntervalForStartingTheTask: 'PleaseEnterTheIntervalForStartingTheTask',
    pleaseSelectAnIntervalType: 'PleaseSelectAnIntervalType',

    // 定时对象
    timingObjectCoding: 'TimingObjectCoding',
    objectCode: 'ObjectCode',
    contract: 'Contract',
    methods: 'Methods',
    theAssembly: 'TheAssembly',

  },

  // 精选
  handPick: {
    // 输入框提示
    material: 'MaterialCode,MaterialNameAndMaterialBarcode',
    // 输入框提示
    deliveryOrderNo: 'DeliveryOrderNo',
    query: 'Query',
    pickingTime: 'PickingTime',
    reviewTime: 'ReviewTime',
    selectPeople: 'SelectPeople',
    reviewer: 'Reviewer',
    state: 'State',
    deliveryOrderNo: 'DeliveryOrderNo',
    materialName: 'MaterialName',
    pickingQuantity: 'PickingQuantity',
    pickedQuantity: 'PickedQuantity',
    cancel: 'Cancel',
    recheckQuantity: 'RecheckQuantity',
    company: 'Company',
    locationCode: 'LocationCode',
    warehouseName: 'WarehouseName',
    areaName: 'AreaName'
  },
  // 入库单据提示
  presentationOfWarehousingDocuments: {
    pleaseScanTheBarCode: 'PleaseScanTheBarCode',
    pleaseEnterTheCorrectNumber: 'PleaseEnterTheCorrectNumber',
    numberOfBarCodesInAPacket: 'Enter the correct number of bar codes in a single packet | 请输入正确的单包条码数量',
    editTheEntryList: 'EditTheEntryList',
    createAnEntryList: 'CreateAnEntryList',
    pleaseEnterTheWarehouseEntryNumber: 'PleaseEnterTheWarehouseEntryNumber',
    pleaseSelectWarehouse: 'PleaseSelectWarehouse',
    pleaseEnterTheNumberOfPackets: 'PleaseEnterTheNumberOfPackets',
    pleaseEnterTheNumberOfCorrectFormats: 'PleaseEnterTheNumberOfCorrectFormats',
    pleaseEnterTheCorrectNumberOfTagGeneration: 'PleaseEnterTheCorrectNumberOfTagGeneration',
    pleaseEnterTheCorrectBatch: 'PleaseEnterTheCorrectBatch',
    pleaseAddMaterialDetails: 'PleaseAddMaterialDetails',
    itHasBeenCancelledAndInvalidated: 'ItHasBeenCancelledAndInvalidated',
    theWarehouseEntryWasCancelledSuccessfully: 'TheWarehouseEntryWasCancelledSuccessfully',
    theWarehouseEntryWasCancelledFailure: 'TheWarehouseEntryWasCancelledFailure',
    pleaseSelectPrinter: 'PleaseSelectPrinter',
    pleaseSelectTheEntryListThatRequiresBarcodePrinting: 'PleaseSelectTheEntryListThatRequiresBarcodePrinting',
    printTaskSentPleaseWait: 'PrintTaskSentPleaseWait',
    songTypeface: 'SongTypeface',
    warehousingOrder: 'WarehousingOrder',
    nonEditable: 'NonEditable',
    updateSuccessful: 'UpdateSuccessful',
    cannotDelete: 'CannotDelete',
    permanentlyDeleteTheReceiptDoc: 'This operation will permanently delete the receipt doc. Do you want to continue? ',
    tips: 'Tips',
    pleaseSelectAMaterial: 'PleaseSelectAMaterial',
    pleaseEnterTheProductionDate: 'PleaseEnterTheProductionDate',
    pleaseEnterTheCorrectNumber: 'Please enter the correct number (number with two decimal places or number without decimal places) ',
    uploadFile: 'OneFileIsCurrentlyRestricted.PleaseDeleteItAndContinue uploading ',
    pleaseScanOrEnterBarcodeFirst: 'PleaseScanOrEnterBarcodeFirst',
    notObtained: 'TheBarcodeDetailsAreNotObtained.PleaseCheckTheBarcodeInformation ',
    noMaterialReceipt: 'TheReceiptDocumentCannotBeCreatedWithoutReceiving ',
    notToBePutOnTheShelf: 'NotToBePutOnTheShelf',
    lightOnTaskSentSuccessfully: 'LightOnTaskSentSuccessfully',
    lightOnTaskSentFailure: 'LightOnTaskSentFailure',

    // 入库任务
    noBarcodeInformationWasObtained: 'NoBarcodeInformationWasObtained',
    itemDoesNotBelongTo: 'This item does not belong to the item received in this line ',
    failedToObtainTheBarCodeInformationOnTheShelf: 'FailedToObtainTheBarCodeInformationOnTheShelf',
    startExecution: 'StartExecution',
    nonExecutable: 'NonExecutable',
    sorry: 'Sorry, you dont have permission to operate this tray',
    manualExecutionCanceled: 'ManualExecutionCanceled',
    unableToStockIn: 'The correct tag quantity is not obtained and cannot be warehoused ',
    excessQuantity: 'Receipt quantity exceeds receivable quantity ',
    successfulExecution: 'SuccessfulExecution',
    failureExecution: 'FailureExecution',

    // 历史入库
    youHaveNotSelectedAnExportCondition: 'you have not selected the export condition. All data will be exported. Do you want to continue? ',
    permanentlyDeleteTheVehicleBox: 'This operation will permanently delete the vehicle box. Do you want to continue? ',
    thisOperationWillExportAllData: 'ThisOperationWillExportAllData',
    article: 'article?',
  },

  // 出库单提示
  deliveryPrompt: {
    editIssuedoc: 'EditIssuedoc',
    createIssuedoc: 'CreateIssuedoc',
    pleaseEnterTheDeliveryOrderNo: 'PleaseEnterTheDeliveryOrderNo',
    pleaseSelectAWarehouse: 'pleaseSelectAWarehouse',
    thisItemIsNotAnIssueItem: 'This item does not belong to the issue item of this line ',
    thisBarcodeBatch: 'This barcode batch does not match the issue task  ',
    failedToGetPickingBarcodeInformation: 'FailedToGetPickingBarcodeInformation',
    trayPermissions: 'Sorry, you dont have permission to operate this tray',
    printBarcodeDeliveryOrder: 'Please select the delivery order to print barcode',
    outboundOrder: 'OutboundOrder',
    notForGoodsToBePicked: 'NotForGoodsToBePicked',
    theBarcodeDetailsWereNotObtained: 'The barcode details are not obtained. Please check the barcode information',
    pleaseScanOrEnterBarcodeFirst: 'PleaseScanOrEnterBarcodeFirst',
    pleaseEnterBatch: 'PleaseEnterBatch',
    pleaseSelectAMaterial: 'PleaseSelectAMaterial',
    permanentlyDeleteThisMo: 'This operation will permanently delete this mo. do you want to continue? ',

    // 出库单据
    thisOperationWillVoidTheIssueDoc: 'This operation will void the issue doc. Do you want to continue? ',
    pickingFailed: 'PickingFailed',
    pickingSucceed: 'PickingSucceed',
    reviewFailed: 'ReviewFailed',
    reviewSucceed: 'ReviewSucceed',
    pleaseConfirmTheRecheckQuantity: 'PleaseConfirmTheRecheckQuantity',
    theRecheckQuantityMustBe: 'The recheck quantity must be less than or equal to the picking quantity ',
    recheckQuantityMustBeNumericOrNot0: 'RecheckQuantityMustBeNumericOrNot0',
    deliverySucceeded: 'DeliverySucceeded',
    pickingHasNotStarted: 'PickingHasNotStarted',
    reviewCompleted: 'Picking has not been completed or review has been completed ',
    pleaseEnterPickingQuantity: 'PleaseEnterPickingQuantity',
    availableStockQuantity: 'The picking quantity should be less than the available inventory quantity ',
    lessThanTheRequiredQuantity: 'The picking quantity must be less than the demand quantity ',
    pickingQuantityMustBeNumericOrNot0: 'PickingQuantityMustBeNumericOrNot0',
    permanentlyDeleteTheIssueDoc: 'This operation will permanently delete the issue doc. Do you want to continue?',
    noBarcodeSelected: 'NoBarcodeSelected',
    inProgressOrCompleted: 'InProgressOrCompleted',
    issueBarcodeGenerationFailed: 'IssueBarcodeGenerationFailed',
    issueBarcodeGeneratedSuccessfully: 'IssueBarcodeGeneratedSuccessfully',
    notToBeSent: 'NotToBeSent',
    failedToVoidDeliveryOrder: 'FailedToVoidDeliveryOrder',
    succeedToVoidDeliveryOrder: 'SucceedToVoidDeliveryOrder',
  },

  // 摸具管理提示
  dieTips: {
    editMoldInformation: 'EditMoldInformation',
    createMoldInformation: 'CreateMoldInformation',
    pleaseEnterCollectingType: 'PleaseEnterCollectingType',

    // 摸具信息/下拉列表
    whole: 'Whole',
    production: 'Production',
    moldRepair: 'MoldRepair',
    cancellation: 'Cancellation',
    collectionLock: 'CollectionLock',
    inLibrary: 'InLibrary',

    // 领用单据
    editCollectingDoc: 'EditCollectingDoc',
    createCollectingDoc: 'CreateCollectingDoc',
    // 下拉列表
    whole: 'Whole',
    toBeImplemented: 'ToBeImplemented',
    haveInHand: 'HaveInHand',
    completed: 'Completed',
    voided: 'Voided',
    issuedSuccessfully: 'IssuedSuccessfully',
    issuedFailure: 'IssuedFailure',
    requisition: 'Requisition',
    voidedSuccessfully: 'VoidedSuccessfully',
    voidedFailure: 'VoidedFailure',
    pleaseAddTheMoldInformationToBeCollected: 'PleaseAddTheMoldInformationToBeCollected',
    pleaseSelectCollectingType: 'PleaseSelectCollectingType',
    pleaseSelectTheReceiver: 'PleaseSelectTheReceiver',

    // 领用任务
    returnFailed: 'ReturnFailed',
    returnSucceed: 'returnSucceed',
    doYouWantToReturnThisItemManually: 'Do you want to return this item manually? Manual execution will not drive the container operation. ',
    doYouWantToManuallyCollectThisMaterial: 'Do you want to manually collect this material? Manual execution will not drive the container operation.',
    pleaseEnterTheCollectingDocNo: 'PleaseEnterTheCollectingDocNo'

  },
  // 盘点管理提示
  inventoryTips: {
    pleaseEnterANumberInTheCorrectFormat: 'Please enter a number in the correct format (a number with two decimal places or a number without decimal places)',
    editCountSheet: 'EditCountSheet',
    createCountSheet: 'CreateCountSheet',
    pleaseEnterTheCountingBarcode: 'PleaseEnterTheCountingBarcode',
    pleaseEnterTheCountingNumber: 'PleaseEnterTheCountingNumber',
    monthlyInventory: 'MonthlyInventory',
    annualInventory: 'AnnualInventory',
    cycleCount: 'CycleCount',
    pleaseSelectTheCountingArea: 'PleaseSelectTheCountingArea',
    pleaseSelectCountingType: 'PleaseSelectCountingType',
    failedToIssueTheOrder: 'FailedToIssueTheOrder',
    theTaskOfIssuingTheOfferOrderIsSuccessful: 'TheTaskOfIssuingTheOfferOrderIsSuccessful',
    thisCountSheet: 'ThisCountSheet',
    notTheInitialState: 'NotTheInitialState',
    countingFailed: 'CountingFailed',
    countingSucceeded: 'CountingSucceeded',
    countQuantityMustBeNumeric: 'CountQuantityMustBeNumeric',
    permanentlyVoidTheCount:'This operation will permanently void the count sheet. Do you want to continue? ',
    submitted:'Submitted',

    // 盘点任务
    editCountingTask: 'EditCountingTask',
    createCountingTask: 'CreateCountingTask',
    inventorySheet: 'InventorySheet',
    inventoryCountingSheetSubmittedSuccessfully: 'InventoryCountingSheetSubmittedSuccessfully',
    inventoryCountingSheetSubmittedFailure: 'InventoryCountingSheetSubmittedFailure',
    pleaseEnterTheCountingQuantity: 'PleaseEnterTheCountingQuantity',
    pleaseEnterTheQuantityInTheCorrectFormat:'Please enter the quantity in the correct format (number with two decimal places or number without decimal places)',
    pleaseEnterTheCountingQuantityInTheCorrectFormat:'Please enter the counting quantity in the correct format (including two decimal places or excluding decimal places) ',
    reCountingIsAllowedOnlyWhenTheCountingSheetIsCompleted: 'ReCountingIsAllowedOnlyWhenTheCountingSheetIsCompleted',
    restartSucceeded:'RestartSucceeded',
    restartFailure:'RestartFailure',
    areYouSureToStartTheCheckedArea: 'AreYouSureToStartTheCheckedArea?',
    ZoneStartFailed: 'ZoneStartFailed',
    ZoneStartSucceed: 'ZoneStartSucceed',
    uncheckedArea: 'UncheckedArea',
    areaCompletionFailed: 'AreaCompletionFailed',
    areaCompletionSucceed: 'AreaCompletionSucceed',
    DetermineTheAreaToCompleteTheCheck: 'DetermineTheAreaToCompleteTheCheck?'


  },

  // 设备管理提示
  deviceTips: {
    allStates: ' AllStates',
    notEnabled: 'NotEnabled',
    inOperation: 'InOperation',
    fault: 'Fault',
    editDevice:'EditDevice',
    createDevice:'CreateDevice',
    loadInStock: 'LoadInStock',
    permanentlyDeleteDeviceInformation:'This operation will permanently delete the device information. Do you want to continue?',

    // 设备管理
    callThePolice:'CallThePolice',
    reset: 'Reset',
    resetSuccessful: 'ResetSuccessful',
    resetFailure: 'ResetFailure',
    articleContinue: 'ArticleContinue',
    thisOperationWillExportAllDataIncluding: 'ThisOperationWillExportAllDataIncluding',
    youHaveNotSelectedAnExportCondition:'You have not selected the export condition. All data will be exported. Do you want to continue?'
  },

  // 库存管理提示
  inventoryTips:{
    pickingLock: 'PickingLock',
    inventoryLock: 'InventoryLock',
    agingCycle: 'AgingCycle',
    xAxisNumber: 'XAxisNumber',
    yAxisNumber: 'YAxisNumber',
    pickingListHasNotBeenChecked:'PickingListHasNotBeenChecked',
    directOffShelfFailure: 'DirectOffShelfFailure',
    barCodeOffTheShelfSuccessfully:'BarCodeOffTheShelfSuccessfully',
    confirmTheBarcodeCheckedOffTheShelf: 'ConfirmTheBarcodeCheckedOffTheShelf?',
    failureToExtinguish:'FailureToExtinguish',
    succeedToExtinguish:'SucceedToExtinguish',
    confirmToLightUpTheCheckedInventory:'ConfirmToLightUpTheCheckedInventory?',
    
    lightingFailure:'LightingFailure',
    lightingSucceed:'LightingSucceed',
    youHaveNotSelectedAnExportConditionATotalOf:'YouHaveNotSelectedAnExportConditionATotalOf',
    dataContinue:'DataContinue',

    // 物料存储
    noExportCriteriaSelected:'No export condition is selected. All data will be exported. Do you want to continue? ',
    thisOperationWillExportAllData:'This operation will export all data. Do you want to continue? ',
    youHaveNotEnteredAnExportCondition:'you have not entered the export condition, a total of will be exported',
    ifNoExportConditionIsSelectedATotalOf: 'IfNoExportConditionIsSelectedATotalOf',
    whole: 'Whole',
    minimumStockReached: 'MinimumStockReached',
    maximumStockReached: 'MaximumStockReached',
    lessThanMinimumStock: 'LessThanMinimumStock',
    greaterThanMaximumInventory: 'GreaterThanMaximumInventory',
    hour: 'Hour',

  },

  // 基础信息提示
  basicTips: {
    // 图片管理
    fileUploadedSuccessfully: 'FileUploadedSuccessfully',
    uploadFailed: 'UploadFailed',
    permanentlyDeleteThisPicture: 'This operation will permanently delete this picture. Do you want to continue? ',
    pictureNotDeleted:'PictureNotDeleted',

    // 物料属性组
    editItemAttributeGroup:' EditItemAttributeGroup',
    createItemAttributeGroup: 'CreateItemAttributeGroup',

    // 仓库信息
    continue: 'Continue',
    printingFailedPleaseSelectARecord: 'PrintingFailedPleaseSelectARecord',
    printingCanceled: 'PrintingCanceled',
    pleaseSelectTheWarehouseLocationForBarcodePrinting: 'PleaseSelectTheWarehouseLocationForBarcodePrinting',
    theWarehouseDoesNotContainLocationInformation: 'TheWarehouseDoesNotContainLocationInformation',
    thisOperationWillPrintUnderThiswrehouse: 'ThisOperationWillPrintUnderThiswrehouse',
    beyondXAxis:'Beyond the storage quantity in x-axis direction, the maximum storage quantity is ',
    beyondYAxis:'Beyond the storage quantity in x-axis direction, the maximum storage quantity is',
    storageLocationInformation: 'StorageLocationInformation',
    trayInformation:'TrayInformation',
    containerInformation: 'ContainerInformation',
    pleaseEnterTheCode: 'PleaseEnterTheCode',
    pleaseEnterAName: 'PleaseEnterAName',
    pleaseSelectADeviceCategory: 'PleaseSelectADeviceCategory',
    pleaseEnterTrayWidth: 'PleaseEnterTrayWidth',
    pleaseEnterTrayDepth: 'PleaseEnterTrayDepth',
    pleaseEnterThexAxisLightNumber: 'PleaseEnterThexAxisLightNumber',
    pleaseEnterTheyAxisLightNumber: 'PleaseEnterTheyAxisLightNumber',
    pleaseEnterASinglePalletLoad: 'PleaseEnterASinglePalletLoad',
    pleaseEnterTheNumberOfDeviceTrays: 'PleaseEnterTheNumberOfDeviceTrays',
    afterClearingTheBoundMaterialInformation:'After clearing the bound material information, please click the confirm button in the lower right corner ',
    gotIt: 'GotIt',
    materialsCurrentlyExistInThisStorageLocation: 'MaterialsCurrentlyExistInThisStorageLocation',
    inventoryPleaseIssueFirst:'InventoryPleaseIssueFirst',
    originCoordinates:'The origin coordinate is greater than the pallet coordinate, please move the origin manually ',
    error: 'Error',
    pleaseSelectATray: 'PleaseSelectATray',
    pleaseSelectADeviceCategory: 'PleaseSelectADeviceCategory',
    resetTheContainer: 'Reset the user operation authority of all pallets under the container ',
    failedToGetEmployeeInformation: 'FailedToGetEmployeeInformation',
    failedToGetWhitelist: 'FailedToGetWhitelist',
    whitelistSettingSucceeded: 'WhitelistSettingSucceeded',
    whitelistSettingFailure: 'WhitelistSettingFailure',
    pleaseSelectAVehicle: 'PleaseSelectAVehicle',
    addErrorInsufficientTraySpace: 'AddErrorInsufficientTraySpace',
    thisOperationWillPermanentlyDeleteThisRecord:'This operation will permanently delete this record. Do you want to continue? ',
    thisWillPrintUnderThisArea: 'ThisWillPrintUnderThisArea ',
    locationLabelContinue: 'LocationLabelContinue?',
    individual: 'Individual',
    thePalletDoesNotContainLocationInformation: 'ThePalletDoesNotContainLocationInformation',
    confirmPrinting: 'ConfirmPrinting?',

    // 物料信息
    editItem: 'EditItem',
    CreateItem: 'CreateItem',
    pleaseEnterTheUnit:'PleaseEnterTheUnit',
    pleaseEnterTheMaterialWeight:'PleaseEnterTheMaterialWeight',
    pleaseEnterTheMaterialName:'PleaseEnterTheMaterialName',
    pleaseEnterTheMaximumStorageQuantity: 'PleaseEnterTheMaximumStorageQuantity',
    pleaseSelectVehicle:'PleaseSelectVehicle',
    material:'Material',
    mould:'Mould',
    pleaseEnterTheMaterialNameInTheBasicInformationTab:'PleaseEnterTheMaterialNameInTheBasicInformationTab',
    pleaseSelectTheAssociatedVehicle:'PleaseSelectTheAssociatedVehicle',
    attributeInformation:'Attribute information: the maximum inventory cannot be less than the minimum location',
    pleaseSelectASupplier: 'PleaseSelectASupplier',
    permanentlyDeleteTheMaterial:'This operation will permanently delete the material. Do you want to continue? ',
    OtherwiseTheImportWillFail:'This operation will replace the materials existing in the system according to the material code in the import file. Please ensure that the imported material code already exists in the system and the imported material name cannot be empty, otherwise the import will fail. Do you want to continue? ',
    importCanceled: 'ImportCanceled',


    // 字典类别/字典类别
    editDictionary: 'EditDictionary',
    createDictionary: 'CreateDictionary',
    pleaseEnterSorting: 'PleaseEnterSorting',
    pleaseEnterAnExtendedValue: 'PleaseEnterAnExtendedValue',
    editDictionaryCategory: 'EditDictionaryCategory',
    createDictionaryCategory: 'CreateDictionaryCategory',
    pleaseEnterTheArticleName: 'PleaseEnterTheArticleName',
    rootDirectory: 'RootDirectory',
    permanentlyDeleteTheDictionaryCategory:'This operation will permanently delete the dictionary category. Do you want to continue? ',
    pleaseEnterTheNameOfTheMaterialAttributeGroup: 'PleaseEnterTheNameOfTheMaterialAttributeGroup',
    nothing: 'Nothing',
    warehousingTime: 'WarehousingTime',
    shelfLifeOfInventory: 'ShelfLifeOfInventory',
    second: 'Second',
    minute: 'Minute',
    permanentlyDeleteTheMaterialAttributeGroup:'This operation will permanently delete the material attribute group. Do you want to continue? ',


    
    //客户管理
    editCustome: 'EditCustome',
    createCustome: 'CreateCustome',
    permanentlyDeleteTheCustomer: 'This operation will permanently delete the customer. Do you want to continue? ',

    // 供应商管理
    editSupplier: 'EditSupplier',
    createSupplier: 'CreateSupplier',
    eermanentlyDeleteTheSupplier:'This operation will permanently delete the supplier. Do you want to continue?',
    

    // 柜货初始化
    failedToStartVerticalLearning: 'FailedToStartVerticalLearning',
    achievements: 'Achievements' ,
    endVerticalLearningSuccessfully: 'EndVerticalLearningSuccessfully',
    endVerticalLearningFailure: 'EndVerticalLearningFailure',
    theHorizontalLearningJourneyHasEnded: 'TheHorizontalLearningJourneyHasEnded',
    failedToStartHorizontalLearningJourney: 'FailedToStartHorizontalLearningJourney',
    endHorizontalJourneyLearningSuccessful: 'EndHorizontalJourneyLearningSuccessful',
    endHorizontalJourneyLearningFailure: 'EndHorizontalJourneyLearningFailure',
    theAutomaticDoorLearningJourneyHasEnded: 'TheAutomaticDoorLearningJourneyHasEnded',
    failedToStartAutomaticDoorLearningJourney: 'FailedToStartAutomaticDoorLearningJourney',
    endAutomaticDoorTravelLearningSuccessful: 'EndAutomaticDoorTravelLearningSuccessful',
    failedToStartTrayScan: 'FailedToStartTrayScan',
    startDefinitionFailed: 'StartDefinitionFailed',
    failedToGetM395Status: 'FailedToGetM395Status',
    failedToStartVerticalLearning: 'FailedToStartVerticalLearning',
    trayScanEndedSuccessfully: 'TrayScanEndedSuccessfully',
    failedToEndTrayScan: 'FailedToEndTrayScan',
    trayNumberWrittenSuccessfully: 'TrayNumberWrittenSuccessfully',
    trayNumberWrittenFailure: 'TrayNumberWrittenFailure',
    failedToGetTheCarrierNumber: 'FailedToGetTheCarrierNumber',
    failedToGetStatus: 'FailedToGetStatus',
    insufficientSpace: 'InsufficientSpace',
    failedToGetHeight: 'FailedToGetHeight',
    confirmToAddSuccessfully: 'ConfirmToAddSuccessfully',
    confirmToAddFailure: 'ConfirmToAddFailure',
    trayInPlace: 'TrayInPlace',
    runFailed: 'RunFailed',
    confirmDeletionSucceeded: 'ConfirmDeletionSucceeded',
    confirmDeletionFailure: 'ConfirmDeletionFailure',
    cancelDeletionSucceeded: 'CancelDeletionSucceeded',
    cancelDeletionFailure: 'CancelDeletionFailure',
    failedToStartDefragmentingStorageSpace: 'FailedToStartDefragmentingStorageSpace',
    finishing: 'Finishing',
    failedToConfirmTheCompletionOfSorting: 'FailedToConfirmTheCompletionOfSorting',


    // 条码信息
    editBarcode: 'EditBarcode',
    createBarcode: 'CreateBarcode',
    pleaseEnterBarcodeCode: 'PleaseEnterBarcodeCode',
    pleaseEnterBarcodeName: 'PleaseEnterBarcodeName',






  },

  // 系统管理提示
  systemManagementTips: {
    pleaseInputAPassword: 'PleaseInputAPassword',
    pleaseEnterThePasswordAgain: 'PleaseEnterThePasswordAgain',
    theTwoPasswordsAreInconsistent: 'TheTwoPasswordsAreInconsistent!',
    pleaseEnterEmployeeName: 'PleaseEnterEmployeeName',
    pleaseEnterTheCorrectMobilePhoneNumber: 'PleaseEnterTheCorrectMobilePhoneNumber',
    newEmployee: 'NewEmployee',
    CreateEmployee: 'CreateEmployee',
    newEmployeePictureNotDeleted: 'NewEmployeePictureNotDeleted',
    personalInformationEditingFailed:'PersonalInformationEditingFailed',
    personalInformationEditingSucceed:'PersonalInformationEditingSucceed',
    failedToGetUserInformation: 'FailedToGetUserInformation',

    // 部门管理
    pleaseEnterDepartmentCode: 'PleaseEnterDepartmentCode',
    pleaseEnterDepartmentName: 'PleaseEnterDepartmentName',
    editorialDepartment: 'EditorialDepartment',
    createDepartment: 'CreateDepartment',
    DepartmentEditedSuccessfully: 'DepartmentEditedSuccessfully',
    DepartmentEditedFailure: 'DepartmentEditedFailure',
    departmentCreatedSuccessfully: 'DepartmentCreatedSuccessfully',
    departmentCreatedFailure: 'DepartmentCreatedFailure',
    permanentlyDeleteThisDepartment: 'This operation will permanently delete the Department. Do you want to continue? ',

    // 用户组
    editUserGroup: 'EditUserGroup',
    createUserGroup: 'CreateUserGroup',
    pleaseSelectADepartment: 'PleaseSelectADepartment',
    permanentlyDeleteThisUserGroup:'This operation will permanently delete the user group. Do you want to continue? ',
    
    // 用户管理
    passwordNoLessThan5Digits: 'The password cannot be less than 5 digits ',
    pleaseSelectAtLeastOneRole: 'PleaseSelectAtLeastOneRole',
    correctMobilePhoneNumber:'Please enter the correct mobile phone number ',
    pleaseEnterUserCode: 'PleaseEnterUserCode',
    pleaseEnterUserName: 'PleaseEnterUserName',
    editUser: 'EditUser',
    createUser: 'CreateUser',
    permanentlyDeleteTheUser:'This operation will permanently delete the user. Do you want to continue? ',
    userEditedSuccessfully: 'UserEditedSuccessfully',
    userEditedFailure: 'UserEditedFailure',
    userCreatedSuccessfully: 'UserCreatedSuccessfully',
    userCreatedFailure: 'UserCreatedFailure',
    failedToGetRoleInformation: 'FailedToGetRoleInformation',

    // 模块管理
    eidtModule: 'EidtModule',
    createModule: 'CreateModule',
    pleaseEnterTheModuleCode: 'PleaseEnterTheModuleCode',
    pleaseEnterModuleName: 'PleaseEnterModuleName',
    pleaseEnterTheModuleAddress: 'PleaseEnterTheModuleAddress',
    permanentlyDeleteTheModule:'This operation will permanently delete the module, sub module and user authorization. Do you want to continue?',

    // 编码规则
    editCodingRule: 'EditCodingRule',
    createCodingRule: 'CreateCodingRule',
    pleaseEnterTheEncodingRuleName: 'PleaseEnterTheEncodingRuleName',
    pleaseSelectResetAssembly: 'PleaseSelectResetAssembly',
    pleaseEnterTheStepSize: 'PleaseEnterTheStepSize',
    PleaseSelectAMappingEntity: 'PleaseSelectAMappingEntity',
    noFilling:'NoFilling',
    left: 'Left',
    right: 'Right',
    permanentlyDeleteTheCodingRule:'This operation will permanently delete the coding rule. Do you want to continue? ',

    // 定时任务
    editScheduledTask: 'EditScheduledTask',
    CreateScheduledTask: 'CreateScheduledTask',
    pleaseEnterTheStartTime: 'PleaseEnterTheStartTime',
    pleaseEnterATimingName: 'PleaseEnterATimingName',
    pleaseSelectAScheduledTaskObject: 'PleaseSelectAScheduledTaskObject',
    pleaseSelectTimeType: 'PleaseSelectTimeType',
    stopFailed: 'StopFailed',
    stopSucceed: 'StopSucceed',
    startFailed: 'StartFailed',
    startSucceed: 'StartSucceed',
    permanentlyDelennteTheScheduledTask:'This operation will permanently delete the scheduled task. Do you want to continue? ',


    // 定时对象
    editTaskObject: 'EditTaskObject',
    createTaskObject: 'CreateTaskObject',
    pleaseEnterTheTaskObjectCode: 'PleaseEnterTheTaskObjectCode',
    pleaseEnterTheTaskObjectContract: 'PleaseEnterTheTaskObjectContract',
    pleaseEnterTheTimedStartMethod: 'PleaseEnterTheTimedStartMethod',
    pleaseEnterATimedStartAssembly: 'PleaseEnterATimedStartAssembly',
    permanentlyDeleteTheTaskObject:'This operation will permanently delete the task object. Do you want to continue?  ',



  }

}
