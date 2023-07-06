import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/PTLProgress/GetPageRecords',
    method: 'get',
    params: query
  })
}

export function GetWareHouseList() {
  return request({
    url: 'api/PTLProgress/GetWareHouseList',
    method: 'get'
  })
}
export function HandFinishOrder(data) {
  return request({
    url: 'api/PTLProgress/HandFinishOrder',
    method: 'post',
    data: data
  })
}
export function HandFinishOneLocation(data) {
  return request({
    url: 'api/PTLProgress/HandFinishOneLocation',
    method: 'post',
    data: data
  })
}
export function GetDetailList(query) {
  return request({
    url: 'api/PTLProgress/GetDetailList',
    method: 'get',

    params: query
  })
}
export function HandStopOrder(data) {
  return request({
    url: 'api/PTLProgress/HandStopOrder',
    method: 'post',
    data: data
  })
}
export function HandContinueOrder(data) {
  return request({
    url: 'api/PTLProgress/HandContinueOrder',
    method: 'post',
    data: data
  })
}
