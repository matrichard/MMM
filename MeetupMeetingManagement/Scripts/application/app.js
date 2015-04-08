(function (angular) {
	"use strict";
	angular.module('myApp', ["ui.bootstrap","ngStorage"]);
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
	angular.module('myApp').service('memberSrv', function ($http, $q, $localStorage) {
		function loadFromSite() {
			var deferred = $q.defer();

			$http.get("/api/members").then(function (result) {
				$localStorage.members = result;
				deferred.resolve(result);
			}).catch(function (error) {
				deferred.reject(error);
			});

			return deferred.promise;
		}

		function loadMembers() {
			if ($localStorage.members) {
				return $q.when($localStorage.members);
			} else {
				return loadFromSite();
			}
		};

		return {
			loadMembers: loadMembers
		};
	});
})(window.angular);

//Events Service
(function (angular) {
	angular.module('myApp').service('eventSrv', function ($http, $q, $localStorage) {

		function saveToStorage(key, data, expiration) {
			if (expiration === undefined) {
				expiration = 1 * 60 * 1000;
			}

			var now = new Date();
			var storedData = {
				timestamp: now.getTime(),
				data: data,
				expiration: new Date(now.getTime() + expiration).getTime()
		}
			
			$localStorage[key] = storedData;
		}

		function loadFromStorage(key) {
			var storedData = $localStorage[key];
			
			if (!storedData || !storedData.expiration || storedData.expiration < Date.now()) {
				delete $localStorage[key];
				return undefined;
			}

			return storedData.data;
		}

		var loadFromSite = function() {
			var deferred = $q.defer();
			$http.get("/api/events").then(function (result) {
				saveToStorage("events", result.data);
				deferred.resolve(result);
			}).catch(function(error) {
				deferred.reject(error);
			});

			return deferred.promise;
		}

		var loadEvents = function() {
			var events = loadFromStorage("events");
			if (events) {
				return $q.when(events);
			} else {
				return loadFromSite();
			}
		};

		return {
			loadEvents: loadEvents
		};
	});
})(window.angular);