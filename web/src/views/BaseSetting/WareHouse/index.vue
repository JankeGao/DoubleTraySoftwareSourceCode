<template>
  <div class="app-container">
    <el-row :gutter="20">
      <el-col :span="4">
        <el-card>
          <div>
            <span>
              <div style="display: inline-block;">
                <h4>仓库信息</h4>
              </div>
            </span>
          </div>
          <hr class="line">
          <div>
            <el-tree
              ref="treeTest"
              :data="treeData"
              :expand-on-click-node="false"
              style="font-size:20px;"
              node-key="Id"
              :height="tableHeight"
              :default-expand-all="true"
              highlight-current
              @node-click="handleNodeClick"
            />
          </div>
        </el-card>
      </el-col>
      <el-col :span="20" />
      <!-- 筛选栏 -->
      <el-card class="search-card">
        <div class="filter-container">
          <el-input
            v-model="listQuery.LocationCode"
            placeholder="请输入储位编码"
            class="filter-item"
            clearable
            @keyup.enter.native="handleQuery"
            @clear="handleQuery"
          />
          <el-input
            v-model="listQuery.MaterialCode"
            placeholder="请输入物料编码或名称"
            class="filter-item"
            clearable
            @keyup.enter.native="handleQuery"
            @clear="handleQuery"
          />
          <el-input
            v-model="listQuery.BoxCode"
            placeholder="请输入载具编码或名称"
            class="filter-item"
            clearable
            @keyup.enter.native="handleQuery"
            @clear="handleQuery"
          />
          <!-- <el-input v-model="listQuery.SearchCode" placeholder="请输入编码" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" /> -->
          <el-button
            v-waves
            class="filter-button"
            type="primary"
            icon="el-icon-search"
            @click="handleQuery"
          >查询</el-button>
          <el-button
            v-if="Level!==3"
            class="filter-button"
            style="margin-left: 10px;"
            type="primary"
            icon="el-icon-edit"
            @click="handleCreate"
          >添加{{ textTitle[Level] }}</el-button>
          <el-button
            v-else
            v-loading.fullscreen.lock="fullscreenLoading"
            type="primary"
            icon="el-icon-edit"
            element-loading-text="正在加载布局信息，请稍等~"
            @click="handleCreate"
          >维护储位</el-button>
          <el-button
            v-if="Level==3"
            type="primary"
            icon="el-icon-remove"
            @click="handleDeleteTrayLocation"
          >删除该托盘下所有库位</el-button>
          <el-button v-waves class="filter-button" type="primary" @click="All_ExportExcel">导出库位</el-button>
          <el-upload
            v-if="Level==3"
            ref="fileupload"
            style="display: inline; margin-left: 10px;margin-right: 10px;"
            action="#"
            :show-file-list="false"
            :http-request="uploadFile"
            :before-upload="beforeUpload"
            :on-exceed="handleExceed"
          >
            <el-button type="primary">
              导入物料库位
            </el-button>
          </el-upload>
        </div>
      </el-card>
      <el-card>
        <div style="margin-bottom:10px">{{ textTitle[Level] }}</div>
        <div>
          <el-table
            :key="0"
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

            <!-- 0 级仓库管理 -->
            <el-table-column
              v-if="Level==0"
              :key="1"
              :label="'仓库编码'"
              width="100"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.Code }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==0"
              :key="2"
              :label="'仓库名称'"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.Name }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==0"
              :key="3"
              :label="'仓库类别'"
              width="200"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.CategoryDict }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==0"
              :key="4"
              :label="'仓库地址'"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.Address }}</span>
              </template>
            </el-table-column>

            <!-- 1 级货柜管理 -->
            <el-table-column
              v-if="Level==1"
              :key="9"
              :label="'货柜编码'"
              width="100"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.Code }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==1"
              :key="10"
              :label="'图片'"
              width="100"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <div class="image_box">
                  <!-- <el-image
                    :src="BaseUrl+scope.row.PictureUrl"
                    fit="contain"
                    :preview-src-list="[BaseUrl+scope.row.PictureUrl]"
                    style="width: 50px; height: 50px"
                  >
                    <div slot="error" class="image-slot">
                      <i class="el-icon-picture-outline" />
                    </div>
                  </el-image>-->
                  <image
                    :src="BaseUrl+scope.row.PictureUrl"
                    fit="contain"
                    :preview-src-list="[BaseUrl+scope.row.PictureUrl]"
                    style="width: 50px; height: 50px"
                  />
                </div>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==1"
              :key="5"
              :label="'货柜品牌'"
              width="100"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.BrandDescription }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==1"
              :key="6"
              :label="'货柜型号'"
              width="100"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.EquipmentTypeDescription }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==1"
              :key="7"
              :label="'Ip地址'"
              align="150"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.Ip }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==1"
              :key="8"
              :label="'端口号'"
              width="100"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.Port }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==1"
              :key="20"
              :label="'序列号'"
              width="center"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.UID }}</span>
              </template>
            </el-table-column>
            <!-- 2级托盘管理 -->
            <el-table-column
              v-if="Level==2"
              :key="12"
              :label="'货柜编码'"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.ContainerCode }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==2"
              :key="11"
              :label="'托盘编码'"
              width="100"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.Code }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==2"
              :key="13"
              :label="'托盘承重(Kg)'"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.MaxWeight/1000 }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==2"
              :key="14"
              :label="'托盘宽度（mm）'"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.TrayWidth }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==2"
              :key="15"
              :label="'托盘深度（mm）'"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.TrayLength }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==2"
              :key="17"
              :label="'X轴灯个数'"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.XNumber }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==2"
              :key="16"
              :label="'Y轴灯个数'"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.YNumber }}</span>
              </template>
            </el-table-column>

            <!-- 3级储位管理 -->
            <el-table-column
              v-if="Level==3"
              :key="18"
              :label="'货柜编码'"
              width="70"
              align="left"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.ContainerCode }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==3"
              :key="30"
              :label="'托盘编号'"
              width="70"
              align="left"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.TrayCode }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==3"
              :key="31"
              :label="'储位编码'"
              width="120"
              align="left"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.Code }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==3"
              :key="22"
              :label="'物料编码'"
              width="150"
              align="left"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.SuggestMaterialCode }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==3"
              :key="23"
              :label="'物料名称'"
              align="left"
              show-overflow-tooltip
              width="200"
            >
              <template slot-scope="scope">
                <span>{{ scope.row.SuggestMaterialName }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==3"
              :label="'图片'"
              width="70"
              align="center"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <div class="image_box">
                  <img
                    :src="BaseUrl+scope.row.BoxUrl "
                    style="height:100%;width:100%; display: block;"
                    fit="fit"
                  >
                </div>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==3"
              :key="19"
              :label="'载具名称'"
              width="120"
              align="left"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.BoxName }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==3"
              :key="24"
              :label="'X轴灯号'"
              width="80"
              align="left"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.XLight }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==3"
              :key="24"
              :label="'X轴灯号长度'"
              width="120"
              align="left"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.XLenght }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==3"
              :key="25"
              :label="'Y轴灯号'"
              width="80"
              align="left"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.YLight }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==3"
              :key="20"
              :label="'载具深度(mm)'"
              width="120"
              align="left"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.BoxLength }}</span>
              </template>
            </el-table-column>
            <el-table-column
              v-if="Level==3"
              :key="21"
              :label="'载具宽度(mm)'"
              width="120"
              align="left"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.BoxWidth }}</span>
              </template>
            </el-table-column>

            <el-table-column v-if="Level==3" :key="26" :label="'启用'" width="80" align="left">
              <template slot-scope="scope">
                <el-switch
                  v-model="scope.row.Enabled"
                  active-color="#13ce66"
                  inactive-color="#ff4949"
                  disabled
                />
              </template>
            </el-table-column>
            <!-- 0级仓库管理 -->
            <el-table-column
              v-if="Level==0"
              :label="'操作'"
              align="center"
              width="150"
              class-name="small-padding fixed-width"
              fixed="right"
            >
              <template slot-scope="scope">
                <el-row>
                  <el-col :span="12">
                    <el-button size="mini" type="primary" @click="handleUpdate(scope.row)">编辑</el-button>
                  </el-col>
                  <el-col :span="12">
                    <el-button size="mini" type="danger" @click="handleDelete(scope.row)">删除</el-button>
                  </el-col>
                </el-row>
              </template>
            </el-table-column>
            <!-- 1 级货柜管理 -->
            <el-table-column
              v-if="Level==1"
              :label="'操作'"
              align="center"
              width="320"
              class-name="small-padding fixed-width"
              fixed="right"
            >
              <template slot-scope="scope">
                <el-row>
                  <el-col :span="8">
                    <el-button size="mini" type="info" @click="handleTrayUserMap(scope.row)">权限</el-button>
                  </el-col>

                  <!-- <el-col :span="8">
                    <el-button size="mini" type="info" @click="handlePrintCode(scope.row,Level)">打印</el-button>
                  </el-col>-->
                  <el-col :span="8">
                    <el-button size="mini" type="primary" @click="handleUpdate(scope.row)">编辑</el-button>
                  </el-col>
                  <el-col :span="8">
                    <el-button size="mini" type="danger" @click="handleDelete(scope.row)">删除</el-button>
                  </el-col>
                </el-row>
              </template>
            </el-table-column>
            <!-- 2级托盘管理 -->
            <el-table-column
              v-if="Level==2"
              :label="'操作'"
              align="center"
              width="300"
              class-name="small-padding fixed-width"
              fixed="right"
            >
              <template slot-scope="scope">
                <el-row>
                  <el-col :span="6">
                    <el-button size="mini" type="info" @click="handleTrayUserMap(scope.row)">权限</el-button>
                  </el-col>
                  <el-col :span="6">
                    <el-button size="mini" type="info" @click="handleTakeOut(scope.row)">取出</el-button>
                  </el-col>
                  <el-col :span="6">
                    <el-button size="mini" type="info" @click="handleTakeIn(scope.row)">存入</el-button>
                  </el-col>
                  <el-col :span="6">
                    <el-button size="mini" type="info" @click="handlePrintCode(scope.row,Level)">库位码</el-button>
                  </el-col>
                  <el-col :span="6">
                    <el-button size="mini" type="primary" @click="handleUpdate(scope.row)">编辑</el-button>
                  </el-col>
                  <el-col :span="6">
                    <el-button size="mini" type="danger" @click="handleDelete(scope.row)">删除</el-button>
                  </el-col>
                </el-row>
              </template>
            </el-table-column>
            <!-- 3级库位管理 -->
            <el-table-column
              v-if="Level==3"
              :label="'操作'"
              align="center"
              width="100"
              class-name="small-padding fixed-width"
              fixed="right"
            >
              <template slot-scope="scope">
                <el-row>
                  <el-col :span="24">
                    <el-button size="mini" type="info" @click="handlePrintCode(scope.row,Level)">库位码</el-button>
                  </el-col>
                </el-row>
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
        </div>
      </el-card>
    </el-row>
    <!-- 创建/编辑 弹出框 -->
    <el-dialog
      v-el-drag-dialog
      :title="textMap[dialogStatus]+textTitle[Level]"
      :visible.sync="dialogFormVisible"
      :close-on-click-modal="false"
      :fullscreen="true"
    >
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="AllEntity"
        class="dialog-form"
        label-width="120px"
        label-position="left"
      >
        <!-- 0 级仓库管理 -->
        <el-form-item v-if="Level==0" :label="'仓库编码'" prop="Code">
          <el-input
            v-model="AllEntity.Code"
            :disabled="dialogStatus=='update'"
            clearable
            class="dialog-input"
            placeholder="请输入仓库编码"
          />
        </el-form-item>
        <el-form-item v-if="Level==0" :label="'仓库名称'" prop="Name">
          <el-input v-model="AllEntity.Name" class="dialog-input" clearable placeholder="请输入仓库名称" />
        </el-form-item>
        <el-form-item v-if="Level==0" :label="'仓库类别'">
          <el-input
            v-model="AllEntity.CategoryDict"
            class="dialog-input"
            clearable
            placeholder="请输入仓库类别"
          />
        </el-form-item>
        <el-form-item v-if="Level==0" :label="'仓库地址'">
          <el-input
            v-model="AllEntity.Address"
            class="dialog-input"
            clearable
            placeholder="请输入仓库地址"
          />
        </el-form-item>
        <el-form-item v-if="Level==0" :label="'具体描述'">
          <el-input
            v-model="AllEntity.Remark"
            :autosize="{ minRows: 2, maxRows: 4}"
            type="textarea"
            placeholder="备注"
            class="dialog-input"
          />
        </el-form-item>

        <!-- 1 级货柜创建 -->
        <el-row :gutter="20">
          <el-col :span="14">
            <el-form-item v-if="Level==1" :label="'设备型号'" prop="EquipmentCode">
              <span>
                <el-input
                  v-model="AllEntity.EquipmentCode"
                  clearable
                  class="dialog-input"
                  :disabled="dialogStatus=='update'"
                  placeholder="请选择设备型号"
                />
              </span>
              <span>
                <el-button
                  v-if="Level==1 && dialogStatus=='create'"
                  class="filter-button"
                  type="primary"
                  icon="el-icon-search"
                  @click=" handleEquipment()"
                />
              </span>
            </el-form-item>
            <el-form-item v-if="Level==1" :label="'设备编码'" prop="Code">
              <el-input
                v-model="AllEntity.Code"
                clearable
                class="dialog-input"
                placeholder="请输入设备唯一序列号"
                :disabled="dialogStatus=='update'"
              />
            </el-form-item>
            <el-form-item
              v-if="Level==1 && dialogStatus=='create'"
              :label="'设备托盘数'"
              prop="TrayNumber"
            >
              <el-input
                v-model="AllEntity.TrayNumber"
                class="dialog-input"
                clearable
                type="text"
                onkeyup="value=value.replace(/[^\d]/g,'')"
                placeholder="请输入设备托盘数量"
              />
            </el-form-item>
            <el-form-item
              v-if="Level==1 && dialogStatus=='create'"
              :label="'单托盘承重(Kg)'"
              prop="MaxWeight"
            >
              <el-input
                v-model="AllEntity.MaxWeight"
                clearable
                class="dialog-input"
                type="text"
                onkeyup="value=value.replace(/[^\d]/g,'')"
                placeholder="为0不开启托盘承重校验"
              />
            </el-form-item>
            <el-form-item
              v-if="Level==1&&dialogStatus=='create'"
              :label="'托盘宽度(mm)'"
              prop="TrayWidth"
            >
              <el-input
                v-model="AllEntity.TrayWidth"
                clearable
                class="dialog-input"
                type="text"
                onkeyup="value=value.replace(/[^\d]/g,'')"
                placeholder="请输入设备托盘宽度"
              />
            </el-form-item>
            <el-form-item
              v-if="Level==1&&dialogStatus=='create'"
              :label="'托盘深度(mm)'"
              prop="TrayLength"
            >
              <el-input
                v-model="AllEntity.TrayLength"
                clearable
                class="dialog-input"
                type="text"
                onkeyup="value=value.replace(/[^\d]/g,'')"
                placeholder="请输入设备托盘深度"
              />
            </el-form-item>
            <el-form-item v-if="Level==1&&dialogStatus=='create'" :label="'X轴灯数'" prop="XNumber">
              <el-input
                v-model="AllEntity.XNumber"
                clearable
                class="dialog-input"
                type="text"
                onkeyup="value=value.replace(/[^\d]/g,'')"
                placeholder="请输入设备X轴灯数"
              />
            </el-form-item>
            <el-form-item v-if="Level==1&&dialogStatus=='create'" :label="'Y轴灯数'" prop="YNumber">
              <el-input
                v-model="AllEntity.YNumber"
                clearable
                class="dialog-input"
                type="text"
                onkeyup="value=value.replace(/[^\d]/g,'')"
                placeholder="请输入设备Y轴灯数"
              />
            </el-form-item>
            <el-form-item v-if="Level==1" :label="'设备IP地址'">
              <el-input
                v-model="AllEntity.Ip"
                clearable
                class="dialog-input"
                placeholder="请输入设备IP地址"
              />
            </el-form-item>
            <el-form-item v-if="Level==1" :label="'设备端口号'">
              <el-input
                v-model="AllEntity.Port"
                clearable
                class="dialog-input"
                type="text"
                onkeyup="value=value.replace(/[^\d]/g,'')"
                placeholder="请输入设备端口号"
              />
            </el-form-item>
          </el-col>
          <el-col :span="10">
            <el-card v-if="Level==1" style="margin:20px">
              <el-image :src="src" fit="contain" style="width: 400px; height: 300px;margin:20px">
                <div slot="error" class="image-slot">
                  <i class="el-icon-picture-outline" />
                </div>
              </el-image>
            </el-card>
            <el-form-item v-if="Level==1" :label="'型号'">
              <el-input v-model="AllEntity.EquipmentCode" clearable class="dialog-input" disabled />
            </el-form-item>
            <el-form-item v-if="Level==1" :label="'品牌'">
              <el-input
                v-model="AllEntity.BrandDescription"
                clearable
                class="dialog-input"
                disabled
              />
            </el-form-item>
            <el-form-item v-if="Level==1" :label="'类别'">
              <el-input
                v-model="AllEntity.EquipmentTypeDescription"
                clearable
                class="dialog-input"
                disabled
              />
            </el-form-item>
          </el-col>
        </el-row>
        <!-- 2级托盘管理 -->
        <el-form-item v-if="Level==2" :label="'托盘编码'" prop="Code">
          <el-input
            v-model="AllEntity.Code"
            clearable
            class="dialog-input"
            type="text"
            onkeyup="value=value.replace(/[^\d]/g,'')"
            placeholder="请输入设备托盘编码"
          />
        </el-form-item>
        <el-form-item v-if="Level==2" :label="'托盘宽度(mm)'" prop="TrayWidth">
          <el-input
            v-model="AllEntity.TrayWidth"
            clearable
            class="dialog-input"
            type="text"
            onkeyup="value=value.replace(/[^\d]/g,'')"
            placeholder="请输入设备托盘宽度"
          />
        </el-form-item>
        <el-form-item v-if="Level==2" :label="'托盘深度(mm)'" prop="TrayLength">
          <el-input
            v-model="AllEntity.TrayLength"
            clearable
            class="dialog-input"
            type="text"
            onkeyup="value=value.replace(/[^\d]/g,'')"
            placeholder="请输入设备托盘深度"
          />
        </el-form-item>
        <el-form-item v-if="Level==2" :label="'X轴灯数'" prop="XNumber">
          <el-input
            v-model="AllEntity.XNumber"
            clearable
            class="dialog-input"
            type="text"
            onkeyup="value=value.replace(/[^\d]/g,'')"
            placeholder="请输入设备X轴灯数"
          />
        </el-form-item>
        <el-form-item v-if="Level==2" :label="'Y轴灯数'" prop="YNumber">
          <el-input
            v-model="AllEntity.YNumber"
            clearable
            class="dialog-input"
            type="text"
            onkeyup="value=value.replace(/[^\d]/g,'')"
            placeholder="请输入设备Y轴灯数"
          />
        </el-form-item>
        <el-form-item v-if="Level==2" :label="'单托盘承重(Kg)'" prop="MaxWeight">
          <el-input
            v-model="AllEntity.MaxWeight"
            clearable
            class="dialog-input"
            type="text"
            onkeyup="value=value.replace(/[^\d]/g,'')"
            placeholder="为0不开启托盘承重校验"
          />
        </el-form-item>
        <el-form-item v-if="Level==2" :label="'托架号'">
          <el-input
            v-model="AllEntity.BracketNumber"
            clearable
            class="dialog-input"
            type="text"
            onkeyup="value=value.replace(/[^\d]/g,'')"
            placeholder="为0不开启托盘承重校验"
          />
        </el-form-item>
        <el-form-item v-if="Level==2" :label="'托盘号'">
          <el-input
            v-model="AllEntity.BracketTrayNumber"
            clearable
            class="dialog-input"
            type="text"
            onkeyup="value=value.replace(/[^\d]/g,'')"
            placeholder="为0不开启托盘承重校验"
          />
        </el-form-item>
        <!-- 3级货架管理 -->
        <el-row :gutter="20">
          <el-col :span="10">
            <el-form-item v-if="Level==3" :label="'存放载具'">
              <span>
                <el-input
                  v-model="AllEntity.BoxName"
                  clearable
                  class="dialog-input"
                  :disabled="dialogStatus=='update'"
                  placeholder="请选择需要存放的载具"
                />
              </span>
              <span>
                <el-button
                  v-if="Level==3"
                  class="filter-button"
                  type="primary"
                  icon="el-icon-search"
                  @click=" handleBox()"
                />
              </span>
            </el-form-item>

            <el-form-item v-if="Level==3" :label="'载具深度(mm)'">
              <el-input
                v-model="AllEntity.BoxLength"
                clearable
                class="dialog-input"
                placeholder="请选择载具类别"
                disabled
              />
            </el-form-item>
            <el-form-item v-if="Level==3" :label="'载具宽度(mm)'">
              <el-input
                v-model="AllEntity.BoxWidth"
                clearable
                class="dialog-input"
                placeholder="请选择载具类别"
                disabled
              />
            </el-form-item>
            <div v-if="Level==3" style="margin:10px">
              <template>
                <el-radio v-model="radio" label="1" @change="changeLayoutDirc">横向摆放</el-radio>
                <el-radio v-model="radio" label="2" @change="changeLayoutDirc">纵向摆放</el-radio>
              </template>
            </div>
            <el-form-item v-if="Level==3" :label="'X轴存放数量'" prop="XCount">
              <el-input v-model="XCount" clearable class="dialog-input" placeholder="请输入X轴存放数量" />
            </el-form-item>
            <el-form-item v-if="Level==3" :label="'Y轴存放数量'" prop="YCount">
              <el-input v-model="YCount" clearable class="dialog-input" placeholder="请输入Y轴存放数量" />
            </el-form-item>
            <div v-if="Level==3" style="padding:10px">
              <span>
                <el-button type="primary" @click="createBox">添加载具</el-button>
              </span>
              <span>
                <el-button type="info" @click="moveInX">原点X轴平移</el-button>
              </span>
              <span>
                <el-button type="info" @click="moveInY">原点Y轴平移</el-button>
              </span>
              <span style="color:red;font-size:20px">当前层:{{ CurTray }}</span>
            </div>
          </el-col>
          <el-col :span="8">
            <el-card v-if="Level==3" :body-style="{padding:'5px'}" align="middle">
              <div style="margin:5px;float:right">
                <span style="margin-right:5px;">
                  <el-button type="danger" icon="el-icon-delete" circle @click="deletedMaterial" />
                </span>
                <span style="margin-right:5px;">灯号</span>
                <span style="margin-right:0px;">{{ CurXLight}}</span>
                <span style="margin-right:0px;">--</span>
                <span style="margin-right:0px;">{{CurYLight }}</span>
                <span style="margin-right:5px;">绑定物料</span>
                <span>
                  <el-select
                    v-model="SuggestMaterial.Code"
                    filterable
                    placeholder="请选择绑定物料"
                    style="width:400PX"
                    clearable
                    @change="handleChangeSuggest"
                  >
                    <el-option
                      v-for="item in BoxMaterialMapList"
                      :key="item.Name"
                      :label="item.Name"
                      :value="item.Code"
                    />
                  </el-select>
                </span>
              </div>
              <!-- <el-image :src="materialUrl" fit="contain" style="width: 300px; height: 200px">
                <div slot="error" class="image-slot">
                  <i class="el-icon-picture-outline" />
                </div>
              </el-image>-->
              <img :src="materialUrl" fit="contain" style="width: 300px; height: 200px">
              <div>
                <span>最大存放数量：</span>
                <span>{{ SuggestMaterial.BoxCount }}</span>
              </div>
            </el-card>
          </el-col>
          <el-col :span="6">
            <el-card v-if="Level==3" :body-style="{padding:'5px'}" align="middle">
              <div slot="header" class="clearfix">
                <span>绑定载具</span>
              </div>
              <img
                :src="src"
                style="width: 300px; height: 200px;margin:5px; display: block;"
                fit="fit"
              >
            </el-card>
          </el-col>
        </el-row>
        <div v-if="Level==3">
          <el-card style="margin-top:20px">
            <div style="margin-bottom:10px;color:#909399;margin-left:50px">
              <img src="./X轴.png"><span style="color:red;font-size:20px">X轴(托盘出口方向)</span></img>
              <div style="color:red;font-size:20px">{{ '原点' }}</div>
            </div>
            <el-row :gutter="20">
              <el-col :span="1">
                <img src="./Y轴.png"><span style="color:red;font-size:20px">Y轴</span></img>
              </el-col>
              <el-col :span="19">
                <topoMain
                  ref="topoMain"
                  style="width:2000px;height:300px "
                  @changeOrginPos="changeTheOrginPos"
                  @moveItem="confimeLightX"
                  @clickBox="handleClickBoxComp"
                  @clearComp="clearCompent"
                />
              </el-col>
            </el-row>
          </el-card>
        </div>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData">确认</el-button>
        <el-button v-else type="primary" @click="updateData">确认</el-button>
      </div>
    </el-dialog>

    <!-- 关联载具 -->
    <el-dialog
      v-el-drag-dialog
      title="载具信息"
      :visible.sync="dialogBoxVisible"
      :width="'60%'"
      :close-on-click-modal="false"
      :fullscreen="true"
    >
      <div class="filter-container" style="margin-bottom:20px">
        <el-input
          v-model="listBoxQuery.MaterialCode"
          placeholder="物料编码或物料名称"
          class="filter-item"
          clearable
          @keyup.enter.native="handleBoxFilter"
          @clear="handleBoxFilter"
        />
        <el-input
          v-model="listBoxQuery.Code"
          placeholder="载具编码"
          class="filter-item"
          clearable
          @keyup.enter.native="handleBoxFilter"
          @clear="handleBoxFilter"
        />
        <el-input
          v-model="listBoxQuery.Name"
          placeholder="载具名称"
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
        >查询</el-button>
      </div>
      <el-table
        key="0"
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
        <el-table-column :label="'载具箱编码'" width="140" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'图片信息'" width="120" align="center" show-overflow-tooltip>
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
        <el-table-column :label="'载具箱名称'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Name }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'载具颜色'" width="80" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BoxColour }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'载具深度(mm)'" width="120" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BoxLength }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'载具宽度(mm)'" width="120" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BoxWidth }}</span>
          </template>
        </el-table-column>

        <el-table-column :label="'类别'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row. Type }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'虚拟载具'" width="80" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <el-switch v-model="scope.row.IsVirtual" disabled />
          </template>
        </el-table-column>
        <el-table-column :label="'描述'" align="center" show-overflow-tooltip>
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
        <el-button @click="dialogBoxVisible = false">取消</el-button>
        <el-button type="primary" @click="updateBox">确认</el-button>
      </div>
    </el-dialog>

    <el-dialog
      v-el-drag-dialog
      :visible.sync="dialogWriteVisible"
      :width="'70%'"
      :close-on-click-modal="false"
      :title="'用户组权限设置-'+ textTitle[Level] +':'+AllEntity.Code"
    >
      <el-row :gutter="20">
        <el-col :span="10">
          <el-table
            :key="1"
            v-loading="listLoading"
            :data="Rolelist"
            :header-cell-style="{background:'#F5F7FA'}"
            height="300"
            size="mini"
            border
            fit
            highlight-current-rows
            style="width: 100%;min-height:100%;"
            @current-change="handleChooseRole"
          >
            <el-table-column type="index" width="50" />
            <el-table-column label="选择用户组" align="center">
              <template slot-scope="scope">
                <span>{{ scope.row.Name }}</span>
              </template>
            </el-table-column>
          </el-table>
          <div class="pagination-container">
            <el-pagination
              :current-page="listRoleQuery.Page"
              :page-sizes="[10,20,30, 50]"
              :page-size="listRoleQuery.Rows"
              :total="Roletotal"
              background
              layout="total, sizes, prev, pager, next, jumper"
              @size-change="handleRoleSizeChange"
              @current-change="handleRoleCurrentChange"
            />
          </div>
        </el-col>
        <el-col :span="14">
          <el-table
            :key="4"
            ref="multipleTable"
            v-loading="listLoading"
            :data="data1"
            :header-cell-style="{background:'#F5F7FA'}"
            height="280"
            size="mini"
            border
            fit
            highlight-current-row
            style="width: 100%;min-height:100%;"
            @selection-change="handleSelectionChange"
          >
            <el-table-column type="selection" width="55" />
            <el-table-column type="index" width="50" />
            <el-table-column label="工号" align="center">
              <template slot-scope="scope">
                <span>{{ scope.row.Code }}</span>
              </template>
            </el-table-column>
            <el-table-column label="姓名" align="center">
              <template slot-scope="scope">
                <span>{{ scope.row.Name }}</span>
              </template>
            </el-table-column>
          </el-table>
        </el-col>
      </el-row>
      <div v-if="Level==2">
        <span>
          <span>
            <div style="display: inline-block;">
              <h4>操作名单</h4>
            </div>
          </span>
          <span>
            <el-button
              type="text"
              icon="el-icon-delete"
              style="margin-left:5px"
              @click="handleClear"
            >清空</el-button>
          </span>
        </span>
        <el-row>
          <el-table
            :key="2"
            v-loading="listLoading"
            :data="writeUserList"
            :header-cell-style="{background:'#F5F7FA'}"
            height="200"
            size="mini"
            border
            fit
            highlight-current-row
            style="width: 100%;min-height:100%;"
          >
            <el-table-column type="index" width="50" />
            <el-table-column label="工号" align="center">
              <template slot-scope="scope">
                <span>{{ scope.row.UserCode }}</span>
              </template>
            </el-table-column>
            <el-table-column label="姓名" align="center">
              <template slot-scope="scope">
                <span>{{ scope.row.UserName }}</span>
              </template>
            </el-table-column>
            <el-table-column
              :label="'操作'"
              align="center"
              width="320"
              class-name="small-padding fixed-width"
              fixed="right"
            >
              <template slot-scope="scope">
                <el-button size="mini" type="danger" @click="handleDeleteUser(scope.row)">删除</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-row>
      </div>

      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogWriteVisible = false">取消</el-button>
        <el-button type="primary" @click="createTrayUserMap">确认</el-button>
      </div>
    </el-dialog>

    <!-- 选择设备类型 -->
    <el-dialog
      v-el-drag-dialog
      title="设备类型"
      :visible.sync="dialogEquipmentVisible"
      :width="'60%'"
      :close-on-click-modal="false"
      :fullscreen="true"
    >
      <div class="filter-container" style="margin-bottom:20px">
        <el-input
          v-model="listEquipmentQuery.Code"
          placeholder="设备类型编码"
          class="filter-item"
          clearable
          @keyup.enter.native="handleEquipmentFilter"
          @clear="handleEquipmentFilter"
        />
        <el-button
          v-waves
          class="filter-button"
          type="primary"
          icon="el-icon-search"
          @click="handleEquipmentFilter"
        >查询</el-button>
      </div>
      <el-table
        key="0"
        v-loading="listLoading"
        :data="Equipmentlist"
        :header-cell-style="{background:'#F5F7FA'}"
        border
        fit
        size="mini"
        highlight-current-row
        style="width:100%;min-height:100%;"
        @row-click="handleEquipmentRowClick"
      >
        <el-table-column type="index" width="50" />
        <el-table-column :label="'设备型号'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'图片'" align="center" width="=50" show-overflow-tooltip>
          <template slot-scope="scope">
            <div class="image-picture">
              <el-image
                :src="BaseUrl+scope.row.PictureUrl"
                fit="contain"
                :preview-src-list="[BaseUrl+scope.row.PictureUrl]"
                style="width: 50px; height: 40px"
              >
                <div slot="error" class="image-slot">
                  <i class="el-icon-picture-outline" />
                </div>
              </el-image>
            </div>
          </template>
        </el-table-column>
        <el-table-column :label="'型号描述'" width="260" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remark }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'品牌'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BrandDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'类型'" width="120" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.TypeDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'添加信息'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedUserName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'添加时间'" align="center" show-overflow-tooltip width="280">
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedTime }}</span>
          </template>
        </el-table-column>
      </el-table>
      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          :current-page="listEquipmentQuery.Page"
          :page-sizes="[10,20,30, 50]"
          :page-size="listEquipmentQuery.Rows"
          :total="Equipmenttotal"
          background
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleEquipmentSizeChange"
          @current-change="handleEquipmentCurrentChange"
        />
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogEquipmentVisible = false">取消</el-button>
        <el-button type="primary" @click="updateEquipment">确认</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import { getLocationByTrayId, ouLoadMaterialLocationInfo, getLocationByLayoutId, getMaterialPageRecords, getTrayUserMapByTrayId, addBatchTrayUserMap, editTrayLocation, getTrayById, getTrayLayoutById, createtray, createContainer, editTray, deleteTray, getWareHouseLocations, getAreaLocations, getTrayLocations, getWareHouseTreeData, getPageRecords, createWareHouse, editWareHouse, editContainer, editLocation, deleteWareHouse, deleteContainer, deleteLocation, PostStartContainer, PostRestoreContainer, postDoDeleteLocationByTrayId } from '@/api/WareHouse'
import { getBoxMaterialMapByCode, getBoxByCode, getBoxPageRecords } from '@/api/Box'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import PrintToLodop from '@/utils/PrintToLodop.js' // 引入附件的js文件
import { PageRecordsEquipmentType } from '@/api/EquipmentType.js'
import topoMain from './components/topo/TopoMain'
const uuidv1 = require('uuid/v1')
import { fetchList } from '@/api/SysManage/Role'
import { getUserInfoByRole } from '@/api/SysManage/User'

export default {
  name: 'WareHouse', // 仓库信息
  directives: {
    elDragDialog,
    waves
  },
  components: {
    topoMain
  },
  data() {
    const validateX = (rule, value, callback) => {
      if (this.XCount > this.LimitXCount) {
        callback(new Error('超出X轴方向可存放数量，最多存放' + this.LimitXCount))
      } else {
        callback()
      }
    }
    const validateY = (rule, value, callback) => {
      if (this.YCount > this.LimitYCount) {
        callback(new Error('超出Y轴方向可存放数量，最多存放' + this.LimitYCount))
      } else {
        callback()
      }
    }
    return {
      treeProps: {
        label: 'Name',
        children: 'children'
      },
      CurXLight: 0,
      CurYLight: 0,
      CurTray: 0,
      // 复选框数据start
      dialogFormModel: false,
      isIndeterminate: false,
      checkAll: false,
      checkedWareHouseCode: [],
      wareHouses: [],
      wareHouseOptions: [],
      loadding: false,
      // 复选框数据end
      treeData: [],
      tableHeight: window.innerHeight - 320,
      tableHeight1: window.innerHeight - 245,
      list: undefined,
      // 分页显示总查询数据
      total: null,
      listLoading: false,
      loading: false,
      materialList: [],
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 15,
        Name: '',
        Code: '',
        Sort: 'Id',
        Level: 0,
        WareHouseCode: '',
        SearchCode: ''
      },
      // 分页查询
      listEquipmentQuery: {
        Page: 1,
        Rows: 10,
        Code: '',
        Status: undefined,
        Sort: 'id',
        Name: ''
      },
      dialogFormRangeCreate: false,
      QueryCode: '',
      // 创建弹出框
      dialogFormVisible: false,
      dialogStatus: '',
      fullscreenLoading: false,
      textMap: {
        update: '编辑',
        create: '创建'
      },
      textTitle: {
        0: '仓库信息',
        1: '货柜信息',
        2: '托盘信息',
        3: '储位信息'
      },
      Level: 0,
      // 输入规则
      rules: {
        Code: [{ required: true, message: '请输入编码', trigger: 'blur' }],
        Name: [{ required: true, message: '请输入名称', trigger: 'blur' }],
        EquipmentCode: [{ required: true, message: '请选择设备类别', trigger: 'blur' }],
        XCount: [{ required: true, trigger: 'change', validator: validateX }],
        YCount: [{ required: true, trigger: 'change', validator: validateY }],
        TrayWidth: [{ required: true, message: '请输入托盘宽度', trigger: 'blur' }],
        TrayLength: [{ required: true, message: '请输入托盘深度', trigger: 'blur' }],
        XNumber: [{ required: true, message: '请输入X轴灯号', trigger: 'blur' }],
        YNumber: [{ required: true, message: '请输入Y轴灯号', trigger: 'blur' }],
        MaxWeight: [{ required: true, message: '请输入单托盘承重', trigger: 'blur' }],
        TrayNumber: [{ required: true, message: '请输入设备托盘数', trigger: 'blur' }]
      },
      WareHouseEntity: {
        Id: undefined,
        Code: '',
        Name: '',
        Address: '',
        CategoryDict: '',
        IsVirtual: false,
        AllowManage: true,
        CreatedUserCode: '',
        CreatedUserName: '',
        CreatedTime: undefined,
        Remark: ''
      },
      TrayEntity: {
        Id: undefined,
        Code: '',
        Name: '',
        CategoryDict: '',
        WareHouseCode: '',
        CreatedUserCode: '',
        CreatedUserName: '',
        CreatedTime: undefined,
        Remark: ''
      },
      LocationEntity: {
        Id: undefined,
        Code: '',
        Row: '',
        Column: '',
        Depth: 0,
        Height: 0,
        Enabled: true,
        IsLocked: false,
        SuggestMaterialCode: '',
        WareHouseCode: '',
        ContainerCode: '',
        ShelfCode: '',
        Remark: '',
        CreatedUserCode: '',
        CreatedUserName: '',
        CreatedTime: undefined
      },
      AllEntity: {
        Id: undefined,
        Code: '',
        Name: '',
        Address: '',
        CategoryDict: '',
        IsVirtual: false,
        AllowManage: true,
        CreatedUserCode: '',
        CreatedUserName: '',
        CreatedTime: undefined,
        Remark: '',
        WareHouseCode: '',
        XLight: '',
        YLight: '',
        Depth: 0,
        Height: 0,
        MaxWeight: 0,
        Enabled: true,
        IsLocked: false,
        SuggestMaterialCode: '',
        ContainerCode: '',
        ShelfCode: '',
        Level: this.Level,
        DeviceAddress: 0,
        IsScanned: false,
        DeviceType: 1,
        EquipmentCode: '',
        EquipmentType: '',
        TrayWidth: '',
        TrayLength: '',
        Ip: '',
        Port: '',
        XNumber: '',
        YNumber: '',
        BoxCode: '',
        BoxLength: '',
        BoxWidth: '',
        LockQuantity: 0
      },
      // 条码打印
      tempPrintlist: [],
      // 打印时间
      printDate: null,
      page: {
        width: 400, // 240
        height: 300, // //750,
        pagetype: '',
        intOrient: 1// 1纵向打印  2 横向打印
      },
      barCode: '',
      controls: [],
      // 设备型号选择
      dialogEquipmentVisible: false,
      Equipmentlist: [],
      Equipmenttotal: 0,
      src: '/logo.png',
      materialUrl: '/logo.png',
      BaseUrl: window.PLATFROM_CONFIG.baseUrl, // 服务默认地址，
      // 存放载具
      dialogBoxVisible: false,
      // 分页查询
      listBoxQuery: {
        Page: 1,
        Rows: 10,
        Code: '',
        Status: undefined,
        Sort: 'id',
        Name: ''
      },
      Boxlist: [],
      Boxtotal: 0,
      // 布局坐标原点
      origin: {
        xp: 0,
        yp: 0
      },
      XCount: 0,
      YCount: 0,
      LimitXCount: 0,
      LimitYCount: 0,
      Scale: 3, // 托盘尺寸与像素的比例为3：1,
      xLightDist: 0,
      BoxMaterialMapList: [], // 物料载具绑定关系
      SuggestMaterial: {
        Code: ''
      },
      selectComp: {},
      locationList: [],
      orgLocationList: [],
      // 货柜权限维护
      dialogWriteVisible: false,
      data1: [],
      writeUser: [],
      writeUserList: [],
      totalUser: [],
      Rolelist: [],
      Roletotal: 0,
      orgList: [],
      // 分页查询
      listRoleQuery: {
        Page: 1,
        Rows: 10,
        Code: '',
        Sort: 'id',
        Name: ''
      },
      radio: '1',
      multipleSelection: []
    }
  },
  computed: {
    configData: {
      get() {
        return this.$store.state.topoEditor.topoData
      },
      set(newValue) {
        this.Comment = newValue
      }
    }
  },
  watch: {
    // 授权面板关闭，清空原角色查询权限
    dialogFormVisible(value) {
      if (!value) {
        this.resetAllEntity()
        this.Box = undefined
        this.SuggestMaterial = {
          Code: ''
        }
        this.src = '/logo.png'
        this.materialUrl = '/logo.png'
      }
    },
    // 白名单面板开启获取用户信息
    dialogWriteVisible(value) {
      if (!value) {
        this.resetAuth()
      }
    }
  },
  created() {
    this.getTreeData()
    this.handleFilter()
    // 获取人员权限信息
    this.getRoleList()
    this.Comment = this.configData
  },
  methods: {
    // toggleSelection(rows) {
    //   if (rows) {
    //     rows.forEach(row => {
    //       this.$refs.multipleTable.toggleRowSelection(row)
    //     })
    //   } else {
    //     this.$refs.multipleTable.clearSelection()
    //   }
    // },
    // 清空绑定物料
    deletedMaterial() {
      getLocationByLayoutId(this.selectComp.identifier).then(response => {
        var resData = JSON.parse(response.data.Content)
        if (resData.LockMaterialCode === null || resData.LockMaterialCode === '') {
          this.$confirm('清空绑定物料信息后，请点击右下角确认按钮', '提示', {
            confirmButtonText: '知道了',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            // 需要对数据进行数量，否则会出现this导致的作用域错误
            this.SuggestMaterial = {
              Code: ''
            }
            this.materialUrl = '/logo.png'
            if (this.selectComp !== null) {
              // 找到该组件，并更新绑定的物料编码
              this.Comment.components.find((element) => (element.identifier === this.selectComp.identifier)).dataBind.biz = this.SuggestMaterial.Code
              // comment.dataBind.biz = this.SuggestMaterial.Code
              // comment.style.backColor = '#FFFFFF'
              //comment.style.borderColor = '#FFFFFF'
            }
          }).catch(() => {
            this.$message({
              type: 'info',
              message: '已取消删除'
            })
          })
        } else {
          this.$message({
            type: 'info',
            message: '该储位当前存在物料' + resData.LockMaterialCode + '库存，请先出库'
          })
          return
        }
      })
    },
    handleDeleteUser(row) {
      var index = this.writeUserList.findIndex(item => {
        if (item.UserCode === row.UserCode) {
          return true
        }
      })
      this.writeUserList.splice(index, 1)
      // const index = this.writeUserList.find(a => a.UserCode === row.UserCode)
      // console.log(index)
      // this.writeUserList.splice(index)
    },
    handleSelectionChange(val) {
      val.forEach(e => {
        if (!this.writeUserList.find(a => a.UserCode === e.Code)) {
          var entity = {
            UserCode: e.Code,
            UserName: e.Name
          }
          this.writeUserList.push(entity)
        }
      })
    },
    // 切换摆放方向
    changeLayoutDirc(e) {
      // 如果发生的切换，则转换BOX的长宽
      const orgBoxWidth = this.Box.BoxWidth
      const orgBoxLength = this.Box.BoxLength
      this.Box.BoxLength = orgBoxWidth
      this.Box.BoxWidth = orgBoxLength
      this.avaviableBoxNum()
    },
    moveInX() {
      var xp = this.origin.xp
      for (var i = 0; i < this.Comment.components.length; i++) {
        // 查找托盘
        if (this.Comment.components[i].name === 'shelf') {
          const xw = this.Comment.components[i].style.position.x + this.Comment.components[i].style.position.w
          if (xw > xp) {
            xp = xw
          }
        }
      }
      // 原点的长度
      if ((xp + 10) >= this.configData.layer.width) {
        this.$message({
          title: '失败',
          message: '原点坐标大于托盘坐标，请手动移动原点',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.origin.xp = xp
      // 找到该组件，并更新X轴位置
      this.Comment.components.find((element) => (element.name === 'orginPos')).style.position.x = this.origin.xp
      if (this.Box.BoxWidth !== undefined) {
        this.avaviableBoxNum()
      }
    },
    moveInY() {
      var yp = this.origin.yp
      for (var i = 0; i < this.Comment.components.length; i++) {
        // 查找托盘
        if (this.Comment.components[i].name === 'shelf') {
          const yh = this.Comment.components[i].style.position.y + this.Comment.components[i].style.position.h
          if (yh > yp) {
            yp = yh
          }
        }
      }
      // 原点的长度
      if ((yp + 10) >= this.configData.layer.height) {
        this.$message({
          title: '失败',
          message: '原点坐标大于托盘坐标，请手动移动原点',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.origin.yp = yp
      // 找到该组件，并更新X轴位置
      this.Comment.components.find((element) => (element.name === 'orginPos')).style.position.y = this.origin.yp
      if (this.Box.BoxLength !== undefined) {
        this.avaviableBoxNum()
      }
    },
    /** ****************/
    /* 获取仓库信息 */
    /** ****************/
    getList() {
      this.listLoading = true
      getPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        console.log(this.list)
        // this.wareHouses = this.list
        this.total = usersData.total
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    getTreeData() {
      getWareHouseTreeData(0).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.treeData = this.convertTreeData(usersData)
      })
    },
    convertTreeData(data) {
      const treedataList = []
      var entity = {
        Code: 'WareHouse',
        Name: '仓库信息'
      }
      var first = this.generateRouter(entity, true, 0)
      var firstchildren = []
      data.forEach(item => {
        var parent = this.generateRouter(item, true, 1)
        var containerList = []
        var twoChild = []
        containerList = item.ContainerList
        var trayList = []
        trayList = item.TrayList
        containerList.forEach(area => {
          var threeChild = []
          trayList.forEach(channel => {
            if (channel.ContainerCode === area.Code && channel.WareHouseCode === area.WareHouseCode) {
              threeChild.push(this.generateRouter(channel, false, 3))
            }
          })
          var twoParent = this.generateRouter(area, false, 2)
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
    generateRouter(item, isParent, level) {
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
    handleQuery() {
      this.listQuery.Page = 1
      this.Level = 3
      this.getQueryMaterialList()
    },
    // 查询建议物料限行
    getQueryMaterialList() {
      this.listLoading = false
      getMaterialPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        // this.wareHouses = this.list
        this.total = usersData.total
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    // 数据筛选
    handleFilter() {
      this.listQuery.Page = 1
      // this.Level = 3
      this.listQuery.Level = this.Level // 查询库位码
      this.getList()
    },
    // 切换分页数据-行数据
    handleSizeChange(val) {
      this.listQuery.Level = this.Level
      this.listQuery.Rows = val
      this.getList()
    },
    // 切换分页-列数据
    handleCurrentChange(val) {
      this.listQuery.Level = this.Level
      this.listQuery.Page = val
      this.getList()
    },
    /** ****************/
    /* 节点点击 */
    /** ****************/
    handleNodeClick(data) {
      this.Level = data.Level
      this.listQuery.Level = this.Level
      this.listQuery.Code = data.Code
      this.listQuery.Page = 1
      if (data.Level === 1) {
        this.AllEntity.WareHouseCode = data.Code
      }
      if (data.Level === 2) {
        this.AllEntity.ContainerCode = data.Code
        this.AllEntity.WareHouseCode = data.WareHouseCode
        this.listQuery.WareHouseCode = data.WareHouseCode
      }
      if (data.Level === 3) {
        this.Comment = this.configData
        // 重置计算信息
        this.Box = []
        this.XCount = 0
        this.YCount = 0
        this.AllEntity.ContainerCode = data.ContainerCode
        this.AllEntity.WareHouseCode = data.WareHouseCode
        this.listQuery.WareHouseCode = data.WareHouseCode
        this.AllEntity.TrayId = data.Id
        this.listQuery.Code = data.Id
        this.CurTray = data.Code
        // 获取本托盘信息
        getTrayById(data.Id).then(response => {
          var result = JSON.parse(response.data.Content)
          this.TrayEntity = result
        })
        // getTrayLayoutById(data.Id).then(response => {
        //   console.log(response)
        //   var result = JSON.parse(response.data.Content)
        //   console.log(result)
        //   var jsonData = JSON.parse(result.JsonLayout)
        //   this.layjson = jsonData
        //   this.orgLocationList = result.LocationList
        //   // for (this.indexi = 0; this.indexi < jsonData.components.length; this.indexi++) {
        //   //   // 查找托盘
        //   //   if (jsonData.components[this.indexi].name === 'orginPos') {

        //   //   }
        //   // }
        //   setTimeout(() => {
        //     this.transcolor()
        //   }, 1 * 2000)
        // })
      }
      this.getList()
    },
    /** ****************/
    /* 创建按钮 */
    /** ****************/
    handleCreate() {
      this.resetAllEntity()
      this.dialogStatus = 'create'
      // 加载布局
      if (this.Level === 3) {
        if (this.TrayEntity.Code === '') {
          this.$message({
            title: '错误',
            message: '请选择一个托盘',
            type: 'warning',
            duration: 2000
          })
          return
        }
        this.fullscreenLoading = true
        this.loadingLayout()
        setTimeout(() => {
          this.fullscreenLoading = false
          this.dialogFormVisible = true
        }, 1 * 2000)
      } else {
        this.dialogFormVisible = true
      }
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    handleDeleteTrayLocation() {
      if (this.Level != 3 || !this.TrayEntity.Code) {
        this.$message({
          message: '请选择一个托盘',
          type: 'warning'
        })
        return
      }
      postDoDeleteLocationByTrayId(this.TrayEntity).then(res => {
        const resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.$message({
            type: 'success',
            message: '删除成功'
          })
          this.getList()
        } else {
          this.$message({
            type: 'error',
            message: `删除失败：${resData.Message}`
          })
        }
      })
    },
    // 初始加载布局
    loadingLayout() {
      this.configData.layer.width = this.TrayEntity.TrayWidth / this.Scale
      this.configData.layer.height = this.TrayEntity.TrayLength / this.Scale
      // 计算X轴指示灯间隔
      this.xLightDist = this.configData.layer.width / this.TrayEntity.XNumber
      // 计算Y轴指示灯间隔
      this.yLightDist = this.configData.layer.height / this.TrayEntity.YNumber
      if (this.TrayEntity.LayoutJson !== null && this.TrayEntity.LayoutJson !== '') {
        const a = JSON.parse(this.TrayEntity.LayoutJson)
        this.configData.components = a.components
        this.configData.layer = a.layer
        this.configData.name = a.name

        const org = this.Comment.components.find((element) => (element.name === 'orginPos')).style.position
        this.origin.xp = org.x
        this.origin.yp = org.y
      } else {
        this.orginLayout()
      }
      // for (var i = 0; i < this.orgLocationList.length; i++) {
      //   var layout = this.orgLocationList[i].LayoutId
      //   var component = this.layjson.components.find((element) => (element.identifier === layout))
      //   if (component) {
      //     console.log(component)
      //     component.style.backColor = 'red'
      //   }
      // }
      for (var i = 0; i < this.configData.components.length; i++) {
        // var layout = this.locationList[i].LayoutId
        var component = this.configData.components[i]// this.layjson.components.find((element) => (element.identifier === layout))
        if (component) {
          console.log(component)
          //  backColor:component.dataBind.biz==''? component.style.backColor:'#67C23A',
          if (component.dataBind.biz == ''){
            component.style.backColor = '#FFFFFF'
            component.style.borderColor = '#FFFFFF'
          }
          if (component.dataBind.biz != '') {
            component.style.backColor = '#67C23A'
            component.style.borderColor = '#67C23A'
            //  component.style.backColor = '#67C23A'

            //   component.canvas.style.background = "background:red"
            // component.style.borderWidth = component.style.position.w - 5;//component.style.width

            // component.style.position.w + 'px'
          } else {
            if (component.identifier != '') {
              // component.style.backColor = 'red'
              component.style.borderColor = 'red'
              if (component.style.url == 'orgin.svg') {
                component.style.borderColor = 'yellow'
                component.style.backColor = 'yellow'
                //  component.style.width= '20px'
                //  component.style.height='20px'
                component.style.position.w = component.style.position.w + 10
                component.style.position.h = component.style.position.h + 10
              }
            }
          }
        }
      }
    },
    orginLayout() {
      // 初始原点坐标
      this.origin = {
        xp: 0,
        yp: 0
      }
      // 临时使用的原点坐标
      var tempOrigin = {
        xp: 0,
        yp: 0
      }
      for (var i = 0; i < this.TrayEntity.XNumber; i++) {
        var comp = {
          type: 'svg-image',
          action: [],
          dataBind: {
            sn: i + 1,
            title: '',
            biz: 'x',
            queryParam: {}
          },
          style: {
            position: {
              x: tempOrigin.xp + (this.xLightDist / 2),
              y: 0,
              w: 10,
              h: 10
            },
            backColor: 'transparent',
            zIndex: 999,
            url: 'light.svg',
            visible: true,
            transform: 0,
            borderWidth: 1,
            borderStyle: 'solid',
            borderColor: 'transparent',
            temp: {
              position: {
                x: tempOrigin.xp,
                y: 0
              }
            }
          },
          identifier: uuidv1(),
          name: 'light'
        }
        this.configData.components.push(comp)
        tempOrigin.xp = tempOrigin.xp + this.xLightDist
      }
      for (var j = 0; j < this.TrayEntity.YNumber; j++) {
        var compy = {
          type: 'svg-image',
          action: [],
          dataBind: {
            sn: j + 1,
            title: '',
            biz: 'y',
            queryParam: {}
          },
          style: {
            position: {
              x: 0,
              y: tempOrigin.yp + (this.yLightDist / 2),
              w: 10,
              h: 10
            },
            backColor: 'transparent',
            zIndex: 999,
            url: 'light.svg',
            visible: true,
            transform: 0,
            borderWidth: 1,
            borderStyle: 'solid',
            borderColor: 'transparent',
            temp: {
              position: {
                x: 0,
                y: tempOrigin.yp
              }
            }
          },
          identifier: uuidv1(),
          name: 'light'
        }
        this.configData.components.push(compy)
        tempOrigin.yp = tempOrigin.yp + this.yLightDist
      }
      // 增加原点坐标
      var orgin = {
        type: 'svg-image',
        action: [],
        dataBind: {
          sn: '',
          title: '',
          biz: '',
          queryParam: {}
        },
        style: {
          position: {
            x: 0,
            y: 0,
            w: 10,
            h: 10
          },
          backColor: 'transparent',
          zIndex: 999,
          url: 'orgin.svg',
          visible: true,
          transform: 0,
          borderWidth: 1,
          borderStyle: 'solid',
          borderColor: 'transparent',
          temp: {
            position: {
              x: 0,
              y: 0
            }
          }
        },
        identifier: uuidv1(),
        name: 'orginPos'
      }
      this.configData.components.push(orgin)
    },
    // 添加创建
    createData() {
      this.AllEntity.MaxWeight = this.AllEntity.MaxWeight * 1000
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          if (this.Level === 0) {
            createWareHouse(this.AllEntity).then((res) => {
              var resData = JSON.parse(res.data.Content)
              if (resData.Success) {
                this.dialogFormVisible = false
                // this.getList()
                // this.getTreeData()
                this.$message({
                  title: '成功',
                  message: '创建成功',
                  type: 'success',
                  duration: 2000
                })
                this.handleFilter()
                this.getTreeData()
              } else {
                this.$message({
                  title: '失败',
                  message: '创建失败：' + resData.Message,
                  type: 'error',
                  duration: 2000
                })
              }
            })
          } else if (this.Level === 1) {
            createContainer(this.AllEntity).then((res) => {
              var resData = JSON.parse(res.data.Content)
              if (resData.Success) {
                this.dialogFormVisible = false
                // this.getList()
                // this.getTreeData()
                this.$message({
                  title: '成功',
                  message: '创建成功',
                  type: 'success',
                  duration: 2000
                })
                this.handleFilter()
                this.getTreeData()
              } else {
                this.$message({
                  title: '失败',
                  message: '创建失败：' + resData.Message,
                  type: 'error',
                  duration: 2000
                })
                this.AllEntity.MaxWeight = this.AllEntity.MaxWeight / 1000
              }
            })
          } else if (this.Level === 2) {
            createtray(this.AllEntity).then((res) => {
              var resData = JSON.parse(res.data.Content)
              if (resData.Success) {
                this.dialogFormVisible = false
                this.$message({
                  title: '成功',
                  message: '创建成功',
                  type: 'success',
                  duration: 2000
                })
                this.handleFilter()
                this.getTreeData()
              } else {
                this.AllEntity.MaxWeight = this.AllEntity.MaxWeight / 1000
                this.$message({
                  title: '失败',
                  message: '创建失败：' + resData.Message,
                  type: 'error',
                  duration: 2000
                })
              }
            })
          } else {
            // 创建库位
            this.transLocation()
            this.TrayEntity.LocationList = JSON.stringify(this.locationList)
            this.TrayEntity.LayoutJson = JSON.stringify(this.configData)
            editTrayLocation(this.TrayEntity).then((res) => {
              var resData = JSON.parse(res.data.Content)
              if (resData.Success) {
                // this.list.unshift(this.Role)
                this.dialogFormVisible = false

                // this.getList()
                // this.getTreeData()
                this.$message({
                  title: '成功',
                  message: '更新成功',
                  type: 'success',
                  duration: 2000
                })
                this.handleFilter()
                this.getTreeData()
                this.configData.components = []
              } else {
                this.$message({
                  title: '失败',
                  message: '更新失败' + resData.Message,
                  type: 'error',
                  duration: 2000
                })
              }
            })
          }
        }
      })
    },

    /** ****************/
    /* 获取货柜类别信息 */
    /** ****************/
    getEquipmentList() {
      PageRecordsEquipmentType(this.listEquipmentQuery).then(response => {
        var result = JSON.parse(response.data.Content)
        this.Equipmentlist = result.rows
        this.Equipmenttotal = result.total
      })
    },
    handleEquipment() {
      this.dialogEquipmentVisible = true
      this.getEquipmentList()
    },
    // 数据筛选
    handleEquipmentFilter() {
      this.listEquipmentQuery.Page = 1
      this.getEquipmentList()
    },
    // 切换分页数据-行数据
    handleEquipmentSizeChange(val) {
      this.listEquipmentQuery.Rows = val
      this.getEquipmentList()
    },
    // 切换分页-列数据
    handleEquipmentCurrentChange(val) {
      this.listEquipmentQuery.Page = val
      this.getEquipmentList()
    },
    handleEquipmentRowClick(row) {
      this.Equipment = row
    },
    updateEquipment() {
      if (this.Equipment === null) {
        this.$message({
          title: '失败',
          message: '请选择一个设备类别',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.src = this.BaseUrl + this.Equipment.PictureUrl
      this.AllEntity.EquipmentCode = this.Equipment.Code
      this.AllEntity.BrandDescription = this.Equipment.BrandDescription
      this.AllEntity.EquipmentTypeDescription = this.Equipment.TypeDescription
      this.dialogEquipmentVisible = false
    },
    /** ****************/
    /* 权限白名单 */
    /** ****************/
    handleTrayUserMap(row) {
      this.AllEntity = Object.assign({}, row) // copy obj
      // 如果针对单个托盘维护权限
      if (this.Level === 1) {
        this.$confirm('是否重置该货柜下所有托盘的用户操作权限', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          this.dialogWriteVisible = true
        }).catch(() => {
          this.$message({
            type: 'info',
            message: '已取消删除'
          })
        })
      }
      // 如果针对单个托盘维护权限
      if (this.Level === 2) {
        this.getWriteList() // 获取该托盘下的用户
        this.dialogWriteVisible = true
      }
    },
    // 数据筛选
    handleRoleFilter() {
      this.listRoleQuery.Page = 1
      // this.Level = 3
      this.listRoleQuery.Level = this.Level // 查询库位码
      this.getRoleList()
    },
    // 切换分页数据-行数据
    handleRoleSizeChange(val) {
      this.listRoleQuery.Level = this.Level
      this.listRoleQuery.Rows = val
      this.getRoleList()
    },
    // 切换分页-列数据
    handleRoleCurrentChange(val) {
      this.listRoleQuery.Level = this.Level
      this.listRoleQuery.Page = val
      this.getRoleList()
    },
    getRoleList() {
      this.listLoading = true
      fetchList(this.listRoleQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.Rolelist = usersData.rows
        this.Roletotal = usersData.total

        // Just to simulate the time of the request
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 100)
      })
    },
    // 获取用户数据
    handleChooseRole(row) {
      // 清空当前数据
      this.data1 = []
      getUserInfoByRole(row.Code).then(response => {
        if (response.status === 200) {
          var resData = JSON.parse(response.data.Content)
          this.data1 = resData
          // resData.forEach((item, index) => {
          //   this.data1.push({
          //     label: item.Name,
          //     key: item.Code
          //   })
          // })
        } else {
          this.$message({
            title: '失败',
            message: '获取员工信息失败',
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handleClear() {
      this.writeUser = []
      this.writeUserList = []
    },
    getWriteList() {
      getTrayUserMapByTrayId(this.AllEntity.Id).then((res) => {
        if (res.status === 200) {
          var resData = JSON.parse(res.data.Content)
          // this.writeUser = resData
          // console.log(resData)
          // resData.forEach((item, index) => {
          //   this.writeUser.push(item.UserCode)
          // })
          this.writeUserList = resData
          //   this.handleChooseUser()
        } else {
          this.$message({
            title: '失败',
            message: '获取白名单失败',
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    createTrayUserMap() {
      // 批量维护货柜权限
      const temp = []
      this.writeUserList.forEach((item, index) => {
        temp.push(item.UserCode)
      })
      const map = {
        WareHouseCode: this.AllEntity.WareHouseCode,
        ContainerCode: this.AllEntity.Code,
        userCode: JSON.stringify(temp)
      }
      // 维护单个托盘权限
      if (this.Level === 2) {
        map.TrayId = this.AllEntity.Id
        map.ContainerCode = this.AllEntity.ContainerCode
      }
      addBatchTrayUserMap(map).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.$message({
            title: '成功',
            message: '白名单设置成功',
            type: 'success',
            duration: 2000
          })
          this.resetAuth()
          window.location.reload()
        } else {
          this.$message({
            title: '失败',
            message: '白名单设置失败',
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    /** ****************/
    /* 储位创建--载具创建 */
    /** ****************/
    handleBox() {
      this.dialogBoxVisible = true
      this.SuggestMaterial.Code = ''
      this.materialUrl = '/logo.png'
      this.getBoxList()
    },
    getBoxList() {
      getBoxPageRecords(this.listBoxQuery).then(response => {
        var result = JSON.parse(response.data.Content)
        this.Boxlist = result.rows
        this.Boxtotal = result.total
      })
    },
    updateBox() {
      if (this.Box === null) {
        this.$message({
          title: '失败',
          message: '请选择一条载具',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.src = this.BaseUrl + this.Box.PictureUrl
      this.AllEntity.BoxCode = this.Box.Code
      this.AllEntity.BoxName = this.Box.Name
      this.AllEntity.BoxLength = this.Box.BoxLength
      this.AllEntity.BoxWidth = this.Box.BoxWidth
      // 查询载具类别信息
      getBoxMaterialMapByCode(this.Box.Code).then(response => {
        var result = JSON.parse(response.data.Content)
        this.BoxMaterialMapList.push('')
        this.BoxMaterialMapList = result
        console.log(this.BoxMaterialMapList)
      })
      // 重新计算可存放数量
      this.avaviableBoxNum()
      this.dialogBoxVisible = false
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
    // 确认选择点击载具
    handleBoxRowClick(row) {
      this.Box = row
    },
    // 添加载具
    createBox() {
      if (this.XCount <= 0 || this.YCount <= 0) {
        this.$message({
          title: '失败',
          message: '添加错误,托盘空间不足',
          type: 'error',
          duration: 2000
        })
        return
      }
      // X 轴方向增加
      const boxW = this.Box.BoxWidth / this.Scale
      const boxL = this.Box.BoxLength / this.Scale

      // 记录初始坐标
      var tempOrg = {
        xp: this.origin.xp,
        yp: this.origin.yp
      }
      for (var i = 0; i < this.XCount; i++) {
        // Y 轴方向增加
        for (var j = 0; j < this.YCount; j++) {
          var comp = {
            type: 'rect',
            action: [],
            dataBind: {
              sn: this.Box.Code, // 载具类型
              title: 0, // 灯号
              biz: this.SuggestMaterial.Code, // 物料编码
              queryParam: {}
            },
            style: {
              position: {
                x: tempOrg.xp,
                y: tempOrg.yp,
                w: boxW,
                h: boxL
              },
              backColor: '#fff',
              zIndex: 1,
              url: '',
              visible: true,
              transform: 0,
              borderWidth: 1,
              borderStyle: 'solid',
              borderColor: 'red',
              temp: {
                position: {
                  x: tempOrg.xp,
                  y: tempOrg.yp
                }
              }
            },
            identifier: uuidv1(),
            name: 'shelf'
          }
          this.$refs.topoMain.onAddBox(comp) // 方法2:直接调用
          tempOrg.yp = boxL + tempOrg.yp
        }
        // 重置
        tempOrg.yp = this.origin.yp
        tempOrg.xp = boxW + tempOrg.xp
      }
      // 如果是X轴方向
      if (this.radio === '1') {
        this.origin.xp = this.origin.xp + (boxW * this.XCount)
        // 找到该组件，并更新X轴位置
        this.Comment.components.find((element) => (element.name === 'orginPos')).style.position.x = this.origin.xp
        if (this.Box.BoxWidth !== undefined) {
          this.avaviableBoxNum()
        }
      } else {
        this.origin.yp = this.origin.yp + (boxL * this.YCount)
        // 找到该组件，并更新X轴位置
        this.Comment.components.find((element) => (element.name === 'orginPos')).style.position.y = this.origin.yp
        if (this.Box.BoxLength !== undefined) {
          this.avaviableBoxNum()
        }
      }
      // 计算灯号
      this.confimeLightX()
    },
    /** ****************/
    /* 可视化效果--储位载具维护 */
    /** ****************/
    // 点击可视化布局中的载具
    handleClickBoxComp(component) {
      this.selectComp = component
      this.SuggestMaterial.Code = ''
      this.materialUrl = '/logo.png'
      this.CurXLight = component.dataBind.xlight
      if (this.CurXLight == undefined) {
        this.CurXLight = 1
      }
      this.CurYLight = component.dataBind.ylight
      if (this.CurYLight == undefined) {
        this.CurYLight = 1
      }
      // getLocationByLayoutId(component.identifier).then(response => {
      //   var resData = JSON.parse(response.data.Content)
      //   console.log('resData')
      //   console.log(resData)
      // })
      // 根据载具编码查询可存放物料
      getBoxByCode(component.dataBind.sn).then(response => {
        var result = JSON.parse(response.data.Content)
        this.Box = result
        this.src = this.BaseUrl + this.Box.PictureUrl
        this.AllEntity.BoxCode = this.Box.Code
        this.AllEntity.BoxName = this.Box.Name
        this.AllEntity.BoxLength = this.Box.BoxLength
        this.AllEntity.BoxWidth = this.Box.BoxWidth
        this.avaviableBoxNum()

        // 查询载具类别信息
        getBoxMaterialMapByCode(this.Box.Code).then(response => {
          var result = JSON.parse(response.data.Content)
          this.BoxMaterialMapList = result
          // if (result.length > 0) {
          //   this.SuggestMaterial = JSON.parse(JSON.stringify(result[0]))
          //   this.materialUrl = this.BaseUrl + this.SuggestMaterial.PictureUrl
          // }
          // 查找该载具所关联的物料
          this.SuggestMaterial = JSON.parse(JSON.stringify(this.BoxMaterialMapList.find((element) => (element.Code === component.dataBind.biz))))
          this.materialUrl = this.BaseUrl + this.SuggestMaterial.PictureUrl
        })
      })
    },
    // 清除选中组件
    clearCompent() {
      this.selectComp = null
    },
    // 更改建议物料
    handleChangeSuggest(data) {
      console.log('R---' + data)
      getLocationByLayoutId(this.selectComp.identifier).then(response => {
        var resData = JSON.parse(response.data.Content)
        if (resData.LockMaterialCode === null || resData.LockMaterialCode === '') {
          // 需要对数据进行数量，否则会出现this导致的作用域错误
          if (data == '') {
            this.SuggestMaterial = {
              Code: '',
              PictureUrl: ''
            }
          } else {
            this.SuggestMaterial = JSON.parse(JSON.stringify(this.BoxMaterialMapList.find((element) => (element.Code === data))))
          }

          this.materialUrl = this.BaseUrl + this.SuggestMaterial.PictureUrl
          if (this.selectComp !== null) {
            // 找到该组件，并更新绑定的物料编码
            var comment = this.Comment.components.find((element) => (element.identifier === this.selectComp.identifier))
            comment.dataBind.biz = this.SuggestMaterial.Code
            comment.style.backColor = '#67C23A'
            comment.style.borderColor = '#67C23A'
          }
        } else {
          this.$message({
            type: 'info',
            message: '该储位当前存在物料' + resData.LockMaterialCode + '库存，请先出库'
          })
          this.SuggestMaterial = {
            Code: ''
          }
          this.materialUrl = '/logo.png'
          return
        }
      })
    },
    // 计算X轴灯的位置
    confimeLightX() {
      const xlightDist = this.xLightDist
      const ylightDist = this.yLightDist
      for (var i = 0; i < this.Comment.components.length; i++) {
        // 查找是载具属性的
        if (this.Comment.components[i].name === 'shelf') {
          this.Comment.components[i].dataBind.xlenght = 0
          for (var j = 0; j < this.Comment.components.length; j++) {
            // 找到灯的属性
            if (this.Comment.components[j].name === 'light') {
              // 找到X轴方向灯的属性
              if (this.Comment.components[j].dataBind.biz === 'x') {
                // 载具的X方向坐标
                const shelfX = this.Comment.components[i].style.position.x
                // 载具的宽度
                const shelfW = this.Comment.components[i].style.position.w
                // 灯的X轴方向坐标
                const lightX = this.Comment.components[j].style.position.x
                console.log('计算灯号了')
                // 如果载具宽度大于X轴的灯间距
                if (shelfW > xlightDist) {
                  if (shelfX <= lightX && lightX <= (shelfX + shelfW)) {
                    this.Comment.components[i].dataBind.xlight = this.Comment.components[j].dataBind.sn
                    this.Comment.components[i].dataBind.xlenght = this.Comment.components[i].dataBind.xlenght + 1
                    console.log(this.Comment.components[i].dataBind)
                  }
                } else { // 载具的宽度小于X轴的灯间距
                  if (lightX < shelfX && shelfX <= (lightX + xlightDist)) {
                    this.Comment.components[i].dataBind.xlight = this.Comment.components[j].dataBind.sn + 1
                    this.Comment.components[i].dataBind.xlenght = 1
                    console.log(this.Comment.components[i].dataBind)
                  }
                  //  else if (shelfX >= 0 && shelfX < xlightDist) {
                  //   this.Comment.components[i].dataBind.xlight = 1
                  // }
                }
              }
              if (this.Comment.components[j].dataBind.biz === 'y') {
                const shelfY = this.Comment.components[i].style.position.y
                const shelfH = this.Comment.components[i].style.position.h
                const lightY = this.Comment.components[j].style.position.y
                if (shelfY > ylightDist) {
                  if (shelfY <= lightY && lightY <= (shelfY + shelfH)) {
                    this.Comment.components[i].dataBind.ylight = this.Comment.components[j].dataBind.sn
                  }
                } else {
                  if (lightY < shelfY && shelfY <= (lightY + ylightDist)) {
                    this.Comment.components[i].dataBind.ylight = this.Comment.components[j].dataBind.sn + 1
                  }
                  //  else if (shelfY >= 0 && shelfY < ylightDist) {
                  //   this.Comment.components[i].dataBind.ylight = 1
                  // }
                }
              }
            }
          }
        }
      }
    },
    // 移动坐标原点
    changeTheOrginPos(component) {
      this.origin = {
        xp: component.style.position.x,
        yp: component.style.position.y
      }
      this.avaviableBoxNum()
    },
    // 计算可存放的托盘数量
    avaviableBoxNum() {
      // 缩放处理
      const tempTrayWidth = this.TrayEntity.TrayWidth / this.Scale
      const tempTrayLenght = this.TrayEntity.TrayLength / this.Scale
      const tempBoxWidth = this.Box.BoxWidth / this.Scale
      const tempBoxHeight = this.Box.BoxLength / this.Scale
      // 重新核算可存放数量
      this.XCount = Math.floor((tempTrayWidth - this.origin.xp) / tempBoxWidth)
      this.YCount = Math.floor((tempTrayLenght - this.origin.yp) / tempBoxHeight)
      this.LimitXCount = this.XCount
      this.LimitYCount = this.YCount
    },
    // 将图形载具转换成Location实体
    transLocation() {
      var num = 0
      this.locationList = []
      for (var i = 0; i < this.Comment.components.length; i++) {
        // 查找托盘
        if (this.Comment.components[i].name === 'shelf') {
          var locationEntity = {
            XLight: this.Comment.components[i].dataBind.xlight,
            XLenght: this.Comment.components[i].dataBind.xlenght,
            YLight: this.Comment.components[i].dataBind.ylight,
            BoxCode: this.Comment.components[i].dataBind.sn,
            SuggestMaterialCode: this.Comment.components[i].dataBind.biz,
            WareHouseCode: this.TrayEntity.WareHouseCode,
            TrayId: this.TrayEntity.Id,
            ContainerCode: this.AllEntity.ContainerCode,
            Enabled: true,
            LayoutId: this.Comment.components[i].identifier,
            LockQuantity: 0
          }
          this.locationList.push(locationEntity)
          num = num + 1
        }
      }
      console.log(this.locationList)
      // console.log('this.locationList')
      // console.log(this.locationList)
    },
    /** ****************/
    /* 编辑按钮 */
    /** ****************/
    handleUpdate(row) {
      this.AllEntity = Object.assign({}, row) // copy obj
      // 托盘编辑-折算重量
      if (this.Level === 2) {
        this.AllEntity.MaxWeight = this.AllEntity.MaxWeight / 1000
      }
      this.src = this.BaseUrl + this.AllEntity.PictureUrl
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      // this.$nextTick(() => {
      //   this.$refs['dataForm'].clearValidate()
      // })
    },
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          if (this.Level === 0) {
            const data = Object.assign({}, this.AllEntity)
            editWareHouse(data).then((res) => {
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
                  title: '成功',
                  message: '更新失败：' + resData.Message,
                  type: 'error',
                  duration: 2000
                })
              }
            })
          } else if (this.Level === 1) {
            const data = Object.assign({}, this.AllEntity)
            editContainer(data).then((res) => {
              var resData = JSON.parse(res.data.Content)
              if (resData.Success) {
                this.dialogFormVisible = false
                this.getList()
                this.getTreeData()
                this.$message({
                  title: '成功',
                  message: '更新成功',
                  type: 'success',
                  duration: 2000
                })
              } else {
                this.$message({
                  title: '成功',
                  message: '创建失败：' + resData.Message,
                  type: 'error',
                  duration: 2000
                })
              }
            })
          } else if (this.Level === 2) {
            editTray(this.AllEntity).then((res) => {
              var resData = JSON.parse(res.data.Content)
              if (resData.Success) {
                // this.list.unshift(this.Role)
                this.dialogFormVisible = false
                // this.getList()
                // this.getTreeData()
                this.$message({
                  title: '成功',
                  message: '更新成功',
                  type: 'success',
                  duration: 2000
                })
                this.handleFilter()
              } else {
                this.$message({
                  title: '失败',
                  message: '更新失败' + resData.Message,
                  type: 'error',
                  duration: 2000
                })
              }
            })
          } else {
            const data = Object.assign({}, this.AllEntity)
            editLocation(data).then((res) => {
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
                  title: '成功',
                  message: '更新失败' + resData.Message,
                  type: 'error',
                  duration: 2000
                })
              }
            })
          }
        }
      })
    },
    handleTakeOut(row) {
      this.AllEntity = Object.assign({}, row) // copy obj
      PostStartContainer(this.AllEntity).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.$message({
            title: '成功',
            message: '执行成功',
            type: 'success',
            duration: 2000
          })
          this.handleFilter()
        } else {
          this.$message({
            title: '失败',
            message: '执行失败' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handleTakeIn(row) {
      this.AllEntity = Object.assign({}, row) // copy obj
      PostRestoreContainer(this.AllEntity).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.$message({
            title: '成功',
            message: '执行成功',
            type: 'success',
            duration: 2000
          })
          this.handleFilter()
        } else {
          this.$message({
            title: '失败',
            message: '执行失败' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    /** ****************/
    /* 删除 */
    /** ****************/
    handleDelete(row) {
      this.$confirm('此操作将永久删除该条记录, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.AllEntity = Object.assign({}, row) // copy obj
        this.deleteData(this.AllEntity)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        })
      })
    },
    deleteData(data) {
      if (this.Level === 0) {
        deleteWareHouse(data).then((res) => {
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
            this.getTreeData()
          } else {
            this.$message({
              title: '成功',
              message: '删除失败：' + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.Level === 1) {
        deleteContainer(data).then((res) => {
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
            this.getTreeData()
          } else {
            this.$message({
              title: '成功',
              message: '删除失败：' + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else if (this.Level === 2) {
        deleteTray(this.AllEntity).then((res) => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.dialogFormVisible = false
            this.getList()
            this.getTreeData()
            this.$message({
              title: '成功',
              message: '删除成功',
              type: 'success',
              duration: 2000
            })
          } else {
            this.$message({
              title: '失败',
              message: '删除失败' + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      } else {
        deleteLocation(data).then((res) => {
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
            this.getTreeData()
          } else {
            this.$message({
              title: '成功',
              message: '删除失败：' + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      }
    },
    /** ****************/
    /* 重置 */
    /** ****************/
    resetAllEntity() {
      this.AllEntity = {
        Id: undefined,
        Code: '',
        Name: '',
        Address: '',
        CategoryDict: '',
        IsVirtual: false,
        AllowManage: true,
        CreatedUserCode: '',
        CreatedUserName: '',
        CreatedTime: undefined,
        WareHouseCode: this.AllEntity.WareHouseCode,
        ContainerCode: this.AllEntity.ContainerCode,
        TrayId: this.AllEntity.TrayId,
        Remark: '',
        XLight: '',
        YLight: '',
        Depth: 0,
        Height: 0,
        Enabled: true,
        IsLocked: false,
        SuggestMaterialCode: '',
        ShelfCode: '',
        Level: this.Level,
        DeviceAddress: 0,
        IsScanned: false,
        DeviceType: 1,
        EquipmentCode: '',
        EquipmentType: '',
        TrayWidth: '',
        TrayLength: '',
        Ip: '',
        Port: '',
        XNumber: '',
        YNumber: '',
        BoxCode: '',
        BoxLength: '',
        BoxWidth: '',
        CurXLight: 0,
        CurYLight:0,
        LockQuantity: 0
      }
      this.configData.components = []
    },
    restQuery() {
      // 分页查询
      this.listQuery = {
        Page: 1,
        Rows: 15,
        Name: '',
        Code: '',
        Sort: 'Id',
        Level: 0
      }
    },
    resetAuth() {
      this.data1 = []
      this.writeUser = []
      this.writeUserList = []
    },
    /** ****************/
    /* 打印 */
    /** ****************/
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
    handlePrintCode(row, level) {
      // console.log(level)
      if (row === null) {
        this.$message({
          title: '失败',
          message: '请选择需要打印条码的仓库区域库位',
          type: 'error',
          duration: 2000
        })
        return
      }
      // 获取该仓库下的所以库位
      if (level === 0) {
        getWareHouseLocations(row.Code).then(response => {
          var result = JSON.parse(response.data.Content)
          if (result.length <= 0) {
            this.$message({
              title: '失败',
              message: '该仓库下未包含库位信息',
              type: 'error',
              duration: 2000
            })
            return
          } else {
            this.$confirm('此操作将打印该仓库下 ( ' + result.length + '个 ) 库位标签, 是否继续?', '提示', {
              confirmButtonText: '确定',
              cancelButtonText: '取消',
              type: 'warning'
            }).then(() => {
              this.printCode(result)
            }).catch(() => {
              this.$message({
                type: 'info',
                message: '已取消删除'
              })
            })
          }
        })
      } else if (level === 1) { // 获取该区域下的所以库位
        getAreaLocations(row.Code).then(response => {
          var result = JSON.parse(response.data.Content)
          if (result.length <= 0) {
            this.$message({
              title: '失败',
              message: '该仓库下未包含库位信息',
              type: 'error',
              duration: 2000
            })
            return
          } else {
            this.$confirm('此操作将打印该区域下 ( ' + result.length + '个 ) 库位标签, 是否继续?', '提示', {
              confirmButtonText: '确定',
              cancelButtonText: '取消',
              type: 'warning'
            }).then(() => {
              this.printCode(result)
            }).catch(() => {
              this.$message({
                type: 'info',
                message: '已取消删除'
              })
            })
          }
        })
      } else if (level === 2) {
        getTrayLocations(row.Id).then(response => {
          var result = JSON.parse(response.data.Content)
          if (result.length <= 0) {
            this.$message({
              title: '失败',
              message: '该托盘未包含库位信息',
              type: 'error',
              duration: 2000
            })
            return
          } else {
            this.$confirm('此操作将打印该托盘下 ( ' + result.length + '个 ) 库位标签, 是否继续?', '提示', {
              confirmButtonText: '确定',
              cancelButtonText: '取消',
              type: 'warning'
            }).then(() => {
              this.printCode(result)
            }).catch(() => {
              this.$message({
                type: 'info',
                message: '已取消打印'
              })
            })
          }
        })
      } else if (level === 3) {
        var printlist = []
        printlist.push(row)
        this.$confirm('确认打印?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          this.printCode(printlist)
        }).catch(() => {
          this.$message({
            type: 'info',
            message: '已取消打印'
          })
        })
      } else {
        this.$message({
          title: '成功',
          message: '打印失败，请选择一条记录',
          type: 'error',
          duration: 2000
        })
      }
    },
    printCode(resData) {
      const data = {}

      resData.forEach(element => {
        this.createBarCode(element.Code)

        // 一维码  孚杰
        // this.controls.push({
        //   id: 1,
        //   type: 'aimage',
        //   data: {
        //     value: element.SuggestMaterialName,
        //     x: 5,
        //     y: 5,
        //     width: 70,
        //     height: 70,
        //     itemtype: 0,
        //     databindtype: 0,
        //     databind: {
        //       id: '',
        //       text: ''
        //     },
        //     style: {
        //       backgroundSize: 0,
        //       defaultimgtype: 0,
        //       defaultimg: this.barCode,
        //       HOrient: 0,
        //       VOrient: 0
        //     }
        //   }
        // })

        // // 建议库位码
        // this.controls.push({
        //   id: 111,
        //   type: 'atext',
        //   data: {
        //     value: '库位编码:' + element.Code,
        //     width: 400,
        //     height: 15,
        //     x: 80,
        //     y: 5,
        //     itemtype: 0,
        //     databind: {
        //       id: '',
        //       text: ''
        //     },
        //     style: {
        //       color: '#000',
        //       fontFamily: '宋体',
        //       fontSize: '10px',
        //       fontSpacing: 0,
        //       fontWeight: 'normal',
        //       fontStyle: 'normal',
        //       textAlign: 'left',
        //       border: '',
        //       borderType: 0,
        //       HOrient: 0,
        //       VOrient: 0
        //     }
        //   }
        // })
        // // 建议物料名称
        // this.controls.push({
        //   id: 111,
        //   type: 'atext',
        //   data: {
        //     value: '物料编码:' + element.SuggestMaterialCode,
        //     width: 400,
        //     height: 15,
        //     x: 80,
        //     y: 25,
        //     itemtype: 0,
        //     databind: {
        //       id: '',
        //       text: ''
        //     },
        //     style: {
        //       color: '#000',
        //       fontFamily: '宋体',
        //       fontSize: '10px',
        //       fontSpacing: 0,
        //       fontWeight: 'normal1',
        //       fontStyle: 'normal',
        //       textAlign: 'left',
        //       border: '',
        //       borderType: 0,
        //       HOrient: 0,
        //       VOrient: 0
        //     }
        //   }
        // })
        // // 建议物料编码
        // this.controls.push({
        //   id: 111,
        //   type: 'atext',
        //   data: {
        //     value: '物料名称:' + element.SuggestMaterialName,
        //     width: 400,
        //     height: 15,
        //     x: 80,
        //     y: 45,
        //     itemtype: 0,
        //     databind: {
        //       id: '',
        //       text: ''
        //     },
        //     style: {
        //       color: '#000',
        //       fontFamily: '宋体',
        //       fontSize: '10px',
        //       fontSpacing: 0,
        //       fontWeight: 'normal1',
        //       fontStyle: 'normal',
        //       textAlign: 'left',
        //       border: '',
        //       borderType: 0,
        //       HOrient: 0,
        //       VOrient: 0
        //     }
        //   }
        // })
        // 建议库位码
        this.controls.push({
          id: 111,
          type: 'atext',
          data: {
            value: element.Code + '-托盘:' + element.TrayCode,
            width: 300,
            height: 20,
            x: 10,
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
        // 建议物料名称
        this.controls.push({
          id: 111,
          type: 'atext',
          data: {
            value: element.SuggestMaterialName,
            width: 300,
            height: 20,
            x: 10,
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
        // 建议物料编码
        this.controls.push({
          id: 111,
          type: 'atext',
          data: {
            value: element.SuggestMaterialCode + '--X轴:' + element.XLight + '--Y轴' + element.YLight,
            width: 300,
            height: 20,
            x: 10,
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
            x: 10,
            y: 65,
            width: 300,
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
      })
    },
    All_ExportExcel() {
      this.$confirm('是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/WareHouse/DoDownLocaitonInfo?Level=' + this.listQuery.Level + '&Code=' + this.listQuery.Code
        window.open(url)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消'
        })
      })
    },
    beforeUpload(file) {
      const isText = file.type === 'application/vnd.ms-excel'
      const isTextComputer = file.type === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      return (isText | isTextComputer)
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
      ouLoadMaterialLocationInfo(form).then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          // this.getlist()
          this.handleUpdateLayout()
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
    handleUpdateLayout() {
      getLocationByTrayId(this.TrayEntity.Id).then(response => {
        var result = JSON.parse(response.data.Content)
        if (this.TrayEntity.LayoutJson !== null && this.TrayEntity.LayoutJson !== '') {
          const a = JSON.parse(this.TrayEntity.LayoutJson)
          this.configData.components = a.components
          result.forEach((item, index) => {
            // console.log(item)
            this.Comment.components.find((element) => (element.identifier === item.LayoutId)).dataBind.biz = item.SuggestMaterialCode
          })
        }
        this.configData.components = this.Comment.components
        // // 创建库位
        this.transLocation()
        this.TrayEntity.LocationList = JSON.stringify(this.locationList)
        this.TrayEntity.LayoutJson = JSON.stringify(this.configData)
        editTrayLocation(this.TrayEntity).then((res) => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.configData.components = []
          } else {
            this.$message({
              title: '失败',
              message: '更新失败' + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      })
    },
    // 上传文件个数超过定义的数量
    handleExceed(files, fileList) {
      this.$message.warning(`当前限制选择 1 个文件，请删除后继续上传`)
    },
    handleDownUpload() {
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoDownLoadTemp'
      window.open(url)
    }
  }
}
</script>
<style rel="stylesheet/scss" lang="scss" scoped>
// .el-icon-my-model{
//   background: url("../../../icons/svg/model.svg") center no-repeat;
// }
.icons-container {
  margin: 0;
  overflow: hidden;
}
.icon-item {
  margin-left: 20px;
  height: 85px;
  text-align: center;
  width: 100px;
  float: left;
  font-size: 15px;
  color: #24292e;
  cursor: pointer;
}

.disabled {
  pointer-events: none;
}
// .el-card__body{
//   margin-top: 5px;
//   padding: 0px;
// }
.icon-item /deep/ .el-card__body {
  margin-top: 5px;
  padding: 0;
}

.topo-fullscreen {
  height: 100%;
  width: 100%;
}

.topo-render {
  overflow: auto;
  background-color: white;
  background-clip: padding-box;
  background-origin: padding-box;
  background-repeat: no-repeat;
  //background-size: 100% 100%;
  position: relative;
  height: 80%;

  .topo-render-wrapper {
    position: absolute;
  }
  .topo-render-wrapper-clickable {
    cursor: pointer;
  }
}
</style>
