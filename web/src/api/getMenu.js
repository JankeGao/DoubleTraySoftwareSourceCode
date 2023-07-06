import request from '@/utils/request'

export function getMenu() {
  return request({
    url: 'api/login/GetMenu',
    method: 'get',
    params: {}
  })
}
