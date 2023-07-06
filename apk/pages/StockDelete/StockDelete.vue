<template>
	<view class="content">
		<view class="uni-flex uni-column">
			<view class="Scan">
				<view class="uni-form-item">
					<label class="title">物料条码:</label>
					<input :focus="labelFoucs" class="uni-input" v-model="StockEntity.MaterialLabel" placeholder="请输入物料条码"
					 confirm-type="search" @confirm="getMaterialLabelInfo()" />
				</view>
				<view class="uni-form-item">
					<label class="title">库位码:</label>
					<input :focus="locationFoucs" class="uni-input" v-model="StockEntity.LocationCode" placeholder="请输入库位码"
					 confirm-type="search" @confirm="getMaterialLabelInfo()" />
				</view>
				<view class="uni-form-item">
					<button type="primary" @click="getMaterialLabelInfo()" size="mini">扫描</button>
				</view>
			</view>
			<view class="flex-item flex-item-V">
				<uni-card title="条码信息">
					<view>
						<view class="uni-form-item">
							<text>物料条码:</text>
							<text>{{this.StockEntity.MaterialLabel}}</text>
						</view>
						<view class="uni-form-item">
							<text>物料编码:</text>
							<text>{{this.StockEntity.MaterialCode}}</text>
						</view>
						<view class="uni-form-item">
							<text>物料数量:</text>
							<text>{{this.StockEntity.Quantity}}</text>
						</view>
						<view class="uni-form-item">
							<text>批号:</text>
							<text>{{this.StockEntity.BatchCode}}</text>
						</view>
						<view class="uni-form-item">
							<text>生产日期:</text>
							<text>{{this.StockEntity.ManufactureDate}}</text>
						</view>
					</view>
				</uni-card>

			</view>
			<view class="flex-item flex-item-V">
				<view class="uni-form-item">
					<button type="primary" size="mini" @click="ConfirmDelete()">确定</button>
					<button type="primary" size="mini" @click="CancelDelete()">取消</button>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				StockEntity: {
					NewLocationCode: '',
					LocationCode: '',
					MaterialLabel: ''
				},
				labelFoucs: true,
				locationFoucs: false,
			}
		},
		methods: {
			getMaterialLabelInfo() {
				this.api.baseRequest({
					url:  'Stock/GetStockByMaterialLabel',
					method: "get",
					data: {
						MaterialLabel: this.StockEntity.MaterialLabel,
						LocationCode: this.StockEntity.LocationCode
					},
					success: res => {
						var resData = JSON.parse(res.data.Content)
						if (resData.Success) {
							this.StockEntity = resData.Data
							this.StockEntity.NewLocationCode=''
							this.newLocationFouce=true
						} else {
							uni.showModal({
								content: resData.Message,
								showCancel: false
							});
							return
						}

					},
					fail: () => {

					},
					complete: () => {

					}
				})

			},
			ConfirmDelete() {
				//uni.getStorageSync('')
				this.api.baseRequest({
					url:  'Stock/ConfirmDeleteStockByPda',
					method: "post",
					data: this.StockEntity,
					success: res => {
						var resData = JSON.parse(res.data.Content)
						if (resData.Success) {
							this.StockEntity = {
								NewLocationCode: '',
								LocationCode: '',
								MaterialLabel: ''
							}
						} else {
							uni.showModal({
								content: resData.Message,
								showCancel: false
							});
							this.StockEntity.NewLocationCode=''
							this.newLocationFouce=true
							
							return
						}

					},
					fail: () => {

					},
					complete: () => {

					}
				})

			},
			CancelDelete(){
				uni.navigateBack({
					
				})
			}
		}
	}
</script>

<style>

</style>
