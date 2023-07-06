import request from '@/utils/request'

// 起来前查询
export function fetchList(query) {
  return request({
    url: 'api/DeviceInfo/GetDevices',
    method: 'get',
    params: query
  })
}

// 创建
export function createVideo(data) {
  return request({
    url: 'api/DeviceInfo/PostDoCreate',
    method: 'post',
    data
  })
}

// 编辑
export function editVideo(data) {
  return request({
    url: 'api/DeviceInfo/PostDoEdit',
    method: 'post',
    data
  })
}

// 删除
// export function deleteVideo(data) {
//   return request({
//     url: 'api/DeviceInfo/PostDoDelete',
//     method: 'post',
//     data
//   })
// }

// 查询仓库信息
export function getWareHouseIDData(query) {
  return request({
    url: 'api/DeviceInfo/GetDeviceInfo',
    method: 'get',
    params: query
  })
}

