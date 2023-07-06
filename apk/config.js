const app = {
    apiUrl: 'http://127.0.0.1:40005/api/', //请求的地址
    baseRequest(obj) {
        try {
            const userToken = uni.getStorageSync('userToken');
            if (userToken) {
                if (obj.header) {
                    obj.header["token"] = userToken;
					obj.header['Content-Type']='application/json; charset=utf-8'
                } else {
                    obj.header = { "token": userToken ,"Content-Type":'application/json; charset=utf-8'};
                }
                obj.url = this.apiUrl + obj.url;
                uni.request(obj)
            }
            else{
                console.log("获取不到userToken")
                
            }
        } catch (e) {
            console.log(e)
            console.log("获取不到userToken")
        } 
    },
}
export default app;