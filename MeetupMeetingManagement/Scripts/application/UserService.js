//user service
(function(angular) {
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

		var authenticate = function(username, password, user) {
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

		var logout = function(user) {
			delete $localStorage.user;
			user.isAuthenticated = false;
			user.token = "";
			user.username = "";
		}

		return {
			loadUser: loadUser,
			authenticate: authenticate,
			logout: logout
		}
	});
})(window.angular);