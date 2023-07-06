import request from '@/utils/request'

export function getSettings() {
  return request({
    url: 'api/BaseSetting/GetBaseSetting',
    method: 'get'
  })
}

export function saveSettings(data) {
  return request({
    url: 'api/BaseSetting/PostSaveSetting',
    method: 'Post',
    params: { lists: data }
  })
}

export function createSetting(data) {
  return request({
    url: 'api/BaseSetting/PostDoCreate',
    method: 'post',
    data
  })
}
