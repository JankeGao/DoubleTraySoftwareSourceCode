<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input
          v-model="listQuery.Code"
          :placeholder="$t('stockManagement.bomNumberAndBomName')"
          class="filter-item"
          clearable
          @keyup.enter.native="handleFilter"
          @clear="handleFilter"
        />
        <el-input
          v-model="listQuery.Remark"
          :placeholder="$t('headerInformation.remarks')"
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
        >{{ $t('baseBtn.queryBtn') }}</el-button>
        <el-button class="filter-button" type="primary" icon="el-icon-edit" @click="handleCreate">{{ $t('baseBtn.addBtn') }}</el-button>
        <el-upload
          ref="fileupload"
          class="filter-button"
          action="#"
          :show-file-list="false"
          :http-request="uploadFile"
          :before-upload="beforeUpload"
          :on-exceed="handleExceed"
        >
          <el-button type="primary" i class="el-icon-upload">{{ $t('baseBtn.importBtn') }}</el-button>
        </el-upload>
        <el-button
          class="filter-button"
          type="primary"
          icon="el-icon-download"
          @click="handleDownUpload()"
        >{{ $t('baseBtn.Template') }}</el-button>
        <el-dropdown size="small" placement="bottom" trigger="click" @command="batchOperate">
          <el-button class="filter-button" style="margin-left: 10px;" type="primary">
            {{ $t('baseBtn.exportBtn') }}
            <i class="el-icon-arrow-down el-icon--right" />
          </el-button>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item command="All_Export">{{ $t('baseBtn.allExport') }}</el-dropdown-item>
            <el-dropdown-item command="Condition_Export">{{ $t('baseBtn.queryExport') }}</el-dropdown-item>
          </el-dropdown-menu>
        </el-dropdown>
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
          <el-table-column type="expand">
            <template slot-scope="props">
              <el-form label-position="left" inline class="demo-table-expand">
                <el-row :gutter="20">
                  <el-col :span="6">
                    <el-form-item :label="$t('basicInformation.firstInFirstOut')">
                      <span>{{ props.row.FIFOTypeCaption }}</span>
                    </el-form-item>
                    <el-form-item :label="$t('basicInformation.batchesOfMaterial')">
                      <el-switch
                        v-model="props.row.IsBatch"
                        active-color="#13ce66"
                        inactive-color="#ff4949"
                        disabled
                      />
                    </el-form-item>
                    <el-form-item :label="$t('basicInformation.storeLocked')">
                      <el-switch
                        v-model="props.row.IsNeedBlock"
                        active-color="#13ce66"
                        inactive-color="#ff4949"
                        disabled
                      />
                    </el-form-item>
                    <el-form-item :label="$t('basicInformation.whetherTheMixedBatchOf')">
                      <el-switch
                        v-model="props.row.IsMaxBatch"
                        active-color="#13ce66"
                        inactive-color="#ff4949"
                        disabled
                      />
                    </el-form-item>
                  </el-col>
                  <el-col :span="6">
                    <el-form-item :label="$t('basicInformation.expiryDate')">
                      <span>{{ props.row.ValidityPeriod }} {{ $t('basicInformation.day') }}</span>
                    </el-form-item>
                    <el-form-item :label="$t('basicInformation.agingTime')">
                      <span>{{ props.row.AgeingPeriod }} {{ $t('basicInformation.day') }}</span>
                    </el-form-item>
                    <el-form-item :label="$t('basicInformation.singlePackageBarCode')">
                      <el-switch
                        v-model="props.row.IsPackage"
                        active-color="#13ce66"
                        inactive-color="#ff4949"
                        disabled
                      />
                    </el-form-item>
                    <el-form-item :label="$t('basicInformation.singlePackageNumber')">
                      <span>{{ props.row.PackageQuantity }}</span>
                    </el-form-item>
                  </el-col>
                  <el-col :span="6">
                    <el-form-item :label="$t('equipmentManagement.forAVehicle')">
                      <span>{{ props.row.BoxCode }}</span>
                    </el-form-item>
                    <el-form-item :label="$t('headerInformation.vehicleName')">
                      <span>{{ props.row.BoxName }}</span>
                    </el-form-item>
                    <el-form-item :label="$t('basicInformation.maximumStorage')">
                      <span>{{ props.row.BoxCount }}</span>
                    </el-form-item>
                    <el-form-item :label="$t('basicInformation.costCenter')">
                      <span>{{ props.row.CostCenter }}</span>
                    </el-form-item>
                  </el-col>
                  <el-col :span="8">
                    <img
                      :src="BaseUrl+props.row.PictureUrl "
                      style="height:300px;width:400px; display: block;"
                      fit="fit"
                    >
                  </el-col>
                </el-row>
              </el-form>
            </template>
          </el-table-column>
          <el-table-column :label="$t('headerInformation.picture')" width="70" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <div class="image_box">
                <img
                  :src="BaseUrl+scope.row.PictureUrl "
                  style="height:100%;width:100%; display: block;"
                  fit="fit"
                >
              </div>
            </template>
          </el-table-column>
          <el-table-column :label="$t('headerInformation.materialCode')" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Code }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('headerInformation.materialName')" width="200" align="center" :show-overflow-tooltip="true">
            <template slot-scope="scope">
              <span>{{ scope.row.Name }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('basicInformation.materialType')" width="80" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialTypeDescription }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('headerInformation.company')" width="80" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.Unit }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('basicInformation.theWeightOfThe')" width="80" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.UnitWeight }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('basicInformation.note1')" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Remark1 }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('basicInformation.note2')" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Remark2 }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('basicInformation.note3')" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Remark3 }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('basicInformation.note4')" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Remark4 }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('basicInformation.note5')" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Remark5 }}</span>
            </template>
          </el-table-column>
          <el-table-column
            :label="$t('headerInformation.operation')"
            align="center"
            width="260"
            class-name="small-padding fixed-width"
            fixed="right"
          >
            <template slot-scope="scope">
              <el-button
                size="mini"
                type="info"
                style="width:80px"
                @click="handleBarCode(scope.row)"
              >{{ $t('basicInformation.toGenerateTheBarcode') }}</el-button>
              <el-button size="mini" type="primary" @click="handleUpdate(scope.row)">{{ $t('baseBtn.editBtn') }}</el-button>
              <el-button size="mini" type="danger" @click="handleDelete(scope.row)">{{ $t('baseBtn.delete') }}</el-button>
            </template>
          </el-table-column>
        </el-table>
        <!-- 分页 -->
        <div class="pagination-container">
          <el-pagination
            :current-page="listQuery.Page"
            :page-sizes="[15,30,45, 60]"
            :page-size="listQuery.Rows"
            :total="total"
            background
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleSizeChange"
            @current-change="handleCurrentChange"
          />
        </div>
      </el-card>
    </el-row>

    <!-- 创建/编辑 弹出框 -->
    <el-dialog
      v-el-drag-dialog
      :title="textMap[dialogStatus]"
      :visible.sync="dialogFormVisible"
      fullscreen
      :close-on-click-modal="false"
    >
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="Material"
        class="dialog-form"
        label-width="120px"
        label-position="left"
        style="margin:20px"
      >
        <el-tabs v-model="activeName" type="card" style="width:900px">
          <el-tab-pane :label="$t('basicInformation.basicInformation')" name="first">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item :label="$t('headerInformation.materialCategory')">
                  <el-select
                    v-model="materialTypeValue"
                    size="small"
                    class="dialog-input"
                    :placeholder="$t('headerInformation.materialCategory')"
                    :disabled="dialogStatus=='update'"
                    @change="handleChangeMaterailType"
                  >
                    <el-option
                      v-for="item in materialType"
                      :key="item.value"
                      :label="item.label"
                      :value="parseInt(item.value)"
                    />
                  </el-select>
                </el-form-item>
                <el-form-item :label="$t('headerInformation.materialName')" prop="Name">
                  <el-input
                    v-model="Material.Name"
                    clearable
                    class="dialog-input"
                    :placeholder="$t('basicInformation.pleaseEnterTheMaterialName')"
                  />
                </el-form-item>
                <el-form-item :label="$t('headerInformation.materialCode')">
                  <el-input
                    v-model="Material.Code"
                    clearable
                    class="dialog-input"
                    :placeholder="$t('basicInformation.ifItIsEmpty')"
                    :disabled="dialogStatus=='update'"
                  />
                </el-form-item>
                <el-form-item :label="$t('dataReport.materialUnit')" prop="Unit">
                  <el-input
                    v-model="Material.Unit"
                    clearable
                    class="dialog-input"
                    :placeholder="$t('basicInformation.pleaseEnterTheMaterialUnit')"
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.theWeightOfThe')" prop="UnitWeight">
                  <el-input
                    v-model="Material.UnitWeight"
                    class="dialog-input"
                    type="text"
                    onkeyup="value=value.replace(/[^\d.]/g,'').replace(/^(\d*\.\d{0,2}).*$/g, '$1')"
                    :placeholder="$t('basicInformation.pleaseEnterUnitWeight') "
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.note1')">
                  <el-input
                    v-model="Material.Remark1"
                    :autosize="{ minRows: 1, maxRows: 4}"
                    type="textarea"
                    :placeholder="$t('basicInformation.pleaseEnterMaterialRemarks')"
                    class="dialog-input"
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.note2')">
                  <el-input
                    v-model="Material.Remark2"
                    :autosize="{ minRows: 1, maxRows: 4}"
                    type="textarea"
                    :placeholder="$t('basicInformation.pleaseEnterMaterialPrice')"
                    class="dialog-input"
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.note3')">
                  <el-input
                    v-model="Material.Remark3"
                    :autosize="{ minRows: 1, maxRows: 4}"
                    type="textarea"
                    :placeholder="$t('basicInformation.pleaseEnterMaterialPurpose')"
                    class="dialog-input"
                  />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <img
                  :src="materailsrc"
                  style="width: 400px; height: 300px;object-fit:cover; display: block;"
                  fit="fit"
                >
                <div style="padding: 10px;">
                    <div>
                      <span style="float:right">
                        <el-button type="text" @click="handleRemove()"><i class="el-icon-delete" /></el-button>
                      </span>
                    </div>
                  </div>
                <el-upload
                  :show-file-list="false"
                  :on-success="uploadingPicture"
                  :action="uploadFileLibrary"
                  :before-upload="PictureStatus"
                  class="filter-button"
                >
                  <el-button style="margin-bottom: 70px;">{{ $t('basicInformation.choosePicture') }}</el-button>
                </el-upload>
                <el-form-item :label="$t('basicInformation.note4')">
                  <el-input
                    v-model="Material.Remark4"
                    :autosize="{ minRows: 1, maxRows: 4}"
                    type="textarea"
                    :placeholder="$t('basicInformation.pleaseEnterMaterialRemarks')"
                    class="dialog-input"
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.note5')">
                  <el-input
                    v-model="Material.Remark5"
                    :autosize="{ minRows: 1, maxRows: 4}"
                    type="textarea"
                    :placeholder="$t('basicInformation.pleaseEnterMaterialRemarks')"
                    class="dialog-input"
                  />
                </el-form-item>
              </el-col>
            </el-row>
          </el-tab-pane>
          <el-tab-pane :label="$t('basicInformation.attributeInformation')" name="second">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item :label="$t('basicInformation.attributeGroup')">
                  <el-select
                    v-model="Material.PackageUnit"
                    :multiple="false"
                    filterable
                    clearable
                    reserve-keyword
                    :placeholder="$t('basicInformation.batchMaintenance')"
                    style="width:300px"
                    @change="handleChooseMaterialProperty"
                  >
                    <el-option
                      v-for="item in MaterialPropertyList"
                      :key="item.Name"
                      :label="item.Name"
                      :value="item.Name"
                    />
                  </el-select>
                </el-form-item>
                <el-form-item :label="$t('basicInformation.largestInventory')">
                  <el-input
                    v-model="Material.MaxNum"
                    class="dialog-input"
                    type="text"
                    onkeyup="value=value.replace(/[^\d]/g,'')"
                    :placeholder="$t('basicInformation.pleaseInputTheMaximumStockOfMaterials')"
                    @change="maxNumChange"
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.theMinimumInventory')">
                  <el-input
                    v-model="Material.MinNum"
                    class="dialog-input"
                    type="text"
                    onkeyup="value=value.replace(/[^\d]/g,'')"
                    :placeholder="$t('basicInformation.pleaseEnterTheMinimumInventoryOfMaterials')"
                    @change="minNumChange"
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.batchesOfMaterial')">
                  <el-switch
                    v-model="Material.IsBatch"
                    active-color="#13ce66"
                    inactive-color="#ff4949"
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.storeLocked')">
                  <el-switch
                    v-model="Material.IsNeedBlock"
                    active-color="#13ce66"
                    inactive-color="#ff4949"
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.singlePackageBarCode')">
                  <el-switch
                    v-model="Material.IsPackage"
                    :disabled="Material.MaterialType===1"
                    active-color="#13ce66"
                    inactive-color="#ff4949"
                  />
                  <div v-if="Material.MaterialType ===1">{{ $t('basicInformation.singlePackageManagement') }}</div>
                </el-form-item>
                <el-form-item :label="$t('basicInformation.numberOfPackages')">
                  <el-input
                    v-model="Material.PackageQuantity"
                    class="dialog-input"
                    type="text"
                    onkeyup="value=value.replace(/[^\d]/g,'')"
                    :placeholder="$t('basicInformation.pleaseEnterPackingQuantity')"
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.whetherTheMixedBatchOf')">
                  <el-switch
                    v-model="Material.IsMaxBatch"
                    active-color="#13ce66"
                    inactive-color="#ff4949"
                  />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item :label="$t('basicInformation.firstInFirstOut')">
                  <el-select
                    v-model="Material.FIFOType"
                    size="small"
                    class="dialog-input"
                    :placeholder="$t('basicInformation.pleaseSelectFifO')"
                  >
                    <el-option
                      v-for="item in options"
                      :key="item.value"
                      :label="item.label"
                      :value="parseInt(item.value)"
                    />
                  </el-select>
                </el-form-item>
                <el-form-item :label="$t('basicInformation.precision')" prop="IntervalType">
                  <el-select
                    v-model="Material.FIFOAccuracy"
                    :placeholder="$t('basicInformation.pleaseSelectFifOAccuracy')"
                    class="dialog-input"
                    :disabled="Material.FIFOType===0"
                  >
                    <el-option
                      v-for="item in timeoptions"
                      :key="item.value"
                      :label="item.label"
                      :value="item.value"
                    />
                  </el-select>
                </el-form-item>
                <el-form-item :label="$t('basicInformation.expiryDateDays')">
                  <el-input
                    v-model="Material.ValidityPeriod"
                    class="dialog-input"
                    type="text"
                    onkeyup="value=value.replace(/[^\d]/g,'')"
                    :placeholder="$t('basicInformation.pleaseEnterTheValidityPeriod')"
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.agingTimeDays')">
                  <el-input
                    v-model="Material.AgeingPeriod"
                    class="dialog-input"
                    type="text"
                    onkeyup="value=value.replace(/[^\d]/g,'')"
                    :placeholder="$t('basicInformation.pleaseEnterAgingTime')"
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.costCenter')">
                  <el-input
                    v-model="Material.CostCenter"
                    class="dialog-input"
                    :placeholder="$t('basicInformation.pleaseEnterMaterialCostCenter')"
                  />
                </el-form-item>
              </el-col>
            </el-row>
          </el-tab-pane>
          <el-tab-pane :label="$t('equipmentManagement.forAVehicle')" name="third">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item :label="$t('equipmentManagement.forAVehicle')">
                  <el-input
                    v-model="Material.BoxCode"
                    class="dialog-input"
                    type="text"
                    :placeholder="$t('basicInformation.vehicleClassCode')"
                    disabled
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.nameOfVehicleBox')">
                  <el-input
                    v-model="Material.BoxName"
                    class="dialog-input"
                    :placeholder="$t('basicInformation.nameOfVehicleBox')"
                    disabled
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.theLengthOfThe')">
                  <el-input
                    v-model="Material.BoxLength"
                    class="dialog-input"
                    type="text"
                    :placeholder="$t('basicInformation.lengthOfVehicleBox')"
                    disabled
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.theWidthOfThe')">
                  <el-input
                    v-model="Material.BoxWidth"
                    class="dialog-input"
                    type="text"
                    :placeholder="$t('basicInformation.widthOfVehicleBox')"
                    disabled
                  />
                </el-form-item>
                <el-form-item :label="$t('basicInformation.maximumStorageQuantity')" prop="BoxCount">
                  <el-input
                    v-model="Material.BoxCount"
                    class="dialog-input"
                    type="text"
                    :placeholder="$t('basicInformation.maximumStorageQuantity')"
                  />
                </el-form-item>
                <el-button
                  :loading="downloadLoading"
                  class="filter-button"
                  type="primary"
                  icon="el-icon-search"
                  @click=" handleBox()"
                >{{ $t('basicInformation.chooseAVehicle') }}</el-button>
              </el-col>
              <el-col :span="12">
                <img
                  :src="BaseUrl+Material.PictureUrl "
                  style="width: 400px; height: 300px;object-fit:cover; display: block;"
                  fit="fit"
                >
              </el-col>
            </el-row>
          </el-tab-pane>
        </el-tabs>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="creatCancel">{{ $t('baseBtn.cancel') }}</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData">{{ $t('baseBtn.confirm') }}</el-button>
        <el-button v-else type="primary" @click="updateData">{{ $t('baseBtn.confirm') }}</el-button>
      </div>
    </el-dialog>

    <!-- 生成条码 -->
    <el-dialog
      v-el-drag-dialog
      title="$t('headerInformation.printBarcode')"
      :show-close="false"
      :visible.sync="dialogBarCodeVisible"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <el-form
        ref="dataLabelForm"
        :rules="labelrules"
        :model="Material"
        class="dialog-form"
        label-width="100px"
        label-position="left"
      >
        <el-form-item :label="$t('headerInformation.materialCode')">
          <el-input v-model="Material.Code" class="dialog-input" disabled />
        </el-form-item>
        <el-form-item :label="$t('headerInformation.materialName')">
          <el-input v-model="Material.Name" class="dialog-input" disabled />
        </el-form-item>
        <el-form-item :label="$t('headerInformation.supplierCode')">
          <span>
            <el-input v-model="Supply.Code" class="dialog-input" disabled />
          </span>
          <span>
            <el-button
              v-waves
              class="filter-button"
              type="primary"
              icon="el-icon-search"
              @click="handleSupply"
            />
          </span>
        </el-form-item>
        <el-form-item :label="$t('headerInformation.supplierName')">
          <el-input v-model="Supply.Name" class="dialog-input" disabled />
        </el-form-item>
        <el-form-item :label="$t('headerInformation.batch')">
          <el-input v-model="Label.Batchcode" class="dialog-input" />
        </el-form-item>
        <el-form-item :label="$t('commonData.quantity')">
          <el-input v-model="Label.Quantity" class="dialog-input" />
        </el-form-item>
        <el-form-item :label="$t('headerInformation.dateOfManufacture')">
          <el-date-picker v-model="productionDate" type="date" :placeholder="$t('headerInformation.selectDate')" />
        </el-form-item>
        <el-form-item :label="$t('basicInformation.toGenerateTheNumber')" prop="Unit">
          <el-input v-model="labelCount" class="dialog-input" />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogBarCodeVisibleClose()">{{ $t('baseBtn.cancel') }}</el-button>
        <el-button
          v-loading.fullscreen.lock="fullscreenLoading"
          type="primary"
          element-loading-text="$t('printBar.justAMomentPlease')"
          @click="handleCreateLabel() "
        >{{ $t('shelf.generate') }}</el-button>
      </div>
    </el-dialog>
    <!-- 存放载具 -->
    <el-dialog
      v-el-drag-dialog
      title="$t('basicInformation.vehicleInformation')"
      :visible.sync="dialogBoxVisible"
      :width="'60%'"
      :close-on-click-modal="false"
      :fullscreen="true"
    >
      <div class="filter-container" style="margin-bottom:20px">
        <el-input
          v-model="listBoxQuery.Code"
          :placeholder="$t('basicInformation.vehicleCode')"
          class="filter-item"
          clearable
          @keyup.enter.native="handleBoxFilter"
          @clear="handleBoxFilter"
        />
        <el-input
          v-model="listBoxQuery.Name"
          :placeholder="$t('headerInformation.vehicleName')"
          class="filter-item"
          clearable
          @keyup.enter.native="handleBoxFilter"
          @clear="handleBoxFilter"
        />
        <el-button
          v-waves
          class="filter-button"
          type="primary"
          icon="el-icon-search"
          @click="handleBoxFilter"
        >{{ $t('baseBtn.queryBtn') }}</el-button>
      </div>

      <el-table
        :key="TableKey"
        v-loading="listLoading"
        :data="Boxlist"
        :header-cell-style="{background:'#F5F7FA'}"
        border
        fit
        size="mini"
        highlight-current-row
        style="width:100%;min-height:100%;"
        @row-click="handleBoxRowClick"
      >
        <el-table-column type="index" width="50" />
        <el-table-column :label="$t('basicInformation.typeOfVehicleBox')" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.imageInformation')" width="120" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <div>
              <span>
                <div class="block">
                  <el-image
                    :src="BaseUrl+scope.row.PictureUrl"
                    fit="contain"
                    :preview-src-list="[BaseUrl+scope.row.PictureUrl]"
                  >
                    <div slot="error" class="image-slot">
                      <i class="el-icon-picture-outline" />
                    </div>
                  </el-image>
                </div>
              </span>
            </div>
          </template>
        </el-table-column>
        <el-table-column :label=" $t('basicInformation.nameOfVehicleBox')" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Name }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.vehicleColor')" width="80" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BoxColour }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.theWidthOfThe')" width="120" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BoxLength }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.theLengthOfThe')" width="120" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BoxWidth }}</span>
          </template>
        </el-table-column>

        <el-table-column :label="$t('box.Type')" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row. Type }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('box.IsVirtual')" width="80" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <el-switch v-model="scope.row.IsVirtual" disabled />
          </template>
        </el-table-column>
        <el-table-column :label="$t('equipmentManagement.describe')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.IntroduceBox }}</span>
          </template>
        </el-table-column>
      </el-table>
      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          :current-page="listBoxQuery.Page"
          :page-sizes="[10,20,30, 50]"
          :page-size="listBoxQuery.Rows"
          :total="Boxtotal"
          background
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleBoxSizeChange"
          @current-change="handleBoxCurrentChange"
        />
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogBoxVisible = false">{{ $t('baseBtn.cancel') }}</el-button>
        <el-button type="primary" @click="updateBox">{{ $t('baseBtn.confirm') }}</el-button>
      </div>
    </el-dialog>
    <!-- 条码信息 -->
    <el-dialog
      v-el-drag-dialog
      title=" $t('basicInformation.theBarcodeInformation')"
      :show-close="false"
      :visible.sync="dialogLabelVisible"
      :width="'60%'"
      :close-on-click-modal="false"
    >
      <el-table
        :key="TableKey"
        :data="labelList"
        :header-cell-style="{background:'#F5F7FA'}"
        :height="400"
        border
        fit
        size="mini"
        highlight-current-row
        style="width:100%;min-height:100%;"
        @row-click="handleBoxRowClick"
      >
        <el-table-column type="index" width="50" />
        <el-table-column :label="$t('basicInformation.coding')" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('headerInformation.materialCode')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('headerInformation.supplierCode')" width="120" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.SupplierCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('headerInformation.supplierName')" width="120" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.SupplyName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('headerInformation.dateOfManufacture')" width="140" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ManufactrueDate }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('headerInformation.batch')" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Batchcode }}</span>
          </template>
        </el-table-column>
      </el-table>
      <!-- 分页 -->
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogLabelVisibleClose()">{{ $t('baseBtn.cancel') }}</el-button>
        <el-button
          v-loading.fullscreen.lock="fullscreenLoading1"
          type="primary"
          element-loading-text="$t('basicInformation.barCodePrintingTask')"
          @click="handlePrintCode()"
        >打印</el-button>
      </div>
    </el-dialog>
    <!-- 供应商 -->
    <el-dialog
      v-el-drag-dialog
      title="$t('basicInformation.supplierInformation')"
      :visible.sync="dialogSupplyVisible"
      :width="'60%'"
      :close-on-click-modal="false"
    >
      <div class="filter-container" style="margin-bottom:20px">
        <el-input
          v-model="listSupplyQuery.Code"
          :placeholder="$t('headerInformation.supplierCode')"
          class="filter-item"
          clearable
          @keyup.enter.native="handleSupplyFilter"
          @clear="handleSupplyFilter"
        />
        <el-input
          v-model="listSupplyQuery.Name"
          :placeholder="$t('headerInformation.supplierName')"
          class="filter-item"
          clearable
          @keyup.enter.native="handleSupplyFilter"
          @clear="handleSupplyFilter"
        />
        <el-button
          v-waves
          class="filter-button"
          type="primary"
          icon="el-icon-search"
          @click="handleSupplyFilter"
        >{{ $t('baseBtn.queryBtn') }}</el-button>
      </div>
      <el-table
        :key="TableKey"
        v-loading="listLoading"
        :data="supplylist"
        :header-cell-style="{background:'#F5F7FA'}"
        border
        fit
        size="mini"
        highlight-current-row
        style="width:100%;min-height:100%;"
        @row-click="handleSupplyRowClick"
      >
        <el-table-column type="index" width="50" />
        <el-table-column :label="$t('headerInformation.supplierCode')" width="150" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('headerInformation.supplierName')" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Name }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('headerInformation.theContact')" width="100" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Linkman }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('headerInformation.contact')" width="100" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Phone }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('headerInformation.address')" width="80" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Address }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('headerInformation.remarks')" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Remark }}</span>
          </template>
        </el-table-column>
      </el-table>
      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          :current-page="listSupplyQuery.Page"
          :page-sizes="[10,20,30, 50]"
          :page-size="listSupplyQuery.Rows"
          :total="supplytotal"
          background
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSupplySizeChange"
          @current-change="handleSupplyCurrentChange"
        />
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogSupplyVisible = false">{{ $t('baseBtn.cancel') }}</el-button>
        <el-button type="primary" @click="updateSupply">{{ $t('baseBtn.confirm') }}</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import { doUpLoadMaterialInfo, getPageRecords, createMaterial, editMaterial, deleteMaterial } from '@/api/MaterialSetting'
import { getMaterialPropertyList } from '@/api/MaterialProperty'
import { deletePicture } from '@/api/FileLibrary'
import { createLabel } from '@/api/Label'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import { getBoxPageRecords } from '@/api/Box'
import { getSupplyPageRecords } from '@/api/Supply'
// import QRCode from 'qrcode'
import PrintToLodop from '@/utils/PrintToLodop.js' // 引入附件的js文件
import { isFloat } from '@/utils/validate.js'

export default {
  name: 'Material', // 物料信息
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
      fullscreenLoading1: false,
      JudgeFileLibraty: false,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 15,
        Code: '',
        Status: undefined,
        Remark: '',
        // Sort: 'id',
        Name: ''
      },
      downloadLoading: false,
      TableKey: 0,
      list: null,
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: this.$t('basicTips.editItem'),
        create: this.$t('basicTips.CreateItem')
      },
      // 物料实体
      Material: {
        Id: undefined,
        Code: '',
        Name: '',
        ShortName: '',
        MinNum: 0,
        MaxNum: 0,
        BoxCode: '',
        BoxName: '',
        BoxLength: '',
        BoxWidth: '',
        Remark: '',
        Unit: '',
        PackageUnit: '',
        PackageQuantity: 0,
        IsQuality: false,
        IsBatch: false,
        IsDeleted: false,
        MinOutQuantity: 0,
        MinOutUnit: '',
        CreatedTime: undefined,
        CreatedUserCode: '',
        CreatedUserName: '',
        IsNeedSplit: false,
        Material_Level: '',
        OverRatio: 0,
        MaterialType: 0,
        FIFOType: 0,
        FileID: '',
        IsNeedBlock: undefined,
        IsMaxBatch: undefined,
        ValidityPeriod: undefined,
        AgeingPeriod: undefined,
        CostCenter: undefined
      },
      // 输入规则
      rules: {
        Unit: [{ required: true, message: this.$t('basicTips.pleaseEnterTheUnit'), trigger: 'blur' }],
        UnitWeight: [{ required: true, message: this.$t('basicTips.pleaseEnterTheMaterialWeight'), trigger: 'blur' }],
        Name: [{ required: true, message:this.$t('basicTips.pleaseEnterTheMaterialName'), trigger: 'blur' }],
        BoxCount: [{ required: true, message: this.$t('basicTips.pleaseEnterTheMaximumStorageQuantity'), trigger: 'blur' }]
      },
      // 输入规则
      labelrules: {
        Code: [{ required: true, message: this.$t('basicTips.pleaseSelectVehicle'), trigger: 'blur' }]
      },
      // 生成条码
      dialogBarCodeVisible: false,
      dialogBoxVisible: false,
      Label: {
        Code: null,
        BoxCode: null,
        MaterialCode: null,
        ProductionDate: null,
        Batchcode: null,
        Quantity: 0
      },
      // 供应商
      Supply: {
        Code: null,
        Name: null
      },
      supplylist: [],
      supplytotal: 0,
      // 分页查询
      listSupplyQuery: {
        Page: 1,
        Rows: 10,
        Code: '',
        Status: undefined,
        Sort: 'id',
        Name: ''
      },
      dialogSupplyVisible: false,
      // 载具
      Box: {
        Code: null,
        Name: null
      },
      Boxlist: [],
      Boxtotal: 0,
      // 分页查询
      listBoxQuery: {
        Page: 1,
        Rows: 10,
        Code: '',
        Status: undefined,
        Sort: 'id',
        Name: ''
      },
      productionDate: null,
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
      activeName: 'first',
      // 物料类别枚举
      materialTypeValue: 0,
      materialType: [{
        value: 0,
        label: this.$t('basicTips.material')
      }, {
        value: 1,
        label: this.$t('basicTips.mould')
      }],
      // 先进先出枚举
      options: [{
        value: '0',
        label: this.$t('basicTips.nothing')
      }, {
        value: '1',
        label: this.$t('historicalWarehousing.warehousingTime')
      }, {
        value: '2',
        label: this.$t('headerInformation.dateOfManufacture')
      }, {
        value: '3',
        label: this.$t('basicTips.shelfLifeOfInventory')
      }],
      // 时间间隔-后端枚举
      timeoptions: [
        {
          value: '0',
          label: this.$t('basicTips.nothing')
        }, {
          value: '1',
          label: this.$t('basicTips.second')
        }, {
          value: '2',
          label: this.$t('basicTips.minute')
        }, {
          value: '3',
          label: this.$t('inventoryTips.hour')
        }, {
          value: '4',
          label: this.$t('basicInformation.day')
        }],
      // 属性组
      MaterialPropertyList: [],
      BaseUrl: window.PLATFROM_CONFIG.baseUrl, // 服务默认地址
      src: '/logo.png',
      materailsrc: '/logo.png',
      // 访问的服务器地址
      uploadFileLibrary: window.PLATFROM_CONFIG.baseUrl + '/api/FileLibrary/PostDoFileLibraryUpload'
    }
  },
  watch: {
    // 创建面板关闭，清空原角色查询权限
    dialogFormVisible(value) {
      if (!value) {
        this.JudgeFileLibraty = false
        this.resetMaterial()
        this.src = '/logo.png'
        this.materailsrc = '/logo.png'
      }
    }
  },
  created() {
    this.getList()
  },
  methods: {
    // 标签面板
    dialogLabelVisibleClose() {
      this.dialogLabelVisible = false
      this.labelList = []
      return
    },
    // 生成条码面板
    dialogBarCodeVisibleClose() {
      this.dialogBarCodeVisible = false
    },

    // 更改物料类别
    handleChangeMaterailType(data) {
      this.Material.MaterialType = data
      if (data === 1) {
        this.Material.IsPackage = true // 如果为模具，则自动启用单包管理
      }
    },
    /** ****************/
    /* 信息查询 */
    /** ****************/
    // 获取物料信息
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
      getMaterialPropertyList().then((response) => {
        var data = JSON.parse(response.data.Content)
        // console.log(data)
        this.MaterialPropertyList = data
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
    /** ****************/
    /* 创建按钮 */
    /** ****************/
    handleCreate() {
      this.resetMaterial()
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.Material.MaterialType = 0 // 设置默认物料类型为物料
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    // 确认添加
    createData() {
      if (this.Material.Name === '') {
        this.$message({
          title: this.$t('messageTips.Failure'),
          message: this.$t('basicTips.pleaseEnterTheMaterialNameInTheBasicInformationTab'),
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.Material.BoxCode === '') {
        this.$message({
          title: this.$t('messageTips.Failure') ,
          message:  this.$t('basicTips.pleaseSelectTheAssociatedVehicle'),
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.Material.MaxNum < this.Material.MinNum) {
        this.$message({
          type: 'error',
          message: this.$t('basicTips.attributeInformation')
        })
        return
      }
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          createMaterial(this.Material).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              // this.list.unshift(this.Role)
              this.dialogFormVisible = false
              this.getList()
              this.$message({
                title: this.$t('messageTips.Succeed') ,
                message: this.$t('messageTips.messageSucceed') ,
                type: 'success',
                duration: 2000
              })
            } else {
              this.$message({
                title: this.$t('messageTips.Failure') ,
                message: this.$t('messageTips.messageFailure')+'：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    // 是否创建了图片实例
    PictureStatus(datas) {
      this.JudgeFileLibraty = true
    },
    // 上传图片
    uploadingPicture(data) {
      var resData = JSON.parse(data.Content)
      if (resData.Success) {
        this.Material.PictureUrl = resData.Data.FilePath
        this.materailsrc = this.BaseUrl + this.Material.PictureUrl
        this.Material.FileID = resData.Data.Id
      } else {
        this.$message({
        })
      }
    },
    //删除图片
    handleRemove() {
      this.Material.PictureUrl = null
      this.materailsrc = null
      this.Material.FileID = null
    },
    // 添加编辑时选择图片后不确认添加或修改
    creatCancel() {
      // 删除图片
      if (this.JudgeFileLibraty) {
        deletePicture(this.Material).then((res) => {
          var resDatas = JSON.parse(res.data.Content)
          if (!resDatas.Success) {
            this.$message({
              title: this.$t('messageTips.Succeed') ,
              type: 'success',
              message: this.$t('systemManagementTips.newEmployeePictureNotDeleted'),
              duration: 2000
            })
          }
        })
      }
      this.dialogFormVisible = false
    },
    // 根据属性组批量维护
    handleChooseMaterialProperty(data) {
      const MaterialProperty = this.MaterialPropertyList.find((element) => (element.Name === data))
      this.Material.FIFOType = MaterialProperty.FIFOType
      this.$set(this.Material, 'IsPackage', MaterialProperty.IsPackage)
      this.$set(this.Material, 'MinNum', MaterialProperty.MinNum)
      this.$set(this.Material, 'MaxNum', MaterialProperty.MaxNum)
      this.$set(this.Material, 'IsBatch', MaterialProperty.IsBatch)
      this.$set(this.Material, 'IsNeedBlock', MaterialProperty.IsNeedBlock)
      this.$set(this.Material, 'IsMaxBatch', MaterialProperty.IsMaxBatch)
      this.$set(this.Material, 'ValidityPeriod', MaterialProperty.ValidityPeriod)
      this.$set(this.Material, 'AgeingPeriod', MaterialProperty.AgeingPeriod)
      this.$set(this.Material, 'CostCenter', MaterialProperty.CostCenter)
      if (this.materialTypeValue === 1) {
        this.$set(this.Material, 'IsPackage', true)
      }
    },
    /** ****************/
    /* 编辑按钮 */
    /** ****************/
    handleUpdate(row) {
      this.Material = Object.assign({}, row) // copy obj
      this.src = this.BaseUrl + this.Material.BoxPictureUrl // 展示载具图片
      this.materailsrc = this.BaseUrl + this.Material.PictureUrl // 展示物料图片
      this.materialTypeValue = this.Material.MaterialType
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      if (this.Material.MaxNum < this.Material.MinNum) {
        this.$message({
          type: 'error',
          message: this.$t('basicTips.attributeInformation')
        })
        return
      }
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          const materialData = Object.assign({}, this.Material)
          editMaterial(materialData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            console.log(resData)
            if (resData.Success) {
              for (const v of this.list) {
                if (v.Id === this.Material.Id) {
                  const index = this.list.indexOf(v)
                  this.list.splice(index, 1, this.Material)
                  // 判断是否更改图片
                  if (v.FileID !== this.Material.FileID) {
                    deletePicture(v)
                  }
                  break
                }
              }
              this.getList()
              this.dialogFormVisible = false
              this.$message({
                title: this.$t('messageTips.Succeed') ,
                message: this.$t('presentationOfWarehousingDocuments.updateSuccessful'),
                type: 'success',
                duration: 2000
              })

              // this.$confirm('是否跳转到仓库信息界面', '提示', {
              //   confirmButtonText: '确定',
              //   cancelButtonText: '取消',
              //   type: 'warning'
              // }).then(() => {
              //   this.$router.push('/BaseSetting/WareHouseSetting/index');
              // }).catch(() => {
              //   this.$message({
              //     type: 'info',
              //     message: '已取消执行'
              //   })
              // })
            } else {
              this.$message({
                title: this.$t('messageTips.Succeed') ,
                message: this.$t('messageTips.messageFailure')+'：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },

    /** ****************/
    /* 存放载具 */
    /** ****************/
    // 查询载具编码
    handleBox() {
      this.dialogBoxVisible = true
      this.getBoxList()
    },
    getBoxList() {
      getBoxPageRecords(this.listBoxQuery).then(response => {
        var result = JSON.parse(response.data.Content)
        this.Boxlist = result.rows
        this.Boxtotal = result.total
      })
    },
    // 数据筛选
    handleBoxFilter() {
      this.listBoxQuery.Page = 1
      this.getBoxList()
    },
    // 切换分页数据-行数据
    handleBoxSizeChange(val) {
      this.listBoxQuery.Rows = val
      this.getBoxList()
    },
    // 切换分页-列数据
    handleBoxCurrentChange(val) {
      this.listBoxQuery.Page = val
      this.getBoxList()
    },
    handleBoxRowClick(row) {
      this.Box = row
      this.src = this.BaseUrl + this.Box.PictureUrl
      this.Material.BoxCode = this.Box.Code
      this.Material.BoxName = this.Box.Name
      this.Material.BoxLength = this.Box.BoxLength
      this.Material.BoxWidth = this.Box.BoxWidth
    },
    updateBox() {
      if (this.Box === null) {
        this.$message({
          title: this.$t('messageTips.Failure') ,
          message: this.$t('basicTips.pleaseSelectAVehicle') ,
          type: 'error',
          duration: 2000
        })
        return
      }
      this.dialogBoxVisible = false
    },
    /** ****************/
    /* 供应商编码 */
    /** ****************/
    // 查询供应商编码
    handleSupply() {
      this.dialogSupplyVisible = true
      this.getSupplyList()
    },
    getSupplyList() {
      getSupplyPageRecords(this.listSupplyQuery).then(response => {
        var result = JSON.parse(response.data.Content)
        this.supplylist = result.rows
        this.supplytotal = result.total
      })
    },
    // 数据筛选
    handleSupplyFilter() {
      this.listSupplyQuery.Page = 1
      this.getSupplyList()
    },
    // 切换分页数据-行数据
    handleSupplySizeChange(val) {
      this.listSupplyQuery.Rows = val
      this.getSupplyList()
    },
    // 切换分页-列数据
    handleSupplyCurrentChange(val) {
      this.listSupplyQuery.Page = val
      this.getSupplyList()
    },
    handleSupplyRowClick(row) {
      this.Supply = row
    },
    updateSupply() {
      if (this.Supply === null) {
        this.$message({
          title: this.$t('messageTips.Failure') ,
          message: this.$t('basicTips.pleaseSelectASupplier') ,
          type: 'error',
          duration: 2000
        })
        return
      }
      this.dialogSupplyVisible = false
    },
    /** ****************/
    /* 删除按钮 */
    /** ****************/
    handleDelete(row) {
      this.$confirm(this.$t('basicTips.permanentlyDeleteTheMaterial'), this.$t('messageTips.Tips') , {
        confirmButtonText: this.$t('baseBtn.confirm')  ,
        cancelButtonText: this.$t('baseBtn.cancel')  ,
        type: 'warning'
      }).then(() => {
        this.Material = Object.assign({}, row) // copy obj
        this.deleteData(this.Material)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: this.$t('messageTips.cancelledDelete')
        })
      })
    },
    deleteData(data) {
      console.log(data)
      deleteMaterial(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          if (resData.Success) {
            this.$message({
              title: this.$t('messageTips.Succeed') ,
              message: this.$t('messageTips.deleteSucceed'),
              type: 'success',
              duration: 2000
            })
          }
          if (data.FileID !== null) {
            deletePicture(data)
          }
          this.getList()
        } else {
          this.$message({
            title: this.$t('messageTips.Succeed') ,
            message: this.$t('messageTips.deleteFailure')+'：' + resData.Message,
            type: 'error',
            duration: 4000
          })
        }
      })
    },
    /** ****************/
    /* 导出 */
    /** ****************/
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
      if (this.listQuery.Code === '' && this.listQuery.Remark === '') {
        this.$confirm(this.$t('inventoryTips.youHaveNotEnteredAnExportCondition')  + this.total +  this.$t('deviceTips.articleContinue') , this.$t('messageTips.Tips') , {
          confirmButtonText: this.$t('baseBtn.confirm')  ,
          cancelButtonText: this.$t('baseBtn.cancel')  ,
          type: 'warning'
        }).then(() => {
          var url = window.PLATFROM_CONFIG.baseUrl + '/api/Material/DoDownLoad'
          window.open(url)
          console.log(url)
        }).catch(() => {
          this.$message({
            type: 'info',
            message: this.$t('messageTips.messageCancel')
          })
        })
      } else {
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/Material/DoDownLoad?Code=' + this.listQuery.Code + '&Remark=' + this.listQuery.Remark
        window.open(url)
      }
    },

    // 全部导出
    All_ExportExcel() {
      this.$confirm(this.$t('deviceTips.thisOperationWillExportAllDataIncluding') + this.total + this.$t('deviceTips.articleContinue') , this.$t('messageTips.Tips') , {
        confirmButtonText: this.$t('baseBtn.confirm')  ,
        cancelButtonText: this.$t('baseBtn.cancel')  ,
        type: 'warning'
      }).then(() => {
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/Material/DoDownLoad'
        window.open(url)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: this.$t('messageTips.messageCancel')
        })
      })
    },
    maxNumChange() {
      if (this.Material.MaxNum < this.Material.MinNum) {
        this.Material.MaxNum = this.Material.MinNum
      }
    },
    minNumChange() {
      if (this.Material.MaxNum < this.Material.MinNum) {
        this.Material.MinNum = this.Material.MaxNum
      }
    },
    /** ****************/
    /* 导入 */
    /** ****************/
    beforeUpload(file) {
      const isText = file.type === 'application/vnd.ms-excel'
      const isTextComputer = file.type === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      return (isText | isTextComputer)
    },
    handleDownUpload() {
      // var url = window.PLATFROM_CONFIG.baseUrl + '/Assets/themes/Excel/material.xlsx'
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/Material/DoDownLoadTemp'
      window.open(url)
    },
    // 上传文件个数超过定义的数量
    handleExceed(files, fileList) {
      this.$message.warning(this.$t('messageTips.ImportJudge'))
    },
    // 上传文件
    uploadFile(item) {
      this.$confirm(this.$t('basicTips.OtherwiseTheImportWillFail'), this.$t('messageTips.Tips') , {
        confirmButtonText: this.$t('baseBtn.confirm')  ,
        cancelButtonText: this.$t('baseBtn.cancel')  ,
        type: 'warning'
      }).then(() => {
        const fileObj = item.file
        // FormData 对象
        const form = new FormData()
        // 文件对象
        form.append('file', fileObj)
        form.append('comId', this.comId)
        doUpLoadMaterialInfo(form).then((res) => {
          var resData = typeof res.data === 'string' ? JSON.parse(res.data) : res.data
          console.log(res)
          if (resData.Success) {
            this.getList()
            this.$message({
              title: this.$t('messageTips.Succeed') ,
              message: this.$t('messageTips.ImportSucceed'),
              type: 'success',
              duration: 2000
            })
          } else {
            this.getList()
            this.$message({
              title: this.$t('messageTips.Failure') ,
              message:  this.$t('messageTips.ImportFailure') + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      }).catch(() => {
        this.$message({
          type: 'info',
          message: this.$t('basicTips.importCanceled')
        })
      })
    },
    /** ****************/
    /* 生成条码 */
    /** ****************/
    handleBarCode(row) {
      this.Box.Code = null
      this.Box.Name = null
      this.Label.Batchcode = null
      this.Label.Quantity = 0
      this.productionDate = null
      this.labelCount = 1
      this.Material = row
      this.dialogBarCodeVisible = true
    },
    // 生成条码信息
    handleCreateLabel() {
      if (!isFloat(this.Label.Quantity) || this.Label.Quantity === 0) {
        this.$message({
          title: this.$t('messageTips.Failure') ,
          message:this.$t('presentationOfWarehousingDocuments.pleaseEnterTheNumberOfCorrectFormats') ,
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.labelCount <= 0) {
        this.$message({
          title: this.$t('messageTips.Failure') ,
          message: this.$t('presentationOfWarehousingDocuments.pleaseEnterTheCorrectNumberOfTagGeneration'),
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.Label.Batchcode === null) {
        this.$message({
          title: this.$t('messageTips.Failure') ,
          message: this.$t('presentationOfWarehousingDocuments.pleaseEnterTheCorrectBatch'),
          type: 'error',
          duration: 2000
        })
        return
      }
      this.$refs['dataLabelForm'].validate((valid) => {
        if (valid) {
          this.fullscreenLoading = true
          this.Label.SupplierCode = this.Supply.Code
          this.Label.MaterialCode = this.Material.Code
          this.Label.ManufactrueDate = this.productionDate
          for (var i = 0; i < this.labelCount; i++) {
            createLabel(this.Label).then((res) => {
              var resData = JSON.parse(res.data.Content)
              if (resData.Success) {
                this.labelList.push(resData.Data)
                console.log(this.labelList)
              } else {
                this.$message({
                  title: this.$t('messageTips.Failure') ,
                  message: this.$t('messageTips.messageFailure')+'：' + resData.Message,
                  type: 'error',
                  duration: 2000
                })
              }
            })
          }
          setTimeout(() => {
            this.fullscreenLoading = false
            this.dialogLabelVisible = true
          }, 1 * 2000)
        }
      })
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
      this.labelList.forEach(element => {
        this.createBarCode(element.Code)
        // 物料编码
        this.controls.push({
          id: 111,
          type: 'atext',
          data: {
            value: element.MaterialCode,
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
        }
        )
        // 物料名称
        this.controls.push({
          id: 111,
          type: 'atext',
          data: {
            value: element.MaterialName,
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
            value: element.Code,
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
          this.dialogBarCodeVisible = false
          this.dialogLabelVisible = false
          this.labelList = []
        }, 1 * 2000)
      })
    },
    /** ****************/
    /* 重置 */
    /** ****************/
    resetMaterial() {
      this.materialTypeValue = 0
      this.activeName = 'first'
      // 载具
      this.Box = {
        Code: null,
        Name: null
      }
      this.Material = {
        Id: undefined,
        Code: '',
        Name: '',
        ShortName: '',
        BoxCode: '',
        BoxName: '',
        BoxLength: '',
        BoxWidth: '',
        MinNum: 0,
        MaxNum: 0,
        Remark: '',
        Unit: '',
        PackageUnit: '',
        PackageQuantity: 0,
        IsQuality: false,
        IsPackage: false,
        IsBatch: false,
        IsDeleted: false,
        MinOutQuantity: 0,
        MinOutUnit: '',
        CreatedTime: undefined,
        CreatedUserCode: '',
        CreatedUserName: '',
        IsNeedSplit: false,
        Material_Level: '',
        OverRatio: 0,
        MaterialType: 0,
        FIFOType: 0,
        FileID: ''
      }
    }
  }
}
</script>
<style>
.demo-table-expand {
  font-size: 0;
}
.demo-table-expand label {
  width: 90px;
  color: #99a9bf;
}
.demo-table-expand .el-form-item {
  margin-right: 0;
  margin-bottom: 0;
  width: 100%;
}
.image_box {
  width: 50px;
  height: 50px;
}
</style>
