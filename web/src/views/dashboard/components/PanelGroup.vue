<template>
  <el-row :gutter="40" class="panel-group">
    <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
      <div class="card-panel" @click="handleSetLineChartData('purchases')">
        <el-row>
          <el-col :xs="0" :sm="12" :lg="12">
            <div class="card-panel-icon-wrapper icon-money">
              <svg-icon icon-class="clipboard" class-name="card-panel-icon" />
            </div>
          </el-col>
          <el-col :xs="24" :sm="12" :lg="12">
            <div class="card-panel-description">
              <div class="card-panel-text">今日入库单据</div>
              <count-to :start-val="0" :end-val="Ins " :duration="3200" class="card-panel-num" />
            </div>
          </el-col>
        </el-row>
      </div>
    </el-col>
    <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
      <div class="card-panel" @click="handleSetLineChartData('shoppings')">
        <el-row>
          <el-col :xs="0" :sm="12" :lg="12">
            <div class="card-panel-icon-wrapper icon-shoppingCard">
              <svg-icon icon-class="component" class-name="card-panel-icon" />
            </div>
          </el-col>
          <el-col :xs="24" :sm="12" :lg="12">
            <div class="card-panel-description">
              <div class="card-panel-text">今日出库单据</div>
              <count-to :start-val="0" :end-val="Outs" :duration="3200" class="card-panel-num" />
            </div>
          </el-col>
        </el-row>
      </div>
    </el-col>
    <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
      <div class="card-panel" @click="handleSetLineChartData('newVisitis')">
        <el-row>
          <el-col :xs="0" :sm="12" :lg="12">
            <div class="card-panel-icon-wrapper icon-people">
              <svg-icon icon-class="form" class-name="card-panel-icon" />
            </div>
          </el-col>
          <el-col :xs="24" :sm="12" :lg="12">
            <div class="card-panel-description">
              <div class="card-panel-text">今日盘点单据</div>
              <count-to :start-val="0" :end-val="Checks" :duration="3200" class="card-panel-num" />
            </div>
          </el-col>
        </el-row>
      </div>
    </el-col>
    <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
      <div class="card-panel" @click="handleSetLineChartData('messages')">
        <el-row>
          <el-col :xs="0" :sm="12" :lg="12">
            <div class="card-panel-icon-wrapper icon-message">
              <svg-icon icon-class="dashboard" class-name="card-panel-icon" />
            </div>
          </el-col>
          <el-col :xs="24" :sm="12" :lg="12">
            <div class="card-panel-description">
              <div class="card-panel-text">当前库存预警</div>
              <!-- <span class="card-panel-num">{{ waitOrdersNums }}</span> -->
              <count-to :start-val="0" :end-val="Alarms" :duration="3200" class="card-panel-num" />
            </div>
          </el-col>
        </el-row>
      </div>
    </el-col>
  </el-row>
</template>

<script>
import CountTo from 'vue-count-to'
import { getTodayAlarm, getTodayIn, getTodayOut, getTodayCheck } from '@/api/dashboard'

export default {

  components: {
    CountTo
  },
  data() {
    return {
      listQuery: {
        Page: 1,
        Rows: 8,
        Name: undefined,
        Goods: undefined,
        Type: undefined,
        Mobilephone: undefined,
        BeginDate: undefined,
        EndDate: undefined,
        Status: undefined,
        Sort: 'id'
      },
      Ins: 0,
      Outs: 0,
      Checks: 0,
      Alarms: 0
    }
  },
  created() {
    this.getTodayIn()
    this.getTodayOut()
    this.getTodayCheck()
    this.getTodayAlarm()
  },
  methods: {
    handleSetLineChartData(type) {
      this.$emit('handleSetLineChartData', type)
    },
    // 获取本日入库单数据
    getTodayIn() {
      getTodayIn().then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.Ins = usersData.length
      })
    },
    // 获取本日出库单数据
    getTodayOut() {
      getTodayOut().then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.Outs = usersData.length
      })
    },
    // 获取盘点
    getTodayCheck() {
      getTodayCheck().then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.Checks = usersData.length
      })
    },
    // 获取盘点
    getTodayAlarm() {
      getTodayAlarm().then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.Alarms = usersData.length
      })
    },
    resetQuery() {
      this.listQuery.BeginDate = undefined
      this.listQuery.EndDate = undefined
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
.panel-group {
//  margin-top: 18px;
  .card-panel-col{
    margin-bottom: 28px;
  }
  .card-panel {
    height: 108px;
    cursor: pointer;
    font-size: 12px;
    position: relative;
    overflow: hidden;
    color: #666;
    background: #fff;
    box-shadow: 4px 4px 40px rgba(0, 0, 0, .05);
    border-color: rgba(0, 0, 0, .05);
    &:hover {
      .card-panel-icon-wrapper {
        color: #fff;
      }
      .icon-people {
         background: #40c9c6;
      }
      .icon-message {
        background: #36a3f7;
      }
      .icon-money {
        background: #f4516c;
      }
      .icon-shoppingCard {
        background: #34bfa3
      }
    }
    .icon-people {
      color: #40c9c6;
    }
    .icon-message {
      color: #36a3f7;
    }
    .icon-money {
      color: #f4516c;
    }
    .icon-shoppingCard {
      color: #34bfa3
    }
    .card-panel-icon-wrapper {
      float: left;
      margin: 14px 0 0 14px;
      padding: 16px;
      transition: all 0.38s ease-out;
      border-radius: 6px;
    }
    .card-panel-icon {
      float: left;
      font-size: 48px;
    }
    .card-panel-description {
      float: right;
      font-weight: bold;
      margin: 26px;
      margin-left: 0px;
      .card-panel-text {
        line-height: 18px;
        color: rgba(0, 0, 0, 0.45);
        font-size: 16px;
        margin-bottom: 12px;
      }
      .card-panel-num {
        font-size: 20px;
      }
    }
  }
}
</style>
