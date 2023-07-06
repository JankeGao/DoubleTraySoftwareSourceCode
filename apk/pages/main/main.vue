<template>
	<view>
		<view class="content">
			<!-- 		<view class="title">下划线非滑动切换</view> -->
			<tabs :type="moduleList" v-model="selectIndex" itemColor="#03A9F4" lineColor="#03A9F4" :lineAnimated="true"></tabs>
			<view v-show="selectIndex==0">
				<view class="module">
					<view class="circular">
						<image @click="StartMaterialShelf" style="height: 240upx; width: 240upx; margin: 40upx" mode='aspectFit' src="../../static/images/Shelf.png"></image>
					</view>
					<view class="description">
						<view class="uni-flex uni-column">
							<view style="font-size: 40rpx;">上架物料条码</view>
							<view style="font-size: 35rpx;">主要操作</view>
							<view class="tag-body">
								<uni-tag class="tag-view" text="扫描条码" size="small" type="primary" :circle="true"></uni-tag>
								<uni-tag class="tag-view" text="扫描库位码" size="small" type="primary" :circle="true"></uni-tag>
							</view>
						</view>
					</view>
									
				</view>
			</view>
			<view v-show="selectIndex==1">
				<view class="module">
					<view class="circular">
						<image @click="StartMaterialDown" style="height: 240upx; width: 240upx; margin: 40upx" mode='aspectFit'src="../../static/images/down.png"></image>
					</view>
					<view class="description">
						<view class="uni-flex uni-column">
							<view style="font-size: 40rpx;">拣货单下架</view>
							<view style="font-size: 35rpx;">主要操作</view>
							<view class="tag-body">
								<uni-tag class="tag-view" text="亮灯" size="small" type="primary" :circle="true"></uni-tag>
								<uni-tag class="tag-view" text="灭灯" size="small" type="primary" :circle="true"></uni-tag>
							</view>
						</view>
					</view>
				
				</view>

				<view class="module">
					<view class="circular">
						<image @click="StartStockDelete()" style="height: 240upx; width: 240upx; margin: 40upx" mode='aspectFit' src="../../static/images/stockDelete.png"></image>
					</view>
					<view class="description">
						<view class="uni-flex uni-column">
							<view style="font-size: 40rpx;">直接下架库存</view>
							<view style="font-size: 35rpx; font-weight: 1000;">主要操作</view>
							<view class="tag-body">
								<uni-tag class="tag-view" text="下架" size="small" type="primary" :circle="true"></uni-tag>
							</view>
						</view>
					</view>
				</view>

			</view>
			<view v-show="selectIndex==2">
				<view class="module">
					<view class="circular">
					<image @click="StartConfirmPickLabel()" style="height: 240upx; width: 240upx; margin: 40upx" mode='aspectFit' src="../../static/images/check.png"></image>
					</view>
					<view class="description">
						<view class="uni-flex uni-column">
							<view style="font-size: 40rpx;">复核出库条码</view>
							<view style="font-size: 35rpx; font-weight: 1000;">主要操作</view>
							<view class="tag-body">
								<uni-tag class="tag-view" text="确定" size="small" type="primary" :circle="true"></uni-tag>
								<uni-tag class="tag-view" text="作废" size="small" type="primary" :circle="true"></uni-tag>
							</view>
						</view>
					</view>
				</view>

			</view>
			<view v-show="selectIndex==3">
				<view class="module">
					<view class="circular">
			        	<image @click="StartSplitDown()" style="height: 240upx; width: 240upx;  margin: 40upx" mode='aspectFit' src="../../static/images/split.png"></image>
					</view>
					<view class="description">
						<view class="uni-flex uni-column">
							<view style="font-size: 40rpx;">拆盘单下架</view>
							<view style="font-size: 35rpx; font-weight: 1000;">主要操作</view>
							<view class="tag-body">
								<uni-tag class="tag-view" text="亮灯" size="small" type="primary" :circle="true"></uni-tag>
								<uni-tag class="tag-view" text="灭灯" size="small" type="primary" :circle="true"></uni-tag>
							</view>
						</view>
					</view>
				</view>
				<view class="module" style="	background-image: linear-gradient(to right, #fffca1 , #fcff2c);">
					<view class="circular">
						<image @click="StartSplitConfirm()" style="height: 240upx; width: 240upx; margin: 40upx;" mode='aspectFit' src="../../static/images/stockDelete.png"></image>
					</view>
					<view class="description">
						<view class="uni-flex uni-column">
							<view style="font-size: 40rpx;">拆盘条码确认拆分</view>
							<view style="font-size: 35rpx; font-weight: 1000;">主要操作</view>
							<view class="tag-body">
								<uni-tag class="tag-view" text="确定" size="small" type="primary" :circle="true"></uni-tag>
								<uni-tag class="tag-view" text="作废" size="small" type="primary" :circle="true"></uni-tag>
							</view>
						</view>
					</view>
				</view>
				<view class="module">
					<view class="circular">
					<image @click="StartSplitShelf()" style="height: 240upx; width: 240upx;  margin: 40upx" mode='aspectFit' src="../../static/logo.png"></image>
					</view>
					<view class="description">
						<view class="uni-flex uni-column">
							<view style="font-size: 40rpx;">拆盘条码上架</view>
							<view style="font-size: 35rpx; font-weight: 1000;">主要操作</view>
							<view class="tag-body">
								<uni-tag class="tag-view" text="扫描条码" size="small" type="primary" :circle="true"></uni-tag>
								<uni-tag class="tag-view" text="扫描库位码" size="small" type="primary" :circle="true"></uni-tag>
							</view>
						</view>
					</view>
				</view>
				
				
			
				
			</view>
			<view v-show="selectIndex==4">
				<view class="module">
					<view class="circular">
						<image @click="StartStockMove()" style="height: 240upx; width: 240upx; margin: 40upx;" mode='aspectFit' src="../../static/images/move.png"></image>
					</view>
					<view class="description">
						<view class="uni-flex uni-column">
							<view style="font-size: 40rpx;">物料条码库位移动</view>
							<view style="font-size: 35rpx; font-weight: 1000;">主要操作</view>
							<view class="tag-body">
								<uni-tag class="tag-view" text="扫描条码" size="small" type="primary" :circle="true"></uni-tag>
								<uni-tag class="tag-view" text="扫描库位码" size="small" type="primary" :circle="true"></uni-tag>
							</view>
						</view>
					</view>
				</view>
				
			
			</view>
		</view>

	</view>
</template>

<script>
	import Tabs from '@/components/tabs/tabs.vue'
	import uniCard from '@/components/uni-card/uni-card.vue'
	import uniTag from "@/components/uni-tag/uni-tag.vue"
	export default {
		components: {
			uniCard,
			uniTag,
			Tabs
		},
		data() {
			return {
				selectIndex: 0,
				moduleList: [{
					title: '上架'
				}, {
					title: '下架'
				}, {
					title: '复核'
				}, {
					title: '拆盘'
				}, {
					title: '移库'
				}]
			}
		},
		methods: {
			//上架
			StartMaterialShelf(e) {
				console.log(e)
				uni.showModal({
					content: e.currentTarget.dataset.testid,
					showCancel: false
				});
				uni.navigateTo({
					url: '../Shelf/Shelf?SplitNo='
				})
			},
			//下架
			StartMaterialDown(e) {
				uni.navigateTo({
					url: '../ShelfDown/ShelfDown'
				})
			},
			//库内移动
			StartStockMove() {
				uni.navigateTo({
					url: '../StockMove/StockMove'
				})
			},
			//复核
			StartConfirmPickLabel() {
				uni.navigateTo({
					url: '../PickLabelConfirm/PickLabelConfirm'
				})
			},
			//库存直接下架
			StartStockDelete() {
				uni.navigateTo({
					url: '../StockDelete/StockDelete'
				})
			},
			//拆盘下架
			StartSplitDown() {
				uni.navigateTo({
					url: '../SplitManage/SplitDown'
				})
			},
			StartSplitConfirm() {
				uni.navigateTo({
					url: '../SplitManage/SplitConfirm'
				})
			},

			StartSplitShelf() {
				uni.navigateTo({
					url: '../SplitManage/SplitShelf'
				})
			},
		}
	}
</script>

<style>
	.dis {
		display: block;
	}

	.end-cont {
		display: none;
	}

	.circular {
		display: flex;
		width: 35%;
		height: 230upx;
		margin: 30upx auto;
		margin-left: 30upx;
		background: #FFFFFF;
		border-radius: 50%;
		text-align: center;
		align-items: center;
		justify-content: center;
	}

	.description {
		display: flex;
		width: 60%;
		margin: 50upx auto;
		margin-left: 50upx;
		text-align: left;
		align-items: auto;
		justify-content: left;
	}

	.module {
		width: 90%;
		height: 300upx;
		display: flex;
		margin: 40upx auto;
		background: #b2d5cf;
		border-radius: 30rpx;
		/* 从左到右 */
		background-image: linear-gradient(to right, #7777d5 , #adabd5); 
	    /* 从上到下 */
/* 	    background-image: linear-gradient(#e66465, #9198e5); */
		 /* 从左上角到右下角 */
		/* background-image: linear-gradient(to bottom right, red, yellow); */
	}

	.tag-body {
		/* #ifndef APP-PLUS-NVUE */
		display: flex;
		/* #endif */
		flex-direction: row;
		justify-content: flex-start;
		flex-wrap: wrap;
		/* 		padding: 0rpx; */
		margin-top: 20rpx;
		text-align: left;
		justify-content: left;
		align-items: auto;
	}

	.tag-view {
		/* #ifndef APP-PLUS-NVUE */
		display: flex;
		/* #endif */
		flex-direction: column;
		/* margin: 10rpx 15rpx; */
		margin-right: 10rpx;
		margin-top: 10rpx;
		justify-content: center;
		height: 50upx;
		text-align: center;
		align-items: center;
/* 		background: blue;
		border: 0rpx; */
	}
</style>
