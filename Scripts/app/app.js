document.addEventListener("DOMContentLoaded", function (event) {
    var app = new Vue({
        el: "#app",
        data: {
            activeDevices: [],
            signalrHub: null,
            windows: [
                {
                    style: {
                        width: 34 + 'px',
                        height: 41 + 'px',
                        top: 433 + 'px',
                        left: 15 + 'px'
                    },
                    isOn: false,
                    bulbId: null
                },
                {
                    style: {
                        width: 53 + 'px',
                        height: 54 + 'px',
                        top: 270 + 'px',
                        left: 458 + 'px'
                    },
                    isOn: false,
                    bulbId: null
                },
                {
                    style: {
                        width: 40 + 'px',
                        height: 50 + 'px',
                        top: 358 + 'px',
                        left: 752 + 'px'
                    },
                    isOn: false,
                    bulbId: null
                },
                {
                    style: {
                        width: 42 + 'px',
                        height: 60 + 'px',
                        top: 223 + 'px',
                        left: 910 + 'px'
                    },
                    isOn: false,
                    bulbId: null
                }
            ]
        },
        methods: {
            signalrInitialized() {
                console.log('SignalR connection initialized!');
            },
            sendMessage() {
                console.log('Sending message');
                this.signalrHub.server.sendMessage("Hello world from another client!");
            },
            checkIfActivityIsRegistered(activity) {

                var isActivityAlreadyRegistered = false;

                if (activity) {
                    for (var i = 0; i < this.windows.length; i++) {
                        if (this.windows[i].BulbId === activity.Device.BulbId)
                            isActivityAlreadyRegistered = true;
                    }
                }


                return isActivityAlreadyRegistered;
            },
            turnOnWindowsForActiveDevices(receivedActivities) {
                for (var i = 0; i < receivedActivities.length; i++) {
                    // If the activity is not already shown, then we go and find a free window to light up. 
                    if (!this.checkIfActivityIsRegistered(receivedActivities[i])) {
                        for (var j = 0; j < this.windows.length; j++) {

                            if (!this.windows[j].isOn) {
                                this.windows[j].isOn = true;
                                this.windows[j].BulbId = receivedActivities[i].BulbId;

                                break;
                            }
                        }
                    }
                }
            },
            shutOffExpiredActivities(expiredActivities) {
                for (var j = 0; j < expiredActivities.length; j++) {

                    for (var i = 0; i < this.windows.length; i++) {
                        if (this.windows[i].BulbId === expiredActivities[j].BulbId) {
                            this.windows[i].BulbId = null;
                            this.windows[i].isOn = false;
                        }
                    }
                }
            }
        },
        created: function () {
           
        },
        mounted: function () {
            var vueInstance = this;
            this.signalrHub = $.connection.statusUpdateHub;
            // ---------
            // Call client methods from hub
            // ---------

            // Add a client-side hub method that the server will call
            this.signalrHub.client.receiveDataUpdate = function (currentActivities, expiredActivities) {
                var deserializedCurrentActivities = JSON.parse(currentActivities);
                var deserializedExpiredActivities = JSON.parse(expiredActivities);

                console.log('current activities', deserializedCurrentActivities)

                console.log('expired activities', deserializedExpiredActivities)
                console.log(deserializedCurrentActivities.length);
                if (deserializedCurrentActivities.length > 0)
                    vueInstance.turnOnWindowsForActiveDevices(deserializedCurrentActivities);
                if (deserializedExpiredActivities !== null && deserializedExpiredActivities.length > 0)
                    vueInstance.shutOffExpiredActivities(deserializedExpiredActivities);

                vueInstance.activeDevices = deserializedCurrentActivities;
            }

            // Start the connection
            $.connection.hub.start().done(this.signalrInitialized);
        }
    });
});
