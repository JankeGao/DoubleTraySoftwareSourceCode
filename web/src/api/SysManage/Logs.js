import request from '@/utils/request'

export function fetchList(query) {
  return request({
    url: 'api/Log/GetLogList',
    method: 'get',
    params: query
  })
}

