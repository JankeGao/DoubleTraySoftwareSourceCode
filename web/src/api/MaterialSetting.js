import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/Material/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function createMaterial(data) {
  return request({
    url: 'api/Material/PostDoCreate',
    method: 'post',
    data
  })
}
export function editMaterial(data) {
  return request({
    url: 'api/Material/PostDoEdit',
    method: 'post',
    data
  })
}
export function deleteMaterial(data) {
  return request({
    url: 'api/Material/PostDoDelete',
    method: 'post',
    data: { id: data.Id }
  })
}

export function doUpLoadMaterialInfo(data) {
  return request({
    url: 'api/Material/DoUpLoadMaterialInfo',
    method: 'post',
    data: data
  })
}

export function getLocationList() {
  return request({
    url: 'api/Material/GetLocationList',
    method: 'get'
  })
}

