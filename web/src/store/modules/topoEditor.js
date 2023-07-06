import Vue from 'vue'
import {v1 as uuidv1} from 'uuid'
// const uuidv1 = v1
import { deepCopy } from '@/assets/libs/utils'
// 状态
const state = {
  topoData: {
    name: '--',
    layer: {
      backColor: '#ADD8E6',
      backgroundImage: '',
      widthHeightRatio: '',
      width: 1600,
      height: 900
    },
    components: []
  }, // 当前场景的组态数据
  selectedIsLayer: true, // 当前选择的是不是layer层
  selectedComponent: null, // 当前选择的单个组件--仅仅当只有一个组件选中有效，当有多个组件选中，则置为null
  selectedComponents: [], // 当前选择的组件--只存identifier
  selectedComponentMap: {}, // 当前选择的组件--key=identifier，本数据和selectedComponents同步，主要用于渲染判断
  copySrcItems: [], // 当前是否使用了CTRL+C命令
  copyCount: 0, // copy计数，对于同一个复制源，每次复制后计数+1
  undoStack: [], //
  redoStack: [] //
}

const mutations = {
  selectedComponents: (state, identifier) => {
    state.selectedComponents = identifier
  },
  selectedComponentMap: (state, map) => {
    state.selectedComponentMap = map
  },
  selectedComponent: (state, component) => {
    state.selectedComponent = component
  },
  SET_AVATAR: (state, avatar) => {
    state.avatar = avatar
  },
  SET_ROLES: (state, roles) => {
    state.roles = roles
  },
  SET_USERCODE: (state, code) => {
    state.code = code
  }
}

const actions = {

  /**
 * 执行编辑命令
 * 注意：这里不要用箭头函数，防止this无法调用
 * @param {*} state
 * @param {*} command 命令对象
 */
  execute(command, component) {
    // 暂时不做参数校验
    // 在这里分发命令--这里暂时先用switch分发，应该用表格分发
    switch (command.op) {
      case 'add':
        component = command.component
        var fuid = uuidv1
        if (!component.identifier) {
          component.identifier = fuid()
        }
        component.identifier = fuid()
        component.name = component.type + state.topoData.components.length
        component.style.visible = true
        component.style.transform = 0
        component.style.borderWidth = component.style.borderWidth
          ? component.style.borderWidth
          : 0
        component.style.borderStyle = component.style.borderStyle
          ? component.style.borderStyle
          : 'solid'
        component.style.borderColor = component.style.borderColor
          ? component.style.borderColor
          : '#ccccccff'
        // component.style.fontFamily = "Arial";
        state.topoData.components.push(component)
        break
      case 'del':
        var keys = []
        for (var i = 0; i < state.topoData.components.length; i++) {
          var identifier = state.topoData.components[i].identifier
          if (state.selectedComponentMap[identifier] !== undefined) {
            keys.push(i)
          }
        }
        // 排序
        keys.sort((a, b) => {
          return a - b
        })
        // 逆向循环删除
        for (var j = keys.length - 1; j >= 0; j--) {
          state.topoData.components.splice(keys[j], 1)
        }
        break
      case 'move':
        var dx = command.dx
        var dy = command.dy
        for (var key in command.items) {
          component = command.items[key]
          component.style.position.x = component.style.position.x + dx
          component.style.position.y = component.style.position.y + dy
        }
        break
      case 'copy-add':
        debugger
        this.commit('topoEditor/clearSelectedComponent')
        fuid = uuidv1
        if (!component.identifier) {
          component.identifier = fuid()
        }
        for (let i = 0; i < command.items.length; i++) {
          var t = command.items[i]
          component = deepCopy(t)
          component.identifier = fuid()
          component.name = component.type + state.topoData.components.length
          component.style.visible = true
          // 这里应该根据选中的的组件确定位置-暂时用个数
          component.style.position.x =
          component.style.position.x + 25 * (state.copyCount + 1)
          component.style.position.y =
          component.style.position.y + 25 * (state.copyCount + 1)
          state.topoData.components.push(component)
          this.commit('topoEditor/addSelectedComponent', component)
          this.commit('topoEditor/increaseCopyCount')
        }
        break
      default:
        console.warn('不支持的命令.')
        break
    }
    // 记录操作
    state.undoStack.push(command)
  },
  /**
 * 设置 当前选中的组件-单选
 * @param {*} component
 */
  setSelectedComponent({ commit }, component) {
    return new Promise(async resolve => {
      var fuid = uuidv1
      if (!component.identifier) {
        component.identifier = fuid()
      }
      state.selectedComponents = [component.identifier]
      state.selectedComponentMap = {}
      Vue.set(state.selectedComponentMap, component.identifier, component)
      Vue.set(state, 'selectedComponent', component)
      resolve()
    })
  },

  /**
 * 增加选中的组件--多选模式
 * @param {*} component
 */
  addSelectedComponent({ commit }, component) {
    return new Promise(async resolve => {
      var fuid = uuidv1
      if (!component.identifier) {
        component.identifier = fuid()
      }
      if (state.selectedComponentMap[component.identifier]) {
        return
      }
      state.selectedComponents.push(component.identifier)
      Vue.set(state.selectedComponentMap, component.identifier, component)
      if (state.selectedComponents.length === 1) {
        Vue.set(state, 'selectedComponent', component)
      } else {
        Vue.set(state, 'selectedComponent', null)
      }
      resolve()
    })
  },

  /**
 * 将一个组件从已选中当中移除
 * @param {*} state
 * @param {*} component
 */
  removeSelectedComponent({ commit }, component) {
    return new Promise(async resolve => {
      if (!component.identifier) { return }
      var index = -1
      for (var i = 0; i < state.selectedComponents.length; i++) {
        if (state.selectedComponents[i] === component.identifier) {
          index = i
          break
        }
      }
      if (index > -1) {
        state.selectedComponents.splice(index, 1)
      }
      Vue.delete(state.selectedComponentMap, component.identifier)
      // 如果移除的是选中组件
      if (state.selectedComponent != null && component.identifier === state.selectedComponent.identifier) {
        Vue.set(state, 'selectedComponent', null)
      }
      // 如果只有一个组件，则默认选中
      if (state.selectedComponents.length === 1) {
        var _component = state.selectedComponentMap[state.selectedComponents[0]]
        Vue.set(state, 'selectedComponent', _component)
      }
      resolve()
    })
  },

  /**
 * 清理所有选中的组件
 * @param {*} state
 */

  setLayerSelected({ commit }, selected) {
    return new Promise(async resolve => {
      state.selectedIsLayer = selected
      resolve()
    })
  }

}
export default {
  namespaced: true,
  state,
  mutations,
  actions
}
