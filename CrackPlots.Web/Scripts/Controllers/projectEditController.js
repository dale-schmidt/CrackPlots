(function () {
    "use strict";
    angular.module('myApp')
        .controller('projectController', ProjectController);

    ProjectController.$inject = ["$http", "$window", "$sanitize", "$mdToast", "$mdDialog", "$rootScope", "$scope", "$timeout", "razorViewModel"];

    function ProjectController($http, $window, $sanitize, $mdToast, $mdDialog, $rootScope, $scope, $timeout, razorViewModel) {
        var vm = this;
        vm.$http = $http;
        vm.$window = $window;
        vm.$sanitize = $sanitize;
        vm.$mdToast = $mdToast;
        vm.$mdDialog = $mdDialog;
        vm.$rootScope = $rootScope;
        vm.$scope = $scope;
        vm.$timeout = $timeout;
        vm.razorViewModel = razorViewModel;

        vm.project = null;
        vm.collaborators = [];
        vm.newCollaborator = null;
        vm.deletedCollaborator = null;
        vm.editCollborator = false;
        vm.characters = [];
        vm.selectedCharacter = null;
        vm.timeout = {
            project: null,
            act: null,
            character: null
        };
        vm.activeAct = 0;
        vm.newCharacter = false;
        vm.characterWatches = false;
        vm.wordCount = {
            actSummary: 0
        };

        vm.saveCollaborator = _saveCollaborator;
        vm.deleteCollaborator = _deleteCollaborator;
        vm.goToAct = _goToAct;
        vm.saveAct = _saveAct;
        vm.openCharacter = _openCharacter;
        vm.openEdit = _openEdit;
        vm.saveCharacter = _saveCharacter;
        vm.closeCharacter = _closeCharacter;
        vm.saveProject = _saveProject;
        vm.addCharacter = _addCharacter;
        vm.deleteCharacter = _deleteCharacter;
        vm.goToIndex = _goToIndex;
        vm.deleteProject = _deleteProject;
        vm.openCollaboratorEdit = _openCollaboratorEdit;
        vm.selectAct = _selectAct;

        _getProject();
        _getCharacters();

        function _openEdit(entity, prop, subProp) {
            subProp = subProp || '';
            switch (entity) {
                case 'project':
                    switch (prop) {
                        case 'title':
                            vm.edit.project.save = true;
                            vm.edit.project.title = true;
                            break;
                        case 'logline':
                            vm.edit.project.save = true;
                            vm.edit.project.logline = true;
                            break;
                        case 'notes':
                            vm.edit.project.save = true;
                            vm.edit.project.notes = true;
                            break;
                        case 'plot':
                            switch (subProp) {
                                case 'a':
                                    vm.edit.project.plots.a = true;
                                    vm.edit.project.save = true;
                                    break;
                                case 'b':
                                    vm.edit.project.plots.b = true;
                                    vm.edit.project.save = true;
                                    break;
                                case 'c':
                                    vm.edit.project.plots.c = true;
                                    vm.edit.project.save = true;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 'collaborators':
                            vm.edit.project.collaborators = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case 'act':
                    vm.edit.act.save = true;
                    switch (prop) {
                        case 'title':
                            vm.edit.act.title = true;
                            break;
                        case 'summary':
                            vm.edit.act.summary = true;
                            break;
                        case 'notes':
                            vm.edit.act.notes = true;
                            break;
                        case 'centralQuestion':
                            vm.edit.act.centralQuestion = true;
                        default:
                            break;
                    }
                    break;
                case 'character':
                    vm.edit.character.save = true;
                    switch (prop) {
                        case 'name':
                            vm.edit.character.name = true;
                            break;
                        case 'want':
                            vm.edit.character.want = true;
                            break;
                        case 'need':
                            vm.edit.character.need = true;
                            break;
                        case 'biography':
                            vm.edit.character.biography = true;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        function _setWatches() {
            vm.$scope.$watch('pVm.project.title', _debounceSaveProject);
            vm.$scope.$watch('pVm.project.logline', _debounceSaveProject);
            vm.$scope.$watch('pVm.project.notes', _debounceSaveProject);
            _setActWatches();
        }

        function _wordCount(str) {
            str = _htmlToPlaintext(str);
            return str ? str.split(" ").length : 0;
        }

        function _htmlToPlaintext(text) {
            return text ? String(text).replace(/<[^>]+>/gm, '') : '';
        }

        // Navigation functions
        function _goToIndex() {
            vm.$window.location.href = "/projects";
        }

        function _goToAct(id) {
            window.location.href = "/projects/acts/" + id;
        }

        // Project functions
        function _getProject() {
            vm.$http({
                method: 'GET',
                url: '/api/projects/' + vm.razorViewModel.Item
            }).then(_getProjectSuccess, _getProjectError);
        }

        function _getProjectSuccess(resp) {
            console.log(resp);
            console.log(vm.razorViewModel);
            vm.project = resp.data.item;
            if (vm.project.plots) {
                vm.project.aPlotDescription = vm.project.plots.find(p => p.plotName == 'a').description;
                vm.project.bPlotDescription = vm.project.plots.find(p => p.plotName == 'b').description;
                vm.project.cPlotDescription = vm.project.plots.find(p => p.plotName == 'c').description;
            }
            for (var i = 0; i < vm.project.users.length; i++) {
                if (vm.project.users[i].id != vm.razorViewModel.Id) {
                    vm.collaborators.push(vm.project.users[i]);
                }
            }
            vm.wordCount.actSummary = _wordCount(vm.project.acts[0].summary);
            _setWatches();
            vm.$rootScope.$broadcast('show-title', vm.project.title, vm.project.id);
        }

        function _getProjectError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to load project')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
        }

        function _saveProject() {
            vm.$rootScope.$broadcast('saving-started');
            var url = '/api/projects/' + vm.project.id;
            vm.project.notes = vm.$sanitize(vm.project.notes);
            vm.project.storyTypeId = vm.project.storyType.id;
            vm.$http.put(url, vm.project)
                .then(_saveProjectSuccess, _saveProjectError);
        }

        function _saveProjectSuccess(resp) {
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _saveProjectError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to save project')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
        }

        function _deleteProject(e) {
            var confirm = vm.$mdDialog.confirm()
                .title('Are you sure you want to delete this project?')
                .textContent('You will not be able to recover it')
                .targetEvent(e)
                .ok('Ok')
                .cancel('Cancel');
            vm.$mdDialog.show(confirm).then(function () {
                var url = "/api/projects/" + vm.project.id;

                vm.$http.delete(url)
                    .then(_deleteProjectSuccess, _deleteProjectError);
            });
        }

        function _deleteProjectSuccess(resp) {
            vm.$window.location.href = "/projects";
        }

        function _deleteProjectError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to delete project')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
        }

        function _debounceSaveProject(newVal, oldVal) {
            if (newVal !== oldVal) {
                if (vm.timeout.project) {
                    vm.$timeout.cancel(vm.timeout.project);
                }
                vm.timeout.project = vm.$timeout(_liveProjectUpdates, 1000);
            }
        }

        function _liveProjectUpdates() {
            _saveProject();
        }

        // Collaborator functions
        function _openCollaboratorEdit() {
            vm.editCollborator = true;
        }

        function _saveCollaborator() {
            vm.$rootScope.$broadcast('saving-started');
            var url = '/api/projectsperson';
            var data = {
                projectId: vm.project.id,
                email: vm.newCollaborator
            };

            vm.$http.post(url, data)
                .then(_saveCollaboratorSuccess, _saveCollaboratorError);
        }

        function _saveCollaboratorSuccess(resp) {
            vm.editCollborator = false;
            vm.$rootScope.$broadcast('saving-finished');
            if (resp.data.item) {
                vm.collaborators.push(resp.data.item);
                vm.newCollaborator = "";
            }
        }

        function _saveCollaboratorError(resp) {
            console.log(resp);
            vm.$rootScope.$broadcast('saving-finished');
            if (resp.status === 400) {
                alert("Could not find user");
            } else {
                var toast = vm.$mdToast.simple()
                    .content('Failed to add collaborator')
                    .position('top right')
                    .hideDelay(1000)
                    .parent(".main-wrapper")
                    .theme('error-toast');

                vm.$mdToast.show(toast);
            }
        }

        function _deleteCollaborator(person) {
            vm.$rootScope.$broadcast('saving-started');
            var url = '/api/projectsperson/' + vm.project.id + "/" + person.id;
            vm.deletedCollaborator = person;

            vm.$http.delete(url)
                .then(_deleteCollaboratorSuccess, _deleteCollaboratorError);
        }

        function _deleteCollaboratorSuccess(resp) {
            vm.$rootScope.$broadcast('saving-finished');
            var index = vm.collaborators.indexOf(c => c.id == vm.deletedCollaborator.id);
            vm.collaborators.splice(index, 1);
        }

        function _deleteCollaboratorError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to delete collaborator')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        // Act functions
        function _selectAct() {
            vm.wordCount.actSummary = _wordCount(vm.project.acts[vm.activeAct].summary);
            _setActWatches();
        }

        function _setActWatches() {
            vm.$scope.$watch('pVm.project.acts[' + vm.activeAct + '].centralQuestion', _debounceSaveAct);
            vm.$scope.$watch('pVm.project.acts[' + vm.activeAct + '].summary', _debounceSaveAct);
            vm.$scope.$watch('pVm.project.acts[' + vm.activeAct + '].notes', _debounceSaveAct);
        }

        function _saveAct(id) {
            vm.$rootScope.$broadcast('saving-started');
            var url = '/api/acts/' + id;
            var act = vm.project.acts.find(a => a.id == id);
            var data = {
                id: act.id,
                title: act.title,
                summary: vm.$sanitize(act.summary),
                notes: vm.$sanitize(act.notes),
                centralQuestion: act.centralQuestion,
                projectId: act.projectId
            };
            vm.$http.put(url, data)
                .then(_saveActSuccess, _saveActError);
        }

        function _saveActSuccess(resp) {
            //$.connection.hub.start().done(function () {
            //    vm.projectHub.server.updateAct(vm.groupName, vm.act);
            //});
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _saveActError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to save act')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _debounceSaveAct(newVal, oldVal) {
            if (newVal !== oldVal) {
                if (vm.timeout.act) {
                    vm.$timeout.cancel(vm.timeout.act);
                }
                vm.timeout.act = vm.$timeout(_liveActUpdates, 1000);
            }
        }

        function _liveActUpdates() {
            _saveAct(vm.project.acts[vm.activeAct].id);
            vm.wordCount.actSummary = _wordCount(vm.project.acts[vm.activeAct].summary);
        }

        // Character functions
        function _setCharacterWatches() {
            if (!vm.characterWatches) {
                vm.characterWatches = true;
                vm.$scope.$watch('pVm.selectedCharacter.name', _debounceSaveCharacter);
                vm.$scope.$watch('pVm.selectedCharacter.want', _debounceSaveCharacter);
                vm.$scope.$watch('pVm.selectedCharacter.need', _debounceSaveCharacter);
                vm.$scope.$watch('pVm.selectedCharacter.biography', _debounceSaveCharacter);
            }
        }

        function _destroyCharacterWatches() {

        }

        function _getCharacters() {
            vm.$http({
                method: 'GET',
                url: '/api/characters/project/' + vm.razorViewModel.Item
            }).then(_getCharactersSuccess, _getCharactersError)
        }

        function _getCharactersSuccess(resp) {
            console.log(resp);
            vm.characters = resp.data.items;
        }

        function _getCharactersError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to get characters')
                .position('top right')
                .hideDelay(1000)
                .parent("#selectedCharacterPanel")
                .theme('error-toast');

            vm.$mdToast.show(toast);
        }

        function _openCharacter(id) {
            vm.newCharacter = false;
            vm.selectedCharacter = vm.characters.find(ch => ch.id == id);
            _setCharacterWatches();
        }

        function _addCharacter() {
            vm.selectedCharacter = {
                name: 'NAME',
                want: '',
                need: '',
                biography: ''
            };
            _setCharacterWatches();
            vm.newCharacter = true;
        }

        function _saveCharacter() {
            if (vm.selectedCharacter) {
                vm.$rootScope.$broadcast('saving-started');
                if (vm.selectedCharacter.need) {
                    vm.selectedCharacter.need = vm.$sanitize(vm.selectedCharacter.need);
                }
                if (vm.selectedCharacter.want) {
                    vm.selectedCharacter.want = vm.$sanitize(vm.selectedCharacter.want);
                }
                if (vm.selectedCharacter.biography) {
                    vm.selectedCharacter.biography = vm.$sanitize(vm.selectedCharacter.biography);
                }
                if (vm.newCharacter) {
                    var url = '/api/characters/';
                    var data = vm.selectedCharacter;
                    vm.selectedCharacter.projectId = vm.project.id;
                    vm.$http.post(url, data)
                        .then(_newCharacterSuccess, _newCharacterError);
                } else {
                    var url = '/api/characters/' + vm.selectedCharacter.id;
                    vm.$http.put(url, vm.selectedCharacter)
                        .then(_saveCharacterSuccess, _saveCharacterError);
                }
            }
        }

        function _newCharacterSuccess(resp) {
            vm.selectedCharacter.id = resp.data.item;
            if (vm.characters == null) {
                vm.characters = [];
            }
            vm.characters.push(vm.selectedCharacter);
            vm.newCharacter = false;
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _newCharacterError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to add character')
                .position('top right')
                .hideDelay(1000)
                .parent("#selectedCharacterPanel")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _saveCharacterSuccess(resp) {
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _saveCharacterError(resp) {
            console.log(resp);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _closeCharacter() {
            vm.selectedCharacter = null;
        }

        function _deleteCharacter() {
            vm.$rootScope.$broadcast('saving-started');
            var url = '/api/characters/' + vm.selectedCharacter.id;
            vm.$http.delete(url)
                .then(_deleteCharacterSuccess, _deleteCharacterError);
        }

        function _deleteCharacterSuccess(resp) {
            var index = vm.characters.indexOf(vm.characters.find(c => c.id == vm.selectedCharacter.id));
            vm.characters.splice(index, 1);
            vm.selectedCharacter = null;
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _deleteCharacterError(resp) {
            console.log(resp);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _debounceSaveCharacter(newVal, oldVal) {
            if (newVal !== oldVal) {
                if (vm.timeout.character) {
                    vm.$timeout.cancel(vm.timeout.character);
                }
                vm.timeout.character = vm.$timeout(_liveCharacterUpdates, 1000);
            }
        }

        function _liveCharacterUpdates() {
            _saveCharacter();
        }
    }
})();