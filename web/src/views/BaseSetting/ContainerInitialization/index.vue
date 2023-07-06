<template>
  <div class="app-container">
    <el-tabs v-model="activeName" type="card" @tab-click="handleClick">
      <el-tab-pane :label=" $t('basicInformation.verticalStrokeLearning') " name="first">
        <el-steps :active="firstActive" finish-status="success">
          <el-step :title=" $t('basicInformation.step1') " :description=" $t('basicInformation.beginVerticalStrokeLearning') "></el-step>
          <el-step :title=" $t('basicInformation.step2') " :description=" $t('basicInformation.monitoringTheM340') "></el-step>
          <el-step :title=" $t('basicInformation.step3') " :description=" $t('basicInformation.endOfTheVertical') "></el-step>
        </el-steps>
        <el-button style="margin-top: 12px;" @click="firstNext">{{ $t('basicInformation.theNextStep') }}</el-button>
      </el-tab-pane>
      <el-tab-pane :label=" $t('basicInformation.horizontalStrokeLearning') " name="second">
        <el-steps :active="secondActive" finish-status="success">
          <el-step :title=" $t('basicInformation.step1') " :description=" $t('basicInformation.theLevelOfLearning') "></el-step>
          <el-step :title=" $t('basicInformation.step2') " :description=" $t('basicInformation.monitoringLevelLearning') "></el-step>
          <el-step :title=" $t('basicInformation.step3') " :description=" $t('basicInformation.endOfTheLevel') "></el-step>
        </el-steps>
        <el-button style="margin-top: 12px;" @click="secondNext">{{ $t('basicInformation.theNextStep') }}</el-button>
      </el-tab-pane>
      <el-tab-pane :label=" $t('basicInformation.automaticDoorTravelLearning') " name="third">
        <el-steps :active="thirdActive" finish-status="success">
          <el-step :title=" $t('basicInformation.step1') " :description=" $t('basicInformation.automaticLearning') "></el-step>
          <el-step :title=" $t('basicInformation.step2') " :description=" $t('basicInformation.automaticLearningState') "></el-step>
          <el-step :title=" $t('basicInformation.step3') " :description=" $t('basicInformation.automaticLearningEnd') "></el-step>
        </el-steps>
        <el-button style="margin-top: 12px;" @click="thirdNext">{{ $t('basicInformation.theNextStep') }}</el-button>
      </el-tab-pane>
      <el-tab-pane :label=" $t('basicInformation.trayScanned') " name="fourth">
        <el-steps :active="fourthActive" finish-status="success">
          <el-step :title=" $t('basicInformation.step1') " :description=" $t('basicInformation.startTrayScan') "></el-step>
          <el-step :title=" $t('basicInformation.step2') " :description=" $t('basicInformation.monitorTrayScanStatus') "></el-step>
          <el-step :title=" $t('basicInformation.step3') " :description=" $t('basicInformation.startDefining') "></el-step>
          <el-step :title=" $t('basicInformation.step4') " :description=" $t('basicInformation.monitorTheTray') "></el-step>
          <el-step :title=" $t('basicInformation.step5') " :description=" $t('basicInformation.defineTheNextOne') "></el-step>
          <el-step :title=" $t('basicInformation.step6') " :description=" $t('basicInformation.monitorM395') "></el-step>
          <el-step :title=" $t('basicInformation.step7') " :description=" $t('basicInformation.completeAllPallets') "></el-step>
        </el-steps>
        <el-button style="margin-top: 12px;" @click="fourthNext">{{ $t('basicInformation.theNextStep') }}</el-button>
      </el-tab-pane>
      <el-tab-pane :label=" $t('basicInformation.automaticAccessTray') " name="five">
        <el-steps :active="fiveActive" finish-status="success">
          <el-step :title=" $t('basicInformation.step1') " :description=" $t('basicInformation.writeTraySerialNumberToD650') "></el-step>
          <el-step :title=" $t('basicInformation.step2') " :description=" $t('basicInformation.queryM650') "></el-step>
          <el-step :title=" $t('basicInformation.step3') " :description=" $t('basicInformation.startTheM651') "></el-step>
          <el-step :title=" $t('basicInformation.step4') " :description=" $t('basicInformation.displayMaterialHeightInD652') "></el-step>
        </el-steps>
        <el-button style="margin-top: 12px;" @click="fiveNext">{{ $t('basicInformation.theNextStep') }}</el-button>
      </el-tab-pane>
      <el-tab-pane :label=" $t('basicInformation.addTheTray') " name="six">
        <el-steps :active="sixActive" finish-status="success">
          <el-step :title=" $t('basicInformation.step1') " :description=" $t('basicInformation.writeTheSerialNumberOfTrayD700') "></el-step>
          <el-step :title=" $t('basicInformation.step2') " :description=" $t('basicInformation.buttonM700ToStartExecution') "></el-step>
          <el-step :title=" $t('basicInformation.step3') " :description=" $t('basicInformation.scanMaterialHeightM701') "></el-step>
          <el-step :title=" $t('basicInformation.step4') " :description=" $t('basicInformation.confirmThatM702IsAdded') "></el-step>
        </el-steps>
        <el-button style="margin-top: 12px;" @click="sixNext">{{ $t('basicInformation.theNextStep') }}</el-button>
      </el-tab-pane>
      <el-tab-pane :label=" $t('basicInformation.deleteTheTray') " name="seven">
        <el-steps :active="sevenActive" finish-status="success">
          <el-step :title=" $t('basicInformation.step1') " :description=" $t('basicInformation.writeTheTraySerialNumberToBeDeletedToD750') "></el-step>
          <el-step :title=" $t('basicInformation.step2') " :description=" $t('basicInformation.buttonM750ToStartExecution') "></el-step>
          <el-step :title=" $t('basicInformation.step3') " :description=" $t('basicInformation.displayTrayTrayNumberInD751') "></el-step>
          <el-step :title=" $t('basicInformation.step4') " :description=" $t('basicInformation.startRunningDeviceM751') "></el-step>
          <el-step :title=" $t('basicInformation.step5') " :description=" $t('basicInformation.monitoringTrayInPlaceMarkerM752TRUEInPlace') "></el-step>
          <el-step :title=" $t('basicInformation.step6') " :description=" $t('basicInformation.confirmOrCancelTheDeletion') "></el-step>
        </el-steps>
        <el-button style="margin-top: 12px;" @click="sevenNext">{{ $t('basicInformation.theNextStep') }}</el-button>
      </el-tab-pane>
      <el-tab-pane :label=" $t('basicInformation.defragmentStorageSpace') " name="eight">
        <el-steps :active="eightActive" finish-status="success">
          <el-step :title=" $t('basicInformation.step1') " :description=" $t('basicInformation.pressToStartSorting') "></el-step>
          <el-step :title=" $t('basicInformation.step2') " :description=" $t('basicInformation.monitorM801') "></el-step>
          <el-step :title=" $t('basicInformation.step3') " :description=" $t('basicInformation.confirmFinishing') "></el-step>
        </el-steps>
        <el-button style="margin-top: 12px;" @click="eightNext">{{ $t('basicInformation.theNextStep') }}</el-button>
      </el-tab-pane>
    </el-tabs>
    <el-dialog
      v-el-drag-dialog
      :title=" $t('basicInformation.numberOfFrontAndRearBrackets') "
      :visible.sync="dialogFirst"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <span>{{ $t('basicInformation.numberOfFrontBrackets') }}:{{firstNumber}}</span>
      <span>{{ $t('basicInformation.numberOfRearBrackets') }}:{{lastNumber}}</span>
    </el-dialog>
    <el-dialog
      v-el-drag-dialog
      :title=" $t('basicInformation.numberOfTraysInFrontAndRear') "
      :visible.sync="dialogFourth"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <span>{{ $t('basicInformation.numberOfFrontPallets') }}:{{frontPallets}}</span>
      <span>{{ $t('basicInformation.numberOfRearTrays') }}:{{rearPallets}}</span>
    </el-dialog>
    <el-dialog
      v-el-drag-dialog
      :title=" $t('basicInformation.startdefiningWriteTrayNumber') "
      :visible.sync="dialogFourthWrite"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <span>{{ $t('basicInformation.thePalletNo') }}</span>
      <el-input v-model="RunningContainer.TrayCode" class="dialog-input" />
      <div slot="footer" class="dialog-footer">
        <el-button @click="confirmWriteD392()" type="primary">{{ $t('baseBtn.confirm') }}</el-button>
      </div>
    </el-dialog>
    <el-dialog
      v-el-drag-dialog
      :title=" $t('basicInformation.writeTheDestinationTrayNumber') "
      :visible.sync="dialogFiveWrite"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <span>{{ $t('basicInformation.thePalletNo') }}</span>
      <el-input v-model="RunningContainer.TrayCode" class="dialog-input" />
      <div slot="footer" class="dialog-footer">
        <el-button @click="confirmWriteD650()" type="primary">{{ $t('baseBtn.confirm') }}</el-button>
      </div>
    </el-dialog>

    <el-dialog
      v-el-drag-dialog
      :title=" $t('basicInformation.theBracketNumberShownInD651') "
      :visible.sync="dialogFiveShelfCode"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <span>{{ $t('basicInformation.bracket') }}:{{shlefCode}}</span>
    </el-dialog>

    <el-dialog
      v-el-drag-dialog
      :title=" $t('basicInformation.theHeightShownInD652') "
      :visible.sync="dialogFiveHeightCode"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <span>{{ $t('basicInformation.height') }}:{{heightCode}}</span>
    </el-dialog>

    <el-dialog
      v-el-drag-dialog
      :title=" $t('basicInformation.writeTheDestinationTrayNumber') "
      :visible.sync="dialogSixWrite"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <span>{{ $t('basicInformation.thePalletNo') }}</span>
      <el-input v-model="RunningContainer.TrayCode" class="dialog-input" />
      <div slot="footer" class="dialog-footer">
        <el-button @click="confirmWriteD700()" type="primary">{{ $t('baseBtn.confirm') }}</el-button>
      </div>
    </el-dialog>

    <el-dialog
      v-el-drag-dialog
      :title=" $t('basicInformation.targetTrayNumber') "
      :visible.sync="dialogSevenWrite"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <span>{{ $t('basicInformation.thePalletNo') }}</span>
      <el-input v-model="RunningContainer.TrayCode" class="dialog-input" />
      <div slot="footer" class="dialog-footer">
        <el-button @click="confirmWriteD750()" type="primary">{{ $t('baseBtn.confirm') }}</el-button>
      </div>
    </el-dialog>

    <el-dialog
      v-el-drag-dialog
      title=" $t('basicInformation.confirmOrCancelTheDeletion') "
      :visible.sync="dialogSevenDelete"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <span>{{ $t('basicInformation.thePalletNo') }}</span>
      <el-input v-model="RunningContainer.TrayCode" class="dialog-input" />
      <div slot="footer" class="dialog-footer">
        <el-button @click="confirmM753()" type="primary">{{ $t('baseBtn.confirmDelete') }}</el-button>
        <el-button @click="confirmM754()" type="primary">{{ $t('baseBtn.cancelDelete') }}</el-button>
      </div>
    </el-dialog>

    <el-dialog
      v-el-drag-dialog
      :title=" $t('basicInformation.spaceUtilization') "
      :visible.sync="dialogEight"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <span>{{ $t('basicInformation.spaceUtilization') }}:{{spaceUserRate}}</span>
    </el-dialog>
  </div>
</template>
<script>
import { deletePicture } from '@/api/FileLibrary'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import { StartM300, GetM340, GetD300, GetD301, FinishM341 } from '@/api/ContainerInitialization'
import { StartM400, GetM440, FinishM441 } from '@/api/ContainerInitialization'
import { StartM450, GetM490, FinishM491 } from '@/api/ContainerInitialization'
import { StartM350, GetM390, GetD390, GetD391, StartM391, GetM392, WriteD392, ConfirmM393, ConfirmM394, GetM395, ConfirmM396 } from '@/api/ContainerInitialization'
import { WriteD650, GetM650, GetD651, StartM651, GetD652 } from '@/api/ContainerInitialization'
import { WriteD700, StartM700, GetM701, ConfirmM702 } from '@/api/ContainerInitialization'
import { WriteD750, StartM750, GetD751, StartM751, GetM752, ConfirmM753, ConfirmM754 } from '@/api/ContainerInitialization'
import { StartM800, GetM801, GetD800, ConfirmM802 } from '@/api/ContainerInitialization'
export default {
  name: 'Box', // 载具箱管理
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {
      activeName: 'first',
      firstActive: 0,
      dialogFirst: false,
      firstNumber: 0,
      lastNumber: 0,
      secondActive: 0,
      thirdActive: 0,
      fourthActive: 0,
      dialogFourth: false,
      frontPallets: 0,
      rearPallets: 0,
      RunningContainer: {
        TrayCode: undefined,
        ContainerCode: '',
        XLight: undefined
      },
      dialogFourthWrite: false,
      fiveActive: 0,
      dialogFiveWrite: false,
      shlefCode: undefined,
      dialogFiveShelfCode: false,
      heightCode: undefined,
      dialogFiveHeightCode: false,
      sixActive: 0,
      dialogSixWrite: false,
      sevenActive: 0,
      eightActive: 0,
      dialogSevenDelete: false,
      dialogSevenWrite: false,
      spaceUserRate: undefined,
      dialogEight: false
      // firstTimer: undefined
    }
  },
  computed: {

  },
  watch: {

  },
  created() {
  },
  beforeDestroy() {
    clearInterval(this.firstTimer)
  },
  destroyed() {
    if (this.firstTimer) {
      clearInterval(this.firstTimer)
    }
  },
  methods: {
    firstNext() {
      if (this.firstActive === 0) {
        // 执行
        StartM300().then((res) => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            const _this = this
            _this.firstActive++

            _this.firstTimer = setInterval(function() { // 定时器开始
              GetM340().then(res => {
                var data = JSON.parse(res.data.Content)
                if (data.Success && data.Data === true) {
                  GetD300().then(res => {
                    var data = JSON.parse(res.data.Content)
                    _this.firstNumber = data.Data
                  })
                  GetD301().then(res => {
                    var data = JSON.parse(res.data.Content)
                    _this.lastNumber = data.Data
                  })
                  _this.dialogFirst = true
                  clearInterval(_this.firstTimer)// 满足条件时 停止计时
                  _this.firstActive++
                }
              })
            }, 5000)
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.failedToStartVerticalLearning') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.firstActive === 1) {
        // 关闭弹框
        if (this.firstActive++ > 2) {
          this.firstActive = 0
        }
      } else if (this.firstActive === 2) {
        // 确认完成
        FinishM341().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.$message({
              title: this.$t('basicTips.achievements'),
              message: this.$t('basicTips.endVerticalLearningSuccessfully') + resData.Message,
              type: 'success',
              duration: 2000
            })
            if (this.firstActive++ > 2) {
              this.firstActive = 0
            }
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.endVerticalLearningFailure') + resData.Message,
              type: 'error',
              duration: 2000
            })
            if (this.firstActive++ > 2) {
              this.firstActive = 0
            }
          }
        })
      } else {
        if (this.firstActive++ > 2) {
          this.firstActive = 0
        }
      }
    },
    secondNext() {
      if (this.secondActive === 0) {
        // 执行
        StartM400().then((res) => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            const _this = this
            _this.secondActive++

            _this.firstTimer = setInterval(function() { // 定时器开始
              GetM440().then(res => {
                var data = JSON.parse(res.data.Content)
                if (data.Success && data.Data === true) {
                  clearInterval(_this.firstTimer)// 满足条件时 停止计时
                  _this.secondActive++
                  _this.$message({
                    title: this.$t('messageTips.Succeed'),
                    message: this.$t('basicTips.theHorizontalLearningJourneyHasEnded'),
                    type: 'success',
                    duration: 2000
                  })
                }
              })
            }, 5000)
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.failedToStartHorizontalLearningJourney') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.secondActive === 1) {
        // 关闭弹框
        if (this.secondActive++ > 2) {
          this.secondActive = 0
        }
      } else if (this.secondActive === 2) {
        // 确认完成
        FinishM441().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.$message({
              title: this.$t('basicTips.achievements'),
              message: this.$t('basicTips.endHorizontalJourneyLearningSuccessful') + resData.Message,
              type: 'success',
              duration: 2000
            })
            if (this.secondActive++ > 2) {
              this.secondActive = 0
            }
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.endHorizontalJourneyLearningFailure') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else {
        if (this.secondActive++ > 2) {
          this.secondActive = 0
        }
      }
    },
    thirdNext() {
      if (this.thirdActive === 0) {
        // 执行
        StartM450().then((res) => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            const _this = this
            _this.thirdActive++

            _this.firstTimer = setInterval(function() { // 定时器开始
              GetM490().then(res => {
                var data = JSON.parse(res.data.Content)
                if (data.Success && data.Data === true) {
                  _this.thirdActive++
                  clearInterval(_this.firstTimer)// 满足条件时 停止计时
                  _this.$message({
                    title: this.$t('messageTips.Succeed'),
                    message: this.$t('basicTips.theAutomaticDoorLearningJourneyHasEnded'),
                    type: 'success',
                    duration: 2000
                  })
                }
              })
            }, 5000)
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.failedToStartAutomaticDoorLearningJourney') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.thirdActive === 1) {
        // 关闭弹框
        if (this.thirdActive++ > 2) {
          this.thirdActive = 0
        }
      } else if (this.thirdActive === 2) {
        // 确认完成
        FinishM491().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.$message({
              title: this.$t('basicTips.achievements'),
              message: this.$t('basicTips.endAutomaticDoorTravelLearningSuccessful') + resData.Message,
              type: 'success',
              duration: 2000
            })
            if (this.thirdActive++ > 2) {
              this.thirdActive = 0
            }
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.endHorizontalJourneyLearningFailure') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else {
        if (this.thirdActive++ > 2) {
          this.thirdActive = 0
        }
      }
    },
    fourthNext() {
      console.log(this.fourthActive)
      if (this.fourthActive === 0) {
        // 执行
        StartM350().then((res) => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            const _this = this
            _this.fourthActive++

            _this.firstTimer = setInterval(function() { // 定时器开始
              GetM390().then(res => {
                var data = JSON.parse(res.data.Content)
                if (data.Success && data.Data === true) {
                  GetD390().then(res => {
                    var data = JSON.parse(res.data.Content)
                    _this.frontPallets = data.Data
                  })
                  GetD391().then(res => {
                    var data = JSON.parse(res.data.Content)
                    _this.rearPallets = data.Data
                  })
                  _this.dialogFourth = true
                  clearInterval(_this.firstTimer)// 满足条件时 停止计时
                  _this.fourthActive++
                }
              })
            }, 5000)
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.failedToStartTrayScan') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.fourthActive === 1) {
        // 关闭弹框
        this.fourthActive++
      } else if (this.fourthActive === 2) {
        StartM391().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            const _this = this
            _this.fourthActive++
            _this.firstTimer = setInterval(function() { // 定时器开始
              GetM392().then(res => {
                var data = JSON.parse(res.data.Content)
                if (data.Success && data.Data === true) {
                  _this.dialogFourthWrite = true
                  clearInterval(_this.firstTimer)// 满足条件时 停止计时
                }
              })
            }, 5000)
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.startDefinitionFailed') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.fourthActive === 4) {
        // 确认完成
        ConfirmM394().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            const _this = this
            _this.fourthActive++
            GetM395().then(res => {
              var data = JSON.parse(res.data.Content)
              if (data.Success && data.Data === true) {
                _this.fourthActive++
              } else if (data.Success && data.Data === false) {
                _this.fourthActive = 3
                _this.firstTimer = setInterval(function() { // 定时器开始
                  GetM392().then(res => {
                    var data = JSON.parse(res.data.Content)
                    if (data.Success && data.Data === true) {
                      _this.dialogFourthWrite = true
                      clearInterval(_this.firstTimer)// 满足条件时 停止计时
                    }
                  })
                }, 5000)
              } else {
                this.$message({
                  title: this.$t('messageTips.Failure'),
                  message: this.$t('basicTips.failedToGetM395Status') + data.Message,
                  type: 'error',
                  duration: 2000
                })
              }
            })
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.failedToStartVerticalLearning') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.fourthActive === 6) {
        // 确认完成
        ConfirmM396().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.$message({
              title: this.$t('messageTips.Succeed'),
              message: this.$t('basicTips.trayScanEndedSuccessfully') + resData.Message,
              type: 'success',
              duration: 2000
            })
            if (this.fourthActive++ > 6) {
              this.fourthActive = 0
            }
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.failedToEndTrayScan') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else {
        if (this.fourthActive++ > 6) {
          this.fourthActive = 0
        }
      }
    },
    confirmWriteD392() {
      WriteD392(this.RunningContainer).then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          ConfirmM393().then(res => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.$message({
                title: this.$t('messageTips.Succeed'),
                message: this.$t('basicTips.trayNumberWrittenSuccessfully') + resData.Message,
                type: 'success',
                duration: 2000
              })
              this.fourthActive++
              this.dialogFourthWrite = false
            } else {
              this.$message({
                title: this.$t('messageTips.Failure'),
                message: this.$t('basicTips.trayNumberWrittenSuccessfully') + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        } else {
          this.$message({
            title: this.$t('messageTips.Failure'),
            message: this.$t('basicTips.trayNumberWrittenSuccessfully') + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    fiveNext() {
      if (this.fiveActive === 0) {
        this.dialogFiveWrite = true
      } else if (this.fiveActive === 1) {
        GetM650().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success && resData.Data === true) {
            GetD651().then(res => {
              var data = JSON.parse(res.data.Content)
              if (data.Success) {
                this.dialogFiveShelfCode = true
                this.shlefCode = data.Data
                this.fiveActive++
              } else {
                this.$message({
                  title: this.$t('messageTips.Failure'),
                  message: this.$t('basicTips.failedToGetTheCarrierNumber') + resData.Message,
                  type: 'error',
                  duration: 2000
                })
              }
            })
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.failedToGetStatus') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.fiveActive == 2) {
        StartM651().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.fiveActive++
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('systemManagementTips.startFailed') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.fiveActive === 3) {
        GetD652().then(res => {
          var data = JSON.parse(res.data.Content)
          if (data.Success) {
            this.dialogFiveHeightCode = true
            this.heightCode = data.Data
            this.fiveActive++
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.failedToGetHeight') + data.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else {
        if (this.fiveActive++ > 3) {
          this.fiveActive = 0
        }
      }
    },
    confirmWriteD650() {
      WriteD650(this.RunningContainer).then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.fiveActive++
          this.dialogFiveWrite = false
        } else {
          this.$message({
            title: this.$t('messageTips.Failure'),
            message: this.$t('basicTips.trayNumberWrittenFailure') + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    sixNext() {
      if (this.sixActive === 0) {
        this.dialogSixWrite = true
      } else if (this.sixActive === 1) {
        StartM700().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.sixActive++
            this.$message({
              title: this.$t('messageTips.Succeed'),
              message: this.$t('systemManagementTips.startSucceed') + resData.Message,
              type: 'success',
              duration: 2000
            })
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('systemManagementTips.startFailed') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.sixActive == 2) {
        GetM701().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            if (resData.Data == true) {
              this.sixActive++
            } else {
              this.$message({
                title: this.$t('messageTips.Failure'),
                message: this.$t('basicTips.insufficientSpace') + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.failedToGetStatus') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.sixActive === 3) {
        ConfirmM702().then(res => {
          var data = JSON.parse(res.data.Content)
          if (data.Success) {
            this.$message({
              title: this.$t('messageTips.Succeed'),
              message: this.$t('basicTips.confirmToAddSuccessfully') + data.Message,
              type: 'success',
              duration: 2000
            })
            this.sixActive++
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.confirmToAddFailure') + data.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else {
        if (this.sixActive++ > 3) {
          this.sixActive = 0
        }
      }
    },
    confirmWriteD700() {
      WriteD700(this.RunningContainer).then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.sixActive++
          this.dialogSixWrite = false
        } else {
          this.$message({
            title: this.$t('messageTips.Failure'),
            message: this.$t('basicTips.trayNumberWrittenFailure') + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    sevenNext() {
      if (this.sevenActive === 0) {
        this.dialogSevenWrite = true
      } else if (this.sevenActive === 1) {
        StartM750().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.sevenActive++
            this.$message({
              title: this.$t('messageTips.Succeed'),
              message: this.$t('systemManagementTips.startSucceed') + resData.Message,
              type: 'success',
              duration: 2000
            })
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('systemManagementTips.startFailed') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.sevenActive === 2) {
        GetD751().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.sevenActive++
            this.$message({
              title: this.$t('messageTips.Succeed'),
              message: this.$t('basicInformation.bracket') + resData.Data,
              type: 'success',
              duration: 2000
            })
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.failedToGetStatus') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.sevenActive === 3) {
        StartM751().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            const _this = this
            _this.sevenActive++

            _this.firstTimer = setInterval(function() { // 定时器开始
              GetM752().then(res => {
                var data = JSON.parse(res.data.Content)
                if (data.Success && data.Data === true) {
                  _this.$message({
                    title: this.$t('messageTips.Succeed'),
                    message: this.$t('basicTips.trayInPlace'),
                    type: 'success',
                    duration: 2000
                  })
                  clearInterval(_this.firstTimer)// 满足条件时 停止计时
                  _this.sevenActive++
                }
              })
            }, 5000)
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.runFailed') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.sevenActive === 5) {
        this.dialogSevenDelete = true
      } else {
        this.sevenActive++
        if (this.sevenActive > 6) {
          console.log(this.sevenActive)
          this.sevenActive = 0
        }
      }
    },
    confirmWriteD750() {
      WriteD750(this.RunningContainer).then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.sevenActive++
          this.dialogSevenWrite = false
        } else {
          this.$message({
            title: this.$t('messageTips.Failure'),
            message: this.$t('basicTips.trayNumberWrittenFailure') + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    confirmM753() {
      ConfirmM753().then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.$message({
            title: this.$t('messageTips.Succeed'),
            message: this.$t('basicTips.confirmDeletionSucceeded'),
            type: 'success',
            duration: 2000
          })
          this.sevenActive++
          this.dialogSevenDelete = false
        } else {
          this.$message({
            title: this.$t('messageTips.Failure'),
            message: this.$t('basicTips.confirmDeletionFailure') + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    confirmM754() {
      ConfirmM754().then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.$message({
            title: this.$t('messageTips.Succeed'),
            message: this.$t('basicTips.cancelDeletionSucceeded'),
            type: 'success',
            duration: 2000
          })
          this.sevenActive++
          this.dialogSevenDelete = false
        } else {
          this.$message({
            title: this.$t('messageTips.Failure'),
            message: this.$t('basicTips.cancelDeletionFailure') + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    eightNext() {
      if (this.eightActive === 0) {
        // 执行
        StartM800().then((res) => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            const _this = this
            _this.eightActive++

            _this.firstTimer = setInterval(function() { // 定时器开始
              GetM801().then(res => {
                var data = JSON.parse(res.data.Content)
                if (data.Success && data.Data === true) {
                  GetD800().then(res => {
                    var data = JSON.parse(res.data.Content)
                    _this.spaceUserRate = data.Data
                  })
                  _this.dialogEight = true
                  clearInterval(_this.firstTimer)// 满足条件时 停止计时
                  _this.eightActive++
                }
              })
            }, 5000)
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.failedToStartDefragmentingStorageSpace') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.eightActive === 1) {
        // 关闭弹框
        if (this.eightActive++ > 2) {
          this.eightActive = 0
        }
      } else if (this.eightActive === 2) {
        // 确认完成
        ConfirmM802().then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.$message({
              title: this.$t('messageTips.Succeed'),
              message: this.$t('basicTips.finishing') + resData.Message,
              type: 'success',
              duration: 2000
            })
            if (this.eightActive++ > 2) {
              this.eightActive = 0
            }
          } else {
            this.$message({
              title: this.$t('messageTips.Failure'),
              message: this.$t('basicTips.failedToConfirmTheCompletionOfSorting') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else {
        if (this.eightActive++ > 2) {
          this.eightActive = 0
        }
      }
    },
    handleClick(tab, event) {

    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
</style>
