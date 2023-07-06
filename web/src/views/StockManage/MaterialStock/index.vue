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
              placeholder="仓库编码、仓库名称"
              class="filter-item"
              clearable
              @keyup.enter.native="handleFilter"
              @clear="handleFilter"
            />
            <el-input
              v-model="listQuery.ContainerCode"
              placeholder="货柜编码"
              class="filter-item"
              clearable
              @keyup.enter.native="handleFilter"
              @clear="handleFilter"
            />
            <el-input
              v-model="listQuery.MaterialCode"
              placeholder="物料编码、物料名称"
              class="filter-item"
              clearable
              @keyup.enter.native="handleFilter"
              @clear="handleFilter"
            />
            <el-button
              v-waves
              class="filter-button"
              type="primary"
              icon="el-icon-search"
              @click="handleFilter"
            >查询</el-button>
            <el-dropdown size="small" placement="bottom" trigger="click" @command="batchOperate">
              <el-button :loading="downloadLoading" class="filter-button" type="primary">
                导出
                <i class="el-icon-arrow-down el-icon--right" />
              </el-button>
              <el-dropdown-menu slot="dropdown">
                <el-dropdown-item command="All_Export">全部导出</el-dropdown-item>
                <el-dropdown-item command="Condition_Export">按条件导出</el-dropdown-item>
              </el-dropdown-menu>
            </el-dropdown>
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
          >
            <el-table-column type="index" width="50" />
            <el-table-column :label="'仓库编码'" width="90" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.WareHouseCode }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'仓库名称'" width="90" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.WareHouseName }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'货柜编码'" width="90" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.ContainerCode }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'物料编码'" width="150" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.MaterialCode }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'物料名称'" width="150" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.MaterialName }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'价格'" width="80" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.Price }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'用途'" width="80" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.Use }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'上架储位'" width="100" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.LocationCode }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'总数量'" width="80" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span style="color:green">{{ scope.row.Quantity }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'锁定数量'" width="90" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span style="color:red">{{ scope.row.LockedQuantity }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'可用数量'" width="90" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span style="color:green">{{ scope.row.Quantity-scope.row.LockedQuantity }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'单位'" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.MaterialUnit }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'入库时间'" width="100" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.ShelfTime }}</span>
              </template>
            </el-table-column>

            <!-- <el-table-column :label="'操作'" align="center" width="80" class-name="small-padding fixed-width" fixed="right">
            <template slot-scope="scope">
              <el-button size="mini" type="primary" @click="handlePtl(scope.row)">指引</el-button>
            </template>
            </el-table-column>-->
          </el-table>
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
import { getMaterialPageRecords, DoDownLoadMaterialStock } from '@/api/stock'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import { getWareHouseTreeData } from '@/api/WareHouse'
export default {
  name: 'MaterialStock', // 物料库存
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
        WarehouseCode: '',
        Status: undefined,
        Sort: 'id',
        MaterialName: '',
        AreaArray: ''
      },
      treeData: [],
      downloadLoading: false,
      TableKey: 0,
      list: null
    }
  },
  watch: {
    // 授权面板关闭，清空原角色查询权限
    // dialogTreeVisible(value) {
    //   if (!value) {
    //     this.resetModuleAuthData()
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
      getMaterialPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.list.forEach(element => {
        })
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
    handlePtl(row) {

    },

    // 导出选择
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
    // 条件导出
    Condition_ExportExcel() {
      if (this.listQuery.MaterialCode === '' && this.listQuery.WarehouseCode === '' && this.listQuery.AreaArray === '' && this.listQuery.ContainerCode === '') {
        this.$confirm('没有选择导出条件，将导出全部数据, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          // var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoDownLoadMaterialStock'
          // window.open(url)
          DoDownLoadMaterialStock().then(res => {
            console.log(res)
            if (true) {
              // 文件下载
              //let fileName = res.headers['content-disposition'].split('=')[1]
              const blob = new Blob([res.data], {
                type: "application/vnd.ms-excel"
              });
              let link = document.createElement('a');
              link.href = URL.createObjectURL(blob);
              link.setAttribute('download', '物料库存信息.xls');
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
        // var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoDownLoadMaterialStock?MaterialCode=' + this.listQuery.MaterialCode + '&WarehouseCode=' + this.listQuery.WarehouseCode + '&AreaArray=' + this.listQuery.AreaArray + '&ContainerCode=' + this.listQuery.ContainerCode
        // window.open(url)
        DoDownLoadMaterialStock(this.listQuery).then(res => {
          console.log(res)
          if (true) {
            // 文件下载
            //let fileName = res.headers['content-disposition'].split('=')[1]
            const blob = new Blob([res.data], {
              type: "application/vnd.ms-excel"
            });
            let link = document.createElement('a');
            link.href = URL.createObjectURL(blob);
            link.setAttribute('download', '物料库存信息.xls');
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

    // 全部导出
    All_ExportExcel() {
      this.$confirm('此操作将导出全部数据是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        // var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoDownLoadMaterialStock'
        // window.open(url)
        DoDownLoadMaterialStock().then(res => {
          console.log(res)
          if (true) {
            // 文件下载
            //let fileName = res.headers['content-disposition'].split('=')[1]
            const blob = new Blob([res.data], {
              type: "application/vnd.ms-excel"
            });
            let link = document.createElement('a');
            link.href = URL.createObjectURL(blob);
            link.setAttribute('download', '物料库存信息.xls');
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

