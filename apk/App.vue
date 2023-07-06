<script>
	import api from 'config.js'
	export default {
		onLaunch: function() {
			console.log('App Launch')
		},
		onShow: function() {
			console.log('App Show')
		},
		onHide: function() {
			console.log('App Hide')
		},
		data(){
			return {
				apiUrl:'http://127.0.0.1:40005/api/'
			}
		}
	}
</script>

<style>
	/*每个页面公共css */
	    @import './common/uni.css';
</style>
