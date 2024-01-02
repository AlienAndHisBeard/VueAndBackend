<template>
    <div class="post" >
        <div v-if="loading" class="loading">
            Loading...
        </div>

        <div v-if="post && postType=='BusStops'" class="content scroll" >
            <button class="button" @click="fetchData('BusStops')">Refresh</button>
            <vue-good-table 
                :columns="columns"
                :rows="post"
                :search-options="{
                    enabled: true,
                    trigger: 'enter'
                }"
                @row-click="onRowClick"
                theme="nocturnal"
                :pagination-options="{
                    enabled: true,
                    mode: 'records'
                }">
            </vue-good-table>
        </div>

        <div v-if="post && postType=='BusStops/'" class="content" >
            <button class="button" @click="fetchData('BusStops')">All bus stops</button>
            <button class="button" @click="fetchData('BusStops/delays', post['stopId'])">Delays</button>
            <button class="button" @click="addBusStop()">Add to favourites</button>
            <button class="button" @click="delBusStop()">Remove from favourites</button>
            <h3>Detailed info for the selected stop</h3>
            <table style="align-items: center;">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Subname</th>
                        <th>Description</th>
                        <th>On Demand</th>
                    </tr>
                </thead>
                <tbody >
                    <tr @click="fetchData('BusStops/delays', post['stopId'])">
                        <td>{{ post["stopId"] }}</td>
                        <td>{{ post["stopName"] }}</td>
                        <td>{{ post["subName"] }}</td>
                        <td>{{ post["stopDesc"] }}</td>
                        <td>{{ post["onDemand"] }}</td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div v-if="post && postType=='BusStops/delays/'" class="content scroll" >
            <button class="button" @click="fetchData('BusStops')">All bus stops</button>
            <vue-good-table 
                    :columns="delaysColumns"
                    :rows="post['delay']"
                    theme="nocturnal"
                    @row-click="fetchData('BusStops')"
                    :pagination-options="{
                        enabled: true,
                        mode: 'records'
                    }"
                    >
                </vue-good-table>
        </div>
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        data() {
            return {
                loading: false,
                post: null,
                postType: null,
                currentStopId: null,
                columns: [
                    {
                        label: 'Id',
                        field: 'StopId',
                        type: 'number',
                    },
                    {
                        label: 'Name',
                        field: 'StopName',
                    },
                    {
                        label: 'Subname',
                        field: 'SubName',
                    },
                ],
                delaysColumns: [
                    {
                        label: 'Bus line',
                        field: 'routeId',
                        type: 'number',
                    },
                    {
                        label: 'Target',
                        field: 'headsign',
                    },
                    {
                        label: 'Delay',
                        field: 'delayInSeconds',
                        type: 'number',
                    },
                    {
                        label: 'Estimated time',
                        field: 'estimatedTime',
                    },
                    {
                        label: 'Theoretical time',
                        field: 'theoreticalTime',
                    },
                ]
            };
        },
        created() {
            // fetch the data when the view is created and the data is
            // already being observed
            this.fetchData("BusStops");
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchData'
        },
        methods: {
            onRowClick(params) {
                // params.row - row object 
                // params.pageIndex - index of this row on the current page.
                // params.selected - if selection is enabled this argument 
                // indicates selected or not
                // params.event - click event
                this.fetchData("BusStops", params.row.StopId);
            },
            fetchData(url, id=null) {
                this.post = null;
                this.loading = true;
                this.postType = url;
                if (id != null)
                {
                    url+="/"+id;
                    this.postType+="/";
                    this.currentStopId = id;
                }
                fetch('/api/'+url)
                    .then(r => r.json())
                    .then(json => {
                        this.post = json;
                        this.loading = false;
                        return;
                    });
                
            },
            addBusStop() {
                this.loading = true;
                if (this.currentStopId == null || this.$store.state.userId == null)
                {
                    this.loading = false;
                    return;
                }
                const requestOptions = {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify([this.$store.state.userId, this.currentStopId])
                };
                fetch('/api/Users/AddUserBusStop', requestOptions)
                    .then(() => {
                        this.loading = false;
                        return;
                    });
            },
            delBusStop() {
                this.loading = true;
                if (this.currentStopId == null || this.$store.state.userId == null)
                {
                    this.loading = false;
                    return;
                }
                const requestOptions = {
                    method: "DELETE",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify([this.$store.state.userId, this.currentStopId])
                };
                fetch('/api/Users/DeleteUserBusStop', requestOptions)
                    .then(() => {
                        this.loading = false;
                        return;
                    });
            }
            
            
        },
    });
</script>

<style scoped>
.scroll {
    overflow-y:auto;
    max-height: 85vh;
}
</style>