//MemberListCtrl
(function (angular) {
	"use strict";

	angular.module("myApp").controller("MemberListCtrl", function (memberSrv, eventSrv, userSrv, $filter) {
		var vm = this;
		vm.memberList = [];
		vm.events = [];
		vm.isReadOnly = true;
		vm.isSelected = false;
		vm.selectedMember = undefined;
		vm.user = userSrv.loadUser();
		vm.model = {
			username: "",
			password: ""
		};

		if (vm.user.isAuthenticated) {
			memberSrv.loadMembers(vm.user.token).then(function (result) {
				vm.memberList = result;
				vm.isReadOnly = false;
			});

			eventSrv.loadEvents(vm.user.token).then(function (result) {
				vm.events = result;
			});
		}

		vm.authenticate = function () {
			userSrv.authenticate(vm.model.username, vm.model.password, vm.user).
			then(function () {
				memberSrv.loadMembers(vm.user.token).then(function (result) {
					vm.memberList = result;
					vm.isReadOnly = false;
				});

				eventSrv.loadEvents(vm.user.token).then(function (result) {
					vm.events = result;
				});
			});
		}

		vm.logout = function() {
			userSrv.logout(vm.user);
		}

		vm.onSelect = function ($item, $model, $label) {
			vm.isSelected = true;
			angular.forEach(vm.events, function (e, key) {
				var found = $filter('filter')(e.rsvps, { memberId: $item.id }, true);
				if (found.length) {
					e.rsvpStatus = found[0].status;
				} else {
					e.rsvpStatus = "no";
				}
			});
		}

		vm.refreshData = function () {
			vm.isReadOnly = true;
			eventSrv.refresh(vm.user.token);
			memberSrv.refresh(vm.user.token).then(function () {
				vm.isReadOnly = false;
				vm.selectedMember = {};
				vm.isSelected = false;
			});
		};
	});
})(window.angular);