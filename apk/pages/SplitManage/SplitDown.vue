<template>
	<view class="content">
		<view class="box">
			<view class="title">拆盘任务列表</view>

			<t-table>
				<!-- @change="change" :is-check="true" -->
				<t-tr>
					<t-th>拆盘单号</t-th>
					<t-th align="center">操作</t-th>
				</t-tr>
				<t-tr v-for="item in tableList" :key="item.Id">
					<t-td>{{ item.SplitNo }}</t-td>
					<t-td align="left"><button @click="SplitAreaDetail(item)">区域详情</button></t-td>
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
			this.getSplitOrderList()
		},
		data() {
			return {
				tableList: []
			};
		},
		methods: {
			SplitAreaDetail(e) {
				uni.navigateTo({
					url: './SplitArea?SplitNo=' + e.SplitNo
				})
			},
			RefList() {
				this.getSplitOrderList()
			},
			getSplitOrderList() {
				this.api.baseRequest({
					url:  'Split/GetSplitOrderList',
					method: "get",
					data:{type:0},
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
