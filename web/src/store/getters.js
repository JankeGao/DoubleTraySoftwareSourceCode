const getters = {
  sidebar: state => state.app.sidebar,
  size: state => state.app.size,
  device: state => state.app.device,
  visitedViews: state => state.tagsView.visitedViews,
  cachedViews: state => state.tagsView.cachedViews,
  token: state => state.login.token,
  avatar: state => state.login.avatar,
  name: state => state.login.name,
  code: state => state.login.code,
  introduction: state => state.login.introduction,
  roles: state => state.login.roles,
  permission_routes: state => state.permission.routes,
  errorLogs: state => state.errorLog.logs,
  menuList: state => state.getMenu.menuList,
  addRouters: state => state.permission.addRoutes,
  shelfList: state => state.shelfList
}
export default getters
