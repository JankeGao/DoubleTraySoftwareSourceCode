import request from '@/utils/request'

export function getArticalList() {
  return request({
    url: 'api/ArticleInfo/GetArtical',
    method: 'get',
    params: {}
  })
}

export function fetchList(query) {
  return request({
    url: 'api/ArticleInfo/GetArticalList',
    method: 'get',
    params: query
  })
}

export function createArtical(data) {
  return request({
    url: 'api/ArticleInfo/PostDoCreate',
    method: 'post',
    data
  })
}

export function editArtical(data) {
  return request({
    url: 'api/ArticleInfo/PostDoEdit',
    method: 'post',
    data
  })
}

export function deleteArtical(data) {
  return request({
    url: 'api/ArticleInfo/PostDoRemove',
    method: 'post',
    data
  })
}

