(function (angular) {
	"use strict";
	angular.module('myApp', ["ui.bootstrap", 'smart-table']);
})(window.angular);

//MemberListCtrl
(function (angular) {
	"use strict";

	angular.module("myApp").controller("MemberListCtrl", function (memberSrv, $filter) {
		var vm = this;
		vm.memberList = [];
		vm.isReadOnly = true;
		vm.selectedMember = undefined;
		memberSrv.loadMembers().then(function (result) {
			vm.memberList = result.data
			vm.isReadOnly = false;
		});
	});
})(window.angular);

//Member Service
(function (angular) {
	angular.module('myApp').service('memberSrv', function($http) {
		function loadMembers() {
			return $http.get("api/members");
		};

		return {
			loadMembers: loadMembers
		};
	});
})(window.angular);