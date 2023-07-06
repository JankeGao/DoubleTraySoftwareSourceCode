import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/Alarm/GetPageRecords',
    method: 'get',
    params: query
  })
}

export function getAlarmCheck() {
  return request({
    url: 'api/Alarm/PostDoCheck',
    method: 'post'
  })
}

/**
 * 根据仓库编码获取报警信息
 * @param {*} type 
 * @returns 
 */
 export function getAlarmList(type) {
  return request({
    url: 'api/Alarm/GetAlarmList',
    method: 'get',
    params: {
      type: type
    }
  })
}