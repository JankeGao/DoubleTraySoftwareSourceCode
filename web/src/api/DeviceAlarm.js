import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/DeviceAlarm/GetPageRecords',
    method: 'get',
    params: query
  })
}

// 复位
export function postDeviceAlarm(data) {
  return request({
    url: 'api/DeviceAlarm/PostDoEdit',
    method: 'post',
    data
  })
}

// 添加
export function getAlarmCheck(data) {
  return request({
    url: 'api/DeviceAlarm/PostDoCreate',
    method: 'post',
    data
  })
}

