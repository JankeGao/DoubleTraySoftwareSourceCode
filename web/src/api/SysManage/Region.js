import request from '@/utils/request'

export function regionList() {
  return request({
    url: 'api/Region/GetRegionList',
    method: 'get'
  })
}
