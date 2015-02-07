(function (angular) {
	"use strict";

	angular.module('myApp', []);
})(window.angular);

//MainCtrl
(function (angular) {
	"use strict";

	angular.module("myApp").controller("MainCtrl", function() {
			this.greeting = "hola from mainCtrl";
		});
})(window.angular);

//Member Service
(function (angular) {
	var Member = 
	angular.module('myApp').service('memberSrv', function() {
		
	});
})(window.angular);