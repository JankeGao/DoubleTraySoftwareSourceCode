<template>
	<view>
		<view class="content">
			<view class="uni-flex uni-column">
				<view style="height: 150upx;">
					<view class="uni-form-item">
						<label class="title">物料条码:</label>
						<input :focus="labelFoucs" class="uni-input" confirm-type="search"  @confirm="OnEnter()"  v-model="LabelEntity.ReelId" placeholder="请输入物料条码" />
					</view>
					<view class="uni-form-item">
					<button type="primary" size="mini" @click="OnEnter()">扫描</button>
					</view>
				</view>
				<view class="flex-item flex-item-V">
					<uni-card>
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
								<text>{{this.LabelEntity.OrgQuantity}}</text>
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
				<view class="box">
					<t-table>
						<!-- @change="change" :is-check="true" -->
						<t-tr>
							<t-th>拆分条码</t-th>
							<t-th >拆分数量</t-th>
						</t-tr>
						<t-tr v-for="item in tableList" :key="item.Id">
							<t-td>{{ item.SplitReelId }}</t-td>
							<t-td >{{item.SplitQuantity}}</t-td>
						</t-tr>
					</t-table>
				</view>
				<view class="flex-item flex-item-V">
					<view class="uni-form-item">
						<button type="primary" size="mini" @click="ConfirmSplitReel()">确认</button>
						<button type="primary" size="mini" @click="Back()">取消</button>
					</view>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import tTable from '@/components/t-table/t-table.vue';
	import tTh from '@/components/t-table/t-th.vue';
	import tTr from '@/components/t-table/t-tr.vue';
	import tTd from '@/components/t-table/t-td.vue';
	export default {
		components: {
			tTable,
			tTh,
			tTr,
			tTd
		},
		data() {
			return {
				LabelEntity:{
					LocationCode:'',
					SplitNo:'',
					ReelId:'',
				},
				labelFoucs:true,
				SplitNo:'',
				tableList:[]
			}
		},
		onLoad:function(e){
			this.SplitNo=e.SplitNo
			this.LabelEntity.SplitNo=e.SplitNo
		},
		methods: {
			OnEnter(){
				if(this.LabelEntity.LocationCode!='' || this.LabelEntity.ReelId!=''){
					this.api.baseRequest({
						url:  'Split/GetReelIdByLocationCodeForSplit',
						method: "get",
						data:{SplitNo:this.LabelEntity.SplitNo,ReelId:this.LabelEntity.ReelId,LocationCode:this.LabelEntity.LocationCode},
						success: res => {
							var resData = JSON.parse(res.data.Content)
							if (resData.Success) {
								console.log(resData)
								this.LabelEntity =resData.Data
								this.api.baseRequest({
									url:'Split/GetSplitReelDetailList',
									method:'get',
									data:{SplitNo:this.LabelEntity.SplitNo,ReelId:this.LabelEntity.ReelId},
									success: request => {
										var listData = JSON.parse(request.data.Content)
										this.tableList=listData
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
			ConfirmSplitReel(){
				this.api.baseRequest({
					url:  'Split/ConfirmSplitReel',
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
								SplitNo:this.SplitNo,
								LocationCode:'',
								ReeId:''
							}
							this.tableList=[]
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
