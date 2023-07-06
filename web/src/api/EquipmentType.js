import request from '@/utils/request'

// 设备型号路径
export function PageRecordsEquipmentType(query) {
  return request({
    url: 'api/EquipmentType/GetPageRecords',
    method: 'get',
    params: query // 查找
  })
}
export function getBoxPageRecords(query) {
  return request({
    url: 'api/EquipmentType/PostDoEdit',
    method: 'get',
    params: query // 更改
  })
}

export function createEquipmentType(data) {
  return request({
    url: 'api/EquipmentType/PostDoCreate',
    method: 'post', // 添加设备型号信息
    data
  })
}
export function editEquipmentType(data) {
  return request({
    url: 'api/EquipmentType/PostDoEdit', // 修改设备型号信息
    method: 'post',
    data
  })
}
export function deleteEquipmentType(data) { // 删除设备型号
  return request({
    url: 'api/EquipmentType/PostDoDelete',
    method: 'post',
    data: { id: data.Id }
  })
}

export function getBoxlierList(data) {
  return request({
    url: 'api/EquipmentType/GetSupplierList',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function Uploading(data) { // 上传
  return request({
    url: 'api/EquipmentType/DoUpLoadInfo',
    method: 'post',
    data: data
  })
}

export function getEquipmentTypeTypeList(data) { // 调用方法显示字典
  return request({
    url: 'api/Dictionary/GetDictionaryByType',
    method: 'get',
    params: { type: data }
  })
}

