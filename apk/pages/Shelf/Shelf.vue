<template>
	<view class="content">
		<view class="Scan">
			<view class="inputWrapper">
				<label class="label">物料条码:</label>
				<input class="input" :focus="labelFoucs" v-model="ScanMaterialLabel" placeholder="请输入物料条码"
				 @confirm="OnEnter()" />
			</view>
			<view class="inputWrapper">
				<label class="label">库位编码:</label>
				<input class="input" :focus="locationFoucs" v-model="LabelEntity.LocationCode" placeholder="请输入库位码" />
			</view>
			<view class="btn">
				<button class="btnValue" type="primary" :disabled="IsCanStartOrder==false" @click="ScanKeyDown()" size="mini">开始任务</button>
				<button class="btnValue"  type="primary" :disabled="IsCanSkipLocation==false" @click="SkipCurrentLocation()" size="mini">切换库位</button>
			</view>
		</view>
		<view class="Scan">
			<view >
				<uni-card title="条码信息" class="card">
					<view>
						<view class="inputWrapper">
							<label class="label" >物料条码:</label>
							<text class="label">{{this.LabelEntity.Code}}</text>
						</view>
						<view class="inputWrapper">
							<text class="label">物料编码:</text>
							<text class="label">{{this.LabelEntity.MaterialCode}}</text>
						</view>
						<view class="inputWrapper">
							<label class="label">物料数量:</label>
							<input class="input" type="number" v-model="LabelEntity.Quantity" placeholder="请输入数量" />
							
						</view>
						<view class="inputWrapper">
							<text class="label">批号:</text>
							<text class="label">{{this.LabelEntity.Batchcode}}</text>
						</view>
						<view class="inputWrapper">
							<text class="label" >生产日期:</text>
							<text class="label" style="width: 65%;">{{this.LabelEntity.ProductionDate}}</text>
						</view>
					</view>
				</uni-card>
			</view>


		</view>
		<view class="Scan btn">
				<button class="btnValue" type="primary" size="mini" @click="ConfirmShelf()">上架</button>
				<button  class="btnValue" type="primary" size="mini" @click="FinisheCurrentOrder()">完成</button>
		</view>
	</view>
</template>

<script>
	export default {
		onLoad: function(e) {
			console.log(e.SplitNo)
			this.LabelEntity.SplitNo = e.SplitNo
			if (this.LabelEntity.SplitNo != '' && this.LabelEntity.SplitNo != undefined) {
				uni.setNavigationBarTitle({
					title: '拆盘上架:' + e.SplitNo
				})

			}
		},
		data() {
			return {
				LabelEntity: {
					Code: '',
					SupplyCode: '',
					MaterialCode: '',
					ProductionDate: '',
					Batchcode: '',
					Quantity: 0,
					LocationCode: '',
					SplitNo: '',
					ShelfDetailId: 0,
					ReplenishCode: ''
				},
				labelFoucs: false,
				ScanMaterialLabel: '',
				IsFirst: true,
				IsLastFinished: false,
				LastDetailId: 0,
				locationFoucs: false,
				IsCanSkipLocation: false,
				IsCanStartOrder: true
			}
		},
		methods: {
			/* 			ScanCode(){
							uni.scanCode({
								scanType:'barCode',
								 success:function(res){
								 	console.log(res.result)
									this.LabelEntity.Code=res.result
								 }
							})
						}, */
			OnEnter(e) {
				console.log(this.IsFirst)
				this.LabelEntity.Code = this.ScanMaterialLabel
				this.ScanMaterialLabel = ''
				this.textFoucs = true
				if (this.IsFirst == false) {
					this.ScanKeyDown()
				}
			},
			ScanKeyDown() {
				if (this.IsFirst) {
					var ReelId = this.LabelEntity.Code
					if (this.LabelEntity.Code == '') {
						uni.showModal({
							content: '未扫描条码!',
							showCancel: true
						});
						this.labelFoucs = true
						return
					}
					if (this.LabelEntity.LocationCode == '') {
						uni.showModal({
							content: '未扫描库位码!',
							showCancel: true
						});
						this.locationFoucs = true
						return
					}
					this.api.baseRequest({
						url: 'Shelf/FirstShelf',
						method: "post",
						data: this.LabelEntity,
						success: res => {
							var resData = JSON.parse(res.data.Content)
							if (resData.Success) {
								console.log(resData)
								this.LabelEntity = resData.Data
								this.labelFoucs = true
								this.IsFirst = false
								this.IsLastFinished = false
								this.IsCanStartOrder = false
								this.IsCanSkipLocation = true
								console.log(this.LabelEntity)
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

				} else {
					if (this.LabelEntity.Code == '') {
						uni.showModal({
							content: '未扫描条码!',
							showCancel: true
						});
						this.labelFoucs = true
						return
					}
					if (this.LabelEntity.ShelfDetailId != 0 && this.IsLastFinished == false) {
						this.api.baseRequest({
							url: 'Shelf/PdaConfirmShelf',
							method: "post",
							data: this.LabelEntity,
							success: res => {
								var resData = JSON.parse(res.data.Content)
								if (resData.Success) {
									this.LabelEntity = resData.Data
									this.labelFoucs = true
									this.IsFirst = false
									this.IsLastFinished = true
									this.api.baseRequest({
										url: 'Shelf/NextShelf',
										method: "post",
										data: this.LabelEntity,
										success: res => {
											var resData = JSON.parse(res.data.Content)
											if (resData.Success) {
												this.LabelEntity = resData.Data
												this.labelFoucs = true
												this.IsFirst = false
												this.IsLastFinished = false
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

								} else {
									uni.showModal({
										content: resData.Message,
										showCancel: false
									});
									this.labelFoucs = true
									return
								}

							},
							fail: () => {

							},
							complete: () => {

							}
						})
					} else {
						this.api.baseRequest({
							url: 'Shelf/NextShelf',
							method: "post",
							data: this.LabelEntity,
							success: res => {
								var resData = JSON.parse(res.data.Content)
								if (resData.Success) {
									this.LabelEntity = resData.Data
									this.labelFoucs = true
									this.IsFirst = false
									this.IsLastFinished = false
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

					}

				}

			},
			ConfirmShelf() {
				if (this.LabelEntity.ShelfDetailId != 0 && this.IsLastFinished == false) {
					this.api.baseRequest({
						url: 'Shelf/PdaConfirmShelf',
						method: "post",
						data: this.LabelEntity,
						success: res => {
							var resData = JSON.parse(res.data.Content)
							if (resData.Success) {
								this.LabelEntity = resData.Data
								this.labelFoucs = true
								this.IsFirst = false
								this.IsLastFinished = true

							} else {
								uni.showModal({
									content: resData.Message,
									showCancel: false
								});
								this.labelFoucs = true
								return
							}

						},
						fail: () => {

						},
						complete: () => {

						}
					})

				} else {
					uni.showModal({
						content: '尚未获取到上架信息!',
						showCancel: true
					});
					this.labelFoucs = true
				}
			},
			FinisheCurrentOrder() {
				if (this.IsLastFinished == false && this.LabelEntity.ShelfDetailId != 0) {
					uni.showModal({
						title: '提示',
						content: '是否确定完成此次上架任务',
						success: function(res) {
							if (res.confirm) {
								this.api.baseRequest({
									url: 'Shelf/FinishCurrentReplenish',
									method: "post",
									data: this.LabelEntity,
									success: res => {
										var resData = JSON.parse(res.data.Content)
										if (resData.Success) {
											this.LabelEntity = resData.Data
											this.LabelEntity.LocationCode = ''
											this.labelFoucs = true
											this.IsFirst = true
											this.IsLastFinished = false
											this.IsCanSkipLocation = false
											this.IsCanStartOrder = true
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

				} else {
					this.api.baseRequest({
						url: 'Shelf/FinishCurrentReplenish',
						method: "post",
						data: this.LabelEntity,
						success: res => {
							var resData = JSON.parse(res.data.Content)
							if (resData.Success) {
								this.LabelEntity = resData.Data
								this.labelFoucs = true
								this.IsFirst = true
								this.IsLastFinished = false
								this.IsCanSkipLocation = false
								this.IsCanStartOrder = true
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
				}
			},

			SkipCurrentLocation() {
				if (this.IsFirst == false && this.LabelEntity.ShelfDetailId != 0 && this.IsLastFinished == false) {
					this.api.baseRequest({
						url: 'Shelf/SkipCurrentLightLocation',
						method: "post",
						data: this.LabelEntity,
						success: res => {
							var resData = JSON.parse(res.data.Content)
							if (resData.Success) {
								this.LabelEntity = resData.Data
								this.labelFoucs = true
								this.IsFirst = false
								this.IsLastFinished = false

							} else {
								uni.showModal({
									content: resData.Message,
									showCancel: false
								});
								this.labelFoucs = true
								return
							}

						},
						fail: () => {

						},
						complete: () => {

						}
					})

				}
			}
		}
	}
</script>

<style>
	.content{
				display: flex;
				flex-direction: column; 
				height: 80vh;
	}
	.Scan {
		height: auto;
	}

	.label {
		color: black;
		width: 30%;
		font-size:35upx;
		font-weight: bold;
		text-align: left;
		display: inline-block;
		margin-left: 20upx;
		/* display:flex; */

	}

	.input {
		width: 70%;
		height: 100%;
		text-align: left;
		font-size: 35upx;
		display: flex;
		justify-content: center;
		align-items: center;
		background: white;
	    border-bottom: 1upx solid black;
		margin-left: 30upx;
	}

	.inputWrapper {
		width: 90%;

		display: flex;
		justify-content: left;
		align-items: left;

	}

	.btn {
		width: 100%;
		height: 80upx;
		/* 		border-radius: 50upx;
				display: flex;
				justify-content: center;
				align-items: center; */
		display: flex;
	}

	.btnValue {
		color: black;
		width: 100%;
		border-radius: 35upx;
		height: 80upx;
		font-size: 30upx;
		font-weight: bold;
		margin-top: 20upx;
		margin-left: 20upx;
		margin-right: 10upx;
		text-align: center;
		display: inline-block;
		/* display:flex; */
		line-height: 80upx;
		overflow:overlay;
	}

	.card {
		color: black;
		width: 95%;
		height: 50vh;
		font-size: 15px;
		font-weight: bold;
		text-align: left;
		display: inline-block;
		margin-left: 20upx;
		/* display:flex; */
		line-height: 80upx;
		border: 1upx solid #000000;
        margin-top: 50upx;
	}
</style>
