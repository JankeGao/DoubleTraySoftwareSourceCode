<template>
	<view class="content">
		<!-- 		<image class="logo" src="/static/logo.png"></image>
		<view class="text-area">
			<text class="title">{{title}}</text>
		</view> -->
		<view class="avatarWrapper">
<!-- 			<view class="avatar">
				<image class="img" src="../../static/logo.png" ></image>
			</view> -->
		     <image class="logo" src="../../static/logo.png" ></image>
		</view>
		<view class="form">
			<form id="dataForm"  @submit="formSubmit" @reset="formReset">
				<view class="inputWrapper">
					<input name="Code" type="text" placeholder="请输入用户名" />
				</view>
				<view class="inputWrapper">
					<input type="text" password="true" name="Password" placeholder="请输入密码" />
				</view>
				<view class="loginbtn">
					<button class="btnValue" form-type="submit">登陆</button>
				</view>
				<view class="loginbtn">
					<button class="btnValue" form-type="reset"><text  style="text-align: center; width: 100%;">重置</text></button>
				</view>
			</form>
		</view>


	</view>
</template>

<script>
	import md5 from '../../common/MD5/md5.js'
	export default {
		data() {
			return {

			}
		},
		onLoad() {

		},
		methods: {
			formSubmit: function(e) {
				console.log(e.detail.value)
				var formdata = e.detail.value
				formdata.Password = md5(formdata.Password)
				uni.showModal({
					content: 'fromdata.Password：' + JSON.stringify(formdata),
					showCancel: false
				});
				uni.request({
					url: this.apiUrl + 'Login/PostLogin',
					method: "post",
					data: formdata,
					success: res => {
						var resData = JSON.parse(res.data.Content)
						if (resData.Success) {
							uni.setStorageSync('userToken', resData.Data)
							uni.navigateTo({
								url: '../main/main'
							})
						}
						uni.showModal({
							content: resData.Message,
							showCancel: false
						});
					},
					fail: () => {

					},
					complete: () => {

					}
				})
			},
			formReset: function(e) {
				console.log('清空数据')
			}
		}
	}
</script>

<style>
	.content {
/* 		display: flex;
		flex-direction: column; */
/* 		align-items: center;
		justify-content: center; */
		width: 100vm;
		height: 100vh;
		background-color: #007AFF;
	}

	.logo {
		height: 200rpx;
		width: 200rpx;
		margin-top: 200rpx;
		margin-left: auto;
		margin-right: auto;
		margin-bottom: 50rpx;
	}

	.text-area {
		display: flex;
		justify-content: center;
	}

	.title {
		font-size: 36rpx;
		color: #8f8f94;
	}
    .avatarWrapper{
		height: 30vh;
		width: 100vm;
		display: flex;
		justify-content: center;
		align-items: flex-end;
	}
	.avatar{
		width: 200upx;
		height: 200upx;
		overflow: hidden;
	}
	.avatar img{
		width: 100%;
	}
	.form{
		padding: 0,100upx;
		margin-top: 80upx;
	}
	.inputWrapper{
/* 		width: 80%;
		height: 80upx;
		background: white;
		box-sizing: border-box;
		padding: 0 20px;
		border-radius: 20px;
		margin-top: 25px;
		justify-content: center;
		content: center; */
		width: 100%;
		height: 80upx;
		background:#007AFF;
		border-radius: 50upx;
		margin-top: 15px;
		display: flex;
		justify-content: center;
		align-items: center;
		
	}
	.inputWrapper input{
		width: 80%;
		height: 100%;
		text-align: center;
		font-size: 15px;
		display: flex;
		justify-content: center;
		background: white;
		border-radius: 50upx;
		margin-bottom: 20px;
	}
	.btnValue{
		color: black;
		width: 80%;
		border-radius: 50upx;
		height: 80upx;
		font-size: 15px;
		font-weight:bold;
		margin-top: 20upx;
		background-color:  #77B3D7;
	    text-align: center;
		display: inline-block;
		/* display:flex; */
		line-height: 80upx;

	}
	.text{
		 text-align: center;
		 width: 100%;
		 height: 100%;
		
	}
	.loginbtn{
		width: 100%;
		height: 80upx;
		background: #007AFF;
/* 		border-radius: 50upx;
		display: flex;
		justify-content: center;
		align-items: center; */
		 margin-top: 50px;
		 display: flex;
	}
/* 	.loginbtn button{
		color: black;
		width: 80%;
		height: 80upx;
		border-radius: 50upx;
		display: flex;
		height: 80upx;
		font-size: 15px;
		font-weight:bold;
	} */
	.forgotBtn{
		text-align: center;
		color: #77B3D7;
		font-size: 15px;
		margin-top: 20px;
	}
</style>
