<template>
	<view class="content">
		<view class="uni-flex uni-column">
			<view class="Scan">
				<view class="uni-form-item">
					<label class="title">物料条码:</label>
					<input :focus="labelFoucs" class="uni-input" v-model="WmsPickAreaDetail.ReelId" placeholder="请输入物料条码"
					 confirm-type="search" @confirm="getMaterialLabelInfo()" />
				</view>
				<view class="uni-form-item">
					<label class="title">拣货单号:</label>
					<label class="title">{{this.PickOrderCode}}</label>
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
							<text>{{this.WmsPickAreaDetail.ReelId}}</text>
						</view>
						<view class="uni-form-item">
							<text>物料编码:</text>
							<text>{{this.WmsPickAreaDetail.MaterialCode}}</text>
						</view>
						<view class="uni-form-item">
							<text>物料数量:</text>
							<text>{{this.WmsPickAreaDetail.NeedQuantity}}</text>
						</view>
						<view class="uni-form-item">
							<text>下架库位:</text>
							<text>{{this.WmsPickAreaDetail.LocationCode}}</text>
						</view>
						<view class="uni-form-item">
							<text>批号:</text>
							<text>{{this.WmsPickAreaDetail.BatchCode}}</text>
						</view>
						<view class="uni-form-item">
							<text>站台:</text>
							<text>{{this.WmsPickAreaDetail.Fseqno}}</text>
						</view>
					</view>
				</uni-card>

			</view>
			<view class="flex-item flex-item-V">
				<view class="uni-form-item">
					<button type="primary" size="mini" @click="ConfirmCheck()">确定</button>
					<button type="primary" size="mini" @click="CancelLabel()">作废</button>
					<button type="primary" size="mini" @click="CancelCheck()">取消</button>
				</view>
			</view>
		</view>
	</view>
</template>

<script>

	export default {
		onLoad:function(e){
			this.PickOrderCode=e.PickOrderCode
		},
		data() {
			return {
				WmsPickAreaDetail: {
					
				},
				labelFoucs: true,
			    PickOrderCode:'',
				LastMaterialLabel:''
			}
		},
		methods: {
			getMaterialLabelInfo() {
				this.api.baseRequest({
					url:  'PreparationView/IsTheReelIdInPickOrder',
					method: "get",
					data: {
						ReelId: this.WmsPickAreaDetail.ReelId,
						PickOrderCode: this.PickOrderCode
					},
					success: res => {
						var resData = JSON.parse(res.data.Content)
						if (resData.Success) {
							this.WmsPickAreaDetail = resData.Data
						} else {
							uni.showModal({
								content: resData.Message,
								showCancel: false
							});
							this.WmsPickAreaDetail={
								ReelId:''
							}
							this.labelFoucs=true
							return
						}

					},
					fail: () => {

					},
					complete: () => {
                        
					}
				})

			},
			ConfirmCheck() {
				//uni.getStorageSync('')
				this.api.baseRequest({
					url:  'PreparationView/ConfirmReelToSend',
					method: "post",
					data: this.WmsPickAreaDetail,
					success: res => {
						var resData = JSON.parse(res.data.Content)
						if (resData.Success) {
							this.WmsPickAreaDetail = {
							   ReelId:''
							}
						} else {
							uni.showModal({
								content: resData.Message,
								showCancel: false
							});
							this.WmsPickAreaDetail.NewLocationCode=''
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
			CancelCheck(){
				uni.navigateBack({
					
				})
			},
			CancelLabel(){
				if (this.WmsPickAreaDetail.ReelId !='') {
					uni.showModal({
						title: '提示',
						content: '是否作废此条码',
						success: function(res) {
							if (res.confirm) {
								this.api.baseRequest({
									url:  'PreparationView/CancelPickMaterialLabel',
									method: "post",
									data: this.WmsPickAreaDetail,
									success: result => {
										var resData = JSON.parse(result.data.Content)
										if (resData.Success) {
								        	this.WmsPickAreaDetail = {
											   ReelId:''
											}
											uni.showModal({
												content: resData.Message,
												showCancel: false
											});
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
							} else if (res.cancel) {
				
							}
						}.bind(this)
					})
				
				}else{
					uni.showModal({
						content: '没有可作废的条码',
						showCancel: false
					});
				}
			}
		}
	}
</script>

<style>

</style>
