var WebGLFunctions = {    

    RequestPlugConnect: function(cbIndex) {
        dispatchReactUnityEvent("RequestPlugConnect", cbIndex);
    },
    
    CheckPlugConnection: function(cbIndex) {
        dispatchReactUnityEvent("CheckPlugConnection", cbIndex);
    },
    
    GetPlugNfts: function (cbIndex) 
    {
        dispatchReactUnityEvent("GetPlugNfts", cbIndex);
    },
	
	Pay: function (cbIndex, account, amount) 
    {
        dispatchReactUnityEvent("Pay", cbIndex, UTF8ToString(account), amount);
    }
};

mergeInto(LibraryManager.library, WebGLFunctions);