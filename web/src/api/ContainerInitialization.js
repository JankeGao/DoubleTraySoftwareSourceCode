import request from '@/utils/request'
//垂直学习
export function StartM300() {
  return request({
    url: 'api/ContainerInitialization/StartM300',
    method: 'post'
  })
}
export function GetM340() {
  return request({
    url: 'api/ContainerInitialization/GetM340',
    method: 'get'
  })
}
export function GetD300() {
  return request({
    url: 'api/ContainerInitialization/GetD300',
    method: 'get'
  })
}
export function GetD301() {
  return request({
    url: 'api/ContainerInitialization/GetD301',
    method: 'get'
  })
}
export function FinishM341() {
  return request({
    url: 'api/ContainerInitialization/FinishM341',
    method: 'post'
  })
}

//水平学习

export function StartM400() {
  return request({
    url: 'api/ContainerInitialization/StartM400',
    method: 'post'
  })
}
export function GetM440() {
  return request({
    url: 'api/ContainerInitialization/GetM440',
    method: 'get'
  })
}
export function FinishM441() {
  return request({
    url: 'api/ContainerInitialization/FinishM441',
    method: 'post'
  })
}

//自动门学习

export function StartM450() {
  return request({
    url: 'api/ContainerInitialization/StartM450',
    method: 'post'
  })
}
export function GetM490() {
  return request({
    url: 'api/ContainerInitialization/GetM490',
    method: 'get'
  })
}
export function FinishM491() {
  return request({
    url: 'api/ContainerInitialization/FinishM491',
    method: 'post'
  })
}

//托盘扫描

export function StartM350() {
  return request({
    url: 'api/ContainerInitialization/StartM350',
    method: 'post'
  })
}
export function GetM390() {
  return request({
    url: 'api/ContainerInitialization/GetM390',
    method: 'get'
  })
}
export function GetD390() {
  return request({
    url: 'api/ContainerInitialization/GetD390',
    method: 'get'
  })
}
export function GetD391() {
  return request({
    url: 'api/ContainerInitialization/GetD391',
    method: 'get'
  })
}
export function StartM391() {
  return request({
    url: 'api/ContainerInitialization/StartM391',
    method: 'post'
  })
}
export function GetM392() {
  return request({
    url: 'api/ContainerInitialization/GetM392',
    method: 'get'
  })
}
export function WriteD392(data) {
  return request({
    url: 'api/ContainerInitialization/WriteD392',
    method: 'post',
    data:data
  })
}
export function ConfirmM393() {
  return request({
    url: 'api/ContainerInitialization/ConfirmM393',
    method: 'post'
  })
}
export function ConfirmM394() {
  return request({
    url: 'api/ContainerInitialization/ConfirmM394',
    method: 'post'
  })
}
export function GetM395() {
  return request({
    url: 'api/ContainerInitialization/GetM395',
    method: 'get'
  })
}
export function ConfirmM396() {
  return request({
    url: 'api/ContainerInitialization/ConfirmM396',
    method: 'post'
  })
}



//自动存取托盘

export function WriteD650(data) {
  return request({
    url: 'api/ContainerInitialization/WriteD650',
    method: 'post',
    data:data
  })
}
export function GetM650() {
  return request({
    url: 'api/ContainerInitialization/GetM650',
    method: 'get'
  })
}
export function GetD651() {
  return request({
    url: 'api/ContainerInitialization/GetD651',
    method: 'get'
  })
}
export function StartM651() {
  return request({
    url: 'api/ContainerInitialization/StartM651',
    method: 'post'
  })
}
export function GetD652() {
  return request({
    url: 'api/ContainerInitialization/GetD652',
    method: 'get'
  })
}




//添加托盘

export function WriteD700(data) {
  return request({
    url: 'api/ContainerInitialization/WriteD700',
    method: 'post',
    data:data
  })
}
export function StartM700() {
  return request({
    url: 'api/ContainerInitialization/StartM700',
    method: 'post'
  })
}
export function GetM701() {
  return request({
    url: 'api/ContainerInitialization/GetM701',
    method: 'get'
  })
}
export function ConfirmM702() {
  return request({
    url: 'api/ContainerInitialization/ConfirmM702',
    method: 'post'
  })
}


//删除托盘

export function WriteD750(data) {
  return request({
    url: 'api/ContainerInitialization/WriteD750',
    method: 'post',
    data:data
  })
}
export function StartM750() {
  return request({
    url: 'api/ContainerInitialization/StartM750',
    method: 'post'
  })
}
export function GetD751() {
  return request({
    url: 'api/ContainerInitialization/GetD751',
    method: 'get'
  })
}
export function StartM751() {
  return request({
    url: 'api/ContainerInitialization/StartM751',
    method: 'post'
  })
}
export function GetM752() {
  return request({
    url: 'api/ContainerInitialization/GetM752',
    method: 'get'
  })
}
export function ConfirmM753() {
  return request({
    url: 'api/ContainerInitialization/ConfirmM753',
    method: 'post'
  })
}
export function ConfirmM754() {
  return request({
    url: 'api/ContainerInitialization/ConfirmM754',
    method: 'post'
  })
}


//整理存储空间

export function StartM800() {
  return request({
    url: 'api/ContainerInitialization/StartM800',
    method: 'post'
  })
}
export function GetM801() {
  return request({
    url: 'api/ContainerInitialization/GetM801',
    method: 'get'
  })
}
export function GetD800() {
  return request({
    url: 'api/ContainerInitialization/GetD800',
    method: 'get'
  })
}
export function ConfirmM802() {
  return request({
    url: 'api/ContainerInitialization/ConfirmM802',
    method: 'post'
  })
}

