<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-row :gutter="20">
      <el-col :span="5">
        <el-card>
          <div>
            <span>
              <!-- :title="textMap[dialogStatus]" -->
              <div style="display: inline-block;">
                <h4>仓库信息</h4>
              </div>
            </span>
          </div>
          <hr class="line" />
          <div>
            <el-tree
              ref="treeTest"
              :data="treeData"
              :expand-on-click-node="false"
              style="font-size:20px;"
              node-key="Id"
              :default-expand-all="true"
              highlight-current
              show-checkbox
              @check-change="handleCheckChange"
            />
          </div>
        </el-card>
      </el-col>
      <el-col :span="19">
        <el-card>
          <div class="filter-container" style="margin-bottom:10px">
            <el-input
              v-model="listQuery.WarehouseCode"
              placeholder="仓库编码、名称"
              class="filter-item"
              style="width:150px"
              clearable
              @keyup.enter.native="handleFilter"
              @clear="handleFilter"
            />
            <el-input
              v-model="listQuery.ContainerCode"
              placeholder="货柜编码"
              class="filter-item"
              style="width:150px"
              clearable
              @keyup.enter.native="handleFilter"
              @clear="handleFilter"
            />
            <el-input
              v-model="listQuery.TrayCode"
              placeholder="托盘编号"
              class="filter-item"
              style="width:150px"
              clearable
              @keyup.enter.native="handleFilter"
              @clear="handleFilter"
            />
            <el-input
              v-model="listQuery.LocationCode"
              placeholder="储位编码"
              class="filter-item"
              clearable
              @keyup.enter.native="handleFilter"
              @clear="handleFilter"
            />
            <el-input
              v-model="listQuery.MaterialCode"
              placeholder="物料编码、物料名称、物料条码"
              class="filter-item"
              clearable
              @keyup.enter.native="handleFilter"
              @clear="handleFilter"
            />
            <el-input
              v-model="listQuery.SupplierCode"
              placeholder="供应商编码、供应商名称"
              class="filter-item"
              clearable
              @keyup.enter.native="handleFilter"
              @clear="handleFilter"
            />
            <el-button v-waves class="filter-button" type="primary" @click="handleFilter">查询</el-button>
            <el-dropdown size="small" placement="bottom" trigger="click" @command="batchOperate">
              <el-button type="primary">导出</el-button>
              <el-dropdown-menu slot="dropdown">
                <el-dropdown-item command="All_Export">全部导出</el-dropdown-item>
                <el-dropdown-item command="Condition_Export">按条件导出</el-dropdown-item>
              </el-dropdown-menu>
            </el-dropdown>
            <el-upload
              ref="fileupload"
              style="display: inline; margin-left: 10px;margin-right: 10px;"
              action="#"
              :show-file-list="false"
              :http-request="uploadFile"
              :before-upload="beforeUpload"
              :on-exceed="handleExceed"
            >
              <el-button type="primary">
                <i class="el-icon-upload" /> 导入
              </el-button>
            </el-upload>
            <el-button
              :loading="downloadLoading"
              class="filter-button"
              type="primary"
              icon="el-icon-download"
              @click="handleDownUpload"
            >模板</el-button>
          </div>
          <el-table
            :key="TableKey"
            v-loading="listLoading"
            :data="list"
            :header-cell-style="{background:'#F5F7FA'}"
            border
            fit
            size="mini"
            highlight-current-row
            style="width:100%;min-height:100%;"
            @selection-change="CheckedFun"
            @sort-change="sortChange"
            @header-contextmenu="contextmenu"
          >
            <!-- <el-table-column type="selection" width="50" /> -->
            <el-table-column type="index" width="50" />
            <el-table-column :label="'状态'" width="90" align="center">
              <template slot-scope="scope">
                <el-tag v-if="scope.row.MaterialStatus===0">
                  <span>{{ scope.row.MaterialStatusDescription }}</span>
                </el-tag>
                <el-tag v-if="scope.row.MaterialStatus===4" type="warning">
                  <span>{{ scope.row.MaterialStatusDescription }}</span>
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column
              v-for="(item) in tableHeader"
              v-if="item.istrue"
              :key="item.key"
              show-overflow-tooltip
              :label="item.title"
              :prop="item.key"
              align="center"
              :width="item.width"
              :sortable="item.sortable"
            >
              <template slot-scope="scope">
                <span v-if="item.key==='IsLocked'">
                  <el-switch
                    v-model="scope.row.IsLocked"
                    active-color="#13ce66"
                    inactive-color="#ff4949"
                    :disabled="true"
                  />
                </span>
                <span v-else-if="item.key==='IsCheckLocked'">
                  <el-switch
                    v-model="scope.row.IsCheckLocked"
                    active-color="#13ce66"
                    inactive-color="#ff4949"
                    :disabled="true"
                  />
                </span>
                <span v-else>{{ scope.row[item.key] }}</span>
              </template>
            </el-table-column>

            <!-- <el-table-column :label="'仓库编码'" width="90" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WareHouseCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'仓库名称'" width="120" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WareHouseName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'区域编码'" width="90" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.AreaCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'库位地址'" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LocationCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料条码'" width="160" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialLabel }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'总数量'" width="90" align="center">
            <template slot-scope="scope">
              <span style="color:green">{{ scope.row.Quantity }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'拣货锁定'" width="90" align="center">
            <template slot-scope="scope">
              <el-switch
                v-model="scope.row.IsLocked"
                active-color="#13ce66"
                inactive-color="#ff4949"
                :disabled="true"
              />
            </template>
          </el-table-column>
          <el-table-column :label="'锁定数量'" width="90" align="center">
            <template slot-scope="scope">
              <span style="color:red">{{ scope.row.LockedQuantity }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'单位'" width="80" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialUnit }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料编码'" width="160" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料名称'" width="200" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'供应商名称'" width="120" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.SupplierName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'批次'" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.BatchCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'盘点锁定'" width="90" align="center">
            <template slot-scope="scope">
              <el-switch
                v-model="scope.row.IsCheckLocked"
                active-color="#13ce66"
                inactive-color="#ff4949"
                :disabled="true"
              />
            </template>
          </el-table-column>
          <el-table-column sortable="custom" :label="'添加时间'" prop="CreatedTime" width="200" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.CreatedTime }}</span>
            </template>
            </el-table-column>-->
            <!-- <el-table-column :label="'操作'" align="center" width="80" class-name="small-padding fixed-width" fixed="right">
            <template slot-scope="scope">
              <el-button size="mini" type="primary" @click="handlePtl(scope.row)">指引</el-button>
            </template>
            </el-table-column>-->
          </el-table>
          <div
            v-show="menuVisible"
            :style="{top:top+ &quot;px&quot;,left:left+ &quot;px&quot;}"
            class="menu1"
          >
            <el-checkbox-group v-model="colOptions">
              <el-checkbox v-for="(item) in colSelect" :key="item" :label="item" />
            </el-checkbox-group>
            <el-button
              v-waves
              class="filter-button"
              style="float:right"
              type="primary"
              size="mini"
              @click="handleShowClounm"
            >确定</el-button>
          </div>
          <!-- 分页 -->
          <div class="pagination-container">
            <el-pagination
              :current-page="listQuery.Page"
              :page-sizes="[15,30,45,60]"
              :page-size="listQuery.Rows"
              :total="total"
              background
              layout="total, sizes, prev, pager, next, jumper"
              @size-change="handleSizeChange"
              @current-change="handleCurrentChange"
            />
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>
<script>
import { ouLoadStockInfo, getPageRecords, LightStock, OffLightStock, DeleteStockArray, DoDownLoadLabelStock, DoDownLoadMaterialStock } from '@/api/stock'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import { getWareHouseTreeData } from '@/api/WareHouse'
export default {
  name: 'Stock', // 条码库存
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {
      // 分页显示总查询数据
      total: null,
      listLoading: false,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 15,
        MaterialCode: '',
        Status: undefined,
        MaterialName: '',
        WarehouseCode: '',
        SupplierCode: '',
        AreaArray: '',
        ContainerCode: '',
        LocationCode: '',
        TrayCode: ''
      },
      treeData: [],
      downloadLoading: false,
      TableKey: 0,
      list: null,
      checkBoxData: [],
      colOptions: ['仓库编码', '仓库名称', '货柜编码', '库位地址', '物料条码', '总数量', '拣货锁定', '锁定数量', '单位', '物料编码', '物料名称', '供应商名称', '批次', '盘点锁定', '入库时间', '老化周期'],
      colSelect: ['仓库编码', '仓库名称', '货柜编码', '库位地址', '物料条码', '总数量', '拣货锁定', '锁定数量', '单位', '物料编码', '物料名称', '供应商名称', '批次', '盘点锁定', '入库时间', '老化周期'],
      top: 0,		// 右键菜单的位置
      left: 0,
      menuVisible: false, // 右键菜单的显示与隐藏
      tableHeader: [
        { title: '仓库编码', key: 'WareHouseCode', width: '80', istrue: true },
        { title: '仓库名称', key: 'WareHouseName', width: '120', istrue: true },
        { title: '货柜编码', key: 'ContainerCode', width: '80', istrue: true },
        { title: '托盘编码', key: 'TrayCode', width: '80', istrue: true },
        { title: '物料名称', key: 'MaterialName', width: '200', overflow: true, istrue: true },
        { title: '总数量', key: 'Quantity', width: '80', istrue: true },
        { title: '锁定数量', key: 'LockedQuantity', width: '80', istrue: true },
        { title: '单位', key: 'MaterialUnit', width: '80', istrue: true },
        { title: '供应商名称', key: 'SupplierName', width: '120', overflow: true, istrue: true },
        { title: '批次', key: 'BatchCode', width: '120', istrue: true },
        { title: '价格', key: 'Price', width: '80', istrue: true },
        { title: '用途', key: 'Use', width: '120', istrue: true },
        { title: 'X轴编号', key: 'XLight', width: '80', istrue: true },
        { title: 'Y轴编号', key: 'YLight', width: '80', istrue: true },
        { title: '储位编码', key: 'LocationCode', width: '120', istrue: true },
        { title: '物料条码', key: 'MaterialLabel', width: '160', istrue: true },
        { title: '物料编码', key: 'MaterialCode', width: '140', istrue: true },
        { title: '盘点锁定', key: 'IsCheckLocked', width: '80', istrue: true },
        { title: '拣货锁定', key: 'IsLocked', width: '80', istrue: true },
        { title: '入库时间', key: 'ShelfTime', width: '200', prop: 'ShelfTime', sortable: 'custom', istrue: true },
        { title: '老化周期', key: 'AgeingPeriod', width: '80', prop: 'AgeingPeriod', istrue: true }
      ]
    }
  },
  watch: {
    // 授权面板关闭，清空原角色查询权限
    // dialogTreeVisible(value) {
    //   if (!value) {
    //     this.resetModuleAuthData()
    //   }
    // }

    // colOptions(newVal, oldVal) {
    //   if (newVal) { // 如果有值发生变化，即多选框的已选项变化
    //     var arr = this.colSelect.filter(i => newVal.indexOf(i) < 0) 	// 未选中
    //     this.tableHeader.filter(i => {
    //       if (arr.indexOf(i.title) !== -1) {
    //         i.istrue = false
    //       } else {
    //         i.istrue = true
    //       }
    //     })
    //   }
    // }
  },
  created() {
    this.getList()
    this.getWarehouseDataList()
  },
  methods: {
    getWarehouseDataList() {
      getWareHouseTreeData(0).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.treeData = this.convertTreeData2(usersData)
      })
    },
    convertTreeData2(data) {
      const treedataList = []
      var entity = {
        Code: 'WareHouse',
        Name: '仓库信息'
      }
      var first = this.generateRouter2(entity, true, 0)
      var firstchildren = []
      data.forEach(item => {
        var parent = this.generateRouter2(item, true, 1)
        var containerList = []
        var twoChild = []
        containerList = item.ContainerList
        var trayList = []
        trayList = item.TrayList
        containerList.forEach(area => {
          var threeChild = []
          trayList.forEach(channel => {
            if (channel.ContainerCode === area.Code && channel.WareHouseCode === area.WareHouseCode) {
              threeChild.push(this.generateRouter2(channel, false, 3))
            }
          })
          var twoParent = this.generateRouter2(area, false, 2)
          twoParent.children = threeChild
          twoChild.push(twoParent)
        })

        parent.children = twoChild
        firstchildren.push(parent)
      })
      first.children = firstchildren
      treedataList.push(first)
      return treedataList
    },
    generateRouter2(item, isParent, level) {
      if (level === 1) {
        item.Name = item.Name + '(仓库)'
      }
      if (level === 2) {
        item.Name = item.Code + '-' + item.Brand + '(货柜)'
      }
      if (level === 3) {
        item.Name = item.Code + '(托盘)'
      }
      var treeData = {
        label: item.Name,
        Code: item.Code,
        Name: item.Name,
        Level: level,
        WareHouseCode: item.WareHouseCode,
        ContainerCode: item.ContainerCode,
        Id: item.Id
      }
      return treeData
    },
    getList() {
      this.listLoading = true
      getPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.total = usersData.total

        // Just to simulate the time of the request
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    // 数据筛选
    handleFilter() {
      this.listQuery.Page = 1
      this.getList()
    },
    // 切换分页数据-行数据
    handleSizeChange(val) {
      this.listQuery.Rows = val
      this.getList()
    },
    // 切换分页-列数据
    handleCurrentChange(val) {
      this.listQuery.Page = val
      this.getList()
    },
    beforeUpload(file) {
      const isText = file.type === 'application/vnd.ms-excel'
      const isTextComputer = file.type === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      return (isText | isTextComputer)
    },
    // 上传文件个数超过定义的数量
    handleExceed(files, fileList) {
      this.$message.warning(`当前限制选择 1 个文件，请删除后继续上传`)
    },
    // 上传文件
    uploadFile(item) {
      //  console.log(item)
      const fileObj = item.file
      // FormData 对象
      const form = new FormData()
      // 文件对象
      form.append('file', fileObj)
      form.append('comId', this.comId)
      ouLoadStockInfo(form).then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.getList()
          this.$message({
            title: '成功',
            message: '导入成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '导入失败' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handleDownUpload() {
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoDownLoadTemp'
      window.open(url)
    },
    handlePtl(row) {

    },
    batchOperate(command) {
      switch (command) {
        case 'All_Export':
          this.All_ExportExcel()
          break
        case 'Condition_Export':
          this.Condition_ExportExcel()
          break
      }
    },

    All_ExportExcel() {
      this.$confirm('此操作将导出全部数据，共：' + this.total + '条, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        // var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoDownLoadLabelStock'
        // window.open(url)
        //this.exportFile()
        DoDownLoadLabelStock().then(res => {
          console.log(res)
          if (true) {
            // 文件下载
            const blob = new Blob([res.data], {
              type: "application/vnd.ms-excel"
            });
            let link = document.createElement('a');
            link.href = URL.createObjectURL(blob);
            link.setAttribute('download', '单盒物料库存信息.xls');
            link.click();
            link = null;
            this.$message.success('导出成功');
          } else {
            // 返回json
            this.$message.warning('下载失败');
          }
        })
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消'
        })
      })
    },

    // 条件导出
    Condition_ExportExcel() {
      if (this.listQuery.WarehouseCode === '' && this.listQuery.MaterialCode === '' && this.listQuery.SupplierCode === '' && this.listQuery.ContainerCode === '' && this.listQuery.TrayCode === '' && this.listQuery.LocationCode === '' && this.listQuery.AreaArray === '') {
        this.$confirm('您没有选择导出条件，将导出共：' + this.total + '条数据, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          // var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoDownLoadLabelStock'
          // window.open(url)
          DoDownLoadLabelStock(this.listQuery).then(res => {
            if (true) {
              // 文件下载
              const blob = new Blob([res.data], {
                type: "application/vnd.ms-excel"
              });
              let link = document.createElement('a');
              link.href = URL.createObjectURL(blob);
              link.setAttribute('download', '单盒物料库存信息.xls');
              link.click();
              link = null;
              this.$message.success('导出成功');
            } else {
              // 返回json
              this.$message.warning('下载失败');
            }
          })
        }).catch(() => {
          this.$message({
            type: 'info',
            message: '已取消'
          })
        })
      } else {
        // var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoDownLoadLabelStock?WarehouseCode=' + this.listQuery.WarehouseCode + '&MaterialCode=' + this.listQuery.MaterialCode + '&SupplierCode=' + this.listQuery.SupplierCode + '&ContrainerCode=' + this.listQuery.ContainerCode + '&LocationCode=' + this.listQuery.LocationCode + '&TrayCode=' + this.listQuery.TrayCode + '&AreaArray=' + this.listQuery.AreaArray
        // window.open(url)
        DoDownLoadLabelStock(this.listQuery).then(res => {
          console.log(res)
          if (true) {
            // 文件下载
            //let fileName = res.headers['content-disposition'].split('=')[1]
            const blob = new Blob([res.data], {
              type: "application/vnd.ms-excel"
            });
            let link = document.createElement('a');
            link.href = URL.createObjectURL(blob);
            link.setAttribute('download', '单盒物料库存信息.xls');
            link.click();
            link = null;
            this.$message.success('导出成功');
          } else {
            // 返回json
            this.$message.warning('下载失败');
          }
        })
      }
    },
    exportFile() {
      axios({
        method: 'get',
        url: '/api/Stock/DoDownLoadLabelStock',
        headers: {
          token: store.getters.token
        },
        params: this.listQuery,
        responseType: "blob"
      })
        .then(res => {
          if (res.data.type) {
            // 文件下载
            const blob = new Blob([res.data], {
              type: "application/vnd.ms-excel"
            });
            let link = document.createElement('a');
            link.href = URL.createObjectURL(blob);
            link.setAttribute('download', '导出文件.xlsx');
            link.click();
            link = null;
            this.$message.success('导出成功');
          } else {
            // 返回json
            this.$message.warning(res.data.msg);
          }
        })
        .catch(err => {
          this.btnLoading = false;
          this.$message.error("下载失败");
        });
    },

    CheckedFun(val) {
      this.checkBoxData = val
    },
    sortChange(column, prop, order) {
      this.listQuery.Sort = column.prop
      if (column.order === 'ascending') {
        this.listQuery.Order = 'asc'
      } else if (column.order === 'descending') {
        this.listQuery.Order = 'desc'
      } else {
        this.listQuery.Order = null
      }
      this.getList()
    },
    handleLight() {
      if (this.checkBoxData.length > 0) {
        this.$confirm('确定点亮勾选的库存?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          LightStock(this.checkBoxData).then(res => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.$message({
                title: '成功',
                message: '点亮成功',
                type: 'success',
                duration: 2000
              })
              this.dialogFormSort = false
              // this.getList()
            } else {
              this.$message({
                title: '失败',
                message: '点亮失败' + resData.Message,
                type: 'error',
                duration: 5000
              })
            }
          })
        })
      } else {
        this.$message({
          title: '失败',
          message: '尚未勾选拣货单',
          type: 'error',
          duration: 5000
        })
      }
    },
    handleOffLight() {
      this.$confirm('确定点亮勾选的库存?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        OffLightStock(this.checkBoxData).then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.dialogFormVisible = false
            this.$message({
              title: '成功',
              message: '熄灭成功',
              type: 'success',
              duration: 2000
            })
            this.dialogFormSort = false
            // this.getList()
          } else {
            this.$message({
              title: '失败',
              message: '熄灭失败' + resData.Message,
              type: 'error',
              duration: 5000
            })
          }
        })
      })
    },
    handleShelfDwon() {
      if (this.checkBoxData.length > 0) {
        this.$confirm('确定下架勾选的条码?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          DeleteStockArray(this.checkBoxData).then(res => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.$message({
                title: '成功',
                message: '直接下架条码成功',
                type: 'success',
                duration: 2000
              })
              this.dialogFormSort = false
              this.getList()
            } else {
              this.$message({
                title: '失败',
                message: '直接下架失败' + resData.Message,
                type: 'error',
                duration: 5000
              })
            }
          })
        })
      } else {
        this.$message({
          title: '失败',
          message: '尚未勾选拣货单',
          type: 'error',
          duration: 5000
        })
      }
    },
    contextmenu(row, event) {
      // 先把菜单关闭，目的是第二次或者第n次右键鼠标的时候 它默认的是true
      this.menuVisible = false
      // 显示菜单
      this.menuVisible = true
      window.event.returnValue = false // 阻止浏览器自带的右键菜单弹出
      // 给整个document绑定click监听事件， 左键单击任何位置执行foo方法
      // document.addEventListener('click', this.foo)
      // event对应的是鼠标事件，找到鼠标点击位置的坐标，给菜单定位
      this.top = event.clientY
      this.left = event.clientX
    },
    foo() {
      this.menuVisible = false // 关闭菜单栏
      document.removeEventListener('click', this.foo) // 解绑click监听，很重要，具体原因可以看另外一篇博文
    },
    handleShowClounm() {
      this.menuVisible = false // 关闭菜单栏
      this.tableHeader.forEach(element => {
        if (this.colOptions.indexOf(element.title) >= 0) {
          element.istrue = true
        } else {
          element.istrue = false
        }
      })
    },
    handleCheckChange(data, checked, indeterminate) {
      var array = this.$refs['treeTest'].getCheckedNodes()
      var areaCodes = ''
      this.$nextTick(() => {
        array.forEach(element => {
          if (element.Level === 3) {
            areaCodes = areaCodes + element.Id + ','
            //    console.log(areaCodes)
          }
        })

        this.listQuery.AreaArray = areaCodes // JSON.stringify(array).toString()
      })
    }
  }

}
</script>
<style scoped>
.menu1 {
  position: fixed;
  height: auto;
  width: 250px;
  border-radius: 3px;
  border: 1px solid #999999;
  background-color: #f4f4f4;
  padding: 10px;
  z-index: 1000;
}
.el-checkbox {
  display: inline-block;
  height: 20px;
  line-height: 20px;
  padding: 0 5px;
  margin-right: 0;
  font-size: 12px;
  border: 1px solid transparent;
  width: 100px;
  overflow: hidden;
}
</style>

