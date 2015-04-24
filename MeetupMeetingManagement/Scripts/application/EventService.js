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

		var loadFromSite = function (token, url) {
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
			}).catch(function (error) {
				deferred.reject(error);
			});

			return deferred.promise;
		}

		var loadEvents = function (token) {
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