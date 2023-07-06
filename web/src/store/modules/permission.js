import { constantRouterMap } from '@/router'

import Layout from '@/layout/index'

/**
 * Use meta.role to determine if the current user has permission
 * @param roles
 * @param route
 */

/**
 * Filter asynchronous routing tables by recursion
 * @param routes asyncRoutes
 * @param roles
 */

const state = {
  routes: constantRouterMap,
  addRoutes: []
}
const mutations = {
  SET_ROUTERS: (state, routers) => {
    state.addRoutes = routers
    state.routes = constantRouterMap.concat(routers)
  }
}
const actions = {
  GenerateRoutes({ commit }, asyncRouterMap) {
    return new Promise(resolve => {
      const accessedRouters = convertRouter(asyncRouterMap)
      commit('SET_ROUTERS', accessedRouters)
      resolve()
    })
  }
}

function convertRouter(asyncRouterMap) {
  const routerList = [] // 返回的路由数组
  var routerData = asyncRouterMap.data // data中的值为数组
  if (routerData !== '[]') {
    routerData.forEach(item => {
      if (item.ParentCode === null || item.ParentCode === '') { // 如果不存在上级，则为1级菜单，此部分可根据后端返回的数据重新定义完善
        var parent = generateRouter(item, true)
        var children = []
        routerData.forEach(child => {
          if (child.ParentCode === item.Code) { // 查找该父级路由的二级路由
            children.push(generateRouter(child, false))
            parent.children = children
          }
        })
        routerList.push(parent)
      }
    })
  }
  routerList.push({ path: '*', redirect: '/404', hidden: true }) // 最后添加404路由
  return routerList
}

/**
 *将后台返回的路由进行格式化，并根据componentsMap进行匹配
 * @param {*} item
 * @param {*} isParent 是否存在上级路由
 */
function generateRouter(item, isParent) {
  var menuType = true
  if (item.Type === 'Menu') menuType = false
  var router = {
    path: item.Address,
    name: item.Name,
    hidden: menuType,
    meta: {
      menuId: item.Id,
      title: item.Name,
      icon: item.Icon,
      noCache: false
    },
    component: isParent ? Layout : componentsMap[item.Code]
  }
  return router
}

/**
 *前端需要维护对应的componentsMap，对于前后端分离来说，存在一些弊端，后期需要进行优化
 */
export const componentsMap = {
  // 系统管理
  Role: () => import('@/views/SysManage/Role/index'),
  CodeRule: () => import('@/views/SysManage/CodeRule/index'),
  User: () => import('@/views/SysManage/User/index'),
  Log: () => import('@/views/SysManage/log/index'),
  Job: () => import('@/views/SysManage/Job/index'),
  JobObject: () => import('@/views/SysManage/JobObject/index'),
  Module: () => import('@/views/SysManage/Module/index'),
  UserCenter: () => import('@/views/SysManage/UserCenter/index'),

  // 基础数据
  DcitionaryType: () => import('@/views/BaseSetting/DictionaryManage/DictionaryType/index'), // 字典类别
  Dictionary: () => import('@/views/BaseSetting/DictionaryManage/Dictionary/index'), // 字典明细
  MaterialSetting: () => import('@/views/BaseSetting/Material/index'), // 物料主数据
  MaterialProperty: () => import('@/views/BaseSetting/MaterialProperty/index'), // 物料属性组
  WareHouseSetting: () => import('@/views/BaseSetting/WareHouse/index'), // 仓库主数据
  Supply: () => import('@/views/BaseSetting/Supply/index'), // 供应商
  FileLibrary: () => import('@/views/BaseSetting/PictureModule/index'), // 图片管理
  Customer: () => import('@/views/BaseSetting/Customer/index'), // 客户
  Label: () => import('@/views/BaseSetting/Label/index'), // 条码信息
  Box: () => import('@/views/BaseSetting/Box/index'), // 载具箱信息
  Equipment: () => import('@/views/BaseSetting/EquipmentType/index'), // 设备型号信息
  PictureGallery: () => import('@/views/BaseSetting/PictureModule/index'), // 图片信息

  HistoryIn: () => import('@/views/InManage/HistoryIn/index'), // 历史入库信息管理
  HistoryOut: () => import('@/views/OutManage/HistoryOut/index'), // 历史出库管理
  ContainerInitialization: () => import('@/views/BaseSetting/ContainerInitialization/index'), // 货柜初始化

  // 库存管理
  StockInfo: () => import('@/views/StockManage/Stock/index'), // 条码库存
  Alarm: () => import('@/views/StockManage/Alarm/index'), // 库存有效期预警
  NumAlarm: () => import('@/views/StockManage/NumAlarm/index'), // 库存上下限预警
  MaterialStock: () => import('@/views/StockManage/MaterialStock/index'),
  MobileLocation: () => import('@/views/StockManage/MobileLocation/index'), // 库位移动
  StockAge: () => import('@/views/StockManage/StockAge/index'), // 库龄报表
  InactiveStock: () => import('@/views/StockManage/InactiveStock/index'), // 呆滞料报表
  MaterialStatus: () => import('@/views/StockManage/MaterialStatus/index'), // 物料状态报表
  InventoryStatus: () => import('@/views/StockManage/InventoryStatus/index'), // 物料状态报表

  // 设备管理
  DeviceInfo: () => import('@/views/DeviceManage/DeviceInfo/index'), // 载具箱信息
  DeviceAlarm: () => import('@/views/DeviceManage/DeviceAlarm/index'), // 载具箱信息

  // 入库管理

  ReceiptBill: () => import('@/views/InManage/InBill/index'), // 入库单管理
  ReceiptTask: () => import('@/views/InManage/InTask/index'), // 入库任务管理

  // 出库管理
  PickOrder: () => import('@/views/OutManage/OutBill/index'), // 出库
  OutTask: () => import('@/views/OutManage/OutTask/index'), // 出库任务

  // 领用
  Receive: () => import('@/views/Mould/Receive/index'), // 领用信息
  MouldInformation: () => import('@/views/Mould/MouldInformation/index'), // 模具信息
  ReceiveMission: () => import('@/views/Mould/ReceiveTask/index'), // 领用任务信息
  ReceiveHistory: () => import('@/views/Mould/ReceiveHistory/index'), // 领用任务信息

  // 盘点管理
  CheckList: () => import('@/views/StockManage/CheckList/index'), // 盘点单管理
  Inventory: () => import('@/views/StockManage/Check/index'), // 盘点任务管理
  Department: () => import('@/views/SysManage/Department/index')// 部门管理
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}

// export default permission
