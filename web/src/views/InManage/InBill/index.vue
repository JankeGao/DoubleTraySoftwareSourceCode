<template>
  <div class="app-container">
    <el-card class="search-card">
      <div class="filter-container" style="margin-bottom:10px">
        <el-input
          v-model="listQuery.Code"
          placeholder="入库单号"
          class="filter-item"
          clearable
          @keyup.enter.native="handleFilter"
          @clear="handleFilter"
        />
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
        <el-button
          class="filter-button"
          type="primary"
          icon="el-icon-search"
          @click="handleFilter"
        >查询</el-button>
        <el-button
          class="filter-button"
          style="margin-left: 10px;"
          type="primary"
          icon="el-icon-edit"
          @click="handleCreate"
        >添加</el-button>
        <el-button
          class="filter-button"
          style="margin-left: 10px;"
          type="primary"
          icon="el-icon-edit"
          @click="handleInterfaceCreate"
        >同步</el-button>
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
            <i class="el-icon-upload" /> {{ $t('baseBtn.importBtn') }}
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
        size="mini"
        height="331"
        border
        fit
        highlight-current-row
        style="width:100%;min-height:100%;"
        @row-click="handleRowClick"
      >
        <el-table-column type="index" width="50" />
        <el-table-column :label="'状态'" width="110" align="center">
          <template slot-scope="scope">
            <el-tag v-if="scope.row.Status===0" size="mini" type="warning">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===1" size="mini" type="warning">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===2" size="mini">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===3" size="mini" type="success">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===4" size="mini" type="info">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'入库单号'" width="160" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'入库类型'" width="120" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.InDictDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'仓库'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.WareHouseCode }}-{{ scope.row.WareHouseName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'来源单据号'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BillCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'备注'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remark }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'添加信息'" width="120" align="center">
          <template slot-scope="scope">
            <div>{{ scope.row.CreatedUserName }}</div>
          </template>
        </el-table-column>
        <el-table-column :label="'添加时间'" width="160" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedTime }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="'操作'"
          align="center"
          width="260px"
          class-name="small-padding fixed-width"
          fixed="right"
        >
          <template slot-scope="scope">
            <!-- <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)" /> -->
            <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)" />
            <!-- <el-button size="mini" type="info" @click="handlePrint(scope.row)">单据</el-button> -->
            <el-button
              v-loading.fullscreen.lock="fullscreenLoading"
              size="mini"
              type="primary"
              @click="handleCreateTask(scope.row)"
            >下发</el-button>
            <el-button size="mini" type="danger" @click="handleCancel(scope.row)">作废</el-button>
            <!-- <el-button size="mini" type="primary" @click="handleSendReceipt(scope.row)">指引</el-button> -->
          </template>
        </el-table-column>
      </el-table>
      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          :current-page="listQuery.Page"
          :page-sizes="[6,12,18,24]"
          :page-size="listQuery.Rows"
          :total="total"
          background
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>
    <!-- 行项目 -->
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
        <el-table-column :label="'状态'" width="100" align="center">
          <template slot-scope="scope">
            <el-tag v-if="scope.row.Status===0" size="mini" type="warning">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===1" size="mini" type="warning">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===2" size="mini">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===3" size="mini" type="success">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===4" size="mini" type="info">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
          </template>
        </el-table-column>
        <!-- <el-table-column :label="'行号'" width="50" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ItemNo }}</span>
          </template>
        </el-table-column>-->
        <el-table-column :label="'物料编码'" width="200" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料描述'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'类别'" width="60" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialTypeDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'供应商名称'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.SupplierName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'供应商编码'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.SupplierCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'生产日期'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ManufactrueDate }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'到期日期'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ValidityDate }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'批次'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BatchCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'数量'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Quantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'已下发数量'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.SendInQuantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'已入库数量'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.RealInQuantity }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="'操作'"
          align="center"
          width="150px"
          class-name="small-padding fixed-width"
          fixed="right"
        >
          <template slot-scope="scope">
            <el-button
              size="mini"
              type="info"
              style="width:80px"
              @click="handleCreatLabel(scope.row)"
            >打印条码</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    <!-- 创建/编辑 弹出框 -->
    <el-dialog
      v-el-drag-dialog
      :title="textMap[dialogStatus]"
      :visible.sync="dialogFormVisible"
      :fullscreen="true"
      :close-on-click-modal="false"
    >
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="In"
        class="dialog-form"
        label-width="100px"
        label-position="left"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item :label="'仓库编码'" prop="WareHouseCode">
              <el-select
                v-model="In.WareHouseCode"
                :multiple="false"
                filterable
                style="width:300px"
              >
                <el-option
                  v-for="item in WareHouseList"
                  :key="item.Code"
                  :label="item.Name"
                  :value="item.Code"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="'来源单号'" prop="Name">
              <el-input
                v-model="In.BillCode"
                clearable
                class="dialog-input"
                placeholder="请输入来源单据号"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="'入库类型'" :prop="'LocationCode'">
              <el-select v-model="In.InDict" :multiple="false" reserve-keyword style="width:300px">
                <el-option
                  v-for="item in dictionaryList"
                  :key="item.Code"
                  :label="item.Name"
                  :value="item.Code"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="'备注信息'">
              <el-input
                v-model="In.Remark"
                :autosize="{ minRows: 1, maxRows: 1}"
                type="textarea"
                placeholder="入库备注"
                class="dialog-input"
              />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <div>
        <div style="margin-bottom:20px">
          <el-button
            class="filter-button"
            style="margin-left: 10px;"
            type="primary"
            icon="el-icon-edit"
            @click="handleCreateInMaterial"
          >添加</el-button>
          <el-button
            class="filter-button"
            style="margin-left: 10px;"
            type="danger"
            icon="el-icon-delete"
            @click="handleDeleteInMaterial"
          >移除</el-button>
        </div>
        <el-table
          :key="TableKey"
          ref="addMaterialGrid"
          v-loading="false"
          size="mini"
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
              <span v-else>{{ scope.row.MaterialName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料编码'" width="180" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料类别'" width="100" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialTypeDescription }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'供应商名称'" width="180" align="center">
            <template slot-scope="scope">
              <el-select
                v-if="scope.row.Status==0"
                v-model="scope.row.SupplierCode"
                :multiple="false"
                filterable
                remote
                reserve-keyword
                placeholder="请输入供应商名称"
                :remote-method="remoteSupplierMethod"
                :loading="loading"
              >
                <el-option
                  v-for="item in supplierList"
                  :key="item.Code"
                  :label="item.Name"
                  :value="item.Code"
                />
              </el-select>
              <span v-else>{{ scope.row.SupplierName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'供应商编码'" width="180" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.SupplierCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'批次'" width="150" align="center">
            <template slot-scope="scope">
              <el-input v-if="scope.row.Status==0" v-model="scope.row.BatchCode" clearable />
              <span v-else>{{ scope.row.BatchCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'数量'" width="80" align="center">
            <template slot-scope="scope">
              <el-input v-if="scope.row.Status==0" v-model="scope.row.Quantity" />
              <span v-else>{{ scope.row.Quantity }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'生产日期'" width="250" align="center">
            <template slot-scope="scope">
              <el-date-picker
                v-if="scope.row.Status==0"
                v-model="scope.row.ManufactrueDate"
                type="date"
                placeholder="选择日期"
              />
              <span v-else>{{ scope.row.ManufactrueDate }}</span>
            </template>
          </el-table-column>
        </el-table>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData">确认</el-button>
        <el-button v-else type="primary" @click="updateData">确认</el-button>
      </div>
    </el-dialog>
    <!-- 打印条码 -->
    <el-dialog
      v-el-drag-dialog
      title="打印条码"
      :visible.sync="dialogBarCodeVisible"
      :fullscreen="true"
      :close-on-click-modal="false"
    >
      <el-row :gutter="20">
        <el-col :span="8">
          <el-form
            ref="dataLabelForm"
            :rules="labelrules"
            :model="InMaterial"
            class="dialog-form"
            label-width="120px"
            label-position="left"
          >
            <el-form-item :label="'物料编码'">
              <el-input v-model="InMaterial.MaterialCode" class="dialog-input" disabled />
            </el-form-item>
            <el-form-item :label="'物料名称'">
              <el-input v-model="InMaterial.MaterialName" class="dialog-input" disabled />
            </el-form-item>
            <el-form-item :label="'供应商编码'">
              <span>
                <el-input v-model="InMaterial.SupplierCode" class="dialog-input" disabled />
              </span>
            </el-form-item>
            <el-form-item :label="'供应商名称'">
              <el-input v-model="InMaterial.SupplierName" class="dialog-input" disabled />
            </el-form-item>
            <el-form-item :label="'批次'">
              <el-input v-model="InMaterial.BatchCode" class="dialog-input" disabled />
            </el-form-item>
            <el-form-item :label="'入库总数量'">
              <el-input v-model="InMaterial.Quantity" class="dialog-input" disabled />
            </el-form-item>
            <el-form-item :label="'到期日期'">
              <el-date-picker
                v-model="InMaterial.ValidityDate"
                type="date"
                placeholder="选择日期"
                disabled
              />
            </el-form-item>
            <el-form-item :label="'生产日期'">
              <el-date-picker
                v-model="InMaterial.ManufactrueDate"
                type="date"
                placeholder="选择日期"
                disabled
              />
            </el-form-item>
            <el-form-item :label="'最小单包数量'">
              <el-input v-model="InMaterial.PackageQuantity" class="dialog-input" disabled />
            </el-form-item>
            <el-form-item :label="'条码数量'" prop="PackageQuantity">
              <el-input
                v-model="PackageQuantity"
                class="dialog-input"
                :disabled="InMaterial.MaterialType===1"
              />
              <div v-if="InMaterial.MaterialType ===1">每个模具生成唯一码,数量为1，进行模具生命周期管理</div>
            </el-form-item>
            <el-form-item :label="'生成个数'" prop="Unit">
              <el-input v-model="LabelCount" class="dialog-input" />
            </el-form-item>
          </el-form>
          <div style="margin-left:20px">
            <el-button @click="dialogBarCodeVisible = false">取消</el-button>
            <el-button
              v-loading.fullscreen.lock="fullscreenLoading"
              type="primary"
              element-loading-text="正在生成条码信息，请稍等~"
              @click="handleCreateLabel() "
            >生成</el-button>
          </div>
        </el-col>
        <el-col :span="16">
          <el-card class="box-card">
            <div slot="header" class="clearfix">
              <span>历史条码生成记录</span>
              <span style="float:right">
                <el-select
                  v-model="OptionObject.Name"
                  :multiple="false"
                  filterable
                  placeholder="选择其他打印机"
                  style="width:300px"
                >
                  <el-option v-for="item in OptionList" :key="item" :label="item" :value="item" />
                </el-select>
              </span>
            </div>
            <el-table
              :key="TableKey"
              v-loading="listLoading"
              :data="labelList"
              :header-cell-style="{background:'#F5F7FA'}"
              height="500"
              border
              fit
              size="mini"
              highlight-current-row
              style="width:100%;min-height:100%;"
            >
              <el-table-column type="index" width="50" />
              <el-table-column :label="'条码编码'" width="160" align="center" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ scope.row.Code }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'物料名称'" show-overflow-tooltip align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.MaterialName }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'数量'" show-overflow-tooltip align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.Quantity }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'供应商名称'" show-overflow-tooltip width="150" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.SupplyName }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'批次'" width="120" align="center" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ scope.row.BatchCode }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'生产日期'" width="120" align="center" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ scope.row.ManufactrueDateFormat }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'创建人'" width="120" align="center" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ scope.row.CreatedUserName }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'创建时间'" width="180" align="center" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ scope.row.CreatedTime }}</span>
                </template>
              </el-table-column>
              <el-table-column
                :label="'操作'"
                align="center"
                width="80"
                class-name="small-padding fixed-width"
                fixed="right"
              >
                <template slot-scope="scope">
                  <el-button size="mini" type="primary" @click="handlePrintCode(scope.row)">打印</el-button>
                </template>
              </el-table-column>
            </el-table>
            <div />
          </el-card>
          <!-- 分页 -->
          <!-- <div class="pagination-container">
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
          </div>-->
        </el-col>
      </el-row>
    </el-dialog>
    <!-- 单据打印 -->
    <div style="visibility:hidden;height:250px; position: absolute;right:5px;top: 5px">
      <el-button id="printBtn" v-print="'#printMe'" type="text">打印</el-button>
      <span id="printMe" style="width:188px">
        <el-row :gutter="20">
          <el-col :span="17">
            <div style="font-size:20px;margin-bottom:20px">入库单明细</div>
            <div style="font-size:12px;">
              <span>制单日期：{{ printDate }}</span>
              <span style="margin-left:20px">{{ PrintCode.InDictDescription }}</span>
              <span>{{ PrintCode.BillCode }}</span>
            </div>
          </el-col>
          <el-col :span="7">
            <barcode
              :value="PrintCode.Code"
              height="20"
              font="12"
              width="1px"
              margin="0px"
            >Show this if the rendering fails.</barcode>
          </el-col>
        </el-row>
        <hr style="margin:10px 0px" />
        <div style="font-size:12px">
          <div style="font-size:16px;margin:20px 0px">
            <el-row>
              <el-col :span="10">
                <span>名称</span>
              </el-col>
              <el-col :span="2">
                <span>数量</span>
              </el-col>
              <el-col :span="4">
                <span>批次</span>
              </el-col>
              <el-col :span="8">
                <span>物料编码</span>
              </el-col>
            </el-row>
          </div>
          <div v-for="item in tempPrintlist" :key="item.Id" style="margin-bottom:30px">
            <el-row>
              <el-col :span="10" style="margin-top:15px">
                <span>{{ item.MaterialName }}</span>
              </el-col>
              <el-col :span="2" style="margin-top:15px">
                <span>{{ item.Quantity }}</span>
              </el-col>
              <el-col :span="4" style="margin-top:15px">
                <span>{{ item.BatchCode }}</span>
              </el-col>
              <el-col :span="8">
                <barcode
                  :value="item.MaterialCode"
                  height="20px"
                  width="2px"
                  font="8"
                  display-value="false"
                >Show this if the rendering fails.</barcode>
              </el-col>
            </el-row>
          </div>
        </div>
        <div style="font-size:8px">
          <div>入库单信息</div>
        </div>
      </span>
    </div>
  </div>
</template>
<script>
import { getInterfaceIn, cancelIn, handleSendReceiptOrder, ouLoadInInfo, getInDictTypeList, getInList, GetInMaterialList, getMaterialList, createIn, getWarehouseList, deleteIn, editIn, getEditMaterialList } from '@/api/ReceiptManage/ReceiptBill'
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import VueBarcode from 'vue-barcode'
// import QRCode from 'qrcode'
import getLodop from '@/utils/LodopFuncs.js' // 引入附件的js文件
import { isFloat } from '@/utils/validate.js'
import PrintToLodop from '@/utils/PrintToLodop.js' // 引入附件的js文件
import { getLabelByCode, queryHistoryLabel, createBatchLabel } from '@/api/Label'
import { getSupplierList } from '@/api/Supply'
// import { createLabel } from '@/api/Label'
import { createInTask } from '@/api/ReceiptManage/ReceiptTask'

export default {
  name: 'InBill', // 入库单据
  components: { 'barcode': VueBarcode },
  directives: {
    elDragDialog
  },
  data() {
    var validateBarcode = (rule, value, callback) => {
      if (this.Label.Code === '' || this.Label.Code === null) {
        callback(new Error('请扫描条码'))
      } else {
        //  this.handleCheckLabel()
        callback()
      }
    }

    var validateFloat = (rule, value, callback) => {
      if (!isFloat(this.InMaterial.Quantity)) {
        callback(new Error('请输入正确的数字'))
        return
      } else {
        callback()
      }
    }

    const validateLabelNum = (rule, value, callback) => {
      if (this.PackageQuantity < 0) {
        callback(new Error('请输入正确的单包条码数量'))
      } else {
        callback()
      }
    }
    return {
      isDisable: false,
      rowData: null,
      // 分页显示总查询数据
      barcodeValue: 'test',
      total: null,
      listLoading: false,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 6,
        Code: '',
        Status: undefined,
        Sort: 'id'
      },
      OptionList: [],
      OptionObject: {
        Name: '',
        value: 0
      },
      dictionaryList: [],
      downloadLoading: false,
      dialogPrinterVisible: false,
      TableKey: 0,
      list: null,
      listMaterial: null,
      addMaterial: [],
      dialogFormVisible: false,
      printerList: [],
      dialogStatus: '',
      materialList: [],
      editMaterialList: [],
      loading: false,
      addMaterialGridCurrentRowIndex: undefined,
      textMap: {
        update: '编辑入库单',
        create: '创建入库单'
      },
      In: {
        Code: '',
        BillCode: '',
        WareHouseCode: '',
        InDict: undefined,
        Remark: '',
        AddMaterial: [],
        Status: undefined
      },
      InMaterial: {
        MaterialCode: '',
        SendInQuantity: 0,
        MaterialName: '',
        SupplierCode: '',
        SupplierName: '',
        BatchCode: '',
        Quantity: 0,
        MaterialLabel: '',
        LocationCode: '',
        Id: 0
      },
      PackageQuantity: 0,
      LabelCount: 0,
      PrintCode: '',
      controls: [],
      // 输入规则
      rules: {
        Code: [{ required: true, message: '请输入入库单号', trigger: 'blur' }],
        WareHouseCode: [{ required: true, message: '请选择仓库', trigger: 'blur' }],
        InputBarcode: [{ required: true, validator: validateBarcode, trigger: 'blur' }],
        InputNumber: [{ require: true, validator: validateFloat, trigger: 'blur' }]

      },
      // 输入规则
      labelrules: {
        PackageQuantity: [{ required: true, message: '请输入单包数量', trigger: 'change', validator: validateLabelNum }]
      },
      labelList: [],
      Label: {
        Code: null,
        BoxCode: null,
        MaterialCode: null,
        ProductionDate: null,
        Batchcode: null,
        Quantity: 0
      },
      fullscreenLoading: false,
      WareHouseList: [],
      // 条码打印
      tempPrintlist: [],
      // 打印时间
      printDate: null,
      page: {
        width: 520,
        height: 350,
        pagetype: '',
        intOrient: 1
      },
      barCode: '',
      // 料仓收料
      dialogInMaterialVisible: false,
      supplierList: [],
      editSupplierList: [],
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
      ],
      dialogBarCodeVisible: false
    }
  },
  watch: {
    // 创建面板关闭，清空数据
    dialogInMaterialVisible(value) {
      if (!value) {
        this.resstIn()
      }
    },
    // 创建面板关闭，清空数据
    dialogFormVisible(value) {
      if (!value) {
        this.resstIn()
      }
    }
  },
  created() {
    this.getList()
    this.GetWareHouseList()
    this.getInDictTypeList()
    // this.timer()
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
    // 点击打印条码
    handleCreatLabel(row) {
      var LODOP = getLodop()
      var iPrinterCount = LODOP.GET_PRINTER_COUNT()
      for (var i = 0; i < iPrinterCount; i++) {
        this.OptionList[i] = LODOP.GET_PRINTER_NAME(i)
      }

      this.InMaterial = Object.assign({}, row) // copy obj
      if (this.InMaterial.MaterialType === 1) {
        this.PackageQuantity = 1
      } else {
        this.PackageQuantity = this.InMaterial.PackageQuantity
      }
      this.Label.MaterialCode = this.InMaterial.MaterialCode
      this.Label.SupplierCode = this.InMaterial.SupplierCode
      this.Label.BatchCode = this.InMaterial.BatchCode
      this.Label.ManufactrueDate = this.InMaterial.ManufactrueDate
      this.getHistoryLabel()
      if (this.PackageQuantity !== 0) {
        this.LabelCount = Math.ceil(this.InMaterial.Quantity / this.PackageQuantity)
      }
      this.dialogBarCodeVisible = true
    },
    getHistoryLabel() {
      queryHistoryLabel(this.Label).then((res) => {
        var resData = JSON.parse(res.data.Content)
        this.labelList = resData
      })
    },
    // 生成条码信息
    handleCreateLabel() {
      if (!isFloat(this.PackageQuantity) || this.PackageQuantity === 0) {
        this.$message({
          title: '失败',
          message: '请输入正确格式的数量',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.LabelCount <= 0) {
        this.$message({
          title: '失败',
          message: '请输入正确的标签生成数量',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.InMaterial.Batchcode === null) {
        this.$message({
          title: '失败',
          message: '请输入正确的批次',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.$refs['dataLabelForm'].validate((valid) => {
        if (valid) {
          this.fullscreenLoading = true
          this.InMaterial.PackageQuantity = this.PackageQuantity
          this.InMaterial.LabelCount = this.LabelCount
          createBatchLabel(this.InMaterial).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.getHistoryLabel()
            } else {
              this.$message({
                title: '失败',
                message: '创建失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
          setTimeout(() => {
            this.fullscreenLoading = false
            this.dialogLabelVisible = true
          }, 1 * 2000)
        }
      })
    },
    getInDictTypeList() {
      getInDictTypeList('InType').then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.dictionaryList = usersData
      })
    },
    // 获取入库单信息
    getList() {
      this.listLoading = true
      getInList(this.listQuery).then(response => {
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
    // 同步
    handleInterfaceCreate() {
      getInterfaceIn().then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.handleRowClick(this.In)
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
    // 创建
    handleCreate() {
      this.resstIn()
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
      this.isDisable = true
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          this.In.AddMaterial = []
          this.addMaterial.forEach(element => {
            this.In.AddMaterial.push(element)
          })
          // 验证物料行项目数据
          if (!this.checkItem(this.In.AddMaterial[this.In.AddMaterial.length - 1])) {
            return
          }
          createIn(this.In).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.getList()
              this.handleRowClick(this.In)
              this.$message({
                title: '成功',
                message: '创建成功',
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
          setTimeout(() => {
            this.isDisable = false
          }, 2000)
        }
      })
    },
    // 生成入库任务
    handleCreateTask(row) {
      this.fullscreenLoading = true
      createInTask(row).then(response => {
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
    handleCancel(row) {
      this.$confirm('此操作将作废该入库单, 是否继续?', '提示', {
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
      cancelIn(row).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.handleRowClick(row)
          this.$message({
            title: '成功',
            message: '入库单作废成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '入库单作废失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    // 打印单据
    handlePrint(row) {
      this.PrintCode = row
      this.printDate = this.getNowFormatDate()
      GetInMaterialList(row.Code).then(response => {
        var resData = JSON.parse(response.data.Content)
        this.tempPrintlist = resData
        var btn = document.getElementById('printBtn')
        btn.click()
      })
    },
    // 生成二维码
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
    // 打印标签
    handlePrintCode(row) {
      if (this.OptionObject.Name === '') {
        this.$message({
          title: '失败',
          message: '请选择打印机',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (row === null) {
        this.$message({
          title: '失败',
          message: '请选择需要打印条码的入库单',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.$message({
        title: '成功',
        message: '打印任务已发送，请稍等',
        type: 'success',
        duration: 2000
      })
      const data = {}
      getLabelByCode(row.Code).then(response => {
        var resData = JSON.parse(response.data.Content)
        console.log('resData')
        console.log(resData)
        this.createBarCode(resData.Code)
        // 物料编码
        this.controls.push({
          id: 111,
          type: 'atext',
          data: {
            value: resData.MaterialCode,
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
              fontFamily: '宋体',
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
            value: resData.MaterialName,
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
              fontFamily: '宋体',
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
        // 物料条码数量
        this.controls.push({
          id: 111,
          type: 'atext',
          data: {
            value: '数量：',
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
              fontFamily: '宋体',
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
        // 物料条码数量
        this.controls.push({
          id: 113,
          type: 'atext',
          data: {
            value: resData.Quantity,
            width: 400,
            height: 20,
            x: 50,
            y: 50,
            itemtype: 0,
            databind: {
              id: '',
              text: ''
            },
            style: {
              color: '#000',
              fontFamily: '宋体',
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
        // 物料条码批次
        this.controls.push({
          id: 115,
          type: 'atext',
          data: {
            value: '批次：',
            width: 400,
            height: 20,
            x: 85,
            y: 50,
            itemtype: 0,
            databind: {
              id: '',
              text: ''
            },
            style: {
              color: '#000',
              fontFamily: '宋体',
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
        // 物料条码批次
        this.controls.push({
          id: 115,
          type: 'atext',
          data: {
            value: resData.BatchCode,
            width: 400,
            height: 20,
            x: 120,
            y: 50,
            itemtype: 0,
            databind: {
              id: '',
              text: ''
            },
            style: {
              color: '#000',
              fontFamily: '宋体',
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
        // if (this.OptionObject.Name !== '') {
        //   console.log(this.OptionObject.Name)
        //   printobj.definedPrint(this.OptionObject.Name)
        //   this.rowData = null
        //   return
        // }
        printobj.print()
        this.rowData = null
        // var LODOP = getLodop()
        // LODOP.PREVIEW()
        this.controls = []
      })
    },
    // 物料编辑
    handleUpdate(row) {
      this.In = Object.assign({}, row) // copy obj
      if (this.In.Status !== 0) {
        this.$message({
          title: '失败',
          message: '入库单' + this.In.Code + '状态' + this.In.StatusCaption + '不可编辑',
          type: 'error',
          duration: 2000
        })
        return
      }
      GetInMaterialList(row.Code).then(response => {
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
          this.In.AddMaterial = []
          this.addMaterial.forEach(element => {
            this.In.AddMaterial.push(element)
          })
          // 验证物料行项目数据
          if (!this.checkItem(this.In.AddMaterial[this.In.AddMaterial.length - 1])) {
            return
          }
          const inData = Object.assign({}, this.In)
          editIn(inData).then((res) => {
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
    /** ******** */
    /** 入库单删除功能 */
    /** ******** */
    handleDelete(row) {
      this.In = Object.assign({}, row) // copy obj
      if (this.In.Status !== 0) {
        console.log(this.In.Status)
        this.$message({
          title: '失败',
          message: '入库单' + this.In.Code + '状态' + row.StatusCaption + '不可删除',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.$confirm('此操作将永久删除该入库单, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.deleteData(this.In)
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
      deleteIn(data).then((res) => {
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
    // 选择一行
    handleRowClick(row) {
      GetInMaterialList(row.Code).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.listMaterial = usersData
      })
    },
    // 创建入库单行项目
    handleCreateInMaterial() {
      const i = this.addMaterial.length + 1
      if (i > 1) {
        var item = this.addMaterial[this.addMaterial.length - 1]
        if (!this.checkItem(item)) {
          return
        }
      }
      const inMaterial = {
        MaterialCode: '',
        MaterialName: '',
        BatchCode: '',
        Quantity: 0,
        MaterialLabel: '',
        Id: i,
        Status: 0,
        ManufactrueDate: '',
        MaterialTypeDescription: ''
      }
      this.addMaterial.push(inMaterial)
    },
    checkItem(item) {
      if (item.MaterialCode === '') {
        this.$message({
          title: '成功',
          message: '请选择物料',
          type: 'error',
          duration: 2000
        })
        this.isDisable = false
        return false
      }
      // if (item.BatchCode === '') {
      //   this.$message({
      //     title: '成功',
      //     message: '请输入批次',
      //     type: 'error',
      //     duration: 2000
      //   })
      //   this.isDisable = false
      //   return false
      // }
      if (item.Quantity === 0 || item.Quantity === '0' || item.Quantity === '0.0' || item.Quantity === '0.00') {
        this.$message({
          title: '成功',
          message: '请输入数量',
          type: 'error',
          duration: 2000
        })
        this.isDisable = false
        return false
      }
      if (item.ManufactrueDate === '') {
        this.$message({
          title: '成功',
          message: '请输入生产日期',
          type: 'error',
          duration: 2000
        })
        this.isDisable = false
        return false
      }
      if (!isFloat(item.Quantity)) {
        this.$message({
          title: '成功',
          message: '请输入正确的数字（包含两位小数的数字或者不包含小数的数字）',
          type: 'error',
          duration: 2000
        })
        this.isDisable = false
        return false
      }
      return true
    },
    remoteMethod(query) {
      if (query !== '') {
        this.loading = true
        getMaterialList(query).then((response) => {
          var data = JSON.parse(response.data.Content)
          console.log(data)
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
    remoteSupplierMethod(query) {
      if (query !== '') {
        this.loading = true
        getSupplierList(query).then((response) => {
          var data = JSON.parse(response.data.Content)
          this.supplierList = data
        })
        setTimeout(() => {
          this.loading = false
        }, 200)
      } else {
        this.supplierList = []
        this.editSupplierList.forEach(element => {
          this.supplierList.push(element)
        })
      }
    },
    handleDeleteInMaterial() {
      if (this.addMaterialGridCurrentRowIndex !== undefined) {
        this.addMaterial.splice(this.addMaterial.indexOf(this.addMaterialGridCurrentRowIndex), 1)
      }
    },
    materialCodeChange(row) {
      this.addMaterial.find(e => e.MaterialCode === row.MaterialCode).MaterialTypeDescription = this.materialList.find(e => e.Code === row.MaterialCode).MaterialTypeDescription
    },
    addMaterialGridClick(row, column, event) {
      this.addMaterialGridCurrentRowIndex = row
    },
    // 获取仓库列表
    GetWareHouseList() {
      getWarehouseList().then(response => {
        var data = JSON.parse(response.data.Content)
        console.log(data)
        this.WareHouseList = data
      })
    },
    // 上传核查
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
      const fileObj = item.file
      // FormData 对象
      const form = new FormData()
      // 文件对象
      form.append('file', fileObj)
      form.append('comId', this.comId)
      // let formTwo = JSON.stringify(form)
      ouLoadInInfo(form).then(res => {
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
            message: '导入失败:' + resData.Message,
            type: 'error',
            duration: 4000
          })
        }
      })
    },
    handleDownUpload() {
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/In/DoDownLoadTemp'
      //  var url = window.PLATFROM_CONFIG.baseUrl + '/Assets/themes/Excel/入库单导入模版.xlsx'
      window.open(url)
    },
    getNowFormatDate() {
      var date = new Date()
      var seperator1 = '-'
      var year = date.getFullYear()
      var month = date.getMonth() + 1
      var strDate = date.getDate()
      if (month >= 1 && month <= 9) {
        month = '0' + month
      }
      if (strDate >= 0 && strDate <= 9) {
        strDate = '0' + strDate
      }
      var currentdate = year + seperator1 + month + seperator1 + strDate
      return currentdate
    },
    next_id() {
      var current_id = 0
      return function() {
        return ++current_id
      }
    },
    // 料仓收料
    // 获取Lable信息
    handleCheckLabel() {
      if (this.Label.Code === '' || this.Label.Code == null) {
        this.$message({
          title: '失败 ',
          message: '请先扫描或输入条码',
          type: 'error',
          duration: 2000
        })
        return
      }
      getLabelByCode(this.Label.Code).then(response => {
        if (typeof (response.data.Content) !== 'undefined') {
          var result = JSON.parse(response.data.Content)
          if (result === '' || result === null) {
            this.restLabel()
          } else {
            this.Label = result
          }
        } else {
          this.$message({
            title: '失败',
            message: '未获取到该条码明细，请核查输入条码信息',
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handlEntereCheckLabel(e) {
      var keyCode = window.event ? e.keyCode : e.which
      if (keyCode === 13) {
        getLabelByCode(this.Label.Code).then(response => {
          if (typeof (response.data.Content) !== 'undefined') {
            var result = JSON.parse(response.data.Content)
            if (result === '' || result === null) {
              this.restLabel()
            } else {
              this.Label = result
            }
          }
        })
      }
    },
    // 清空数据面板
    dialogInMaterialVisibleOpen() {
      this.dialogInMaterialVisible = true
      this.InMaterial.Quantity = 0
      this.resstIn()
      this.restLabel()
    },
    dialogInMaterialVisibleClose() {
      this.dialogInMaterialVisible = false
      this.resstIn()
    },
    createLabelData() {
      if (this.addMaterial.length <= 0) {
        this.$message({
          type: 'error',
          message: '未进行收料操作，无法创建入库单'
        })
        return
      }
      this.In.AddMaterial = []
      this.addMaterial.forEach(element => {
        this.In.AddMaterial.push(element)
      })
      createIn(this.In).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogInMaterialVisible = false
          this.getList()
          this.$message({
            title: '成功',
            message: '创建成功',
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
    handleSendReceipt(row) {
      this.In = Object.assign({}, row) // copy obj
      if (this.In.Status !== 0) {
        this.$message({
          title: '失败',
          message: '入库单' + this.In.Code + '状态:[' + row.StatusCaption + ']不为待上架',
          type: 'error',
          duration: 2000
        })
        return
      }
      handleSendReceiptOrder(this.In).then((res) => {
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
    restLabel() {
      this.Label = {
        Code: null,
        SupplyCode: null,
        MaterialCode: null,
        ProductionDate: null,
        Batchcode: null
      }
    },
    resstIn() {
      this.In = {
        Code: '',
        BillCode: '',
        WareHouseCode: '',
        InDict: undefined,
        Remark: ''
      }
      this.addMaterial = []
    },
    timer() {
      return setInterval(() => {
        this.getList()
      }, 10000)
    }
  }
}
</script>

