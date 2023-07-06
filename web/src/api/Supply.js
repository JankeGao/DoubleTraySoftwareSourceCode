import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/Supply/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function getSupplyPageRecords(query) {
  return request({
    url: 'api/Supply/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function createSupply(data) {
  return request({
    url: 'api/Supply/PostDoCreate',
    method: 'post',
    data
  })
}
export function editSupply(data) {
  return request({
    url: 'api/Supply/PostDoEdit',
    method: 'post',
    data
  })
}
export function deleteSupply(data) {
  return request({
    url: 'api/Supply/PostDoDelete',
    method: 'post',
    data: { id: data.Id }
  })
}

export function getSupplierList(data) {
  return request({
    url: 'api/Supply/GetSupplierList',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function ouLoadSupplyInfo(data) {
  return request({
    url: 'api/Supply/DoUpLoadSupplyInfo',
    method: 'post',
    data: data
  })
}
