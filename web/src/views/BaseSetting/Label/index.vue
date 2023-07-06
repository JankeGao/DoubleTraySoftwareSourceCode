<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input v-model="listQuery.Code" :placeholder="$t('printBar.barCode')" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
        <el-input v-model="listQuery.MaterialCode" :placeholder="$t('stockManagement.bomNumberAndBomName')" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
        <el-input v-model="listQuery.SupplyCode" :placeholder="$t('stockManagement.supplierCodeSupplierName')" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
        <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">{{ $t('baseBtn.queryBtn') }}</el-button>
      </div>
    </el-card>
    <el-row>
      <el-card>
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
          <el-table-column :label="$t('printBar.barCode')" width="160" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Code }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('headerInformation.materialCode')" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('headerInformation.materialName')" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('commonData.quantity')" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.Quantity }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('headerInformation.supplierCode')" width="140" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.SupplierCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('headerInformation.supplierName')" show-overflow-tooltip width="150" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.SupplyName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('headerInformation.batch')" width="120" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.BatchCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('headerInformation.dateOfManufacture')" width="120" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.ManufactrueDateFormat }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('printBar.founder')" width="120" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.CreatedUserName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('printBar.createTime')" width="180" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.CreatedTime }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('headerInformation.operation')" align="center" width="80" class-name="small-padding fixed-width" fixed="right">
            <template slot-scope="scope">
              <el-button size="mini" type="primary" @click="handlePrintCode(scope.row)">打印</el-button>
            </template>
          </el-table-column>
        </el-table>
        <!-- 分页 -->
        <div class="pagination-container">
          <el-pagination :current-page="listQuery.Page" :page-sizes="[15,30,45,60]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
        </div>
      </el-card>

    </el-row>

  </div>
</template>
<script>
import { getPageRecords } from '@/api/Label'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import PrintToLodop from '@/utils/PrintToLodop.js' // 引入附件的js文件

export default {
  name: 'Label', // 条码信息
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {
      // 生成条码
      dialogBarCodeVisible: false,
      dialogSupplyVisible: false,
      // 打印条码
      labelLoading: false,
      dialogLabelVisible: false,
      labelList: [],
      labelCount: 1,
      controls: [],
      // 打印时间
      printDate: null,
      page: {
        width: 520,
        height: 350,
        pagetype: '',
        intOrient: 1
      },
      barCode: '',
      locationList: [],
      // 分页显示总查询数据
      total: null,
      listLoading: false,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 15,
        Code: '',
        Status: undefined,
        Sort: 'id',
        Name: ''
      },
      downloadLoading: false,
      TableKey: 0,
      list: null,
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: this.$t('basicTips.editBarcode'),
        create: this.$t('basicTips.createBarcode')
      },
      // 条码实体
      Label: {
        Id: undefined,
        Code: '',
        Name: '',
        Linkman: '',
        Phone: 0,
        Remark: '',
        Address: ''
      },
      // 输入规则
      rules: {
        Code: [{ required: true, message: this.$t('basicTips.pleaseEnterBarcodeCode'), trigger: 'blur' }],
        Name: [{ required: true, message: this.$t('basicTips.pleaseEnterBarcodeName'), trigger: 'blur' }]
      }
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
  },
  methods: {
    getList() {
      this.listLoading = true
      getPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.total = usersData.total
        console.log(this.list)
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
    // 生成一维码
    createBarCode(data) {
      var JsBarcode = require('jsbarcode')
      const canvas = document.createElement('canvas')
      var settings = {
        format: this.format,
        height: 40,
        width: 1,
        margin: 0,
        displayValue: false
      }
      JsBarcode(canvas, data, settings)
      this.barCode = canvas.toDataURL('image/jpeg')
    },
    handlePrintCode(row) {
      this.fullscreenLoading1 = true
      const data = {}

      this.createBarCode(row.Code)
      // 物料编码
      this.controls.push({
        id: 111,
        type: 'atext',
        data: {
          value: row.MaterialCode,
          width: 400,
          height: 20,
          x: 20,
          y: 10,
          itemtype: 0,
          databind: {
            id: '',
            text: ''
          },
          style: {
            color: '#000',
            fontFamily: this.$t('presentationOfWarehousingDocuments.songTypeface'),
            fontSize: '12px',
            fontSpacing: 0,
            fontWeight: 'normal',
            fontStyle: 'normal',
            textAlign: 'left',
            border: '',
            borderType: 0,
            HOrient: 0,
            VOrient: 0
          }
        }
      })
      // 物料名称
      this.controls.push({
        id: 111,
        type: 'atext',
        data: {
          value: row.MaterialName,
          width: 400,
          height: 20,
          x: 20,
          y: 30,
          itemtype: 0,
          databind: {
            id: '',
            text: ''
          },
          style: {
            color: '#000',
            fontFamily: this.$t('presentationOfWarehousingDocuments.songTypeface'),
            fontSize: '12px',
            fontSpacing: 0,
            fontWeight: 'normal',
            fontStyle: 'normal',
            textAlign: 'left',
            border: '',
            borderType: 0,
            HOrient: 0,
            VOrient: 0
          }
        }
      })
      // 物料条码
      this.controls.push({
        id: 111,
        type: 'atext',
        data: {
          value: row.Code,
          width: 400,
          height: 20,
          x: 20,
          y: 50,
          itemtype: 0,
          databind: {
            id: '',
            text: ''
          },
          style: {
            color: '#000',
            fontFamily: this.$t('presentationOfWarehousingDocuments.songTypeface'),
            fontSize: '12px',
            fontSpacing: 0,
            fontWeight: 'normal',
            fontStyle: 'normal',
            textAlign: 'left',
            border: '',
            borderType: 0,
            HOrient: 0,
            VOrient: 0
          }
        }
      })
      // 一维码
      this.controls.push({
        id: 1,
        type: 'aimage',
        data: {
          x: 20,
          y: 75,
          width: 400,
          height: 60,
          itemtype: 0,
          databindtype: 0,
          databind: {
            id: '',
            text: ''
          },
          style: {
            backgroundSize: 0,
            defaultimgtype: 0,
            defaultimg: this.barCode,
            HOrient: 0,
            VOrient: 0
          }
        }
      })
      var printobj = new PrintToLodop(this.controls, data, this.page)
      printobj.print()
      this.controls = []
      setTimeout(() => {
        this.fullscreenLoading1 = false
        this.dialogLabelVisible = true
      }, 1 * 2000)
    }
  }

}
</script>

