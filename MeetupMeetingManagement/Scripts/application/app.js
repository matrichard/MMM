(function (angular) {
	"use strict";

	angular.module('myApp', []);
})(window.angular);

//MemberListCtrl
(function (angular) {
	"use strict";

	angular.module("myApp").controller("MemberListCtrl", function (memberSrv) {
		this.memberList = memberSrv.loadMembers();
	});
})(window.angular);

//Member Service
(function (angular) {
	angular.module('myApp').service('memberSrv', function() {
		function loadMembers() {
			return JSON.parse('[{"id":"54d91d8465eb499b75fd7b7a","picture":"http://placehold.it/32x32","status":"active","fName":"Daniel","lName":" Beasley","registered":"2014-01-30T16:16:35 +05:00"},{"id":"54d91d845315eae6bf622cf6","picture":"http://placehold.it/32x32","status":"active","fName":"Cox","lName":" Barron","registered":"2014-05-28T23:19:19 +04:00"},{"id":"54d91d841e73585a7caaff82","picture":"http://placehold.it/32x32","status":"soontobe","fName":"Brenda","lName":" Bridges","registered":"2014-07-13T08:09:31 +04:00"}]');
		};

		return {
			loadMembers: loadMembers
		};
	});
})(window.angular);