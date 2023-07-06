import request from '@/utils/request'
export function getOutLabelList(query) {
  return request({
    url: 'api/Out/GetOutLabelPageRecords',
    method: 'get',
    params: query
  })
}
export function HandShelfDown(data) {
  return request({
    url: 'api/Out/PostDoHandShelfDown',
    method: 'post',
    data: data
  })
}
