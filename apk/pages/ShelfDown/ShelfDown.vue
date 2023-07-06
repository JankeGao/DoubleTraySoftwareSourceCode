<template>
	<view class="content">
		<view class="box">
			<view class="title">拣货任务列表</view>

			<t-table>
				<!-- @change="change" :is-check="true" -->
				<t-tr>
					<t-th>捡货单号</t-th>
					<t-th align="center">操作</t-th>
				</t-tr>
				<t-tr v-for="item in tableList" :key="item.Id">
					<t-td>{{ item.PickOrderCode }}</t-td>
					<t-td align="left"><button @click="getAreaDetail(item)">详情</button></t-td>
				</t-tr>
			</t-table>
		</view>
		<view>
			<view class="flex-item flex-item-V">
				<!-- uni-bg-blue -->
				<view class="uni-form-item">
					<button type="primary" size="mini" @click="RefList()">刷新</button>
				<!-- 	<button type="primary" size="mini" @click="testFunction()">测试</button> -->
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
			this.getPickOrderList()
		},
		data() {
			return {
				tableList: []
			};
		},
		methods: {
			getAreaDetail(e) {
				uni.navigateTo({
					url: '../ShelfDown/PickArea?PickOrderCode=' + e.PickOrderCode
				})
			},
			RefList() {
				this.getPickOrderList()
			},
			getPickOrderList() {
/* 				this.api.baseRequest({
					url:  'PreparationView/GetAllAvailablePickOrder',
					method: "get",
					data: null,
					success: res => {
						var resData = JSON.parse(res.data.Content)
						this.tableList = resData
					},
					fail: () => {

					},
					complete: () => {

					}
				})

 */
				    this.api.baseRequest({
					url:'PreparationView/GetAllAvailablePickOrder',
					method:'get',
					success:res=>{
						var resData = JSON.parse(res.data.Content)
						this.tableList = resData
					}
				})
			},
			change(e) {
				console.log(e.detail)
			},
			testFunction() {
				let common_table_info = []
				for (let i = 0; i < this.tableList.length; i++) {
					var element = this.tableList[i]
					console.log(element)
					common_table_info.push(element)
				}
				this.api.baseRequest({
					url:  'PreparationView/PickTaskDoStart',
					method: "post",
					header: {
						// 'Content-Type': 'application/x-www-form-urlencoded; charset=utf-8',
						'Content-Type': 'application/json; charset=utf-8'
					},
					data: JSON.stringify(this.tableList),

					success: res => {
						var resData = JSON.parse(res.data.Content)
						this.tableList = resData

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
	.title {
		font-size: 32upx;
		color: #666;
	}

	.box {
		margin: 20upx;
		word-wrap: break-word;
		word-break: break-all;
	}

	button {
		font-size: 24upx;
	}
</style>
