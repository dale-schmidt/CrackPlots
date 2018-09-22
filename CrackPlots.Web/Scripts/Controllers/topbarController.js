(function () {
    angular.module('myApp')
        .controller('topbarController', TopbarController);

    TopbarController.$inject = ["$http", "$window", "$scope", "razorViewModel"];

    function TopbarController($http, $window, $scope, razorViewModel) {
        var vm = this;
        vm.$http = $http;
        vm.$window = $window;
        vm.$scope = $scope;
        vm.razorViewModel = razorViewModel;

        vm.saving = false;
        vm.showTitle = false;
        vm.showLeftArrow = false;
        vm.showRightArrow = false;
        vm.projectTitle = '';
        vm.projectId = '';
        vm.leftArrowLink = '';
        vm.rightArrowLink = '';
        vm.leftArrowText = '';
        vm.rightArrowText = '';

        vm.logout = _logout;

        vm.$scope.$on('saving-started', _savingStarted);
        vm.$scope.$on('saving-finished', _savingFinished);
        vm.$scope.$on('show-title', _showTitle);
        vm.$scope.$on('show-arrows', _showArrows);

        function _logout() {
            var url = '/api/users/logout';
            vm.$http.get(url)
                .then(_logoutSuccess, _logoutError);
        }

        function _logoutSuccess(resp) {
            vm.$window.location.href = '/';
        }

        function _logoutError(resp) {
            console.log(resp);
        }

        function _savingStarted() {
            vm.saving = true;
        }

        function _savingFinished() {
            vm.saving = false;
        }

        function _showTitle(e, title, id) {
            vm.showTitle = true;
            vm.projectTitle = title;
            vm.projectId = id;
        }

        function _showArrows(e, entity, id, ids) {
            if (ids.indexOf(id) === 0) {
                vm.showRightArrow = true;
                vm.rightArrowLink = '/projects/' + entity + '/' + ids[1];
            } else if (ids.indexOf(id) === ids.length - 1) {
                vm.showLeftArrow = true;
                vm.leftArrowLink = '/projects/' + entity + '/' + ids[ids.length - 2];
            } else {
                vm.showRightArrow = true;
                vm.showLeftArrow = true;
                vm.rightArrowLink = '/projects/' + entity + '/' + ids[ids.indexOf(id) + 1];
                vm.leftArrowLink = '/projects/' + entity + '/' + ids[ids.indexOf(id) - 1];
            }
            switch (entity) {
                case 'acts':
                    vm.leftArrowText = 'Previous Act';
                    vm.rightArrowText = 'Next Act';
                    break;
                case 'episode':
                    vm.leftArrowText = 'Previous Episode';
                    vm.rightArrowText = 'Next Episode';
                    break;
                default:
                    break;
            }
        }
    }
})();