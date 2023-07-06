import request from '@/utils/request'

export function getJobObjects() {
  return request({
    url: 'api/JobObject/GetJobObjects',
    method: 'get',
    params: {}
  })
}

export function fetchList(query) {
  return request({
    url: 'api/JobObject/GetJobObjectList',
    method: 'get',
    params: query
  })
}

export function createJobObject(data) {
  return request({
    url: 'api/JobObject/PostDoCreate',
    method: 'post',
    data
  })
}

export function editJobObject(data) {
  return request({
    url: 'api/JobObject/PostDoEdit',
    method: 'post',
    data
  })
}

export function deleteJobObject(data) {
  return request({
    url: 'api/JobObject/PostDoRemove',
    method: 'post',
    data
  })
}

