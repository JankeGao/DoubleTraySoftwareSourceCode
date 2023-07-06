import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/PTLError/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function GetPTLExcuteErrorPageRecords(query) {
  return request({
    url: 'api/PTLError/GetPTLExcuteErrorPageRecords',
    method: 'get',
    params: query
  })
}

export function HandleExcuteError(data) {
  return request({
    url: 'api/PTLError/HandleExcuteError',
    method: 'post',
    data: data
  })
}
