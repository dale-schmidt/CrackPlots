(function () {
    "use strict";
    angular.module('myApp')
        .controller('actController', ActController);

    ActController.$inject = ['$http', '$window', '$rootScope', '$scope', '$sanitize', '$timeout', '$mdDialog', '$mdToast', 'razorViewModel'];

    function ActController($http, $window, $rootScope, $scope, $sanitize, $timeout, $mdDialog, $mdToast, razorViewModel) {
        var vm = this;
        vm.$http = $http;
        vm.$window = $window;
        vm.$rootScope = $rootScope;
        vm.$scope = $scope;
        vm.$sanitize = $sanitize;
        vm.$timeout = $timeout;
        vm.$mdDialog = $mdDialog;
        vm.$mdToast = $mdToast;
        vm.razorViewModel = razorViewModel;

        vm.act = null;
        vm.sequences = [];
        vm.scenes = [];
        vm.characters = [];
        vm.selectedSequence = null;
        vm.selectedScene = null;
        vm.characterAdd = null;
        vm.newCharacter = null;
        vm.selectedCharacter = null;
        vm.activeCharacter = null;
        vm.characterSceneExitTypes = [];
        vm.selectedCharacterSceneExitType = null;
        vm.deleteSceneIndex;
        vm.deleteSequenceId;
        vm.deleteSequenceIndex;
        vm.projectHub = null;
        vm.activeSequence = 0;
        vm.activeScene = 0;
        vm.sequenceUpdateIndex = 0;
        vm.sceneUpdateIndex = 0;
        vm.characterUpdateIndex = 0;
        vm.scene = null;
        vm.navButtonWidth = "";
        vm.add = {
            sequence: false,
            scene: false,
            character: false,
            newCharacter: false
        };
        vm.timeout = {
            act: null,
            sequence: null,
            scene: null,
            character: null
        };
        vm.watches = {
            scene: {
                summary: null,
                conflict: null,
                turn: null,
                setting: null
            },
            character: {
                physicalGoal: null,
                emotionalGoal: null,
                obstacle: null,
                characterSceneExitType: null,
                start: null,
                end: null,
                notes: null
            }
        }
        vm.wordCount = {
            actSummary: 0,
            sequenceSummary: 0,
            sceneSummary: 0
        };

        vm.groupName = "";
        vm.projectHub = $.connection.projectHub;
        $.connection.hub.start();

        vm.openEdit = _openEdit;
        vm.saveAct = _saveAct;
        vm.saveSequence = _saveSequence;
        vm.openSequenceAdd = _openSequenceAdd;
        vm.addSequence = _addSequence;
        vm.openSceneAdd = _openSceneAdd;
        vm.addScene = _addScene;
        vm.saveScene = _saveScene;
        vm.addNewCharacter = _addNewCharacter;
        vm.saveNewCharacter = _saveNewCharacter;
        vm.closeNewCharacter = _closeNewCharacter;
        vm.openCharacterSceneAdd = _openCharacterSceneAdd;
        vm.addCharacterScene = _addCharacterScene;
        vm.openCharacterScene = _openCharacterScene;
        vm.saveCharacterScene = _saveCharacterScene;
        vm.deleteCharacterScene = _deleteCharacterScene;
        vm.closeCharacterScene = _closeCharacterScene;
        vm.deleteScene = _deleteScene;
        vm.deleteSequence = _deleteSequence;
        vm.goToProject = _goToProject;
        vm.selectScene = _selectScene;
        vm.sequenceChange = _sequenceChange;
        vm.projectHub.client.broadcastAct = _broadcastAct;

        _getAct();

        function _goToProject(e) {
            var count = 0;
            for (var key in vm.edit) {
                if (vm.edit.hasOwnProperty(key)) {
                    for (var entity in vm.edit[key]) {
                        if (vm.edit[key].hasOwnProperty(entity)) {
                            if (vm.edit[key][entity]) {
                                count++;
                            }
                        }
                    }
                }
            }
            if (count > 0) {
                var confirm = $mdDialog.confirm()
                    .title('You have unsaved changes')
                    .textContent('Would you still like to leave the page?')
                    .ariaLabel('Lucky day')
                    .targetEvent(e)
                    .ok('Yes')
                    .cancel('Cancel');

                vm.$mdDialog.show(confirm).then(function () {
                    vm.$window.location.href = "/projects/edit/" + vm.act.projectId;
                });
            } else {
                vm.$window.location.href = "/projects/edit/" + vm.act.projectId;
            }
        }

        function _openEdit(entity, prop) {
            switch (entity) {
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
                case 'sequence':
                    vm.edit.sequence.save = true;
                    switch (prop) {
                        case 'title':
                            vm.edit.sequence.title = true;
                            break;
                        case 'summary':
                            vm.edit.sequence.summary = true;
                            break;
                        case 'notes':
                            vm.edit.sequence.notes = true;
                            break;
                        case 'centralQuestion':
                            vm.edit.sequence.centralQuestion = true;
                        default:
                            break;
                    }
                    break;
                case 'scene':
                    vm.edit.scene.save = true;
                    switch (prop) {
                        case 'title':
                            vm.edit.scene.title = true;
                            break;
                        case 'summary':
                            vm.edit.scene.summary = true;
                            break;
                        case 'conflict':
                            vm.edit.scene.conflict = true;
                            break;
                        case 'turn':
                            vm.edit.scene.turn = true;
                            break;
                        case 'setting':
                            vm.edit.scene.setting = true;
                        default:
                            break;
                    }
                    break;
                case 'character':
                    vm.edit.character.save = true;
                    switch (prop) {
                        case 'physicalGoal':
                            vm.edit.character.physicalGoal = true;
                            break;
                        case 'emotionalGoal':
                            vm.edit.character.emotionalGoal = true;
                            break;
                        case 'obstacle':
                            vm.edit.character.obstacle = true;
                            break;
                        case 'start':
                            vm.edit.character.start = true;
                            break;
                        case 'end':
                            vm.edit.character.end = true;
                            break;
                        case 'type':
                            vm.edit.character.type = true;
                            break;
                        case 'notes':
                            vm.edit.character.notes = true;
                            break;
                        default:
                            break;
                    }
                default:
                    break;
            }
        }

        function _setWatches() {
            vm.$scope.$watch('aVm.act.centralQuestion', _debounceSaveAct);
            vm.$scope.$watch('aVm.act.summary', _debounceSaveAct);
            vm.$scope.$watch('aVm.act.notes', _debounceSaveAct);
            _setSequenceWatches();
            _setSceneWatches();
        }

        function _wordCount(str) {
            str = _htmlToPlaintext(str);
            return str ? str.split(" ").length : 0;
        }

        function _htmlToPlaintext(text) {
            return text ? String(text).replace(/<[^>]+>/gm, '') : '';
        }

        // SignalR functions

        function _broadcastAct(act) {
            var camelAct = _toCamel(act);
            if (camelAct.id === vm.act.id) {
                var edit = {
                    act: false,
                    sequence: false,
                    scene: false,
                    character: false
                };
                Object.keys(vm.act).forEach(function (key, index) {
                    var count = 0;
                    Object.keys(vm.act[key]).forEach(function (key2, index2) {
                        if (vm.act[key][key2] && key2 != 'save') {
                            switch (key) {
                                case 'act':
                                    camelAct[key2] = vm.act[key2];
                                    break;
                                case 'sequence':
                                    camelAct.sequences[vm.activeSequence][key2] = vm.act.sequences[vm.activeSequence][key2];
                                    break;
                                case 'scene':
                                    camelAct.sequences[vm.activeSequence].scenes[vm.activeScene] = vm.act.sequences[vm.activeSequence].scenes[vm.activeScene];
                                    break;
                                case 'character':
                                    camelAct.sequences[vm.activeSequence].scenes[vm.activeScene].characters.find(ch => ch.id === vm.selectedCharacter.id) = vm.act.sequences[vm.activeSequence].scenes[vm.activeScene].characters.find(ch => ch.id === vm.selectedCharacter.id);
                                    break;
                                default:
                                    break;
                            }
                        }
                    });
                });
                vm.$scope.$apply(function () {
                    vm.act = camelAct;
                });
                vm.activeSequence = vm.sequenceUpdateIndex;
                vm.activeScene = vm.sceneUpdateIndex;
            }
        }

        function _toCamel(o) {
            var newO, origKey, newKey, value
            if (o instanceof Array) {
                newO = []
                for (origKey in o) {
                    value = o[origKey]
                    if (typeof value === "object") {
                        value = _toCamel(value)
                    }
                    newO.push(value)
                }
            } else {
                newO = {}
                for (origKey in o) {
                    if (o.hasOwnProperty(origKey)) {
                        newKey = (origKey.charAt(0).toLowerCase() + origKey.slice(1) || origKey).toString()
                        value = o[origKey]
                        if (value instanceof Array || (value !== null && value.constructor === Object)) {
                            value = _toCamel(value)
                        }
                        newO[newKey] = value
                    }
                }
            }
            return newO
        }

        // Act functions

        function _getAct() {
            vm.$http({
                method: 'GET',
                url: '/api/acts/' + vm.razorViewModel.Item
            }).then(_getActSuccess, _getActError)
        }

        function _getActSuccess(resp) {
            console.log(resp.data);
            vm.act = resp.data.item;
            for (var i = 0; i < vm.act.sequences.length; i++) {
                vm.act.sequences[i].order = parseInt(vm.act.sequences[i].title.match(/\d+/));
                vm.act.sequences.sort(function (a, b) {
                    return a.order - b.order;
                });
                for (var j = 0; j < vm.act.sequences[i].scenes.length; j++) {
                    vm.act.sequences[i].scenes[j].order = parseInt(vm.act.sequences[i].scenes[j].title.match(/\d+/));
                }
                vm.act.sequences[i].scenes.sort(function (a, b) {
                    return a.order - b.order;
                });
            }
            _selectScene(vm.act.sequences[0].scenes[0], vm.act.sequences[0].scenes.length);
            vm.wordCount.actSummary = _wordCount(vm.act.summary);
            vm.selectedSequence = vm.act.sequences[0];
            vm.wordCount.sequenceSummary = _wordCount(vm.selectedSequence.summary);
            vm.groupName = "act" + vm.act.id + vm.act.title;
            $.connection.hub.start().done(function () {
                $.connection.projectHub.server.joinGroup(vm.groupName);
            })
            _getCharacters();
            vm.$timeout(function () {
                angular.element('#sceneNavBar > div > nav > ul > li:nth-child(1) > button').triggerHandler('click');
            });
            _setWatches();
            vm.$rootScope.$broadcast('show-title', vm.act.projectTitle, vm.act.projectId);
            vm.$rootScope.$broadcast('show-arrows', 'acts', vm.act.id, vm.act.actIds);
        }

        function _getActError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to load act')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
        }

        function _saveAct() {
            vm.$rootScope.$broadcast('saving-started');
            var url = '/api/acts/' + vm.act.id;
            var data = {
                id: vm.act.id,
                title: vm.act.title,
                summary: vm.$sanitize(vm.act.summary),
                notes: vm.$sanitize(vm.act.notes),
                centralQuestion: vm.act.centralQuestion,
                projectId: vm.act.projectId
            };
            vm.$http.put(url, data)
                .then(_saveActSuccess, _saveActError);
        }

        function _saveActSuccess(resp) {
            $.connection.hub.start().done(function () {
                vm.projectHub.server.updateAct(vm.groupName, vm.act);
            });
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
            _saveAct();
            vm.wordCount.actSummary = _wordCount(vm.act.summary);
        }

        // Sequence functions

        function _setSequenceWatches() {
            vm.$scope.$watch('aVm.act.sequences[' + vm.activeSequence + '].centralQuestion', _debounceSaveSequence);
            vm.$scope.$watch('aVm.act.sequences[' + vm.activeSequence + '].summary', _debounceSaveSequence);
            vm.$scope.$watch('aVm.act.sequences[' + vm.activeSequence + '].notes', _debounceSaveSequence);
        }

        function _sequenceChange(seq) {
            vm.selectedSequence = seq;
            vm.wordCount.sequenceSummary = _wordCount(vm.selectedSequence.summary);
            _setSequenceWatches();
            _selectScene(vm.selectedSequence.scenes[0], vm.selectedSequence.scenes.length);
        }

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
            vm.add.sequence = true;
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
                .content('Failed to save sequence')
                .position('top right')
                .hideDelay(1000)
                .parent("#sequenceTabs")
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
                .parent("#sequenceTabs")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _debounceSaveSequence(newVal, oldVal) {
            if (newVal !== oldVal) {
                if (vm.timeout.sequence) {
                    vm.$timeout.cancel(vm.timeout.sequence);
                }
                vm.timeout.sequence = vm.$timeout(_liveSequenceUpdates, 1000);
            }
        }

        function _liveSequenceUpdates() {
            _saveSequence(vm.act.sequences[vm.activeSequence].id);
            vm.wordCount.sequenceSummary = _wordCount(vm.selectedSequence.summary);
        }

        // Scene Functions
        function _selectScene(sce, count) {
            if (vm.watches.scene.summary) {
                _destroySceneWatches();
            }
            if (vm.watches.character.physicalGoal) {
                _destroyCharacterSceneWatches();
            }
            vm.scene = sce;
            vm.navButtonWidth = 1 / count * 100 + "%";
            vm.selectedCharacter = null;
            vm.selectedCharacterSceneExitType = null;
            vm.wordCount.sceneSummary = _wordCount(vm.scene.summary);
            _setSceneWatches();
        }

        function _setSceneWatches() {
            if (!vm.watches.scene.summary) {
                vm.$scope.$watch('aVm.scene.summary', _debounceSaveScene);
                vm.watches.scene.summary = vm.$scope.$watch('aVm.scene.summary', _debounceSaveScene);
                vm.$scope.$watch('aVm.scene.conflict', _debounceSaveScene);
                vm.watches.scene.conflict = vm.$scope.$watch('aVm.scene.conflict', _debounceSaveScene);
                vm.$scope.$watch('aVm.scene.turn', _debounceSaveScene);
                vm.watches.scene.turn = vm.$scope.$watch('aVm.scene.turn', _debounceSaveScene);
                vm.$scope.$watch('aVm.scene.setting', _debounceSaveScene);
                vm.watches.scene.setting = vm.$scope.$watch('aVm.scene.setting', _debounceSaveScene);
            }
        }

        function _destroySceneWatches() {
            vm.watches.scene.summary();
            vm.watches.scene.conflict();
            vm.watches.scene.turn();
            vm.watches.scene.setting();
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
            vm.add.scene = true;
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
            vm.add.scene = false;
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
            var index = vm.act.sequences.find(seq => seq.id == seqId).scenes.findIndex(sce => sce.id == id);
            vm.act.sequences.find(seq => seq.id == seqId).scenes[index] = data;
            vm.$http.put(url, data)
                .then(_saveSceneSuccess, _saveSceneError);
        }

        function _saveSceneSuccess(resp) {
            $.connection.hub.start().done(function () {
                vm.projectHub.server.updateAct(vm.groupName, vm.act);
            });
            if (vm.$window.innerWidth < 768) {
                vm.$window.location.reload();
            }
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
                .parent("#sequenceTabs")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _debounceSaveScene(newVal, oldVal) {
            if (newVal !== oldVal) {
                if (vm.timeout.scene) {
                    vm.$timeout.cancel(vm.timeout.scene);
                }
                vm.timeout.scene = vm.$timeout(_liveSceneUpdates, 1000);
            }
        }

        function _liveSceneUpdates() {
            _saveScene(vm.scene.sequenceId, vm.scene.id);
            vm.wordCount.sceneSummary = _wordCount(vm.scene.summary);
        }

        // Character functions 

        function _getCharacters() {
            var url = '/api/characters/project/' + vm.act.projectId;

            vm.$http.get(url)
                .then(_getCharactersSuccess, _getCharactersError);
        }

        function _getCharactersSuccess(resp) {
            console.log(resp.data);
            vm.characters = resp.data.items;
            _getCharacterSceneExitTypes();
        }

        function _getCharactersError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to load characters')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
        }

        function _addNewCharacter() {
            vm.selectedCharacter = null;
            vm.add.newCharacter = true;
        }

        function _saveNewCharacter() {
            vm.$rootScope.$broadcast('saving-started');
            var url = '/api/characters/';
            vm.newCharacter.projectId = vm.act.projectId;
            var data = vm.newCharacter;
            data.want = vm.$sanitize(data.want);
            data.need = vm.$sanitize(data.need);
            data.biography = vm.$sanitize(data.biography);
            vm.$http.post(url, data)
                .then(_saveNewCharacterSuccess, _saveNewCharacterError);
        }

        function _saveNewCharacterSuccess(resp) {
            vm.$rootScope.$broadcast('saving-finished');
            vm.newCharacter.id = resp.data.item;
            vm.characters.push(vm.newCharacter);
            _closeNewCharacter();
            $.connection.hub.start().done(function () {
                vm.projectHub.server.updateAct(vm.act);
            });
        }

        function _saveNewCharacterError(resp) {
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

        function _closeNewCharacter() {
            vm.newCharacter = null;
            vm.add.newCharacter = false;
        }

        // Character Scene functions

        function _setCharacterSceneWatches() {
            if (!vm.watches.character.physicalGoal) {
                vm.$scope.$watch('aVm.selectedCharacter.physicalGoal', _debounceSaveCharacterScene);
                vm.watches.character.physicalGoal = vm.$scope.$watch('aVm.selectedCharacter.physicalGoal', _debounceSaveCharacterScene);
                vm.$scope.$watch('aVm.selectedCharacter.emotionalGoal', _debounceSaveCharacterScene);
                vm.watches.character.emotionalGoal = vm.$scope.$watch('aVm.selectedCharacter.emotionalGoal', _debounceSaveCharacterScene);
                vm.$scope.$watch('aVm.selectedCharacter.obstacle', _debounceSaveCharacterScene);
                vm.watches.character.obstacle = vm.$scope.$watch('aVm.selectedCharacter.obstacle', _debounceSaveCharacterScene);
                vm.$scope.$watch('aVm.selectedCharacterSceneExitType', _debounceSaveCharacterScene);
                vm.watches.character.characterSceneExitType = vm.$scope.$watch('aVm.selectedCharacter.selectedCharacterSceneExitType', _debounceSaveCharacterScene);
                vm.$scope.$watch('aVm.selectedCharacter.start', _debounceSaveCharacterScene);
                vm.watches.character.start = vm.$scope.$watch('aVm.selectedCharacter.start', _debounceSaveCharacterScene);
                vm.$scope.$watch('aVm.selectedCharacter.end', _debounceSaveCharacterScene);
                vm.watches.character.end = vm.$scope.$watch('aVm.selectedCharacter.end', _debounceSaveCharacterScene);
                vm.$scope.$watch('aVm.selectedCharacter.notes', _debounceSaveCharacterScene);
                vm.watches.character.notes = vm.$scope.$watch('aVm.selectedCharacter.notes', _debounceSaveCharacterScene);
            }
        }

        function _destroyCharacterSceneWatches() {
            vm.watches.character.physicalGoal();
            vm.watches.character.emotionalGoal();
            vm.watches.character.obstacle();
            vm.watches.character.characterSceneExitType();
            vm.watches.character.start();
            vm.watches.character.end();
            vm.watches.character.notes();
            vm.watches.character = {
                physicalGoal: null,
                emotionalGoal: null,
                obstacle: null,
                characterSceneExitType: null,
                start: null,
                end: null,
                notes: null
            }
        }

        function _openCharacterSceneAdd() {
            vm.add.character = true;
        }

        function _addCharacterScene(sequenceId, sceneId) {
            vm.$rootScope.$broadcast('saving-started');
            var data = {
                characterId: vm.characterAdd.id,
                sceneId: sceneId
            };
            vm.activeCharacterScene = {
                sequenceId: sequenceId,
                sceneId: sceneId
            };

            var url = '/api/charactersscenes/';

            vm.$http.post(url, data)
                .then(_addCharacterSceneSuccess, _addCharacterSceneError);
        }

        function _addCharacterSceneSuccess(resp) {
            var newCharacter = {
                character: vm.characterAdd,
                sceneId: vm.activeCharacterScene.sceneId
            };

            var characters = vm.act.sequences.find(seq => seq.id == vm.activeCharacterScene.sequenceId).scenes
                .find(sce => sce.id == vm.activeCharacterScene.sceneId).characters;

            if (characters === null) {
                vm.act.sequences.find(seq => seq.id == vm.activeCharacterScene.sequenceId).scenes
                    .find(sce => sce.id == vm.activeCharacterScene.sceneId).characters = [];
            }
            vm.act.sequences.find(seq => seq.id == vm.activeCharacterScene.sequenceId).scenes
                .find(sce => sce.id == vm.activeCharacterScene.sceneId).characters.push(newCharacter);

            vm.add.character = false;
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _addCharacterSceneError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to save character')
                .position('top right')
                .hideDelay(1000)
                .parent("#selectedCharacterPanel")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _openCharacterScene(sequenceId, sceneId, characterId) {
            if (vm.watches.character.physicalGoal) {
                _destroyCharacterSceneWatches();
            }
            vm.selectedCharacter = vm.act.sequences.find(seq => seq.id == sequenceId).scenes
                .find(sce => sce.id == sceneId).characters
                .find(ch => ch.character.id == characterId);
            vm.selectedCharacterSceneExitType = vm.characterSceneExitTypes.find(typ => typ.id == vm.selectedCharacter.characterSceneExitTypeId);
            vm.$timeout(_setCharacterSceneWatches);
        }

        function _saveCharacterScene(sceneId) {
            if (vm.selectedCharacter) {
                vm.$rootScope.$broadcast('saving-started');
                if (!vm.selectedCharacterSceneExitType) {
                    vm.selectedCharacterSceneExitType = vm.characterSceneExitTypes.find(type => type.id == 1);
                }
                var data = {
                    characterId: vm.selectedCharacter.character.id,
                    sceneId: sceneId,
                    physicalGoal: vm.selectedCharacter.physicalGoal,
                    emotionalGoal: vm.selectedCharacter.emotionalGoal,
                    obstacle: vm.selectedCharacter.obstacle,
                    characterSceneExitTypeId: vm.selectedCharacterSceneExitType.id,
                    start: vm.selectedCharacter.start,
                    end: vm.selectedCharacter.end,
                    notes: vm.$sanitize(vm.selectedCharacter.notes)
                };

                var url = '/api/charactersscenes/';

                vm.$http.put(url, data)
                    .then(_saveCharacterSceneSuccess, _saveCharacterSceneError);
            }
        }

        function _saveCharacterSceneSuccess(resp) {
            $.connection.hub.start().done(function () {
                vm.projectHub.server.updateAct(vm.groupName, vm.act);
            });
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _saveCharacterSceneError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to save character')
                .position('top right')
                .hideDelay(1000)
                .parent("#selectedCharacterPanel")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _deleteCharacterScene(e, sequenceId, sceneId, characterId) {
            var confirm = vm.$mdDialog.confirm()
                .title('Are you sure you want to delete this character from this scene?')
                .textContent('You will not be able to recover it')
                .targetEvent(e)
                .ok('Ok')
                .cancel('Cancel');
            vm.$mdDialog.show(confirm).then(function () {
                vm.$rootScope.$broadcast('saving-started');
                var url = '/api/charactersscenes/' + characterId + '/' + sceneId;

                vm.activeCharacter = {
                    sequenceId: sequenceId,
                    sceneId: sceneId,
                    characterId: characterId
                };

                vm.$http.delete(url)
                    .then(_deleteCharacterSuccess, _deleteCharacterError);
            });
        }

        function _deleteCharacterSuccess(resp) {
            var index = vm.act.sequences.find(seq => seq.id == vm.activeCharacter.sequenceId)
                .scenes.find(sce => sce.id == vm.activeCharacter.sceneId)
                .characters.findIndex(ch => ch.id == vm.activeCharacter.characterId);

            vm.act.sequences.find(seq => seq.id == vm.activeCharacter.sequenceId)
                .scenes.find(sce => sce.id == vm.activeCharacter.sceneId)
                .characters.splice(index, 1);

            _closeCharacterScene();
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _deleteCharacterError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to delete character')
                .position('top right')
                .hideDelay(1000)
                .parent("#selectedCharacterPanel")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _closeCharacterScene() {
            vm.selectedCharacter = null;
            _destroyCharacterSceneWatches();
        }

        function _getCharacterSceneExitTypes() {
            var url = '/api/charactersscenes/exits';

            vm.$http.get(url)
                .then(_getCharacterSceneExtiTypesSuccess, _getCharacterSceneExitTypesError);
        }

        function _getCharacterSceneExtiTypesSuccess(resp) {
            vm.characterSceneExitTypes = resp.data.items;
        }

        function _getCharacterSceneExitTypesError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to load exit types')
                .position('top right')
                .hideDelay(1000)
                .parent("#selectedCharacterPanel")
                .theme('error-toast');

            vm.$mdToast.show(toast);
        }

        function _debounceSaveCharacterScene(newVal, oldVal) {
            if (newVal !== oldVal) {
                if (vm.timeout.character) {
                    vm.$timeout.cancel(vm.timeout.character);
                }
                vm.timeout.character = vm.$timeout(_liveCharacterSceneUpdates, 1000);
            }
        }

        function _liveCharacterSceneUpdates() {
            _saveCharacterScene(vm.scene.id);
        }
    }
})();