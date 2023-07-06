<template>
	<view class="content">
		<view class="box">
			<view class="title">拆盘区域列表</view>
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
					<button type="primary" size="mini" @click="startSplitOrderArea()">启动</button>
					<button type="primary" size="mini" @click="finishSplitOrderArea()">熄灭</button>
					<button type="primary" size="mini" @click="cancelSplitMaterialLabel()">作废条码</button>
					<button type="primary" size="mini" @click="getSplitAreaList()">刷新</button>
					<button type="primary" size="mini" @click="back()">返回</button>
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
			this.SplitNo = e.SplitNo
			this.getSplitAreaList()
		},
		data() {
			return {
				tableList: [],
				SplitNo: '',
				CheckedAreaList:[],
			};
		},
		methods: {
			getSplitAreaList() {
				this.api.baseRequest({
					url:  'Split/GetSplitAreaList',
					method: "get",
					data: {
						SplitNo: this.SplitNo
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
			startSplitOrderArea(){
				if(this.CheckedAreaList.length>0){
				   this.api.baseRequest({
				    url:  'Split/SplitTaskDoStart',
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
							this.getSplitAreaList()
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
			finishSplitOrderArea(){
				if(this.CheckedAreaList.length>0){
					this.api.baseRequest({
									url: 'Split/SplitTaskDoFinish',
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
											this.getSplitAreaList()
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
			cancelSplitMaterialLabel(){
				uni.navigateTo({
					url: '../SplitManage/CancelSplitMaterialLabel?SplitNo=' + this.SplitNo
				})
			},
			back(){
				uni.navigateBack({
					
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
