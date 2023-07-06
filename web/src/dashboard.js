import request from '@/utils/request'

export function getGarbageCollections() {
  return request({
    url: 'api/Dashboard/GetGarbageCollections',
    method: 'get'
  })
}

export function getGarbageAmounts() {
  return request({
    url: 'api/Dashboard/GetGarbageAmounts',
    method: 'get'
  })
}

export function getProductionAmounts() {
  return request({
    url: 'api/Dashboard/GetProductionAmounts',
    method: 'get'
  })
}

export function getPowerAmounts() {
  return request({
    url: 'api/Dashboard/GetPowerAmounts',
    method: 'get'
  })
}

export function getWeekProductions() {
  return request({
    url: 'api/Dashboard/GetWeekProductions',
    method: 'get'
  })
}

export function getWeekPowers() {
  return request({
    url: 'api/Dashboard/GetWeekPowers',
    method: 'get'
  })
}

export function getMonthPowers() {
  return request({
    url: 'api/Dashboard/GetMonthPowers',
    method: 'get'
  })
}

export function getTopRequisitions() {
  return request({
    url: 'api/Dashboard/GetTopRequisitions',
    method: 'get'
  })
}

// 累计总产量
export function getProductionTotalAmounts() {
  return request({
    url: 'api/Dashboard/GetProductionTotalAmounts',
    method: 'get'
  })
}

// 当月产出Top 5
export function getMonthProductionAmounts() {
  return request({
    url: 'api/Dashboard/GetMonthProductionAmounts',
    method: 'get'
  })
}

// 当月投料
export function getMonthInComingAmounts() {
  return request({
    url: 'api/Dashboard/GetMonthInComingAmounts',
    method: 'get'
  })
}

// 当月垃圾收集
export function getGarbageMonthAmounts() {
  return request({
    url: 'api/Dashboard/GetGarbageMonthAmounts',
    method: 'get'
  })
}
