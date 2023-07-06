import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/NumAlarm/GetPageRecords',
    method: 'get',
    params: query
  })
}

export function getNumAlarmCheck() {
  return request({
    url: 'api/NumAlarm/PostDoCheck',
    method: 'post'
  })
}

