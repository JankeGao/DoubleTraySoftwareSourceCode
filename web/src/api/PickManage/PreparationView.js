import request from '@/utils/request'
export function GetPageRecords(query) {
  return request({
    url: 'api/PreparationView/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function GetPickOrderIssueList(data) {
  return request({
    url: 'api/PreparationView/GetPickOrderIssueList',
    method: 'get',
    params: { PickOrderCode: data }
  })
}
export function GetPickOrderDetailList(query) {
  return request({
    url: 'api/PreparationView/GetPickOrderDetailList',
    method: 'get',
    params: query
  })
}
export function GetPickOrderAllMaterials(PickOrderCode) {
  return request({
    url: 'api/PreparationView/GetPickOrderAllMaterials',
    method: 'get',
    params: { PickOrderCode: PickOrderCode }
  })
}
export function GetShelfCodeByLocationCode(LocationCode) {
  return request({
    url: 'api/PreparationView/GetShelfCodeByLocationCode',
    method: 'get',
    params: { LocationCode: LocationCode }
  })
}
export function GetPickOrderAreaDetalList(data) {
  return request({
    url: 'api/PreparationView/GetPickOrderAreaDetalList',
    method: 'get',
    params: { PickOrderDetailId: data }
  })
}
export function PostDoCanle(data) {
  return request({
    url: 'api/PreparationView/DoCancel',
    method: 'post',
    data: data
  })
}
export function PostDoCheckStock(data) {
  return request({
    url: 'api/PreparationView/DoCheckStock',
    method: 'post',
    data: data
  })
}
export function PickTaskDoStart(data) {
  return request({
    url: 'api/PreparationView/PickTaskDoStart',
    method: 'post',
    data: data
  })
}
export function PickTaskDoFinish(data) {
  return request({
    url: 'api/PreparationView/PickTaskDoFinish',
    method: 'post',
    data: data
  })
}

export function GetAllAvailablePickArea(data) {
  return request({
    url: 'api/PreparationView/GetAllAvailablePickArea',
    method: 'get',
    params: { PickOrderCode: data }
  })
}

export function GetPickOrderAreaDetailList(data) {
  return request({
    url: 'api/PreparationView/GetPickOrderAreaDetailList',
    method: 'get',
    params: { PickOrderCode: data.PickOrderCode, AreaId: data.AreaId }
  })
}
export function GetPickMaterialLabelList(query) {
  return request({
    url: 'api/PreparationView/GetPickMaterialLabelList',
    method: 'get',
    params: query
  })
}
export function ConfirmReelToSend(data) {
  return request({
    url: 'api/PreparationView/ConfirmReelToSend',
    method: 'post',
    data: data
  })
}
export function CancelPickMaterialLabel(data) {
  return request({
    url: 'api/PreparationView/CancelPickMaterialLabel',
    method: 'post',
    data: data
  })
}

