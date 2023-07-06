<template>
  <div class="app-container">
    <!-- 主表单 -->
    <el-card class="search-card">
      <div class="filter-container" style="margin-bottom:10px">
        <el-input v-model="listQuery.Code" placeholder="出库单号" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
        <el-select
          v-model="listQuery.Status"
          :multiple="false"
          filterable
          remote
          reserve-keyword
          @change="handleFilter"
        >
          <el-option
            v-for="item in statusList"
            :key="item.Code"
            :label="item.Name"
            :value="item.Code"
          />
        </el-select>
        <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">添加</el-button>
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleInterfaceCreate">同步</el-button>
        <!-- <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="dialogOutMaterialVisible=true">退料</el-button> -->
        <el-upload
          ref="fileupload"
          style="display: inline; margin-left: 10px;margin-right: 10px;"
          action="#"
          :show-file-list="false"
          :http-request="uploadFile"
          :before-upload="beforeUpload"
          :on-exceed="handleExceed"
        >
          <el-button type="primary"><i class="el-icon-upload el-icon--right" />  导入</el-button>
        </el-upload>
        <el-button v-waves :loading="downloadLoading" class="filter-button" type="primary" icon="el-icon-download" @click="handleDownUpload">{{ '模板' }}</el-button>
      </div>
      <el-table
        :key="TableKey"
        v-loading="listLoading"
        :data="list"
        :header-cell-style="{background:'#F5F7FA'}"
        height="331"
        size="mini"
        border
        fit
        highlight-current-row
        style="width:100%;min-height:100%;"
        @row-click="handleRowClick"
      >
        <el-table-column type="index" />
        <el-table-column :label="'状态'" width="110" align="center">
          <template slot-scope="scope">
            <el-tag v-if="scope.row.Status===0" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            <el-tag v-if="scope.row.Status===1" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            <el-tag v-if="scope.row.Status===2"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            <el-tag v-if="scope.row.Status===3" type="success"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            <el-tag v-if="scope.row.Status===4" type="info"><span>{{ scope.row.StatusCaption }}</span></el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'出库单号'" width="160" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'出库类型'" width="120" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.OutDictDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'仓库'" width="100" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.WareHouseName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'操作人'" width="150" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedUserName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'添加时间'" width="200" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedTime }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'来源单据号'" width="150" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.BillCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'备注'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remark }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'操作'" align="center" width="250" class-name="small-padding fixed-width">
          <template slot-scope="scope">
            <el-button type="text" icon="el-icon-delete" circle @click="handleDelete(scope.row)" />
            <el-button v-loading.fullscreen.lock="fullscreenLoading" size="mini" type="primary" @click="handleCreateTask(scope.row)">下发</el-button>
            <el-button size="mini" type="danger" @click="handleCancel(scope.row)">作废</el-button>
          </template>
        </el-table-column>
      </el-table>
      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination :current-page="listQuery.Page" :page-sizes="[6,12,18,24]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
      </div>
    </el-card>
    <!-- 行项目表单 -->
    <el-card>
      <el-table
        :key="TableKey"
        v-loading="false"
        :data="listMaterial"
        :header-cell-style="{background:'#F5F7FA'}"
        :height="300"
        size="mini"
        border
        fit
        highlight-current-row
        style="width:100%;min-height:100%;"
      >
        <el-table-column type="index" width="50" />
        <el-table-column :label="'状态'" width="180" align="center">
          <template slot-scope="scope">
            <el-tag v-if="scope.row.Status===0" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            <el-tag v-if="scope.row.Status===1" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            <el-tag v-if="scope.row.Status===2"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            <el-tag v-if="scope.row.Status===3" type="success"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            <el-tag v-if="scope.row.Status===4" type="info"><span>{{ scope.row.StatusCaption }}</span></el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'出库单号'" width="200" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.OutCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料编码'" width="250" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料描述'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'数量'" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Quantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'已下发数量'" width="150" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.SendInQuantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'已拣选数量'" width="150" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.PickedQuantity }}</span>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    <!-- 创建/编辑 弹出框 -->
    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" :width="'85%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="Out" class="dialog-form" label-width="100px" label-position="left">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item :label="'仓库编码'" prop="WareHouseCode">
              <el-select
                v-model="Out.WareHouseCode"
                :multiple="false"
                filterable
                style="width:300px"
                @change="changeWarehouse"
              >
                <el-option
                  v-for="item in WareHouseList"
                  :key="item.Code"
                  :label="item.Name"
                  :value="item.Code"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="'出库类型'" :prop="'LocationCode'">
              <el-select
                v-model="Out.OutDict"
                :multiple="false"
                reserve-keyword
                style="width:300px"
              >
                <el-option
                  v-for="item in dictionaryList"
                  :key="item.Code"
                  :label="item.Name"
                  :value="item.Code"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="'来源单号'" prop="Name">
              <el-input v-model="Out.BillCode" class="dialog-input" clearable placeholder="请输入来源单据号" />
            </el-form-item>
            <el-form-item :label="'备注信息'">
              <el-input 
              v-model="Out.Remark" 
              :autosize="{ minRows: 1, maxRows: 1}" 
              type="textarea" 
              placeholder="出库备注" 
              class="dialog-input" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <div style="margin-bottom:20px">
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreateOutMaterial">添加</el-button>
        <el-button class="filter-button" style="margin-left: 10px;" type="danger" icon="el-icon-delete" @click="handleDeleteOutMaterial">移除</el-button>
      </div>
      <el-table
        :key="TableKey"
        ref="addMaterialGrid"
        v-loading="false"
        :data="addMaterial"
        :header-cell-style="{background:'#F5F7FA'}"
        border
        fit
        highlight-current-row
        style="width:100%;min-height:100%;"
        height="350"
        @row-click="addMaterialGridClick"
      >
        <el-table-column type="index" width="50" />
        <el-table-column :label="'行项目号'" width="180" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.ItemNo }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料名称'" align="center">
          <template slot-scope="scope">
            <el-select
              v-if="scope.row.Status==0"
              v-model="scope.row.MaterialCode"
              :multiple="false"
              style="width:400px"
              filterable
              remote
              reserve-keyword
              placeholder="请输入关键词(物料编码或名称)"
              :remote-method="remoteMethod"
              :loading="loading"
              @change="materialCodeChange(scope.row)"
            >
              <el-option
                v-for="item in materialList"
                :key="item.Code"
                :label="item.Name"
                :value="item.Code"
              />
            </el-select>
            <!-- <el-input v-if="scope.row.Status" v-model="scope.row.MaterialCode" /> -->
            <span v-else>{{ scope.row.MaterialName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料编码'" width="250" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料类别'" width="100" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialTypeDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'可用库存'" width="250" align="center">
          <template slot-scope="scope">
            <!-- <el-input v-if="scope.row.Status==0" v-model="scope.row.AvailableStock" required /> -->
            <span>{{ scope.row.AvailableStock }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'数量'" width="200" align="center">
          <template slot-scope="scope">
            <el-input v-if="scope.row.Status==0" v-model="scope.row.Quantity" clearable />
            <span v-else>{{ scope.row.Quantity }}</span>
          </template>
        </el-table-column>
      </el-table>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData">确认</el-button>
        <el-button v-else type="primary" @click="updateData">确认</el-button>
      </div>
    </el-dialog>
    <!-- 手动拣选 -->
    <el-dialog v-el-drag-dialog :title="'手动拣选'+Out.Code" :visible.sync="dialogFormLabel" :width="'85%'" :close-on-click-modal="false">
      <el-row :gutter="10">
        <el-col :span="8">
          <el-card>
            <el-table
              :key="TableKey"
              v-loading="false"
              :data="outMaterialCombineList"
              :header-cell-style="{background:'#F5F7FA'}"
              border
              fit
              size="mini"
              highlight-current-row
              style="min-height:100%;"
              height="450"
              @row-click="handleMaterialRowClick"
            >
              <el-table-column type="index" width="50" />
              <el-table-column :label="'物料描述'" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.MaterialName }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'数量'" width="80" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.Quantity }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'已拣'" width="80" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.PickedQuantity }}</span>
                </template>
              </el-table-column>
            </el-table>
          </el-card>
        </el-col>
        <el-col :span="16">
          <el-card>
            <el-table
              :key="TableKey"
              v-loading="false"
              :data="StockList"
              :header-cell-style="{background:'#F5F7FA'}"
              border
              fit
              :height="400"
              size="mini"
              highlight-current-row
              style="width:100%;min-height:100%;"
            >
              <el-table-column type="index" width="50" />
              <el-table-column
                label=""
                width="55"
              >
                <template slot-scope="scope">
                  <el-checkbox :ref="scope.row.Id" v-model="scope.row.Checked" :disabled="scope.row.IsCheckLocked" @change="doStockCheck(scope.row)" />
                </template>
              </el-table-column>
              <el-table-column :label="'物料条码'" width="150" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.MaterialLabel }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'数量'" width="80" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.Quantity }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'拣选数量'" width="120" align="center">
                <template slot-scope="scope">
                  <el-input v-if="scope.row.Checked==false" v-model="scope.row.PickedQuantity" style="width:80px" class="edit-input" size="mini" />
                  <span v-else>{{ scope.row.PickedQuantity }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'锁定数量'" width="80" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.LockedQuantity }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'盘点锁定'" width="80" align="center">
                <template slot-scope="scope">
                  <el-switch
                    v-model="scope.row.IsCheckLocked"
                    active-color="#ff4949"
                    inactive-color="#13ce66"
                    :disabled="true"
                  />
                </template>
              </el-table-column>
              <el-table-column :label="'单位'" width="50" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.MaterialUnit }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'仓库名称'" align="center" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ scope.row.WareHouseName }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'库位地址'" width="100" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.LocationCode }}</span>
                </template>
              </el-table-column>
            </el-table>
            <!-- 分页 -->
            <div class="pagination-container">
              <el-pagination :current-page="listStockQuery.Page" :page-sizes="[10,20,30, 50]" :page-size="listStockQuery.Rows" :total="stockTotal" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleStockSizeChange" @current-change="handleStockCurrentChange" />
            </div>
          </el-card>
        </el-col>
      </el-row>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormLabel = false">取消</el-button>
        <el-button type="primary" @click="confirmPicked">确认</el-button>
      </div>
    </el-dialog>
    <!-- 复核 -->
    <el-dialog v-el-drag-dialog :title="'复核'+Out.Code" :visible.sync="dialogFormCheck" :width="'85%'" :close-on-click-modal="false">
      <el-row :gutter="10">
        <el-col :span="12">
          <!-- 筛选栏 -->
          <el-card class="search-card">
            <div class="filter-container">
              <el-input ref="materialLabel" v-model="searchMaterialLabel" placeholder="物料条码" :autofocus="true" class="filter-item" @keyup.enter.native="GetWaitingCheckingLabel" />
              <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="GetWaitingCheckingLabel">查询</el-button>
            </div>
          </el-card>
          <el-card>
            <el-table
              :key="TableKey"
              v-loading="false"
              :data="WaitingForCheckLabelList"
              :header-cell-style="{background:'#F5F7FA'}"
              border
              size="mini"
              fit
              highlight-current-row
              style="width:100%;min-height:100%;"
              height="400"
            >
              <el-table-column type="index" width="50" />
              <el-table-column :label="'物料条码'" width="160" align="center">
                <template slot-scope="scope">
                  <el-tooltip class="item" effect="dark" :content="scope.row.MaterialName " placement="top">
                    <span>{{ scope.row.MaterialLabel }}</span>
                  </el-tooltip>
                </template>
              </el-table-column>
              <el-table-column :label="'应拣数量'" width="90" align="center">
                <template slot-scope="scope">
                  <el-tooltip class="item" effect="dark" :content="scope.row.MaterialCode " placement="top">
                    <span>{{ scope.row.Quantity }}</span>
                  </el-tooltip>
                </template>
              </el-table-column>
              <el-table-column :label="'拣选数量'" width="90" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.RealPickedQuantity }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'批次'" width="120" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.BatchCode }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'操作'" align="center" width="80" class-name="small-padding fixed-width" fixed="right">
                <template slot-scope="scope">
                  <el-button size="mini" type="info" @click="handConfirmCheck(scope.row)">确认</el-button>
                </template>
              </el-table-column>
            </el-table>
          </el-card>
        </el-col>
        <el-col :span="12">
          <el-card>
            <el-table
              :key="TableKey"
              v-loading="false"
              :data="CheckedLabelList"
              :header-cell-style="{background:'#F5F7FA'}"
              border
              fit
              size="mini"
              highlight-current-row
              style="width:100%;min-height:100%;"
              height="500"
            >
              <el-table-column type="index" width="50" />
              <el-table-column :label="'物料名称'" align="center">
                <template slot-scope="scope">
                  <el-tooltip class="item" effect="dark" :content="scope.row.MaterialCode " placement="top">
                    <span>{{ scope.row.MaterialName }}</span>
                  </el-tooltip>
                </template>
              </el-table-column>
              <el-table-column :label="'物料条码'" width="160" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.MaterialLabel }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'应拣数量'" width="90" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.Quantity }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'拣选数量'" width="90" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.RealPickedQuantity }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'库位地址'" width="120" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.LocationCode }}</span>
                </template>
              </el-table-column>
            </el-table>
          </el-card>
        </el-col>
      </el-row>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormCheck = false">取消</el-button>
      </div>
    </el-dialog>
    <!-- 退料 -->
    <el-dialog v-el-drag-dialog title="料仓退料" :visible.sync="dialogOutMaterialVisible" :width="'50%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="Out" class="dialog-form" label-width="100px" label-position="left">
        <el-form-item :label="'条码'">
          <span>
            <el-input v-model="Label.Code" class="dialog-input" />
          </span>
          <span>
            <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleCheckLabel" />
          </span>
        </el-form-item>
        <div style="font-size:14px;">
          <div style="margin:20px">
            <span>
              条码数量:
            </span>
            <span>{{ Label.Quantity }}</span>
          </div>
          <div style="margin:20px">
            <span>
              当前库位:
            </span>
            <span>{{ Label.LocationCode }}</span>
          </div>
          <div style="margin:20px">
            <span>
              物料编码:
            </span>
            <span>{{ Label.MaterialCode }}</span>
          </div>
          <div style="margin:20px">
            <span>
              物料名称:
            </span>
            <span>{{ Label.MaterialName }}</span>
          </div>
          <div style="margin:10px">
            <span>
              供应商编码:
            </span>
            <span>{{ Label.SupplyCode }}</span>
          </div>
          <div style="margin:20px">
            <span>
              供应商名称:
            </span>
            <span>{{ Label.SupplyName }}</span>
          </div>
          <div style="margin:20px">
            <span>
              批次:
            </span>
            <span>{{ Label.Batchcode }}</span>
          </div>
          <div style="margin:20px">
            <span>
              生产日期:
            </span>
            <span>{{ Label.ProductionDateFormat }}</span>
          </div>
        </div>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button type="primary" @click="handleAddLabel()">退料</el-button>
        <el-button type="primary" @click="createLabelData()">完成</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import { getInterfaceOut, cancelOut, ouLoadOutInfo, confirmCheckLabel, getWaiitingForCheckOrCheckedLabel, confirmHandPicked, GetStockPageRecords, GetCombineOutMaterialList, HandleGenerateOutLabel, HandleSendPickOrder, getAvailableStock, getOutDictTypeList, getOutList, GetOutMaterialList, getMaterialList, createOut, getWarehouseList, deleteOut, editOut, getEditMaterialList } from '@/api/PickManage/PickOrder'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import { getStockLabel } from '@/api/stock'
import { createOutTask } from '@/api/PickManage/PicktTask'
// import VueBarcode from 'vue-barcode'

export default {
  name: 'PickOrder',
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {
      // 分页显示总查询数据
      total: null,
      listLoading: false,
      fullscreenLoading: false,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 6,
        Code: '',
        Status: undefined,
        Sort: 'id'
      },
      // 分页查询
      listStockQuery: {
        Page: 1,
        Rows: 10,
        MaterialCode: '',
        Status: undefined,
        Sort: 'id',
        MaterialName: '',
        BatchCode: '',
        WareHouseCode: ''
      },
      dictionaryList: [],
      downloadLoading: false,
      TableKey: 0,
      list: null,
      listMaterial: null,
      addMaterial: [],
      dialogFormVisible: false,
      dialogFormLabel: false,
      dialogStatus: '',
      materialList: [],
      editMaterialList: [],
      loading: false,
      addMaterialGridCurrentRowIndex: undefined,
      textMap: {
        update: '编辑出库单',
        create: '创建出库单'
      },
      count: 0,
      Out: {
        Code: '',
        BillCode: '',
        WareHouseCode: '',
        InDict: undefined,
        Remark: '',
        AddMaterial: [],
        Status: undefined,
        PickedStockList: []

      },
      OutMaterial: {
        MaterialCode: '',
        MaterialName: '',
        BatchCode: '',
        Quantity: 0,
        Id: 0,
        ItemNo: ''
      },
      // 输入规则
      rules: {
        Code: [{ required: true, message: '请输入出库单号', trigger: 'blur' }],
        WareHouseCode: [{ required: true, message: '请选择仓库', trigger: 'blur' }]
        //  timestamp: [{ type: 'date', required: true, message: 'timestamp is required', trigger: 'change' }],
      },
      WareHouseList: [],
      outMaterialLabelList: [],
      outMaterialCombineList: [],
      StockList: null,
      LabelArray: [],
      stockTotal: 0,
      checkedStockArray: [],
      currentPickMaterial: undefined,
      dialogFormCheck: false,
      WaitingForCheckLabelList: [],
      CheckedLabelList: [],
      searchMaterialLabel: '',
      // 料仓退料
      dialogOutMaterialVisible: false,
      Label: {
        Code: null,
        SupplyCode: null,
        MaterialCode: null,
        ProductionDate: null,
        Batchcode: null
      },
      addMaterialLabel: [],
      statusList: [
        {
          Code: undefined, Name: '全部'
        },
        {
          Code: 0, Name: '待下发'
        },
        {
          Code: 1, Name: '部分下发'
        },
        {
          Code: 2, Name: '全部下发'
        },
        {
          Code: 3, Name: '已完成'
        },
        {
          Code: 4, Name: '已作废'
        }
        // {
        //   Code: 5, Name: '已下架'
        // },
        // {
        //   Code: 6, Name: '已复核'
        // }
      ]
    }
  },
  watch: {
    // 授权面板关闭，清空原角色查询权限
    dialogTreeVisible(value) {
      if (!value) {
        this.getList()
      }
    }
  },
  created() {
    this.getList()
    this.GetWareHouseList()
    this.getOutDictTypeList()
    //   this.timer()
  },
  // mounted() {
  //   this.timer = setInterval(() => {
  //     this.handleInterfaceCreate();
  //   }, 10000);
  // },
  // deactivated() {
  //   clearInterval(this.timer);
  // },
  // beforeDestroy() {
  //   clearInterval(this.timer);
  // },
  destroyed() {
    if (this.timer) {
      clearInterval(this.timer)
    }
  },
  methods: {
    timer() {
      return setInterval(() => {
        this.getList()
      }, 10000)
    },
    getOutDictTypeList() {
      getOutDictTypeList('OutType').then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.dictionaryList = usersData
      })
    },
    getList() {
      this.listLoading = true
      getOutList(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.total = usersData.total
        if (this.list.length > 0) {
          this.handleRowClick(this.list[0])
        }
        // Just to simulate the time of the request
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    getStockList() {
      GetStockPageRecords(this.listStockQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.stockTotal = usersData.total
        this.StockList = usersData.rows.map(v => {
          this.checkedStockArray.forEach(element => {
            if (element.Id === v.Id) {
              v.Checked = true
              v.PickedQuantity = element.PickedQuantity
            }
          })
          return v
        })
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

    // 数据筛选
    handleStockFilter() {
      this.listStockQuery.Page = 1
      this.getStockList()
    },
    // 切换分页数据-行数据
    handleStockSizeChange(val) {
      this.listStockQuery.Rows = val
      this.getStockList()
    },
    // 切换分页-列数据
    handleStockCurrentChange(val) {
      this.listStockQuery.Page = val
      this.getStockList()
    },
    // 生成出库任务
    handleCreateTask(row) {
      this.fullscreenLoading = true
      createOutTask(row).then(response => {
        var resData = JSON.parse(response.data.Content)
        if (resData.Success) {
          this.getList()
          this.handleRowClick(row)
        } else {
          this.$message({
            title: '失败',
            message: '创建失败：' + resData.Message,
            type: 'error',
            duration: 5000
          })
        }
        this.fullscreenLoading = false
      })
    },
    // 同步
    handleInterfaceCreate() {
      getInterfaceOut().then((res) => {
        var resData = JSON.parse(res.data.Content)
        console.log(resData)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.handleRowClick(this.Out)
          this.$message({
            title: '成功',
            message: resData.Message,
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '创建失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handleCreate() {
      this.restOut()
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createData() {
      if (this.addMaterial.length <= 0) {
        this.$message({
          title: '失败',
          message: '请添加物料明细',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          this.Out.AddMaterial = []
          this.addMaterial.forEach(element => {
            this.Out.AddMaterial.push(element)
          })
          // 验证物料行项目数据
          if (!this.checkItem(this.Out.AddMaterial[this.Out.AddMaterial.length - 1])) {
            return
          }
          createOut(this.Out).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.getList()
              this.$message({
                title: '成功',
                message: '创建成功',
                type: 'success',
                duration: 2000
              })
              this.handleRowClick(this.Out)
            } else {
              this.$message({
                title: '失败',
                message: '创建失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    handleCancel(row) {
      this.$confirm('此操作将作废该出库单, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.In = Object.assign({}, row) // copy obj
        this.cancelData(this.In)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消作废'
        })
      })
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    cancelData(row) {
      cancelOut(row).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.handleRowClick(row)
          this.$message({
            title: '成功',
            message: '出库单作废成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '出库单作废失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handlePrint(row) {

    },
    handleSendPickOrder(row) {
      this.Out = Object.assign({}, row) // copy obj
      if (this.Out.Status !== 1) {
        this.$message({
          title: '失败',
          message: '出库单' + this.Out.Code + '状态:[' + row.StatusCaption + ']不为待发送',
          type: 'error',
          duration: 2000
        })
        return
      }
      HandleSendPickOrder(this.Out).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.$message({
            title: '成功',
            message: '亮灯任务发送成功',
            type: 'success',
            duration: 2000
          })
          this.getList()
        } else {
          this.$message({
            title: '成功',
            message: '亮灯任务发送失败' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handleShelfDown(row) {
      this.Out = Object.assign({}, row) // copy obj
      if (this.Out.Status !== 0 && this.Out.Status !== 4) {
        this.$message({
          title: '失败',
          message: '出库单' + this.Out.Code + '执行中或已完成',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.outMaterialCombineList.length > 0) {
        this.dialogFormLabel = true
        this.checkedStockArray = []
        this.StockList = null
      }
    },
    handleGenerateOutLabel(row) {
      this.Out = Object.assign({}, row) // copy obj
      if (this.Out.Status !== 0) {
        this.$message({
          title: '失败',
          message: '出库单' + this.Out.Code + '执行中或已完成',
          type: 'error',
          duration: 2000
        })
        return
      }
      HandleGenerateOutLabel(this.Out).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.$message({
            title: '成功',
            message: '出库条码生成成功',
            type: 'success',
            duration: 2000
          })
          this.getList()
        } else {
          this.$message({
            title: '成功',
            message: '出库条码生成失败:' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    // 物料编辑
    handleUpdate(row) {
      this.Out = Object.assign({}, row) // copy obj
      if (this.Out.Status !== 0) {
        this.$message({
          title: '失败',
          message: '出库单' + this.Out.Code + '执行中或已完成',
          type: 'error',
          duration: 2000
        })
        return
      }
      GetOutMaterialList(row.Code).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.addMaterial = usersData
      })
      getEditMaterialList(row.Code).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.editMaterialList = usersData
        this.materialList = []
        this.editMaterialList.forEach(element => {
          this.materialList.push(element)
        })
      })
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          this.Out.AddMaterial = []
          this.addMaterial.forEach(element => {
            this.Out.AddMaterial.push(element)
          })
          const outData = Object.assign({}, this.Out)
          editOut(outData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.getList()
              this.$message({
                title: '成功',
                message: '更新成功',
                type: 'success',
                duration: 2000
              })
            } else {
              this.$message({
                title: '失败',
                message: '创建失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    handleDelete(row) {
      this.$confirm('此操作将永久删除该出库单, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.Out = Object.assign({}, row) // copy obj
        this.deleteData(this.Out)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        })
      })
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    deleteData(data) {
      deleteOut(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.$message({
            title: '成功',
            message: '删除成功',
            type: 'success',
            duration: 2000
          })
          this.getList()
        } else {
          this.$message({
            title: '成功',
            message: '删除失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handleRowClick(row, column, event) {
      GetOutMaterialList(row.Code).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.listMaterial = usersData
      })
      GetCombineOutMaterialList(row.Code).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.outMaterialCombineList = usersData
      })
    },
    handleMaterialRowClick(row, column, event) {
      this.listStockQuery.MaterialCode = row.MaterialCode
      this.listStockQuery.WareHouseCode = row.WareHouseCode
      this.getStockList()
      this.currentPickMaterial = row
    },
    restOut() {
      this.Out = {
        Code: '',
        BillCode: '',
        WareHouseCode: '',
        InDict: undefined,
        Remark: '',
        AddMaterial: [],
        Status: undefined,
        PickedStockList: []
      }
      this.addMaterial = []
    },
    handleCreateOutMaterial() {
      // 核查可用库存需要选择仓库
      if (this.Out.WareHouseCode === '') {
        this.$message({
          title: '提示',
          message: '请选择仓库',
          type: 'warning',
          duration: 2000
        })
        return
      }
      if (i > 1) {
        var item = this.addMaterial[this.addMaterial.length - 1]
        if (!this.checkItem(item)) {
          return
        }
      }
      var j = this.addMaterial.length + 1
      var i = this.addMaterial.length + 1
      var m = 0
      if (this.addMaterial.length > 0) {
        this.addMaterial.forEach(element => {
          m = m + 1
          if (m === this.addMaterial.length) {
            const no = parseInt(element.ItemNo)
            j = (no + 1).toString()
          }
        })
      }
      const outMaterial = {
        MaterialCode: '',
        MaterialName: '',
        BatchCode: '',
        Quantity: 0,
        MaterialLabel: '',
        Id: i,
        Status: 0,
        ItemNo: j.toString().padStart(6, '0'),
        AvailableStock: 0
      }
      this.addMaterial.push(outMaterial)
    },
    remoteMethod(query) {
      if (query !== '') {
        this.loading = true
        getMaterialList(query).then((response) => {
          var data = JSON.parse(response.data.Content)
          this.materialList = data
        })
        setTimeout(() => {
          this.loading = false
        }, 200)
      } else {
        this.materialList = []
        this.editMaterialList.forEach(element => {
          this.materialList.push(element)
        })
      }
    },
    materialCodeChange(row) {
      this.addMaterial.find(e => e.MaterialCode === row.MaterialCode).MaterialTypeDescription = this.materialList.find(e => e.Code === row.MaterialCode).MaterialTypeDescription
      getAvailableStock(row.MaterialCode, this.Out.WareHouseCode).then((response) => {
        // var data = JSON.parse(response.data.Content)
        // this.$message({
        //   title: '成功',
        //   message: response.data,
        //   type: 'success',
        //   duration: 2000
        // })
        row.AvailableStock = response.data
      })
    },
    handleDeleteOutMaterial() {
      if (this.addMaterialGridCurrentRowIndex !== undefined) {
        this.addMaterial.splice(this.addMaterial.indexOf(this.addMaterialGridCurrentRowIndex), 1)
      }
    },
    addMaterialGridClick(row, column, event) {
      // this.$message({
      //   title: '成功',
      //   message: row.MaterialCode,
      //   type: 'success',
      //   duration: 2000
      // })
      // addMaterialGridCurrentRowIndex = row.
      this.addMaterialGridCurrentRowIndex = row
    },
    GetWareHouseList() {
      getWarehouseList().then(response => {
        var data = JSON.parse(response.data.Content)
        this.WareHouseList = data
      })
    },
    confirmPicked() {
      if (this.checkedStockArray.length <= 0) {
        this.$message({
          title: '失败',
          message: '未选择任何条码',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.Out.PickedStockList = []
      this.checkedStockArray.forEach(element => {
        this.Out.PickedStockList.push(element)
      })
      confirmHandPicked(this.Out).then(response => {
        var resData = JSON.parse(response.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.$message({
            title: '成功',
            message: '拣选成功',
            type: 'success',
            duration: 2000
          })
          this.getList()
          this.dialogFormLabel = false
        } else {
          this.$message({
            title: '成功',
            message: '拣选失败' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    doStockCheck(row) {
      if (!row.Checked) { // 未选中
        this.checkedStockArray.forEach(element => {
          if (element.MaterialCode === this.currentPickMaterial.MaterialCode) {
            this.currentPickMaterial.PickedQuantity = this.currentPickMaterial.PickedQuantity - element.PickedQuantity
          }
        })
        // this.checkedStockArray.forEach(element => {
        //   if (element.Id === row.Id) {
        //     this.checkedStockArray.slice(row)
        //   }
        // })
        for (var i = 0; i < this.checkedStockArray.length; i++) {
          if (this.checkedStockArray[i].Id === row.Id) {
            this.checkedStockArray.splice(i, 1)
            break
          }
        }
        row.Checked = false
      } else { // 选中
        // 1判断是否超出 checkedStockArray
        if (row.PickedQuantity !== undefined) {
          const quantity = parseFloat(row.PickedQuantity)
          if (isNaN(quantity) || quantity === 'Nan' || quantity === 0) {
            this.$message({
              message: '拣选数量必须为数值或者不为0',
              type: 'error',
              duration: 2000
            })
            row.PickedQuantity = 0
            row.Checked = false
            return
          }
          const needQuantity = this.currentPickMaterial.Quantity - this.currentPickMaterial.PickedQuantity
          if (quantity > needQuantity) {
            this.$message({
              message: '拣选数量必须为小于需求数量',
              type: 'error',
              duration: 2000
            })
            row.PickedQuantity = 0
            row.Checked = false
            return
          }
          if (row.Quantity - row.LockedQuantity - quantity < 0) {
            this.$message({
              message: '拣选数量应小于可用库存数量',
              type: 'error',
              duration: 2000
            })
            row.PickedQuantity = 0
            row.Checked = false
            return
          }
          this.currentPickMaterial.PickedQuantity = this.currentPickMaterial.PickedQuantity + quantity
          this.checkedStockArray.push(row)
        } else {
          this.$message({
            message: '请输入拣选数量',
            type: 'error',
            duration: 2000
          })
          row.Checked = false
          row.PickedQuantity = 0
        }
      }
    },
    handleCheck(row) {
      this.Out = Object.assign({}, row) // copy obj
      if (this.Out.Status === 0) {
        this.$message({
          title: '失败',
          message: '出库单' + this.Out.Code + '尚未开始拣货',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.Out.Status !== 5) {
        this.$message({
          title: '失败',
          message: '出库单' + this.Out.Code + '尚未完成拣货或已复核完毕',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.GetCheckedAndWaitingCheckingLabel()
      this.dialogFormCheck = true
    },
    GetCheckedAndWaitingCheckingLabel() {
      this.GetCheckedLabel()
      this.GetWaitingCheckingLabel()
    },
    GetWaitingCheckingLabel() {
      const lable = this.searchMaterialLabel
      this.searchMaterialLabel = ''
      // this.$refs.materialLabel.focus()
      // this.$message({
      //   title: '失败',
      //   message: '出库单' + this.$refs.materialLabel,
      //   type: 'error',
      //   duration: 2000
      // })
      var $label = this.$refs.materialLabel
      if ($label !== undefined) {
        $label.focus()
      }
      getWaiitingForCheckOrCheckedLabel(this.Out.Code, 5, lable).then(response => {
        var usersData = JSON.parse(response.data.Content)
        if (usersData.length === 0) {
          getWaiitingForCheckOrCheckedLabel(this.Out.Code, 5, '').then(res => {
            var xData = JSON.parse(res.data.Content)
            this.WaitingForCheckLabelList = xData.map(v => {
              v.CheckedQuantity = v.RealPickedQuantity
              return v
            })
          })
        } else {
          this.WaitingForCheckLabelList = usersData.map(v => {
            v.CheckedQuantity = v.RealPickedQuantity
            return v
          })
        }
        if (this.WaitingForCheckLabelList.length === 0) {
          this.dialogFormCheck = false
          this.getList()
          this.$message({
            title: '成功',
            message: '出库单' + this.Out.Code + '出库成功',
            type: 'success',
            duration: 2000
          })
        }
      })
    },
    GetCheckedLabel() {
      getWaiitingForCheckOrCheckedLabel(this.Out.Code, 6, this.searchMaterialLabel).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.CheckedLabelList = usersData
      })
    },
    // 复核确认
    handConfirmCheck(row) {
      if (row.CheckedQuantity !== undefined) {
        const quantity = parseFloat(row.CheckedQuantity)
        if (isNaN(quantity) || quantity === 'Nan' || quantity === 0) {
          this.$message({
            message: '复核数量必须为数值或者不为0',
            type: 'error',
            duration: 2000
          })
          return
        }

        if (quantity > row.RealPickedQuantity) {
          this.$message({
            message: '复核数量必须为小于或等于拣选数量',
            type: 'error',
            duration: 2000
          })
          return
        }
      } else {
        this.$message({
          message: '请确认复核数量',
          type: 'error',
          duration: 2000
        })
        return
      }
      confirmCheckLabel(row).then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.GetCheckedAndWaitingCheckingLabel()
          this.$message({
            title: '成功',
            message: '复核成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '复核失败' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    beforeUpload(file) {
      // console.log('beforeUpload')
      // console.log(file.type)
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
      console.log(JSON.stringify(form))
      // let formTwo = JSON.stringify(form)
      ouLoadOutInfo(form).then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
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
      // var url = window.PLATFROM_CONFIG.baseUrl + '/Assets/themes/Excel/出库单导入模版.xlsx'
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/Out/DoDownLoadTemp'
      window.open(url)
    },
    // 一步退料
    // 获取Lable信息
    handleCheckLabel() {
      getStockLabel(this.Label.Code).then(response => {
        var result = JSON.parse(response.data.Content)
        this.Label = result
      })
    },
    // 添加收货物料编码
    handleAddLabel() {
      // 出库物料行项目
      const i = this.addMaterialLabel.length + 1
      const outMaterialLabel = {
        MaterialCode: this.Label.MaterialCode,
        MaterialName: this.Label.MaterialName,
        BatchCode: this.Label.Batchcode,
        Quantity: this.Label.Quantity,
        MaterialLabel: this.Label.MaterialLabel,
        LocationCode: this.Label.LocationCode,
        Id: i,
        Status: 0
      }
      this.addMaterialLabel.push(outMaterialLabel)
      // 出库物料条码
      const stockLabel = {
        Id: this.Label.Id,
        MaterialCode: this.Label.MaterialCode,
        BatchCode: this.Label.Batchcode,
        LocationCode: this.Label.LocationCode,
        MaterialLabel: this.Label.MaterialLabel,
        Quantity: this.Label.Quantity,
        PickedQuantity: this.Label.Quantity
      }
      this.Out.PickedStockList.push(stockLabel)
      // 重置条码
      this.Label = {
        Code: null,
        SupplyCode: null,
        MaterialCode: null,
        ProductionDate: null,
        Batchcode: null
      }
    },
    createLabelData() {
      // 整合出库物料行项目，将相同物料编码数量合并成一个行项目
      this.Out.AddMaterial = []
      // 初始状态，必须要填写，否则无法进行一步下架操作
      this.Out.Status = 0
      this.addMaterialLabel.forEach(element => {
        if (this.Out.AddMaterial.length > 0) {
          this.Out.AddMaterial.forEach(material => {
            if (element.MaterialCode === material.MaterialCode) {
              material.Quantity = material.Quantity + element.Quantity
            } else {
              this.Out.AddMaterial.push(element)
            }
          })
        } else {
          this.Out.AddMaterial.push(element)
        }
      })
      // 料仓退料特殊流程
      this.Out.OutDict = 'GetRerurn'
      createOut(this.Out).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.Out.Code = resData.Data.Code
          // 拣选
          confirmHandPicked(this.Out).then(response => {
            var resData1 = JSON.parse(response.data.Content)
            if (resData1.Success) {
              this.addMaterialLabel.forEach(item => {
                // 复核
                item.OutCode = this.Out.Code
                item.CheckedQuantity = item.Quantity
                confirmCheckLabel(item).then(res => {
                  var resData2 = JSON.parse(res.data.Content)
                  if (resData2.Success) {
                    this.dialogOutMaterialVisible = false
                    this.$message({
                      title: '成功',
                      message: '复核成功',
                      type: 'success',
                      duration: 2000
                    })
                  } else {
                    this.$message({
                      title: '失败',
                      message: '复核失败' + resData.Message,
                      type: 'error',
                      duration: 2000
                    })
                  }
                })
              })
            } else {
              this.$message({
                title: '成功',
                message: '拣选失败' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        } else {
          this.$message({
            title: '失败',
            message: '创建失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    checkItem(item) {
      if (item.MaterialCode === '') {
        this.$message({
          title: '成功',
          message: '请选择物料',
          type: 'error',
          duration: 2000
        })
        return false
      }
      if (item.Quantity === 0) {
        this.$message({
          title: '成功',
          message: '请输入数量',
          type: 'error',
          duration: 2000
        })
        return false
      }
      return true
    },
    changeWarehouse() {
      this.addMaterial = []
    }
  }
}
</script>

