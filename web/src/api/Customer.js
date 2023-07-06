import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/Customer/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function getCustomerPageRecords(query) {
  return request({
    url: 'api/Customer/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function createCustomer(data) {
  return request({
    url: 'api/Customer/PostDoCreate',
    method: 'post',
    data
  })
}
export function editCustomer(data) {
  return request({
    url: 'api/Customer/PostDoEdit',
    method: 'post',
    data
  })
}
export function deleteCustomer(data) {
  return request({
    url: 'api/Customer/PostDoDelete',
    method: 'post',
    data: { id: data.Id }
  })
}

export function getCustomerList(data) {
  return request({
    url: 'api/Customer/GetCustomerList',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function ouLoadCustomerInfo(data) {
  return request({
    url: 'api/Customer/DoUpLoadCustomerInfo',
    method: 'post',
    data: data
  })
}
