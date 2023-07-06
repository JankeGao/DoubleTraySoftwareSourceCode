<template>
  <div class="linkage">
    <el-select
      v-model="linkarea.province"
      placeholder="省级地区"
      @change="choseProvince">
      <el-option
        v-for="item in province"
        :key="item.id"
        :label="item.value"
        :value="item.id"/>
    </el-select>
    <el-select
      v-model="linkarea.city"
      placeholder="市级地区"
      @change="choseCity">
      <el-option
        v-for="item in city"
        :key="item.id"
        :label="item.value"
        :value="item.id"/>
    </el-select>
    <el-select
      v-model="linkarea.area"
      placeholder="区级地区"
      @change="choseBlock">
      <el-option
        v-for="item in block"
        :key="item.id"
        :label="item.value"
        :value="item.id"/>
    </el-select>
  </div>
</template>

<script>
import { regionList } from '@/api/SysManage/Region'

export default {
  name: 'Linkarea',
  props:
  {
    linkarea: {
      type: Object,
      'default': null
    }
  },
  data() {
    return {
      province: '',
      city: '',
      block: ''
    }
  },
  created: function() {
    this.getCityData()
  },
  methods: {
    // 加载china地点数据，三级
    getCityData: function() {
      var that = this
      regionList().then(function(response) {
        if (response.status === 200) {
          var data = JSON.parse(response.data.Content)
          that.province = []
          that.city = []
          that.block = []
          // 省市区数据分类
          data.forEach(item => {
            if (item.Lvl === 1) { // 省
              that.province.push({ id: item.Code, value: item.Name, children: [] })
            } else if (item.Lvl === 2) { // 市
              that.city.push({ id: item.Code, value: item.Name, children: [] })
            } else if (item.Lvl === 3) { // 区
              that.block.push({ id: item.Code, value: item.Name, children: [] })
            }
          })
          // 分类市级
          for (var index in that.province) {
            for (var index1 in that.city) {
              if (that.province[index].id.slice(0, 2) === that.city[index1].id.slice(0, 2)) {
                that.province[index].children.push(that.city[index1])
              }
            }
          }
          // 分类区级
          for (var item1 in that.city) {
            for (var item2 in that.block) {
              if (that.block[item2].id.slice(0, 4) === that.city[item1].id.slice(0, 4)) {
                that.city[item1].children.push(that.block[item2])
              }
            }
          }
        } else {
          console.log(response.status)
        }
      }).catch(function(error) { console.log(typeof +error) })
    },
    // 选省
    choseProvince: function(e) {
      for (var index2 in this.province) {
        if (e === this.province[index2].id) {
          this.city = this.province[index2].children
          this.block = this.province[index2].children[0].children
          this.linkarea.city = this.province[index2].children[0].id
          this.linkarea.area = this.province[index2].children[0].children[0].id
          this.E = this.block[0].id
        }
      }
    },
    // 选市
    choseCity: function(e) {
      for (var index3 in this.city) {
        if (e === this.city[index3].id) {
          this.block = this.city[index3].children
          this.linkarea.area = this.city[index3].children[0].id
          this.E = this.block[0].id
        }
      }
    },
    // 选区
    choseBlock: function(e) {
      this.E = e
    }
  }
}
</script>

<style scoped>

</style>
