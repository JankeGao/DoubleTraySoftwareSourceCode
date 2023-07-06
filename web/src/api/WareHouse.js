import request from '@/utils/request'
export function getWareHouseTreeData(query) {
  return request({
    url: 'api/WareHouse/GetWareHouseTreeData',
    method: 'get',
    params: { type: query }
  })
}
// 获取不同类别仓库数据信息，根据用户编码
export function getWareHouseTreeDataByUserCode(data, data1) {
  return request({
    url: 'api/WareHouse/GetWareHouseTreeDataByUserCode',
    method: 'get',
    params: { type: data, userCode: data1 }
  })
}
export function getMaterialPageRecords(query) {
  return request({
    url: 'api/WareHouse/GetMaterialPageRecords',
    method: 'get',
    params: query
  })
}

export function getPageRecords(query) {
  return request({
    url: 'api/WareHouse/GetPageRecords',
    method: 'get',
    params: query
  })
}

export function getWarePageRecords(query) {
  return request({
    url: 'api/WareHouse/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function getMaterialList(data) {
  return request({
    url: 'api/WareHouse/GetMaterialList',
    method: 'get',
    params: { KeyValue: data }
  })
}
export function createWareHouse(data) {
  return request({
    url: 'api/WareHouse/PostDoCreateWareHouse',
    method: 'post',
    data
  })
}
export function createArea(data) {
  return request({
    url: 'api/WareHouse/PostDoCreateArea',
    method: 'post',
    data
  })
}
export function createLocation(data) {
  return request({
    url: 'api/WareHouse/PostDoCreateLocation',
    method: 'post',
    data
  })
}
export function editWareHouse(data) {
  return request({
    url: 'api/WareHouse/PostDoEditWareHouse',
    method: 'post',
    data
  })
}
export function editContainer(data) {
  return request({
    url: 'api/WareHouse/PostDoEditContainer',
    method: 'post',
    data
  })
}
export function editLocation(data) {
  return request({
    url: 'api/WareHouse/PostDoEditLocation',
    method: 'post',
    data
  })
}
export function deleteWareHouse(data) {
  return request({
    url: 'api/WareHouse/PostDoDeleteWareHouse',
    method: 'post',
    data
  })
}
export function deleteContainer(data) {
  return request({
    url: 'api/WareHouse/PostDoDeleteContainer',
    method: 'post',
    data
  })
}

export function deleteLocation(data) {
  return request({
    url: 'api/WareHouse/PostDoDeleteLocation',
    method: 'post',
    data
  })
}

export function deleteTray(data) {
  return request({
    url: 'api/WareHouse/PostDoDeleteTray',
    method: 'post',
    data
  })
}
export function getWareHouseLocations(data) {
  return request({
    url: 'api/WareHouse/GetWareHouseLocations',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function getAreaLocations(data) {
  return request({
    url: 'api/WareHouse/GetAreaLocations',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function getTrayLocations(data) {
  return request({
    url: 'api/WareHouse/GetTrayLocations',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function doUpLoadWarehouseInfo(data) {
  return request({
    url: 'api/WareHouse/DoUpLoadWarehouseInfo',
    method: 'post',
    data: data
  })
}
export function CreateRangeLocation(data) {
  return request({
    url: 'api/WareHouse/CreateRangeLocation',
    method: 'get',
    params: { locationlist: data }
  })
}
// export function CreateRangeLocation(data) {
//   return request({
//     url: 'api/WareHouse/CreateRangeLocation',
//     method: 'post',
//     data: data
//   })
// }
export function createChannel(data) {
  return request({
    url: 'api/WareHouse/PostDoCreateChannel',
    method: 'post',
    data
  })
}

export function editTray(data) {
  return request({
    url: 'api/WareHouse/PostDoEditTray',
    method: 'post',
    data
  })
}
export function editTrayLocation(data) {
  return request({
    url: 'api/WareHouse/PostDoEditTrayLocation',
    method: 'post',
    data
  })
}

export function deleteChannel(data) {
  return request({
    url: 'api/WareHouse/PostDoDeleteChannel',
    method: 'post',
    data
  })
}

export function deletelocationByLayoutId(data) {
  return request({
    url: 'api/WareHouse/PostDoDeleteLocationByLayoutId',
    method: 'post',
    data
  })
}

export function createContainer(data) {
  return request({
    url: 'api/WareHouse/PostDoCreateContainer',
    method: 'post',
    data
  })
}

export function createtray(data) {
  return request({
    url: 'api/WareHouse/PostDoCreateTray',
    method: 'post',
    data
  })
}

export function getTrayById(data) {
  return request({
    url: 'api/WareHouse/GetTrayById',
    method: 'get',
    params: { trayId: data }
  })
}

export function getTrayLayoutById(data) {
  return request({
    url: 'api/WareHouse/GetTrayLayoutById',
    method: 'get',
    params: { trayId: data }
  })
}

export function addBatchTrayUserMap(data) {
  return request({
    url: 'api/WareHouse/AddBatchTrayUserMap',
    method: 'post',
    data
  })
}
export function getTrayUserMapByTrayId(data) {
  return request({
    url: 'api/WareHouse/GetTrayUserMapByTrayId',
    method: 'get',
    params: { trayId: data }
  })
}

// 人员操作权限核验
export function getDoCheckAuth(data) {
  return request({
    url: 'api/WareHouse/GetDoCheckAuth',
    method: 'get',
    params: { trayId: data }
  })
}

// 人员操作权限核验
export function getLocationByLayoutId(data) {
  return request({
    url: 'api/WareHouse/GetLocationByLayoutId',
    method: 'get',
    params: { layoutId: data }
  })
}
export function PostStartContainer(data) {
  return request({
    url: 'api/WareHouse/PostStartContainer',
    method: 'post',
    data: data
  })
}
export function PostRestoreContainer(data) {
  return request({
    url: 'api/WareHouse/PostRestoreContainer',
    method: 'post',
    data: data
  })
}
export function postDoDeleteLocationByTrayId(data) {
  return request({
    url: 'api/WareHouse/PostDoDeleteLocationByTrayId',
    method: 'post',
    data: data
  })
}

export function ouLoadMaterialLocationInfo(data) {
  return request({
    url: 'api/WareHouse/DoUpLoadInInfo',
    method: 'post',
    data: data
  })
}

export function getLocationByTrayId(data) {
  return request({
    url: 'api/WareHouse/GetLocationByTrayId',
    method: 'get',
    params: { trayId: data }
  })
}
