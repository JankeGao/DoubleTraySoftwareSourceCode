import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/HistoryOut/GetPageRecords',
    method: 'get',
    params: query // 查找
  })
}

