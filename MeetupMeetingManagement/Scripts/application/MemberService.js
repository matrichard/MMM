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