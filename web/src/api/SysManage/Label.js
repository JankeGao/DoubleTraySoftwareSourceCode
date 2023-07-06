import request from '@/utils/request'

export function getLabelList() {
  return request({
    url: 'api/Label/GetLabelList',
    method: 'get',
    params: {}
  })
}

export function LabelList(query) {
  return request({
    url: 'api/LabelInfo/GetArticalList',
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

export function addLabel(data) {
  return request({
    url: 'api/Label/PostDoAdd',
    method: 'post',
    data
  })
}

export function deleteLabel(data) {
  return request({
    url: 'api/Label/PostDoRemove',
    method: 'post',
    data
  })
}
