import request from '@/utils/request'

export function getTodayIn() {
  return request({
    url: 'api/Dashboard/GetTodayIn',
    method: 'get'
  })
}

export function getTodayOut() {
  return request({
    url: 'api/Dashboard/GetTodayOut',
    method: 'get'
  })
}

export function getTodayCheck() {
  return request({
    url: 'api/Dashboard/GetTodayCheck',
    method: 'get'
  })
}

export function getWeekIns() {
  return request({
    url: 'api/Dashboard/GetWeekIns',
    method: 'get'
  })
}

export function getTopOutMaterials() {
  return request({
    url: 'api/Dashboard/GetTopOutMaterials',
    method: 'get'
  })
}

export function getTopInMaterials() {
  return request({
    url: 'api/Dashboard/GetTopInMaterials',
    method: 'get'
  })
}

export function getMonthOutMaterials() {
  return request({
    url: 'api/Dashboard/GetMonthOutMaterials',
    method: 'get'
  })
}

export function getTodayAlarm() {
  return request({
    url: 'api/Dashboard/GetTodayAlarm',
    method: 'get'
  })
}
