import request from '@/utils/request'

// 分页数据
export function getBoxPageRecords(query) {
  return request({
    url: 'api/Box/GetPageRecords',
    method: 'get',
    params: query // 查找
  })
}

// 根据载具类别编码，获取单个类别载具
export function getBoxByCode(data) {
  return request({
    url: 'api/Box/GetBoxByCode',
    method: 'get',
    params: { boxCode: data } // 查找
  })
}

// 获取载具类型关联的物料
export function getBoxMaterialMapByCode(data) {
  return request({
    url: 'api/Box/GetBoxMaterialMapByCode',
    method: 'get',
    params: { boxCode: data } // 查找
  })
}

export function createBox(data) {
  return request({
    url: 'api/Box/PostDoCreate',
    method: 'post', // 添加载具信息
    data
  })
}
export function editBox(data) {
  return request({
    url: 'api/Box/PostDoEdit', // 修改载具信息
    method: 'post',
    data
  })
}
export function deleteBox(data) { // 删除载具
  return request({
    url: 'api/Box/PostDoDelete',
    method: 'post',
    data: { id: data.Id }
  })
}

export function getBoxList(data) {
  return request({
    url: 'api/Box/GetBoxList',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function UploadingBox(data) { // 上传文档
  return request({
    url: 'api/Box/DoUpLoadBoxInfo',
    method: 'post',
    data: data
  })
}

export function getBoxTypeList(data) {
  return request({
    url: 'api/Dictionary/GetDictionaryByType',
    method: 'get',
    params: { type: data }
  })
}

