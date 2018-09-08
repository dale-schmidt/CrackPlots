(function () {
    "use strict";
    angular.module('myApp')
        .controller('tvController', TvController)
        .controller('episodeController', EpisodeController);

    TvController.$inject = ['$http', '$window', '$sanitize', '$mdDialog', '$mdToast', '$scope', '$timeout', '$rootScope', 'razorViewModel'];
    EpisodeController.$inject = ['$http', '$window', '$mdDialog', 'items'];

    function TvController($http, $window, $sanitize, $mdDialog, $mdToast, $scope, $timeout, $rootScope, razorViewModel) {
        var vm = this;
        vm.$http = $http;
        vm.$window = $window;
        vm.$sanitize = $sanitize;
        vm.$mdDialog = $mdDialog;
        vm.$mdToast = $mdToast;
        vm.$scope = $scope;
        vm.$timeout = $timeout;
        vm.$rootScope = $rootScope;
        vm.razorViewModel = razorViewModel;

        vm.series = null;
        vm.collaborators = [];
        vm.newCollaborator = null;
        vm.deletedCollaborator = null;
        vm.newSeason = null;
        vm.newEpisode = null;
        vm.addSeasonEdit = false;
        vm.addEpisodeEdit = false;
        vm.addEpisodeSeasonId = null;
        vm.edit = {
            series: {
                collaborators: false
            }
        };
        vm.timeout = {
            series: null
        };

        vm.saveSeries = _saveSeries;
        vm.saveCollaborator = _saveCollaborator;
        vm.deleteCollaborator = _deleteCollaborator;
        vm.openEdit = _openEdit;
        vm.addNewSeason = _addNewSeason;
        vm.addNewEpisode = _addNewEpisode;
        vm.goToIndex = _goToIndex;
        vm.goToEpisode = _goToEpisode;
        vm.deleteSeries = _deleteSeries;

        _getSeries();

        function _goToIndex() {
            vm.$window.location.href = '/projects/index';
        }

        function _goToEpisode(epId) {
            vm.$window.location.href = '/projects/episode/' + epId;
        }

        function _openEdit(entity, prop) {
            switch (entity) {
                case 'series':
                    switch (prop) {
                        case 'title':
                            vm.edit.series.save = true;
                            vm.edit.series.title = true;
                            break;
                        case 'logline':
                            vm.edit.series.save = true;
                            vm.edit.series.logline = true;
                            break;
                        case 'notes':
                            vm.edit.series.save = true;
                            vm.edit.series.notes = true;
                            break;
                        case 'collaborators':
                            vm.edit.series.collaborators = true;
                            break;
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
            vm.$scope.$watch('tVm.series.title', _debounceSaveSeries);
            vm.$scope.$watch('tVm.series.logline', _debounceSaveSeries);
            vm.$scope.$watch('tVm.series.notes', _debounceSaveSeries);
        }

        // Series functions
        function _getSeries() {
            var url = '/api/projects/' + vm.razorViewModel.Item;

            vm.$http.get(url)
                .then(_getSeriesSuccess, _getSeriesError);
        }

        function _getSeriesSuccess(resp) {
            console.log(resp);
            if (resp.data.item) {
                vm.series = resp.data.item;
            }
            _setWatches();
        }

        function _getSeriesError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to load series')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
        }

        function _saveSeries() {
            vm.$rootScope.$broadcast('saving-started');
            vm.edit.saving = true;
            var url = '/api/projects/' + vm.series.id;
            vm.series.notes = vm.$sanitize(vm.series.notes);
            vm.$http.put(url, vm.series)
                .then(_saveSeriesSuccess, _saveSeriesError);
        }

        function _saveSeriesSuccess(resp) {
            vm.$rootScope.$broadcast('saving-finished');
        }

        function _saveSeriesError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to save series')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.edit.saving = false;
        }

        function _deleteSeries(e) {
            var confirm = vm.$mdDialog.confirm()
                .title('Are you sure you want to delete this series?')
                .textContent('You will not be able to recover it')
                .targetEvent(e)
                .ok('Ok')
                .cancel('Cancel');
            vm.$mdDialog.show(confirm).then(function () {
                var url = "/api/projects/" + vm.series.id;

                vm.$http.delete(url)
                    .then(_deleteSeriesSuccess, _deleteSeriesError);
            });
        }

        function _deleteSeriesSuccess(resp) {
            vm.$window.location.href = "/projects";
        }

        function _deleteSeriesError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to delete series')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
        }

        function _debounceSaveSeries(newVal, oldVal) {
            if (newVal !== oldVal) {
                if (vm.timeout.series) {
                    vm.$timeout.cancel(vm.timeout.series);
                }
                vm.timeout.series = vm.$timeout(_liveSeriesUpdates, 1000);
            }
        }

        function _liveSeriesUpdates() {
            _saveSeries();
        }

        // Collaborator functions
        function _saveCollaborator() {
            vm.edit.saving = true;
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
            vm.edit.saving = false;
        }

        function _saveCollaboratorError(resp) {
            console.log(resp);
            vm.edit.saving = false;
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
            vm.edit.saving = true;
            var url = '/api/projectsperson/' + vm.project.id + "/" + person.id;
            vm.deletedCollaborator = person;

            vm.$http.delete(url)
                .then(_deleteCollaboratorSuccess, _deleteCollaboratorError);
        }

        function _deleteCollaboratorSuccess(resp) {
            vm.edit.saving = false;
            var index = vm.collaborators.indexOf(c => c.id == vm.deletedCollaborator.id);
            vm.collaborators.splice(index, 1);
        }

        function _deleteCollaboratorError(resp) {
            console.log(resp);
            vm.edit.saving = false;
            var toast = vm.$mdToast.simple()
                .content('Failed to delete collaborator')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
        }

        // Season functions
        function _addNewSeason(e) {
            var confirm = vm.$mdDialog.confirm()
                .title('Are you sure you want to add a new season?')
                .targetEvent(e)
                .ok('Ok')
                .cancel('Cancel');
            vm.$mdDialog.show(confirm).then(function () {
                var url = '/api/projects/';
                var data = {
                    title: _newSeasonTitle(),
                    autoStructured: true,
                    projectId: vm.series.id,
                    storyTypeId: 7
                };
                vm.$http.post(url, data)
                    .then(_addSeasonSuccess, _addSeasonError);
            });
        }

        function _newSeasonTitle() {
            if (vm.series.seasons) {
                var season = vm.series.seasons[vm.series.seasons.length - 1]
                var lastSeasonNumber = season.title.split(" ")[1];
                return 'Season ' + (parseInt(lastSeasonNumber) + 1);
            } else {
                return 'Season 1';
            }
        }

        function _addSeasonSuccess(resp) {
            vm.$window.location.reload();
        }

        function _addSeasonError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to add season')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.edit.saving = false;
        }

        // Episode functions

        function _addNewEpisode(e, ssnId) {
            var dialogOptions = {
                controller: 'episodeController',
                controllerAs: 'eVm',
                flex: 75,
                templateUrl: 'addEpisodeDialog.html',
                targetEvent: e,
                clickOutsideToClose: true,
                locals: {
                    items: {
                        seasonId: ssnId,
                        projectId: vm.series.id,
                        title: _newEpisodeTitle(ssnId)
                    }
                },
                bindToController: true
            };
            vm.$mdDialog.show(dialogOptions);
        }

        function _newEpisodeTitle(ssnId) {
            var season = vm.series.seasons.find(ssn => ssn.id == ssnId);
            var seasonNumber = season.title.split(" ")[1];
            if (vm.series.seasons.find(ssn => ssn.id == ssnId).episodes) {
                var episode = season.episodes[season.episodes.length - 1];
                var lastEpisodeNumber = parseInt(episode.title.split(" ")[1].split(".")[1]);
                if (lastEpisodeNumber + 1 < 10) {
                    return 'Episode ' + seasonNumber + '.0' + (lastEpisodeNumber + 1);
                } else {
                    return 'Episode ' + seasonNumber + '.' + (lastEpisodeNumber + 1);
                }
            } else {
                return 'Episode ' + seasonNumber + '.01';
            }
        }

        function _addEpisodeSuccess(resp) {
            vm.$window.location.reload();
        }

        function _addEpisodeError(resp) {
            console.log(resp);
            var toast = vm.$mdToast.simple()
                .content('Failed to add episode')
                .position('top right')
                .hideDelay(1000)
                .parent(".main-wrapper")
                .theme('error-toast');

            vm.$mdToast.show(toast);
            vm.edit.saving = false;
        }
    }

    function EpisodeController($http, $window, $mdDialog, items) {
        var vm = this;
        vm.$http = $http;
        vm.$window = $window;
        vm.$mdDialog = $mdDialog;
        vm.items = items;

        vm.actsCount = null;

        vm.saveEpisode = _saveEpisode;
        vm.cancelModal = _cancelModal;

        function _saveEpisode() {
            if (vm.actsCount > 0) {
                var url = '/api/projects/';
                var data = {
                    autoStructured: true,
                    actsCount: vm.actsCount,
                    storyTypeId: 6,
                    seasonId: vm.items.seasonId,
                    projectId: vm.items.projectId,
                    title: vm.items.title
                }
                vm.$http.post(url, data)
                    .then(_addEpisodeSuccess, _addEpisodeError);
            }
        }

        function _addEpisodeSuccess(resp) {
            vm.$window.location.reload();
        }

        function _addEpisodeError(resp) {
            console.log(resp);
        }

        function _cancelModal() {
            vm.$mdDialog.hide();
        }
    }
})();