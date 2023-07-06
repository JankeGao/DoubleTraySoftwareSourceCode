import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/MouldInformation/GetPageRecords',
    method: 'get',
    params: query
  })
}

// 编辑
export function postMouldInformation(data) {
  return request({
    url: 'api/MouldInformation/PostDoEdit',
    method: 'post',
    data
  })
}

// 添加
export function getMouldInformation(data) {
  return request({
    url: 'api/MouldInformation/PostDoCreate',
    method: 'post',
    data
  })
}

export function deleteMouldInformation(data) { // 删除载具
  return request({
    url: 'api/MouldInformation/PostDoDelete',
    method: 'post',
    data: { id: data.Id }
  })
}

