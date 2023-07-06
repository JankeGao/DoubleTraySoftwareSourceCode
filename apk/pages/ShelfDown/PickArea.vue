<template>
	<view class="content">
		<view class="box">
			<view class="title">拣货区域列表</view>
			<t-table @change="checkChange" :is-check="true">
				<t-tr>
					<t-th>仓库编码</t-th>
					<t-th>区域编码</t-th>
					<t-th align="center">状态</t-th>
				</t-tr>
				<t-tr v-for="item in tableList" :key="item.Id">
					<t-td>{{ item.WareHouseCode }}</t-td>
					<t-td>{{ item.AreaId }}</t-td>
					<t-td>
						{{ item.StatusCaption }}
					</t-td>
				</t-tr>
			</t-table>
		</view>
		<view>
			<view class="flex-item flex-item-V">
				<!-- uni-bg-blue -->
				<view class="uni-form-item">
					<button type="primary" size="mini" @click="startPickOrderArea()">启动</button>
					<button type="primary" size="mini" @click="finishPickOrderArea()">熄灭</button>
										<button type="primary" size="mini" @click="cancelPickMaterialLabel()">作废条码</button>
					<button type="primary" size="mini" @click="getPickAreaList()">刷新</button>
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
		onLoad: function(e) {
			this.PickOrderCode = e.PickOrderCode
			this.getPickAreaList()
		},
		data() {
			return {
				tableList: [],
				PickOrderCode: '',
				CheckedAreaList:[],
			};
		},
		methods: {
			getPickAreaList() {
				this.api.baseRequest({
					url:  'PreparationView/GetAllAvailablePickArea',
					method: "get",
					data: {
						PickOrderCode: this.PickOrderCode
					},
					success: res => {
						var resData = JSON.parse(res.data.Content)
						this.tableList = resData
					},
					fail: () => {

					},
					complete: () => {

					}
				})

			},

			checkChange(e) {
				if(e.detail.length>0){
					this.CheckedAreaList=[]
					for (let i = 0; i < e.detail.length; i++) {
						var index = e.detail[i];
						var element = this.tableList[index];
						this.CheckedAreaList.push(element);
					}
				}else{
					this.CheckedAreaList=[]
					
				}	
						console.log(this.CheckedAreaList.length)
			},
			startPickOrderArea(){
				if(this.CheckedAreaList.length>0){
				   this.api.baseRequest({
				    url:'PreparationView/PickTaskDoStart',
					method:"post",
					header: {
						// 'Content-Type': 'application/x-www-form-urlencoded; charset=utf-8',
						'Content-Type': 'application/json; charset=utf-8'
					},
					data:JSON.stringify(this.CheckedAreaList),
					success: result => {
						var resData = JSON.parse(result.data.Content)
						if (resData.Success) {
							uni.showModal({
								content: resData.Message,
								showCancel: false
							});
							this.getPickAreaList()
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
				else{
					uni.showModal({
						content: "尚未勾选任何区域",
						showCancel: false
					});
					return
				}
			},
			finishPickOrderArea(){
				if(this.CheckedAreaList.length>0){
					this.api.baseRequest({
									url: 'PreparationView/PickTaskDoFinish',
									method: "post",
										data:JSON.stringify(this.CheckedAreaList),
										dataType:'json',
									success: res => {
										var resData = JSON.parse(res.data.Content)
										if (resData.Success) {
											uni.showModal({
												content: resData.Message,
												showCancel: false
											});
											this.getPickAreaList()
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
								
				}else{
					uni.showModal({
						content: "尚未勾选任何区域",
						showCancel: false
					});
					return
				}
			},
			cancelPickMaterialLabel(){
				uni.navigateTo({
					url: '../ShelfDown/CancelPickMaterialLabel?PickOrderCode=' + this.PickOrderCode
				})
			}
		}
	}
</script>

<style>
	.title {
		font-size: 32upx;
		color: #666;
	}

	.box {
		margin: 20upx;
	}

	button {
		font-size: 24upx;
	}
</style>
