import request from '@/utils/request'

export function getJobSchedulerList() {
  return request({
    url: 'api/JobScheduler/GetJobScheduler',
    method: 'get',
    params: {}
  })
}

export function fetchList(query) {
  return request({
    url: 'api/JobScheduler/GetJobSchedulerList',
    method: 'get',
    params: query
  })
}

export function createJobScheduler(data) {
  return request({
    url: 'api/JobScheduler/PostDoCreate',
    method: 'post',
    data
  })
}

export function editJobScheduler(data) {
  return request({
    url: 'api/JobScheduler/PostDoEdit',
    method: 'post',
    data
  })
}

export function deleteJobScheduler(data) {
  return request({
    url: 'api/JobScheduler/PostDoRemove',
    method: 'post',
    data
  })
}

export function startJobScheduler(data) {
  return request({
    url: 'api/JobScheduler/DoStart',
    method: 'post',
    data
  })
}

export function stopJobScheduler(data) {
  return request({
    url: 'api/JobScheduler/DoStop',
    method: 'post',
    data
  })
}
