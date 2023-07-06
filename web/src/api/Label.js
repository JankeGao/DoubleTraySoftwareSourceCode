import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/Label/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function getLabelPageRecords(query) {
  return request({
    url: 'api/Label/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function createLabel(data) {
  return request({
    url: 'api/Label/PostDoCreate',
    method: 'post',
    data
  })
}
export function editLabel(data) {
  return request({
    url: 'api/Label/PostDoEdit',
    method: 'post',
    data
  })
}
export function deleteLabel(data) {
  return request({
    url: 'api/Label/PostDoDelete',
    method: 'post',
    data: { id: data.Id }
  })
}

export function getLabelByCode(query) {
  return request({
    url: 'api/Label/GetLabelByCode',
    method: 'get',
    params: { code: query }
  })
}

export function queryHistoryLabel(data) {
  return request({
    url: 'api/Label/PostQueryHistoryLabel',
    method: 'post',
    data
  })
}

export function createBatchLabel(data) {
  return request({
    url: 'api/Label/PostDoCreateBatchLabel',
    method: 'post',
    data
  })
}
