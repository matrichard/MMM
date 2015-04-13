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
			then(function() {
				memberSrv.loadMembers(vm.user.token).then(function (result) {
					vm.memberList = result;
					vm.isReadOnly = false;
				});

				eventSrv.loadEvents(vm.user.token).then(function (result) {
					vm.events = result;
				});
			});
		}

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

		vm.refreshData = function() {
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

//Member Service
(function (angular) {
	angular.module('myApp').service('memberSrv', function ($http, $q, $localStorage) {
		
		function loadFromStorage(key) {
			var storedData = $localStorage[key];

			if (!storedData || !storedData.expiration || storedData.expiration < Date.now()) {
				delete $localStorage[key];
				return undefined;
			}

			return storedData.data;
		}

		function saveToStorage(key, data, expiration) {
			if (expiration === undefined) {
				expiration = 20 * 60 * 1000;
			}

			var now = new Date();
			var storedData = {
				timestamp: now.getTime(),
				data: data,
				expiration: new Date(now.getTime() + expiration).getTime()
			}

			$localStorage[key] = storedData;
		}

		function loadFromSite(token, url) {
			var deferred = $q.defer();
			var config = {
				headers: {
					"Authorization": "Bearer " + token
				}
			};

			if (url === undefined) {
				url = "/api/members";
			}

			$http.get(url, config).then(function (result) {
				$localStorage.members = result;
				saveToStorage("members", result.data);
				deferred.resolve(result.data);
			}).catch(function (error) {
				deferred.reject(error);
			});

			return deferred.promise;
		}

		function loadMembers(token) {
			var members = loadFromStorage("members");
			if (members) {
				return $q.when(members);
			} else {
				return loadFromSite(token);
			}
		};

		function refresh(token) {
			delete $localStorage["members"];

			return loadFromSite(token, "/api/members/refresh");
		}

		return {
			loadMembers: loadMembers,
			refresh: refresh
		};
	});
})(window.angular);

//Events Service
(function (angular) {
	angular.module('myApp').service('eventSrv', function ($http, $q, $localStorage) {

		function saveToStorage(key, data, expiration) {
			if (expiration === undefined) {
				expiration = 20 * 60 * 1000;
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

		var loadFromSite = function(token, url) {
			var deferred = $q.defer();
			var config = {
				headers: {
					"Authorization": "Bearer " + token
				}
			};

			if (url === undefined) {
				url = "/api/events";
			}

			$http.get(url, config).then(function (result) {
				saveToStorage("events", result.data);
				deferred.resolve(result.data);
			}).catch(function(error) {
				deferred.reject(error);
			});

			return deferred.promise;
		}

		var loadEvents = function(token) {
			var events = loadFromStorage("events");
			if (events) {
				return $q.when(events);
			} else {
				return loadFromSite(token);
			}
		};

		function refresh(token) {
			delete $localStorage["events"];

			return loadFromSite(token, "/api/events/refresh");
		}

		return {
			loadEvents: loadEvents,
			refresh: refresh
		};
	});
})(window.angular);

//user service
(function (angular) {
	angular.module('myApp').service('userSrv', function($http, $q, $localStorage) {


		function formEncode(data) {
			
				var pairs = [];
				for (var name in data) {
					pairs.push(encodeURIComponent(name) + '=' + encodeURIComponent(data[name]));
				}
				return pairs.join('&').replace(/%20/g, '+');
		};

		var saveUser = function(user) {
			
			$localStorage.user = user;

			return user;
		}

		var loadUser = function() {

			var user = $localStorage.user;
			return user ||
			{
				token: "",
				isAuthenticated: false
			};

		}

		var authenticate = function(username, password,user) {
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

			return $http.post("/oauth/token", data, config)
				.then(function(response) {
					user.isAuthenticated = true;
					user.token = response.data.access_token;
					user.username = username;
					saveUser(user);

				return $q.when(user);
			});
		}

		return {
			loadUser: loadUser,
			authenticate: authenticate
		}
	});
})(window.angular)