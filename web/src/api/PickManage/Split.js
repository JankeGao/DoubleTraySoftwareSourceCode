import request from '@/utils/request'
export function GetPageRecords(query) {
  return request({
    url: 'api/Split/GetPageRecords',
    method: 'get',
    params: query
  })
}
// 载具箱
export function GetPageRecordsBox(query) {
  return request({
    url: 'api/Split/GetPageRecordsBox',
    method: 'get',
    params: query
  })
}

export function GetSplitOrderIssueList(data) {
  return request({
    url: 'api/Split/GetSplitOrderIssueList',
    method: 'get',
    params: { SplitNo: data }
  })
}
export function GetSplitAreaList(data) {
  return request({
    url: 'api/Split/GetSplitAreaList',
    method: 'get',
    params: { SplitNo: data }
  })
}
export function GetSplitAreaReelList(data) {
  return request({
    url: 'api/Split/GetSplitAreaReelList',
    method: 'get',
    params: { SplitNo: data.SplitNo, WareHouseCode: data.WareHouseCode, AreaId: data.AreaId }
  })
}
export function GetSplitAreaReelDetailList(data) {
  return request({
    url: 'api/Split/GetSplitAreaReelDetailList',
    method: 'get',
    params: { SplitNo: data.SplitNo, WareHouseCode: data.WareHouseCode, AreaId: data.AreaId, ReelId: data.ReelId }
  })
}
export function PostDoCanle(data) {
  return request({
    url: 'api/Split/CancelSplitOrder',
    method: 'post',
    data: data
  })
}
export function SplitTaskDoStart(data) {
  return request({
    url: 'api/Split/SplitTaskDoStart',
    method: 'post',
    data: data
  })
}
export function SplitTaskDoFinish(data) {
  return request({
    url: 'api/Split/SplitTaskDoFinish',
    method: 'post',
    data: data
  })
}
export function GetSplitReelList(data) {
  return request({
    url: 'api/Split/GetSplitReelList',
    method: 'get',
    params: { SplitNo: data.SplitNo }
  })
}
export function GetSplitReelDetailList(data) {
  return request({
    url: 'api/Split/GetSplitReelDetailList',
    method: 'get',
    params: { SplitNo: data.SplitNo, ReelId: data.ReelId }
  })
}
export function ConfirmSplitReel(data) {
  return request({
    url: 'api/Split/ConfirmSplitReel',
    method: 'post',
    data: data
  })
}
export function CancelSplitReel(data) {
  return request({
    url: 'api/Split/CancelSplitReel',
    method: 'post',
    data: data
  })
}
export function ConfirmShelfSplitReel(data) {
  return request({
    url: 'api/Split/ConfirmShelfSplitReel',
    method: 'post',
    data: data
  })
}
export function GetSplitReelDetailLabelInfo(data) {
  return request({
    url: 'api/Split/GetSplitReelDetailLabelInfo',
    method: 'get',
    params: { ReelId: data.SplitReelId }
  })
}
