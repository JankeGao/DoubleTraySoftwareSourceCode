import request from '@/utils/request'
// 查询领用任务信息
export function getReceiveTaskList(query) {
  return request({
    url: 'api/ReceiveTask/GetPageRecords',
    method: 'get',
    params: query
  })
}

// 任务明细
export function getReceiveTaskDetailList(data) {
  return request({
    url: 'api/ReceiveTask/GetReceiveTaskDetailList',
    method: 'get',
    params: { Code: data }
  })
}
// 获取仓库列表信息
export function getWarehouseList() {
  return request({
    url: 'api/ReceiveTask/GetWareHouseList',
    method: 'get'
  })
}

export function deleteReceiveTask(data) {
  return request({
    url: 'api/ReceiveTask/PostDoDelete',
    method: 'post',
    data: data
  })
}

// 添加
export function issueReceiveTaskInfo(data) {
  return request({
    url: 'api/ReceiveTask/PostDoCreate',
    method: 'post',
    data
  })
}

// 执行
export function postHandShelf(data) {
  return request({
    url: 'api/ReceiveTask/PostHandShelfReceiveTask',
    method: 'post',
    data
  })
}

// 归还
export function postReturnReceiveTask(data) {
  return request({
    url: 'api/ReceiveTask/PostReturn',
    method: 'post',
    data
  })
}

// 查询领用历史任务信息
export function getHistoryPageRecords(query) {
  return request({
    url: 'api/ReceiveTask/GetHistoryPageRecords',
    method: 'get',
    params: query
  })
}
