(function () {
    "use strict";
    angular.module('myApp')
        .controller('projectController', ProjectController)
        .controller('modalController', ModalController);

    ProjectController.$inject = ['$scope', '$http', '$mdDialog', '$window'];
    ModalController.$inject = ['$scope', '$http', '$window', '$mdDialog'];

    function ProjectController($scope, $http, $mdDialog, $window) {
        var vm = this;
        vm.$scope = $scope;
        vm.$http = $http;
        vm.$mdDialog = $mdDialog;
        vm.$window = $window;

        vm.projects = [];
        vm.addProjectEdit = false;
        vm.selectedStoryType = null;
        vm.storyTypes = [];
        vm.newProject = null;
        vm.getConfig = {
            method: 'GET',
            url: '/api/projects/',
            headers: {
                'Content-Type': 'application/json'
            }
        };
        vm.postConfig = {
            method: 'POST',
            url: '/api/projects',
            headers: {
                'Content-Type': 'application/json'
            }
        };
        vm.getStoryTypeConfig = {
            method: 'GET',
            url: '/api/storytypes/',
            headers: {
                'Content-Type': 'application/json'
            }
        };

        vm.openProjectAdd = _openProjectAdd;
        vm.openProject = _openProject;
        vm.goToFaq = _goToFaq;

        _loadProjects();
        _getStoryTypes();

        function _getStoryTypes() {
            vm.$http(vm.getStoryTypeConfig)
                .then(_getStoryTypesSuccess)
                , (_getStoryTypesError);
        }

        function _getStoryTypesSuccess(resp) {
            vm.storyTypes = resp.data.items;
            var index = vm.storyTypes.indexOf(typ => typ.name === "TV Episode");
            vm.storyTypes.splice(index, 1);
            vm.selectedStoryType = vm.storyTypes.find(typ => typ.id == 1);
        }

        function _getStoryTypesError(resp) {
            console.log(resp);
        }

        function _loadProjects() {
            vm.$http(vm.getConfig)
                .then(_loadProjectsSuccess)
                , (_loadProjectsError);
        }

        function _loadProjectsSuccess(resp) {
            console.log(resp.data.items);
            if (resp.data.items) {
                for (var i = 0; i < resp.data.items.length; i++) {
                    switch (resp.data.items[i].storyType.id) {
                        case 1:
                            resp.data.items[i].link = '/projects/edit/' + resp.data.items[i].id;
                            break;
                        case 2:
                            resp.data.items[i].link = '/projects/tv/' + resp.data.items[i].id;
                            break;
                    }
                    resp.data.items[i].dateModified = new Date(resp.data.items[i].dateModified).toDateString();
                }
            }
            vm.projects = resp.data.items;
        }

        function _loadProjectsError(resp) {
            console.log(resp);
        }

        function _openProject(proj) {
            switch (proj.storyType.id) {
                case 1:
                    vm.$window.location.href = '/projects/edit/' + proj.id;
                    break;
                case 2:
                    vm.$window.location.href = '/projects/tv/' + proj.id;
                    break;
            }
        }

        function _openProjectAdd(e) {
            var dialogOptions = {
                controller: 'modalController',
                controllerAs: 'mVm',
                flex: 75,
                templateUrl: 'addProjectDialog.html',
                targetEvent: e,
                clickOutsideToClose: true
            };
            vm.$mdDialog.show(dialogOptions);
            //vm.addProjectEdit = true;
        }

        function _cancelProject() {
            vm.newProject = null;
            vm.addProjectEdit = false;
        }

        function _goToFaq() {
            vm.$window.location.href = '/home/faq';
        }
    }

    function ModalController($scope, $http, $window, $mdDialog) {
        var vm = this;
        vm.$scope = $scope;
        vm.$http = $http;
        vm.$window = $window;
        vm.$mdDialog = $mdDialog;

        vm.selectedStoryType = null;
        vm.storyTypes = [];
        vm.getStoryTypeConfig = {
            method: 'GET',
            url: '/api/storytypes/',
            headers: {
                'Content-Type': 'application/json'
            }
        }

        vm.saveProject = _saveProject;
        vm.cancelModal = _cancelModal;

        _getStoryTypes();

        function _getStoryTypes() {
            vm.$http(vm.getStoryTypeConfig)
                .then(_getStoryTypesSuccess)
                , (_getStoryTypesError);
        }

        function _getStoryTypesSuccess(resp) {
            vm.storyTypes = resp.data.items;
            vm.selectedStoryType = vm.storyTypes.find(typ => typ.id == 1);
        }

        function _getStoryTypesError(resp) {
            console.log(resp);
        }

        function _saveProject() {
            vm.newProject.storyTypeId = vm.selectedStoryType.id;
            vm.newProject.storyType = vm.selectedStoryType.name;
            vm.newProject.autoStructured = true;
            var url = '/api/projects/';
            var data = vm.newProject;
            vm.$http.post(url, data)
                .then(_addProjectSuccess, _addProjectError);
        }

        function _addProjectSuccess(resp) {
            vm.$window.location.reload();
        }

        function _addProjectError(resp) {
            console.log(resp);
        }

        function _cancelModal() {
            vm.$mdDialog.hide();
        }
    }
})();