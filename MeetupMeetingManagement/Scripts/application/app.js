(function (angular) {
	"use strict";
	angular.module('myApp', ["ui.bootstrap", 'smart-table']);
})(window.angular);

//MemberListCtrl
(function (angular) {
	"use strict";

	angular.module("myApp").controller("MemberListCtrl", function (memberSrv, eventSrv, $filter) {
		var vm = this;
		vm.memberList = [];
		vm.events = [];
		vm.isReadOnly = true;
		vm.isSelected = false;
		vm.selectedMember = undefined;

		memberSrv.loadMembers().then(function (result) {
			vm.memberList = result.data;
			vm.isReadOnly = false;
		});
		eventSrv.loadEvents().then(function(result) {
			vm.events = result.data;
		});

		vm.onSelect = function ($item, $model, $label) {
			vm.isSelected = true;
			console.log("in method");
			angular.forEach(vm.events, function(e, key) {
				var found = $filter('filter')(e.rsvps, { memberId: $item.id }, true);
				if (found.length) {
					e.rsvpStatus = found[0].status;
				} else {
					e.rsvpStatus = "no";
				}
			});
		}
	});
})(window.angular);

//Member Service
(function (angular) {
	angular.module('myApp').service('memberSrv', function($http) {
		function loadMembers() {
			return $http.get("/api/members");
		};

		return {
			loadMembers: loadMembers
		};
	});
})(window.angular);

//Events Service
(function (angular) {
	angular.module('myApp').service('eventSrv', function ($http) {
		function loadEvents() {
			return $http.get("/api/events");
		};

		return {
			loadEvents: loadEvents
		};
	});
})(window.angular);