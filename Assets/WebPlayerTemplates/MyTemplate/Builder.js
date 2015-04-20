var unity;
var goName;

function setUnity(unityObj) {
	unity = unityObj;
	
	window.onhashchange = function() {
		GetHash();
	};
}

function Start(gameObjectName) {
	unity = unity.getUnity();
	goName = gameObjectName;
}

function GetHash() {
	var hash = window.location.hash
	if (hash) {
		hash = hash.substring(1);
	}
	unity.SendMessage(goName, 'GetHash', hash);
}

function PushState(code) {
	console.log(code);
	history.pushState(null,'','#'+code);
}