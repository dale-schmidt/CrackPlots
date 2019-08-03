(function () {
    "use strict";
    angular.module('myApp')
        .controller('projectController', ProjectController);

    ProjectController.$inject = ["$http", "$window", "$sanitize", "$mdToast", "$mdDialog", "$timeout", "$scope", "$rootScope", "razorViewModel"];

    function ProjectController($http, $window, $sanitize, $mdToast, $mdDialog, $timeout, $scope, $rootScope, razorViewModel) {
        var vm = this;
        vm.$http = $http;
        vm.$window = $window;
        vm.$sanitize = $sanitize;
        vm.$mdToast = $mdToast;
        vm.$mdDialog = $mdDialog;
        vm.$timeout = $timeout;
        vm.$scope = $scope;
        vm.$rootScope = $rootScope;
        vm.razorViewModel = razorViewModel;

        vm.project = null;
        vm.collaborators = [];
        vm.newCollaborator = null;
        vm.deletedCollaborator = null;
        vm.characters = [];
        vm.selectedCharacter = null;
        vm.newCharacter = false;
        vm.projectTimeout = null;
        vm.selectedAct = null;
        vm.add = {
            character: false,
            newCharacter: false
        };
        vm.wordCount = {
            actSummary: 0,
            sceneSummary: 0
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
        vm.selectScene = _selectScene;
        vm.addPlot = _addPlot;
        vm.deletePlot = _deletePlot;
        vm.setAct = _setAct;

        _getProject();
        _getCharacters();

        function _openEdit(entity, prop, subProp) {
            if (subProp) {
                vm.edit.project.plots[subProp] = true;
                vm.edit.project.save = true;
            } else {
                vm.edit[entity][prop] = true;
                vm.edit[entity].save = true;
            }
        }

        function _addPlot() {
            var newName = String.fromCharCode(vm.project.plots[vm.project.plots.length - 1].plotName.charCodeAt(0) + 1);
            var plot = {
                id: 999,
                plotName: newName,
                description: '',
                projectId: vm.project.id
            }
            vm.project.plots.push(plot);
            _saveProject();
            vm.$scope.$watch('pVm.project.plots[' + (vm.project.plots.length - 1) + '].description', _debounceSaveProject);
        }

        function _deletePlot(e, plotId, plotName) {
            var confirm = vm.$mdDialog.confirm()
                .title('Are you sure you want to delete this plot?')
                .textContent('You will not be able to recover it')
                .targetEvent(e)
                .ok('Ok')
                .cancel('Cancel');
            vm.$mdDialog.show(confirm).then(function () {
                var index = vm.project.plots.findIndex(p => p.id === plotId);
                vm.project.plots.splice(index, 1);
                for (var i = index; i < vm.project.plots.length; i++) {
                    vm.project.plots[i].plotName = String.fromCharCode(i + 97);
                    _saveProject();
                }
            });
        }

        function _setWatches() {
            vm.$scope.$watch('pVm.project.notes', _debounceSaveProject);
            vm.$scope.$watch('pVm.project.logline', _debounceSaveProject);
            for (var i = 0; i < vm.project.plots.length; i++) {
                vm.$scope.$watch('pVm.project.plots[' + i + '].description', _debounceSaveProject);
            }
            _setActWatches();
            _setSceneWatches();
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
                url: '/api/projects/episodes/' + vm.razorViewModel.Item
            }).then(_getProjectSuccess, _getProjectError);
        }

        function _getProjectSuccess(resp) {
            console.log(resp);
            console.log(vm.razorViewModel);
            vm.project = resp.data.item;
            for (var i = 0; i < vm.project.users.length; i++) {
                if (vm.project.users[i].id != vm.razorViewModel.Id) {
                    vm.collaborators.push(vm.project.users[i]);
                }
            }
            vm.selectedAct = vm.project.acts[0];
            _setWatches();
            vm.wordCount.actSummary = _wordCount(vm.selectedAct.summary);
            _selectScene(vm.project.acts[0].sequences[0].scenes[0], vm.project.acts[0].sequences[0].scenes.length);
            vm.$timeout(function () {
                angular.element('#sceneNavBar > div > nav > ul > li:nth-child(1) > button').triggerHandler('click');
            });
            vm.$rootScope.$broadcast('show-title', vm.project.title, vm.project.id);
            vm.$rootScope.$broadcast('show-arrows', 'episode', vm.project.id, vm.project.episodeIds);
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
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _deleteProject(e) {
            var confirm = vm.$mdDialog.confirm()
                .title('Are you sure you want to delete this project?')
                .textContent('You will not be able to recover it')
                .targetEvent(e)
                .ok('Ok')
                .cancel('Cancel');
            vm.$mdDialog.show(confirm).then(function () {
                vm.$rootScope.$broadcast('saving-started');
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
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _debounceSaveProject(newVal, oldVal) {
            if (newVal !== oldVal) {
                if (vm.projectTimeout) {
                    vm.$timeout.cancel(vm.projectTimeout);
                }
                vm.projectTimeout = vm.$timeout(_liveProjectUpdates, 1000);
            }
        }

        function _liveProjectUpdates() {
            _saveProject();
        }

        // Collaborator functions
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
            vm.edit.project.collaborators = false;
            if (resp.data.item) {
                vm.collaborators.push(resp.data.item);
                vm.newCollaborator = "";
            }
            var toast = vm.$mdToast.simple()
                .content('Saved collaborator!')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('success-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
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
        function _setAct(act) {
            vm.selectedAct = act;
            vm.wordCount.actSummary = _wordCount(vm.selectedAct.summary);
            _selectScene(vm.selectedAct.sequences[0].scenes[0], vm.selectedAct.sequences[0].scenes.length);
            _setActWatches();
        }

        function _setActWatches() {
            vm.$scope.$watch('pVm.project.acts[' + vm.project.acts.findIndex(a => a.id === vm.selectedAct.id) + '].centralQuestion', _debounceSaveAct);
            vm.$scope.$watch('pVm.project.acts[' + vm.project.acts.findIndex(a => a.id === vm.selectedAct.id) + '].summary', _debounceSaveAct);
            vm.$scope.$watch('pVm.project.acts[' + vm.project.acts.findIndex(a => a.id === vm.selectedAct.id) + '].notes', _debounceSaveAct);
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
                if (vm.projectTimeout) {
                    vm.$timeout.cancel(vm.projectTimeout);
                }
                vm.projectTimeout = vm.$timeout(_liveActUpdates, 1000);
            }
        }

        function _liveActUpdates() {
            _saveAct(vm.selectedAct.id);
            vm.wordCount.actSummary = _wordCount(vm.selectedAct.summary);
        }

        // Sequence functions

        function _saveSequence(id) {
            vm.$rootScope.$broadcast('saving-started');
            vm.sequenceUpdateIndex = vm.activeSequence;
            var url = '/api/sequences/' + id;
            var sequence = vm.act.sequences.find(seq => seq.id == id);
            var data = {
                id: id,
                title: sequence.title,
                summary: vm.$sanitize(sequence.summary),
                notes: vm.$sanitize(sequence.notes),
                centralQuestion: sequence.centralQuestion,
                actId: sequence.actId
            }
            vm.$http.put(url, data)
                .then(_saveSequenceSuccess, _saveSequenceError);
        }

        function _saveSequenceSuccess(resp) {
            $.connection.hub.start().done(function () {
                vm.projectHub.server.updateAct(vm.groupName, vm.act);
            });
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _saveSequenceError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to save sequence')
                .position('top right')
                .hideDelay(1000)
                .parent("#sequenceTabs")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _saveSequenceScenes(id) {
            vm.$rootScope.$broadcast('saving-started');
            var url = '/api/sequences/scenes/' + id;
            var sequence = vm.act.sequences.find(seq => seq.id == id);
            var data = {
                scenes: sequence.scenes
            };
            vm.$http.put(url, data)
                .then(_saveSequenceScenesSuccess, _saveSequenceError);
        }

        function _saveSequenceScenesSuccess(resp) {
            vm.$rootScope.$broadcast('saving-finished');
            var sequenceIndex = null;
            var sceneIndex = null;
            for (var i = 0; i < vm.act.sequences.length; i++) {
                for (var j = 0; j < vm.act.sequences[i].scenes.length; j++) {
                    if (vm.act.sequences[i].scenes[j].id == undefined) {
                        sequenceIndex = i;
                        sceneIndex = j;
                    }
                }
            }
            if (sceneIndex != null) {
                vm.act.sequences[sequenceIndex].scenes[sceneIndex].id = resp.data.item;
            }
            $.connection.hub.start().done(function () {
                vm.projectHub.server.updateAct(vm.groupName, vm.act);
            });
        }

        function _openSequenceAdd() {
            vm.sequences = [];
            var length = vm.act.sequences.length;
            for (var i = 0; i < length; i++) {
                vm.sequences.push(vm.act.sequences[i]);
            }
            var before = {
                id: 0,
                title: 'Before the first sequence'
            };
            vm.sequences.unshift(before);
            vm.edit.sequence.add = true;
        }

        function _addSequence() {
            var sequence = {
                title: _getNewSequenceTitle(),
                actId: vm.act.id
            };
            var index = vm.act.sequences.findIndex(seq => seq.id == vm.selectedSequence.id) + 1;
            if (index <= vm.act.sequences.length) {
                vm.act.sequences.splice(index, 0, sequence);
                index++;
                for (var i = index; i < vm.act.sequences.length; i++) {
                    var title = _formatSequenceTitle(i + 1);
                    vm.act.sequences[i].title = title;
                }
            } else {
                vm.act.sequences.push(sequence);
            }
            _saveNewSequenceTitles();
            vm.edit.sequence.add = false;
        }

        function _saveNewSequenceTitles() {
            vm.$rootScope.$broadcast('saving-started');
            var url = '/api/sequences/all';
            var data = {
                sequences: vm.act.sequences
            };
            vm.$http.put(url, data)
                .then(_saveNewSequenceTitlesSuccess, _saveNewSequenceTitlesError);
        }

        function _saveNewSequenceTitlesSuccess(resp) {
            vm.$rootScope.$broadcast('saving-finished');
            $.connection.hub.start().done(function () {
                vm.projectHub.server.updateAct(vm.groupName, vm.act);
            });
            vm.$window.location.reload();
        }

        function _saveNewSequenceTitlesError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to save')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _getNewSequenceTitle() {
            if (vm.selectedSequence.id == 0) {
                var ord = 1;
                return _formatSequenceTitle(ord);
            } else {
                var ord = vm.selectedSequence.title.match(/\d+/);
                var num = parseInt(ord[0]) + 1;
                return _formatSequenceTitle(num);
            }
        }

        function _formatSequenceTitle(num) {
            if (num == 11) {
                return '11th Sequence';
            } else if (num == 12) {
                return '12th Sequence';
            } else if (num == 13) {
                return '13th Sequence';
            }
            var arr = num.toString().split('');
            switch (arr[arr.length - 1].toString()) {
                case '1':
                    arr.push('st');
                    break;
                case '2':
                    arr.push('nd');
                    break;
                case '3':
                    arr.push('rd');
                    break;
                default:
                    arr.push('th');
                    break;
            }
            arr.push(' Sequence');
            return arr.join('');
        }

        function _deleteSequence(e, id) {
            var confirm = vm.$mdDialog.confirm()
                .title('Are you sure you want to delete this sequence?')
                .textContent('You will not be able to recover it or the scenes inside it')
                .targetEvent(e)
                .ok('Ok')
                .cancel('Cancel');
            vm.$mdDialog.show(confirm).then(function () {
                vm.$rootScope.$broadcast('saving-started');
                vm.deleteSequenceIndex = vm.act.sequences.findIndex(seq => seq.id == id);

                var url = '/api/sequences/' + id;

                vm.$http.delete(url)
                    .then(_deleteSequenceSuccess, _deleteSequenceError);
            });
        }

        function _deleteSequenceSuccess(resp) {
            vm.act.sequences.splice(vm.deleteSequenceIndex, 1);
            for (var i = vm.deleteSequenceIndex; i < vm.act.sequences.length; i++) {
                var title = _formatSequenceTitle(i + 1);
                vm.act.sequences[i].title = title;
            }
            _saveNewSequenceTitles();
        }

        function _deleteSequenceError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to delete sequence')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        // Scene Functions

        function _selectScene(sce, count) {
            vm.scene = sce;
            vm.navButtonWidth = 1 / count * 100 + "%";
            vm.selectedCharacter = null;
            vm.selectedCharacterSceneExitType = null;
            vm.wordCount.sceneSummary = _wordCount(vm.scene.summary);
            _setSceneWatches();
        }

        function _openSceneAdd(sequenceId) {
            vm.scenes = [];
            var length = vm.act.sequences.find(seq => seq.id == sequenceId).scenes.length;
            for (var i = 0; i < length; i++) {
                var scene = vm.act.sequences.find(seq => seq.id == sequenceId).scenes[i];
                vm.scenes.push(scene);
            }
            var before = {
                id: 0,
                title: 'Before the first scene'
            };
            vm.scenes.unshift(before);
            vm.edit.scene.add = true;
        }

        function _addScene(sequenceId) {
            var scene = {
                title: _getNewSceneTitle(sequenceId),
                sequenceId: sequenceId
            };
            var index = vm.act.sequences.find(seq => seq.id == sequenceId).scenes.findIndex(sce => sce.id == vm.selectedScene.id) + 1;
            if (index <= vm.act.sequences.find(seq => seq.id == sequenceId).scenes.length) {
                vm.act.sequences.find(seq => seq.id == sequenceId).scenes.splice(index, 0, scene);
                vm.navButtonWidth = 1 / vm.act.sequences.find(seq => seq.id == sequenceId).scenes.length * 100 + "%";
                index++;
                var length = vm.act.sequences.find(seq => seq.id == sequenceId).scenes.length;
                for (var i = index; i < length; i++) {
                    var title = _formatTitle(i + 1);
                    vm.act.sequences.find(seq => seq.id == sequenceId).scenes[i].title = title;
                }
            } else {
                vm.act.sequences.find(seq => seq.id == sequenceId).scenes.push(scene);
            }
            _saveSequenceScenes(sequenceId);
            vm.edit.scene.add = false;
        }

        function _getNewSceneTitle(sequenceId) {
            if (vm.selectedScene.id == 0) {
                var ord = 1;
                return _formatTitle(ord);
            } else {
                var ord = vm.selectedScene.title.match(/\d+/);
                var num = parseInt(ord[0]) + 1;
                return _formatTitle(num);
            }
        }

        function _formatTitle(num) {
            if (num == 11) {
                return '11th';
            } else if (num == 12) {
                return '12th';
            } else if (num == 13) {
                return '13th';
            }
            var arr = num.toString().split('');
            switch (arr[arr.length - 1].toString()) {
                case '1':
                    arr.push('st');
                    break;
                case '2':
                    arr.push('nd');
                    break;
                case '3':
                    arr.push('rd');
                    break;
                default:
                    arr.push('th');
                    break;
            }
            return arr.join('');
        }

        function _saveScene(seqId, id) {
            vm.$rootScope.$broadcast('saving-started');
            vm.sequenceUpdateIndex = vm.activeSequence;
            vm.sceneUpdateIndex = vm.activeScene;
            var url = '/api/scenes/' + id;
            var data = vm.scene;
            data.summary = vm.$sanitize(data.summary);
            data.conflict = vm.$sanitize(data.conflict);
            data.turn = vm.$sanitize(data.turn);
            var index = vm.project.acts.find(a => a.id === vm.selectedAct.id).sequences.find(seq => seq.id == seqId).scenes.findIndex(sce => sce.id == id);
            vm.project.acts.find(a => a.id === vm.selectedAct.id).sequences.find(seq => seq.id == seqId).scenes[index] = data;
            vm.$http.put(url, data)
                .then(_saveSceneSuccess, _saveSceneError);
        }

        function _saveSceneSuccess(resp) {
            $.connection.hub.start().done(function () {
                vm.projectHub.server.updateAct(vm.groupName, vm.act);
            });
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _saveSceneError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to save scene')
                .position('top right')
                .hideDelay(1000)
                .parent("#sceneTabs")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _deleteScene(e, seqId, sceId) {
            var confirm = vm.$mdDialog.confirm()
                .title('Are you sure you want to delete this scene?')
                .textContent('You will not be able to recover it')
                .targetEvent(e)
                .ok('Ok')
                .cancel('Cancel');
            vm.$mdDialog.show(confirm).then(function () {
                vm.$rootScope.$broadcast('saving-started');
                vm.deleteSceneIndex = vm.act.sequences.find(seq => seq.id == seqId).scenes.findIndex(sce => sce.id == sceId);
                vm.deleteSequenceId = seqId;

                var url = '/api/scenes/' + sceId;

                vm.$http.delete(url)
                    .then(_deleteSceneSuccess, _deleteSceneError);
            });
        }

        function _deleteSceneSuccess(resp) {
            vm.act.sequences.find(seq => seq.id == vm.deleteSequenceId).scenes.splice(vm.deleteSceneIndex, 1);
            var length = vm.act.sequences.find(seq => seq.id == vm.deleteSequenceId).scenes.length;
            for (var i = vm.deleteSceneIndex; i < length; i++) {
                var title = _formatTitle(i + 1);
                vm.act.sequences.find(seq => seq.id == vm.deleteSequenceId).scenes[i].title = title;
            }
            vm.$timeout(function () {
                angular.element('#sceneNavBar > div > nav > ul > li:nth-child(1) > button').triggerHandler('click');
            });
            _saveSequenceScenes(vm.deleteSequenceId);
        }

        function _deleteSceneError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to delete scene')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _debounceSaveScene(newVal, oldVal) {
            if (newVal !== oldVal) {
                if (vm.projectTimeout) {
                    vm.$timeout.cancel(vm.projectTimeout);
                }
                vm.projectTimeout = vm.$timeout(_liveSceneUpdates, 1000);
            }
        }

        function _liveSceneUpdates() {
            _saveScene(vm.scene.sequenceId, vm.scene.id);
            vm.wordCount.sceneSummary = _wordCount(vm.scene.summary);
        }

        function _setSceneWatches() {
            vm.$scope.$watch('pVm.scene.summary', _debounceSaveScene);
            vm.$scope.$watch('pVm.scene.conflict', _debounceSaveScene);
            vm.$scope.$watch('pVm.scene.turn', _debounceSaveScene);
            vm.$scope.$watch('pVm.scene.setting', _debounceSaveScene);
        }

        // Character functions
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
            vm.add.character = true;
            vm.newCharacter = false;
            vm.selectedCharacter = vm.characters.find(ch => ch.id == id);
        }

        function _addCharacter() {
            vm.add.newCharacter = true;
            vm.selectedCharacter = {
                name: 'NAME',
                want: '',
                need: '',
                biography: ''
            }
            vm.newCharacter = true;
        }

        function _saveCharacter() {
            vm.$rootScope.$broadcast('saving-started');
            vm.selectedCharacter.need = vm.$sanitize(vm.selectedCharacter.need);
            vm.selectedCharacter.want = vm.$sanitize(vm.selectedCharacter.want);
            vm.selectedCharacter.biography = vm.$sanitize(vm.selectedCharacter.biography);
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

        function _newCharacterSuccess(resp) {
            vm.selectedCharacter.id = resp.data.item;
            if (vm.characters == null) {
                vm.characters = [];
            }
            vm.characters.push(vm.selectedCharacter);
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
            var toast = vm.$mdToast.simple()
                .content('Failed to save character')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
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
            var toast = vm.$mdToast.simple()
                .content('Failed to delete character')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }
    }
})();