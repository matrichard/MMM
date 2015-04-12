(function (angular) {
	"use strict";
	angular.module('myApp', ["ui.bootstrap","ngStorage"]);
})(window.angular);

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
		vm.model= {
			username: "",
			password:""
		};


		vm.authenticate = function() {
			userSrv.authenticate(vm.model.username, vm.model.password)
				.then(function(user) {
					vm.user = user;
				});
		}

		memberSrv.loadMembers(vm.user.token).then(function (result) {
			vm.memberList = result.data;
			vm.isReadOnly = false;
		});
		eventSrv.loadEvents().then(function(result) {
			vm.events = result.data;
		});

		vm.onSelect = function ($item, $model, $label) {
			vm.isSelected = true;
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
		function loadFromSite(token) {
			var deferred = $q.defer();

			$http.get("/api/members").then(function (result) {
				$localStorage.members = result;
				deferred.resolve(result);
			}).catch(function (error) {
				deferred.reject(error);
			});

			return deferred.promise;
		}

		function loadMembers(token) {
			if ($localStorage.members) {
				return $q.when($localStorage.members);
			} else {
				return loadFromSite(token);
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

//user service
(function (angular) {
	angular.module('myApp').service('userSrv', function($http, $q, $localStorage) {


		var formEncode = function() {
			return function(data) {
				var pairs = [];
				for (var name in data) {
					pairs.push(encodeURIComponent(name) + '=' + encodeURIComponent(data[name]));
				}
				return pairs.join('&').replace(/%20/g, '+');
			};
		};

		var saveUser = function(username, token) {
			var user = {
				token: token,
				isAuthenticated: true,
				username: username
			}

			$localStorage.user = user;
		}

		var loadUser = function() {

			var user = $localStorage.user;
			return user ||
			{
				token: "",
				isAuthenticated: false
			};

		}



		var authenticate = function(username, password) {
			var config = {
				headers: {
					"Content-Type": "application/x-www-form-urlencoded"
				}
			}

			var data = formEncode({
				username: username,
				password: password,
				grant_type: "password"
			});

			return $http.post("/oauth/token", data, config).then(saveUser(username, response.data.access_token));
		}

		return {
			loadUser: loadUser,
			authenticate: authenticate
		}
	});
})(window.angular)