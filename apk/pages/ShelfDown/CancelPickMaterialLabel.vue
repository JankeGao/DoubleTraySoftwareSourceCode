<template>
	<view>
		<view class="content">
			<view class="uni-flex uni-column">
				<view style="height: 300upx;">
					<view class="uni-form-item">
						<label class="title">库位码:</label>
						<input :focus="locationFoucs" class="uni-input" confirm-type="search"  @confirm="OnEnter()"  v-model="LabelEntity.LocationCode" placeholder="请输入库位码" />
					</view>
					<view class="uni-form-item">
						<label class="title">物料条码:</label>
						<input :focus="labelFoucs" class="uni-input" confirm-type="search"  @confirm="OnEnter()"  v-model="LabelEntity.ReelId" placeholder="请输入物料条码" />
					</view>
					<view class="uni-form-item">
					<button type="primary" size="mini" @click="OnEnter()">扫描</button>
					</view>
				</view>
				<view class="flex-item flex-item-V">
					<uni-card title="条码信息">
						<view>
							<view class="uni-form-item">
								<text>物料条码:</text>
								<text>{{this.LabelEntity.ReelId}}</text>
							</view>
							<view class="uni-form-item">
								<text>物料编码:</text>
								<text>{{this.LabelEntity.MaterialCode}}</text>
							</view>
							<view class="uni-form-item">
								<text>物料数量:</text>
								<text>{{this.LabelEntity.NeedQuantity}}</text>
							</view>
							<view class="uni-form-item">
								<text>批号:</text>
								<text>{{this.LabelEntity.BatchCode}}</text>
							</view>
							<view class="uni-form-item">
								<text>生产日期:</text>
								<text>{{this.LabelEntity.ReelCreateCode}}</text>
							</view>
						</view>
					</uni-card>
		
				</view>
				<view class="flex-item flex-item-V">
					<view class="uni-form-item">
						<button type="primary" size="mini" @click="ConfirmCancel()">作废</button>
						<button type="primary" size="mini" @click="Back()">取消</button>
					</view>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				LabelEntity:{
					LocationCode:'',
					PickOrderCode:'',
					ReelId:''
				},
				locationFoucs:true,
				labelFoucs:false,
				PickOrderCode:''
			}
		},
		onLoad:function(e){
			this.PickOrderCode=e.PickOrderCode
			this.LabelEntity.PickOrderCode=e.PickOrderCode
		},
		methods: {
			OnEnter(){
				if(this.LabelEntity.LocationCode!='' || this.LabelEntity.ReelId!=''){
					this.api.baseRequest({
						url:  'PreparationView/GetReelIdByLocationCodeForPick',
						method: "get",
						data:{PickOrderCode:this.LabelEntity.PickOrderCode,ReelId:this.LabelEntity.ReelId, LocationCode:this.LabelEntity.LocationCode},
						success: res => {
							var resData = JSON.parse(res.data.Content)
							if (resData.Success) {
								console.log(resData)
								this.LabelEntity =resData.Data
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
			Back(){
				uni.navigateBack({
					
				});
			},
			ConfirmCancel(){
				this.api.baseRequest({
					url:  'PreparationView/CancelPickMaterialLabel',
					method: "post",
					data: this.LabelEntity,
					success: res => {
						var resData = JSON.parse(res.data.Content)
						if (resData.Success) {
			                uni.showModal({
								content: resData.Message,
								showCancel: false
							});
							this.LabelEntity={
								PickOrderCode:this.PickOrderCode,
								LocationCode:'',
								ReelId:''
							}
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
	}
</script>

<style>

</style>
