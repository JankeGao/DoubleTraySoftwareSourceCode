import request from '@/utils/request'
export function createWareHousePlan(data) {
  return request({
    url: 'api/WareHouse/CreateWareHousePlan',
    method: 'post',
    data: data
  })
}

export function getWareHousePlan(WareHouseCode) {
  return request({
    url: 'api/WareHouse/GetWareHousePlan',
    method: 'get',
    params: { WareHouseCode: WareHouseCode }
  })
}

export function getPickOrderAllMaterialsStatusCaption(pickOrderCode) {
  return request({
    url: 'api/PreparationView/GetPickOrderAllMaterialsStatusCaption',
    method: 'get',
    params: { PickOrderCode: pickOrderCode }
  })
}
