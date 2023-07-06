// const req = require.context('../../../views/SysManage/Layout/components/topo/static/svg', false, /\.svg$/)
const req = require.context('../../../icons/wareHousePlanSvg', false, /\.svg$/)
const requireAll = requireContext => requireContext.keys()

const re = /\.\/(.*)\.svg/

const wareSvgIcons = requireAll(req).map(i => {
  return i.match(re)[1]
})

export default wareSvgIcons
