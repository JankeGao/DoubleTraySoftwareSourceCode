import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/MaterialProperty/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function getMaterialPropertyPageRecords(query) {
  return request({
    url: 'api/MaterialProperty/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function createMaterialProperty(data) {
  return request({
    url: 'api/MaterialProperty/PostDoCreate',
    method: 'post',
    data
  })
}
export function editMaterialProperty(data) {
  return request({
    url: 'api/MaterialProperty/PostDoEdit',
    method: 'post',
    data
  })
}
export function deleteMaterialProperty(data) {
  return request({
    url: 'api/MaterialProperty/PostDoDelete',
    method: 'post',
    data: { id: data.Id }
  })
}

export function getMaterialPropertyList() {
  return request({
    url: 'api/MaterialProperty/GetMaterialPropertyList',
    method: 'get'
  })
}
