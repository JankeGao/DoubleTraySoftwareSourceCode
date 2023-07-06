import request from '@/utils/request'

// 图片库的路径
export function PageRecordsPicture(query) {
  return request({
    url: 'api/FileLibrary/GetFileLibraryListOrPages',
    method: 'get',
    params: query // 获取图片信息
  })
}
// 删除图片
export function deletePicture(data) {
  return request({
    url: 'api/FileLibrary/PostDoRemove',
    method: 'post',
    data: { id: data.Id }
    // params: data
  })
}

